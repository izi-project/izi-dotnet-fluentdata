namespace Izi.FluentData.Transformer;

public sealed partial class PropertyRule<T, TProperty>
{
    /// <summary>
    /// Converts the property with <see cref="TransformerRules.Map{TIn, TOut}"/>.
    /// Same-type maps stay on <see cref="TransformerRuleExtensions.Map{T, TProperty}(PropertyRule{T, TProperty}, Func{TProperty, TProperty})"/>.
    /// </summary>
    /// <typeparam name="TOut">The type the function produces.</typeparam>
    /// <param name="map">The function that converts the property value.</param>
    /// <returns>The conversion, so <see cref="PropertyConversion{T, TIn, TOut}.Into"/> can store it.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="map"/> is <see langword="null"/>.</exception>
    public PropertyConversion<T, TProperty, TOut> MapTo<TOut>(Func<TProperty, TOut> map)
        => Convert(TransformerRules.Map(map));

    /// <summary>
    /// Converts the property with <see cref="TransformerRules.MapAsync{TIn, TOut}"/>.
    /// </summary>
    /// <typeparam name="TOut">The type the function produces.</typeparam>
    /// <param name="map">The function that converts the property value.</param>
    /// <returns>The conversion, so <see cref="PropertyConversion{T, TIn, TOut}.Into"/> can store it.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="map"/> is <see langword="null"/>.</exception>
    public PropertyConversion<T, TProperty, TOut> MapToAsync<TOut>(
        Func<TProperty, CancellationToken, ValueTask<TOut>> map)
        => Convert(TransformerRules.MapAsync(map));
}

public sealed partial class PropertyConversion<T, TIn, TOut>
{
    /// <summary>Continues with a type-changing <see cref="TransformerRules.Map{TIn, TOut}"/>.</summary>
    /// <typeparam name="TNext">The type the function produces.</typeparam>
    /// <param name="map">The function that converts the current value.</param>
    /// <returns>The continued conversion.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="map"/> is <see langword="null"/>.</exception>
    public PropertyConversion<T, TIn, TNext> MapTo<TNext>(Func<TOut, TNext> map)
        => Then(TransformerRules.Map(map));

    /// <summary>Continues with a type-changing <see cref="TransformerRules.MapAsync{TIn, TOut}"/>.</summary>
    /// <typeparam name="TNext">The type the function produces.</typeparam>
    /// <param name="map">The function that converts the current value.</param>
    /// <returns>The continued conversion.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="map"/> is <see langword="null"/>.</exception>
    public PropertyConversion<T, TIn, TNext> MapToAsync<TNext>(
        Func<TOut, CancellationToken, ValueTask<TNext>> map)
        => Then(TransformerRules.MapAsync(map));
}
