namespace Izi.FluentData.Validation;

/// <summary>A rule backed by a predicate and a fixed message.</summary>
/// <typeparam name="T">The value type the rule reads.</typeparam>
/// <remarks>
/// The message is generic. It does not name a property and it is not a template.
/// A predicate that finishes synchronously does not allocate an async state machine.
/// A passing check returns <see cref="ValidationRuleResult.Success"/>.
/// </remarks>
public sealed class ValidatorRule<T> : IValidatorRule<T>
{
    private readonly Func<T, CancellationToken, ValueTask<bool>> _predicate;
    private readonly string _message;

    /// <summary>Creates a synchronous rule.</summary>
    /// <param name="predicate">Returns true when the value passes.</param>
    /// <param name="message">The message used when the value fails. It does not name a property.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="predicate"/> or <paramref name="message"/> is <see langword="null"/>.
    /// </exception>
    public ValidatorRule(Func<T, bool> predicate, string message)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(message);
        _predicate = (value, _) => new ValueTask<bool>(predicate(value));
        _message = message;
    }

    /// <summary>Creates an asynchronous rule.</summary>
    /// <param name="predicate">Returns true when the value passes.</param>
    /// <param name="message">The message used when the value fails. It does not name a property.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="predicate"/> or <paramref name="message"/> is <see langword="null"/>.
    /// </exception>
    public ValidatorRule(Func<T, CancellationToken, ValueTask<bool>> predicate, string message)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(message);
        _predicate = predicate;
        _message = message;
    }

    /// <summary>Checks <paramref name="value"/>.</summary>
    /// <param name="value">The value to check.</param>
    /// <param name="cancellationToken">Checked before the predicate runs, then passed to the predicate.</param>
    /// <returns>The rule result. A pass is <see cref="ValidationRuleResult.Success"/>.</returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> is already cancelled.</exception>
    public ValueTask<ValidationRuleResult> ValidateAsync(T value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var pending = _predicate(value, cancellationToken);
        if (pending.IsCompletedSuccessfully)
        {
            return new ValueTask<ValidationRuleResult>(pending.Result ? ValidationRuleResult.Success : ValidationRuleResult.Failed(_message));
        }

        if (pending.IsCompleted)
        {
            pending.GetAwaiter().GetResult();
        }

        return WaitAsync(pending);
    }

    private async ValueTask<ValidationRuleResult> WaitAsync(ValueTask<bool> pending)
    {
        var passed = await pending.ConfigureAwait(false);
        return passed ? ValidationRuleResult.Success : ValidationRuleResult.Failed(_message);
    }
}
