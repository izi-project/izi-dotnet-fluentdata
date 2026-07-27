namespace Izi.FluentData.Validation.Rules;

/// <summary>
/// The default <see cref="IValidatorRule{T}"/> implementation: wraps a single predicate and the single
/// message reported when it fails. This is the concrete type returned by every factory on
/// <see cref="ValidatorRules"/>.
/// </summary>
/// <remarks>
/// The message is produced by a <see cref="Func{T, TResult}"/> that is invoked <em>only</em> when the predicate
/// fails, so a rule that passes never builds its message. The constant-message constructor is a thin adapter
/// over that delegate; the <see cref="ValidatorRule{T}(Func{T, CancellationToken, ValueTask{bool}}, Func{T, string})"/>
/// overload is the one to use when the message needs to mention the value that actually failed, or to compose
/// anything more elaborate than a constant.
/// </remarks>
/// <typeparam name="T">The type of value this rule validates.</typeparam>
public class ValidatorRule<T> : IValidatorRule<T>
{
    private readonly Func<T, CancellationToken, ValueTask<bool>> _evaluateFunc;
    private readonly Func<T, string> _messageFunc;

    /// <summary>Creates a rule from a predicate and a constant message reported on failure.</summary>
    /// <param name="evaluateFunc">Returns <see langword="true"/> when the value is valid.</param>
    /// <param name="message">The message reported when the value is invalid. Reported verbatim, so braces
    /// carry no special meaning — a regex pattern or JSON snippet embedded in the text survives intact.</param>
    public ValidatorRule(Func<T, CancellationToken, ValueTask<bool>> evaluateFunc, string message) : this(evaluateFunc, (_) => message)
    {
    }

    /// <summary>Creates a rule from a predicate and a factory that builds the message from the failing value.</summary>
    /// <param name="evaluateFunc">Returns <see langword="true"/> when the value is valid.</param>
    /// <param name="messageFunc">Builds the message reported when the value is invalid. Invoked only on
    /// failure, and receives the value that failed — e.g. <c>value =&gt; $"'{value}' is not a valid code."</c>.
    /// Any formatting, localisation, or lookup the message needs belongs inside this delegate.</param>
    public ValidatorRule(Func<T, CancellationToken, ValueTask<bool>> evaluateFunc, Func<T, string> messageFunc)
    {
        _evaluateFunc = evaluateFunc;
        _messageFunc = messageFunc;
    }

    /// <summary>Evaluates the predicate against <paramref name="instance"/>.</summary>
    /// <param name="instance">The value to validate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The failure message, or <see langword="null"/> when the value is valid.</returns>
    public ValueTask<string?> ValidateAsync(T instance, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var pending = _evaluateFunc(instance, cancellationToken);
        // Fast path: synchronous predicates (all the built-ins) skip the async state machine entirely.
        if (pending.IsCompletedSuccessfully)
        {
            return pending.Result ? ValueTask.FromResult<string?>(null) : ValueTask.FromResult<string?>(_messageFunc(instance));
        }
        return ValidateSlowAsync(pending, instance, cancellationToken);
    }

    // Reached only when the predicate genuinely yields (e.g. a real async lookup).
    private async ValueTask<string?> ValidateSlowAsync(ValueTask<bool> pending, T instance, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await pending.ConfigureAwait(false);
        return result ? null : _messageFunc(instance);
    }
}
