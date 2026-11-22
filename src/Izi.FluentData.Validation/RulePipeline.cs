namespace Izi.FluentData.Validation;

/// <summary>Runs a rule list. The synchronous pass does not allocate when every rule passes.</summary>
/// <typeparam name="TValue">The value the rules read.</typeparam>
internal static class RulePipeline<TValue>
{
    /// <summary>The synchronous portion of a rule list.</summary>
    internal readonly struct SyncRules
    {
        /// <summary>True when every rule finished without waiting.</summary>
        internal bool Completed { get; init; }

        /// <summary>The rule that is still running. Meaningful when <see cref="Completed"/> is false.</summary>
        internal ValueTask<ValidationRuleResult> Pending { get; init; }

        /// <summary>The index of the next rule after <see cref="Pending"/>.</summary>
        internal int NextIndex { get; init; }

        /// <summary>Messages collected from rules that already failed, or null when none have.</summary>
        internal List<string>? Messages { get; init; }
    }

    /// <summary>Runs <paramref name="rules"/> until one waits or the list ends.</summary>
    /// <param name="rules">The rules, in order.</param>
    /// <param name="value">The value to check.</param>
    /// <param name="cancellationToken">Checked between rules.</param>
    /// <returns>The messages so far, and the waiting rule when one did not finish.</returns>
    internal static SyncRules Read(BoundRule<TValue>[] rules, TValue value, CancellationToken cancellationToken)
    {
        List<string>? messages = null;
        for (var i = 0; i < rules.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var pending = rules[i].ValidateAsync(value, cancellationToken);
            if (!pending.IsCompletedSuccessfully)
            {
                if (pending.IsCompleted)
                {
                    pending.GetAwaiter().GetResult();
                }

                return new SyncRules
                {
                    Completed = false,
                    Pending = pending,
                    NextIndex = i + 1,
                    Messages = messages,
                };
            }

            messages = ValidationRun.Collect(messages, pending.Result, rules[i].Message);
        }

        return new SyncRules { Completed = true, Messages = messages };
    }

    /// <summary>Finishes a waiting rule, then the rules that follow it.</summary>
    /// <param name="pending">The rule that did not finish synchronously.</param>
    /// <param name="rules">The full rule list.</param>
    /// <param name="index">The index of the rule after <paramref name="pending"/>.</param>
    /// <param name="value">The value to check.</param>
    /// <param name="messages">Messages collected before the waiting rule.</param>
    /// <param name="cancellationToken">Checked between rules.</param>
    /// <returns>Every message from this value.</returns>
    internal static async ValueTask<List<string>?> ContinueAsync(
        ValueTask<ValidationRuleResult> pending,
        BoundRule<TValue>[] rules,
        int index,
        TValue value,
        List<string>? messages,
        CancellationToken cancellationToken)
    {
        var result = await pending.ConfigureAwait(false);
        messages = ValidationRun.Collect(messages, result, rules[index - 1].Message);
        for (; index < rules.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            result = await rules[index].ValidateAsync(value, cancellationToken).ConfigureAwait(false);
            messages = ValidationRun.Collect(messages, result, rules[index].Message);
        }

        return messages;
    }
}
