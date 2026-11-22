namespace Izi.FluentData.Validation;

/// <summary>
/// One compiled step inside <see cref="ImmutableValidator{T}"/>.
/// A step that finds nothing wrong returns the same failure list it was given, which is null when nothing has failed yet.
/// </summary>
/// <typeparam name="T">The object being validated.</typeparam>
internal interface ICompiledPropertyValidator<T>
{
    /// <summary>Appends failures for this step.</summary>
    /// <param name="instance">The object being validated.</param>
    /// <param name="failures">Failures recorded by earlier steps, or null when none have failed.</param>
    /// <param name="cancellationToken">Checked while the step runs.</param>
    /// <param name="depth">The nesting depth of this call. The root is zero.</param>
    /// <returns>The failure list, or null when this step and every earlier step passed.</returns>
    ValueTask<List<PropertyValidationResult>?> AppendAsync(
        T instance,
        List<PropertyValidationResult>? failures,
        CancellationToken cancellationToken,
        int depth);
}
