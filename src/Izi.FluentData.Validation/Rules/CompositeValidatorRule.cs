namespace Izi.FluentData.Validation.Rules;

/// <summary>
/// Runs an ordered set of <see cref="ValidatorRule{T}"/> against a single value and aggregates every failure
/// message. Unlike short-circuiting validation, all rules are evaluated so the caller sees the complete set
/// of problems in one pass.
/// </summary>
/// <typeparam name="T">The type of value validated by the composed rules.</typeparam>
public class CompositeValidatorRule<T>
{
    // Shared, immutable result so the passing path (no errors) allocates nothing.
    private readonly static IReadOnlyList<string> EmptyErrors = [];

    private readonly List<ValidatorRule<T>> _rules;


    /// <summary>Composes one or more rules, with optional additional rules supplied as a span.</summary>
    /// <param name="rule">The first rule (required).</param>
    /// <param name="additionalRules">Any further rules, evaluated in order after <paramref name="rule"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> is <see langword="null"/>.</exception>
    public CompositeValidatorRule(ValidatorRule<T> rule, params ReadOnlySpan<ValidatorRule<T>> additionalRules)
    {
        ArgumentNullException.ThrowIfNull(rule);
        _rules = [rule, .. additionalRules];
    }

    /// <summary>Composes a sequence of rules.</summary>
    /// <param name="rules">The rules to evaluate, in order.</param>
    /// <exception cref="ArgumentNullException"><paramref name="rules"/> is <see langword="null"/>.</exception>
    public CompositeValidatorRule(IEnumerable<ValidatorRule<T>> rules)
    {
        ArgumentNullException.ThrowIfNull(rules);
        _rules = [.. rules];
    }

    /// <summary>Evaluates every composed rule and returns the union of their failure messages.</summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The aggregated failure messages; an empty list means valid.</returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled.</exception>
    public ValueTask<IReadOnlyList<string>> ValidateAsync(T value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // Lazily allocated: stays null (no allocation) until at least one rule reports an error.
        List<string>? errors = null;

        for (int i = 0; i < _rules.Count; i++)
        {
            var pendingTask = _rules[i].ValidateAsync(value, cancellationToken);
            // Fast path: while rules complete synchronously the whole pass stays synchronous and allocation-free.
            if (pendingTask.IsCompletedSuccessfully)
            {
                var result = pendingTask.Result;
                if (result.Count > 0)
                {
                    (errors ??= []).AddRange(result);
                }
            }
            else
            {
                // Hand off the remaining (and the in-flight) rules to the async continuation.
                return ValidateSlowAsync(_rules, i, pendingTask, errors, value, cancellationToken);
            }
        }

        return ValueTask.FromResult(errors ?? EmptyErrors);
    }

    // Async tail resumed once a rule yields; finishes awaiting the current one then drains the rest.
    private static async ValueTask<IReadOnlyList<string>> ValidateSlowAsync(List<ValidatorRule<T>> rules, int currentIndex, ValueTask<IReadOnlyList<string>> currentPendingTask, List<string>? errors, T value, CancellationToken cancellationToken)
    {
        var currentResult = await currentPendingTask;
        if (currentResult.Count > 0)
        {
            (errors ??= []).AddRange(currentResult);
        }

        for (int i = currentIndex + 1; i < rules.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await rules[i].ValidateAsync(value, cancellationToken);
            if (result.Count > 0)
            {
                (errors ??= []).AddRange(result);
            }
        }

        return errors ?? EmptyErrors;
    }
}
