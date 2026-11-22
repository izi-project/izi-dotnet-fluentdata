using System.Runtime.CompilerServices;

namespace Izi.FluentData.Validation;

/// <summary>Stops a validator from recursing until the process runs out of stack.</summary>
internal static class ValidationLimits
{
    /// <summary>
    /// The root call is depth 0. Each <c>SetValidator</c> or <c>Include</c> adds one.
    /// Past this depth the run throws <see cref="InvalidOperationException"/> instead of overflowing the stack.
    /// A custom <see cref="IValidator{T}"/> is counted too: the depth is stored in
    /// <see cref="ValidationDepth"/> while that wrapper runs, and the next <see cref="Validator{T}"/> reads it back.
    /// </summary>
    internal const int MaxDepth = 128;

    /// <summary>Throws when <paramref name="depth"/> is past <see cref="MaxDepth"/> or, on a nested call, the stack is nearly exhausted.</summary>
    /// <param name="depth">The depth of the call about to run. Zero skips the stack check.</param>
    internal static void Check(int depth)
    {
        if (depth > MaxDepth)
        {
            throw new InvalidOperationException(
                "Validation nested more than " + MaxDepth.ToString(System.Globalization.CultureInfo.InvariantCulture)
                + " levels. A child validator is walking a cycle, or validators include each other in a cycle.");
        }

        if (depth > 0 && !RuntimeHelpers.TryEnsureSufficientExecutionStack())
        {
            throw new InvalidOperationException("Insufficient execution stack to continue validation.");
        }
    }
}

/// <summary>
/// Depth carried across a custom <see cref="IValidator{T}"/>.
/// It is written only at that boundary. A direct <see cref="Validator{T}"/> call leaves it unset, which reads as zero and does not allocate.
/// </summary>
internal static class ValidationDepth
{
    private static readonly AsyncLocal<int> CurrentLocal = new();
    private static int _used;

    /// <summary>The depth a custom validator should hand to the next <see cref="Validator{T}"/>. Zero when unset.</summary>
    internal static int Current
    {
        get => CurrentLocal.Value;
        set
        {
            Volatile.Write(ref _used, 1);
            CurrentLocal.Value = value;
        }
    }

    /// <summary>Zero until a custom validator has been entered in this process, so the fast path does not touch <see cref="AsyncLocal{T}"/>.</summary>
    internal static int ReadIfUsed()
        => Volatile.Read(ref _used) == 0 ? 0 : CurrentLocal.Value;
}
