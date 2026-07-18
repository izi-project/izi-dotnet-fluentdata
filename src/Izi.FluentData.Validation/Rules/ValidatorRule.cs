namespace Izi.FluentData.Validation.Rules;

/// <summary>
/// The default <see cref="IValidatorRule{T}"/> implementation: wraps a single predicate and the single
/// message reported when it fails. This is the concrete type returned by every factory on
/// <see cref="ValidatorRules"/>.
/// </summary>
/// <typeparam name="T">The type of value this rule validates.</typeparam>
public class ValidatorRule<T> : IValidatorRule<T>
{
    private readonly Func<T, CancellationToken, ValueTask<bool>> _evaluateFunc;
    private readonly string _message;

    /// <summary>Creates a rule from a predicate and the message reported on failure.</summary>
    /// <param name="evaluateFunc">Returns <see langword="true"/> when the value is valid.</param>
    /// <param name="message">The message reported when the value is invalid.</param>
    public ValidatorRule(Func<T, CancellationToken, ValueTask<bool>> evaluateFunc, string message)
    {
        _evaluateFunc = evaluateFunc;
        _message = message;
    }

    /// <summary>Evaluates the predicate against <paramref name="instance"/>.</summary>
    /// <param name="instance">The value to validate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The failure message, or <see langword="null"/> when the value is valid.</returns>
    public ValueTask<string?> ValidateAsync(T instance, CancellationToken cancellationToken = default)
    {
        var pending = _evaluateFunc(instance, cancellationToken);
        // Fast path: synchronous predicates (all the built-ins) skip the async state machine entirely.
        if (pending.IsCompletedSuccessfully)
        {
            return pending.Result ? ValueTask.FromResult<string?>(null) : ValueTask.FromResult<string?>(_message);
        }
        return ValidateSlowAsync(pending, cancellationToken);
    }

    // Reached only when the predicate genuinely yields (e.g. a real async lookup).
    private async ValueTask<string?> ValidateSlowAsync(ValueTask<bool> pending, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await pending.ConfigureAwait(false);
        return result ? null : _message;
    }
}
