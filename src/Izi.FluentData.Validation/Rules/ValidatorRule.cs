namespace Izi.FluentData.Validation.Rules;

/// <summary>
/// The atomic unit of validation: a predicate over a <typeparamref name="T"/> value plus the message(s)
/// reported when it fails. A rule may carry <em>dependent</em> rules that run only after it itself passes,
/// modelling short-circuited checks (e.g. only validate length once a value is known non-null).
/// </summary>
/// <typeparam name="T">The type of value this rule validates.</typeparam>
public class ValidatorRule<T>
{
    // Shared, immutable result so the passing path (no errors) allocates nothing.
    private readonly static IReadOnlyList<string> EmptyErrors = [];


    private readonly Func<T, CancellationToken, ValueTask<bool>> _evaluateFunc;
    // Pre-sized to 1: a rule almost always carries exactly one failure message.
    private readonly List<string> _messages = new(1);
    private readonly List<ValidatorRule<T>> _dependents = [];


    /// <summary>Creates a rule with no message; supply one later via <see cref="WithMessage"/>.</summary>
    /// <param name="evaluateFunc">Returns <see langword="true"/> when the value is valid.</param>
    public ValidatorRule(Func<T, CancellationToken, ValueTask<bool>> evaluateFunc)
    {
        _evaluateFunc = evaluateFunc;
    }

    /// <summary>Creates a rule with a single failure message.</summary>
    /// <param name="evaluateFunc">Returns <see langword="true"/> when the value is valid.</param>
    /// <param name="message">The message reported when the value is invalid.</param>
    public ValidatorRule(Func<T, CancellationToken, ValueTask<bool>> evaluateFunc, string message)
    {
        _evaluateFunc = evaluateFunc;
        _messages.Add(message);
    }

    /// <summary>Creates a rule with several failure messages.</summary>
    /// <param name="evaluateFunc">Returns <see langword="true"/> when the value is valid.</param>
    /// <param name="messages">The messages reported when the value is invalid.</param>
    public ValidatorRule(Func<T, CancellationToken, ValueTask<bool>> evaluateFunc, IEnumerable<string> messages)
    {
        _evaluateFunc = evaluateFunc;
        _messages.AddRange(messages);
    }


    /// <summary>Replaces this rule's failure message, overriding any default supplied at construction.</summary>
    /// <param name="message">The message to report on failure.</param>
    /// <returns>The same rule, for chaining.</returns>
    public ValidatorRule<T> WithMessage(string message)
    {
        _messages.Clear();
        _messages.Add(message);
        return this;
    }

    /// <summary>Replaces this rule's failure messages, overriding any default supplied at construction.</summary>
    /// <param name="messages">The messages to report on failure.</param>
    /// <returns>The same rule, for chaining.</returns>
public ValidatorRule<T> WithMessages(IEnumerable<string> messages)
{
    ArgumentNullException.ThrowIfNull(messages);
    _messages.Clear();
    _messages.AddRange(messages);
    return this;
}


    /// <summary>Attaches a dependent rule, built from a predicate, that runs only if this rule passes.</summary>
    /// <param name="evaluateFunc">Returns <see langword="true"/> when the value is valid.</param>
    /// <returns>The same rule, for chaining.</returns>
    public ValidatorRule<T> WithDependent(Func<T, CancellationToken, ValueTask<bool>> evaluateFunc)
    {
        _dependents.Add(new ValidatorRule<T>(evaluateFunc));
        return this;
    }

    /// <summary>Attaches a dependent rule that runs only if this rule passes.</summary>
    /// <param name="dependent">The rule to run after this one succeeds.</param>
    /// <returns>The same rule, for chaining.</returns>
    public ValidatorRule<T> WithDependent(ValidatorRule<T> dependent)
    {
        _dependents.Add(dependent);
        return this;
    }

    /// <summary>Attaches several dependent rules that run only if this rule passes.</summary>
    /// <param name="dependents">The rules to run after this one succeeds.</param>
    /// <returns>The same rule, for chaining.</returns>
    public ValidatorRule<T> WithDependents(IEnumerable<ValidatorRule<T>> dependents)
    {
        _dependents.AddRange(dependents);
        return this;
    }


    /// <summary>
    /// Evaluates the rule against <paramref name="value"/>. On failure the rule's own messages are returned and
    /// dependents are skipped; on success the dependent rules (if any) are evaluated and their errors returned.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The failure messages; an empty list means the value (and its dependents) are valid.</returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled.</exception>
    public ValueTask<IReadOnlyList<string>> ValidateAsync(T value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var pending = _evaluateFunc(value, cancellationToken);

        // Fast path: synchronous predicates (all the built-ins) skip the async state machine entirely.
        if (pending.IsCompletedSuccessfully)
        {
            bool isValid = pending.Result;
            // Short-circuit: a failing rule never runs its dependents, and returns the shared message list directly.
            return isValid
                ? ValidateDependentsAsync(_dependents, value, cancellationToken)
                : ValueTask.FromResult<IReadOnlyList<string>>(_messages);
        }

        return ValidateSlowAsync(this, pending, value, cancellationToken);
    }

    // Static so the awaiting state machine captures no `this`; reached only when the predicate genuinely yields.
    private static async ValueTask<IReadOnlyList<string>> ValidateSlowAsync(ValidatorRule<T> rule, ValueTask<bool> pending, T value, CancellationToken cancellationToken)
    {
        var isValid = await pending;

        if (!isValid)
        {
            return rule._messages;
        }

        return await ValidateDependentsAsync(rule._dependents, value, cancellationToken);
    }

    // Aggregates dependent errors, staying synchronous (and allocating no error list) until a dependent
    // actually fails or yields. EmptyErrors is returned when there is nothing to report.
    private static ValueTask<IReadOnlyList<string>> ValidateDependentsAsync(List<ValidatorRule<T>> dependents, T value, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (dependents.Count == 0) return ValueTask.FromResult(EmptyErrors);

        // Lazily allocated: stays null (no allocation) until at least one dependent reports an error.
        List<string>? errors = null;
        for (int i = 0; i < dependents.Count; i++)
        {
            var dependentErrors = dependents[i].ValidateAsync(value, cancellationToken);
            if (dependentErrors.IsCompletedSuccessfully)
            {
                var result = dependentErrors.Result;
                if (result.Count > 0)
                {
                    (errors ??= []).AddRange(result);
                }
            }
            else
            {
                // Hand off the remaining (and the in-flight) dependents to the async continuation.
                return ValidateDependentsSlowAsync(dependents, i, dependentErrors, errors, value, cancellationToken);
            }
        }

        return ValueTask.FromResult(errors ?? EmptyErrors);
    }

    // Async tail resumed once a dependent yields; finishes awaiting the current one then drains the rest.
    private static async ValueTask<IReadOnlyList<string>> ValidateDependentsSlowAsync(List<ValidatorRule<T>> dependents, int currentIndex, ValueTask<IReadOnlyList<string>> currentPendingTask, List<string>? errors, T value, CancellationToken cancellationToken)
    {
        var currentResult = await currentPendingTask;

        if (currentResult.Count > 0)
        {
            (errors ??= []).AddRange(currentResult);
        }

        for (int i = currentIndex + 1; i < dependents.Count; i++)
        {
            var result = await dependents[i].ValidateAsync(value, cancellationToken);
            if (result.Count > 0)
            {
                (errors ??= []).AddRange(result);
            }
        }

        return errors ?? EmptyErrors;
    }
}
