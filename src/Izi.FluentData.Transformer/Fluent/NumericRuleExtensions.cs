using System.Numerics;

namespace Izi.FluentData.Transformer;

/// <summary>
/// Fluent wrappers that append a catalogue rule and return the builder.
/// Same-type steps call <see cref="PropertyRule{T, TProperty}.Add(ITransformerRule{TProperty, TProperty})"/>
/// or <see cref="PropertyConversion{T, TIn, TOut}.Then{TNext}(ITransformerRule{TOut, TNext})"/>.
/// Steps that change type call <see cref="PropertyRule{T, TProperty}.Convert{TOut}(ITransformerRule{TProperty, TOut})"/>
/// or <c>Then</c>.
/// </summary>

public static partial class TransformerRuleExtensions
{
    /// <summary>Appends the catalogue numeric Add rule.</summary>
    public static PropertyRule<T, TNumber> Add<T, TNumber>(this PropertyRule<T, TNumber> rule, TNumber value)
        where TNumber : INumberBase<TNumber>
        => rule.Add(TransformerRules.Add(value));

    /// <summary>Appends the catalogue numeric Add rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Add<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion, TNumber value)
        where TNumber : INumberBase<TNumber>
        => conversion.Then(TransformerRules.Add(value));

    /// <summary>Appends the catalogue numeric Subtract rule.</summary>
    public static PropertyRule<T, TNumber> Subtract<T, TNumber>(this PropertyRule<T, TNumber> rule, TNumber value)
        where TNumber : INumberBase<TNumber>
        => rule.Add(TransformerRules.Subtract(value));

    /// <summary>Appends the catalogue numeric Subtract rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Subtract<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion, TNumber value)
        where TNumber : INumberBase<TNumber>
        => conversion.Then(TransformerRules.Subtract(value));

    /// <summary>Appends the catalogue Multiply rule.</summary>
    public static PropertyRule<T, TNumber> Multiply<T, TNumber>(this PropertyRule<T, TNumber> rule, TNumber value)
        where TNumber : INumberBase<TNumber>
        => rule.Add(TransformerRules.Multiply(value));

    /// <summary>Appends the catalogue Multiply rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Multiply<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion, TNumber value)
        where TNumber : INumberBase<TNumber>
        => conversion.Then(TransformerRules.Multiply(value));

    /// <summary>Appends the catalogue Divide rule.</summary>
    public static PropertyRule<T, TNumber> Divide<T, TNumber>(this PropertyRule<T, TNumber> rule, TNumber value)
        where TNumber : INumberBase<TNumber>
        => rule.Add(TransformerRules.Divide(value));

    /// <summary>Appends the catalogue Divide rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Divide<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion, TNumber value)
        where TNumber : INumberBase<TNumber>
        => conversion.Then(TransformerRules.Divide(value));

    /// <summary>Appends the catalogue Abs rule.</summary>
    public static PropertyRule<T, TNumber> Abs<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : INumberBase<TNumber>
        => rule.Add(TransformerRules.Abs<TNumber>());

    /// <summary>Appends the catalogue Abs rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Abs<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : INumberBase<TNumber>
        => conversion.Then(TransformerRules.Abs<TNumber>());

    /// <summary>Appends the catalogue numeric Clamp rule.</summary>
    public static PropertyRule<T, TNumber> Clamp<T, TNumber>(this PropertyRule<T, TNumber> rule, TNumber min, TNumber max)
        where TNumber : INumber<TNumber>
        => rule.Add(TransformerRules.Clamp(min, max));

    /// <summary>Appends the catalogue numeric Clamp rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Clamp<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion, TNumber min, TNumber max)
        where TNumber : INumber<TNumber>
        => conversion.Then(TransformerRules.Clamp(min, max));

    /// <summary>Appends the catalogue Invert rule.</summary>
    public static PropertyRule<T, TNumber> Invert<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : INumberBase<TNumber>
        => rule.Add(TransformerRules.Invert<TNumber>());

    /// <summary>Appends the catalogue Invert rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Invert<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : INumberBase<TNumber>
        => conversion.Then(TransformerRules.Invert<TNumber>());

    /// <summary>Appends the catalogue Round rule to the nearest integer.</summary>
    public static PropertyRule<T, TNumber> Round<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IFloatingPoint<TNumber>
        => rule.Add(TransformerRules.Round<TNumber>());

    /// <summary>Appends the catalogue Round rule to the nearest integer.</summary>
    public static PropertyConversion<T, TIn, TNumber> Round<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IFloatingPoint<TNumber>
        => conversion.Then(TransformerRules.Round<TNumber>());

    /// <summary>Appends the catalogue Round rule for a digit count.</summary>
    public static PropertyRule<T, TNumber> Round<T, TNumber>(this PropertyRule<T, TNumber> rule, int digits)
        where TNumber : IFloatingPoint<TNumber>
        => rule.Add(TransformerRules.Round<TNumber>(digits));

    /// <summary>Appends the catalogue Round rule for a digit count.</summary>
    public static PropertyConversion<T, TIn, TNumber> Round<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion, int digits)
        where TNumber : IFloatingPoint<TNumber>
        => conversion.Then(TransformerRules.Round<TNumber>(digits));

    /// <summary>Appends the catalogue Round rule for a midpoint mode.</summary>
    public static PropertyRule<T, TNumber> Round<T, TNumber>(this PropertyRule<T, TNumber> rule, MidpointRounding mode)
        where TNumber : IFloatingPoint<TNumber>
        => rule.Add(TransformerRules.Round<TNumber>(mode));

    /// <summary>Appends the catalogue Round rule for a midpoint mode.</summary>
    public static PropertyConversion<T, TIn, TNumber> Round<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion, MidpointRounding mode)
        where TNumber : IFloatingPoint<TNumber>
        => conversion.Then(TransformerRules.Round<TNumber>(mode));

    /// <summary>Appends the catalogue Round rule for a digit count and midpoint mode.</summary>
    public static PropertyRule<T, TNumber> Round<T, TNumber>(this PropertyRule<T, TNumber> rule, int digits, MidpointRounding mode)
        where TNumber : IFloatingPoint<TNumber>
        => rule.Add(TransformerRules.Round<TNumber>(digits, mode));

    /// <summary>Appends the catalogue Round rule for a digit count and midpoint mode.</summary>
    public static PropertyConversion<T, TIn, TNumber> Round<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion, int digits, MidpointRounding mode)
        where TNumber : IFloatingPoint<TNumber>
        => conversion.Then(TransformerRules.Round<TNumber>(digits, mode));

    /// <summary>Appends the catalogue Floor rule.</summary>
    public static PropertyRule<T, TNumber> Floor<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IFloatingPoint<TNumber>
        => rule.Add(TransformerRules.Floor<TNumber>());

    /// <summary>Appends the catalogue Floor rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Floor<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IFloatingPoint<TNumber>
        => conversion.Then(TransformerRules.Floor<TNumber>());

    /// <summary>Appends the catalogue Ceiling rule.</summary>
    public static PropertyRule<T, TNumber> Ceiling<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IFloatingPoint<TNumber>
        => rule.Add(TransformerRules.Ceiling<TNumber>());

    /// <summary>Appends the catalogue Ceiling rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Ceiling<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IFloatingPoint<TNumber>
        => conversion.Then(TransformerRules.Ceiling<TNumber>());

    /// <summary>Appends the catalogue numeric Truncate rule.</summary>
    public static PropertyRule<T, TNumber> Truncate<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IFloatingPoint<TNumber>
        => rule.Add(TransformerRules.Truncate<TNumber>());

    /// <summary>Appends the catalogue numeric Truncate rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Truncate<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IFloatingPoint<TNumber>
        => conversion.Then(TransformerRules.Truncate<TNumber>());

    /// <summary>Appends the catalogue Normalize rule.</summary>
    public static PropertyRule<T, TNumber> Normalize<T, TNumber>(this PropertyRule<T, TNumber> rule, TNumber min, TNumber max)
        where TNumber : INumber<TNumber>
        => rule.Add(TransformerRules.Normalize(min, max));

    /// <summary>Appends the catalogue Normalize rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Normalize<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion, TNumber min, TNumber max)
        where TNumber : INumber<TNumber>
        => conversion.Then(TransformerRules.Normalize(min, max));

    /// <summary>Appends the catalogue Sqrt rule.</summary>
    public static PropertyRule<T, TNumber> Sqrt<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IRootFunctions<TNumber>
        => rule.Add(TransformerRules.Sqrt<TNumber>());

    /// <summary>Appends the catalogue Sqrt rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Sqrt<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IRootFunctions<TNumber>
        => conversion.Then(TransformerRules.Sqrt<TNumber>());

    /// <summary>Appends the catalogue Cbrt rule.</summary>
    public static PropertyRule<T, TNumber> Cbrt<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IRootFunctions<TNumber>
        => rule.Add(TransformerRules.Cbrt<TNumber>());

    /// <summary>Appends the catalogue Cbrt rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Cbrt<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IRootFunctions<TNumber>
        => conversion.Then(TransformerRules.Cbrt<TNumber>());

    /// <summary>Appends the catalogue RootN rule.</summary>
    public static PropertyRule<T, TNumber> RootN<T, TNumber>(this PropertyRule<T, TNumber> rule, int n)
        where TNumber : IRootFunctions<TNumber>
        => rule.Add(TransformerRules.RootN<TNumber>(n));

    /// <summary>Appends the catalogue RootN rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> RootN<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion, int n)
        where TNumber : IRootFunctions<TNumber>
        => conversion.Then(TransformerRules.RootN<TNumber>(n));

    /// <summary>Appends the catalogue Pow rule.</summary>
    public static PropertyRule<T, TNumber> Pow<T, TNumber>(this PropertyRule<T, TNumber> rule, TNumber exponent)
        where TNumber : IPowerFunctions<TNumber>
        => rule.Add(TransformerRules.Pow(exponent));

    /// <summary>Appends the catalogue Pow rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Pow<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion, TNumber exponent)
        where TNumber : IPowerFunctions<TNumber>
        => conversion.Then(TransformerRules.Pow(exponent));

    /// <summary>Appends the catalogue Exp rule.</summary>
    public static PropertyRule<T, TNumber> Exp<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IExponentialFunctions<TNumber>
        => rule.Add(TransformerRules.Exp<TNumber>());

    /// <summary>Appends the catalogue Exp rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Exp<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IExponentialFunctions<TNumber>
        => conversion.Then(TransformerRules.Exp<TNumber>());

    /// <summary>Appends the catalogue Exp2 rule.</summary>
    public static PropertyRule<T, TNumber> Exp2<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IExponentialFunctions<TNumber>
        => rule.Add(TransformerRules.Exp2<TNumber>());

    /// <summary>Appends the catalogue Exp2 rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Exp2<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IExponentialFunctions<TNumber>
        => conversion.Then(TransformerRules.Exp2<TNumber>());

    /// <summary>Appends the catalogue Exp10 rule.</summary>
    public static PropertyRule<T, TNumber> Exp10<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IExponentialFunctions<TNumber>
        => rule.Add(TransformerRules.Exp10<TNumber>());

    /// <summary>Appends the catalogue Exp10 rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Exp10<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IExponentialFunctions<TNumber>
        => conversion.Then(TransformerRules.Exp10<TNumber>());

    /// <summary>Appends the catalogue natural Log rule.</summary>
    public static PropertyRule<T, TNumber> Log<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ILogarithmicFunctions<TNumber>
        => rule.Add(TransformerRules.Log<TNumber>());

    /// <summary>Appends the catalogue natural Log rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Log<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ILogarithmicFunctions<TNumber>
        => conversion.Then(TransformerRules.Log<TNumber>());

    /// <summary>Appends the catalogue Log rule for a base.</summary>
    public static PropertyRule<T, TNumber> Log<T, TNumber>(this PropertyRule<T, TNumber> rule, TNumber newBase)
        where TNumber : ILogarithmicFunctions<TNumber>
        => rule.Add(TransformerRules.Log(newBase));

    /// <summary>Appends the catalogue Log rule for a base.</summary>
    public static PropertyConversion<T, TIn, TNumber> Log<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion, TNumber newBase)
        where TNumber : ILogarithmicFunctions<TNumber>
        => conversion.Then(TransformerRules.Log(newBase));

    /// <summary>Appends the catalogue Log2 rule.</summary>
    public static PropertyRule<T, TNumber> Log2<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ILogarithmicFunctions<TNumber>
        => rule.Add(TransformerRules.Log2<TNumber>());

    /// <summary>Appends the catalogue Log2 rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Log2<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ILogarithmicFunctions<TNumber>
        => conversion.Then(TransformerRules.Log2<TNumber>());

    /// <summary>Appends the catalogue Log10 rule.</summary>
    public static PropertyRule<T, TNumber> Log10<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ILogarithmicFunctions<TNumber>
        => rule.Add(TransformerRules.Log10<TNumber>());

    /// <summary>Appends the catalogue Log10 rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Log10<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ILogarithmicFunctions<TNumber>
        => conversion.Then(TransformerRules.Log10<TNumber>());

    /// <summary>Appends the catalogue Sin rule.</summary>
    public static PropertyRule<T, TNumber> Sin<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TransformerRules.Sin<TNumber>());

    /// <summary>Appends the catalogue Sin rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Sin<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ITrigonometricFunctions<TNumber>
        => conversion.Then(TransformerRules.Sin<TNumber>());

    /// <summary>Appends the catalogue Cos rule.</summary>
    public static PropertyRule<T, TNumber> Cos<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TransformerRules.Cos<TNumber>());

    /// <summary>Appends the catalogue Cos rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Cos<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ITrigonometricFunctions<TNumber>
        => conversion.Then(TransformerRules.Cos<TNumber>());

    /// <summary>Appends the catalogue Tan rule.</summary>
    public static PropertyRule<T, TNumber> Tan<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TransformerRules.Tan<TNumber>());

    /// <summary>Appends the catalogue Tan rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Tan<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ITrigonometricFunctions<TNumber>
        => conversion.Then(TransformerRules.Tan<TNumber>());

    /// <summary>Appends the catalogue Asin rule.</summary>
    public static PropertyRule<T, TNumber> Asin<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TransformerRules.Asin<TNumber>());

    /// <summary>Appends the catalogue Asin rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Asin<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ITrigonometricFunctions<TNumber>
        => conversion.Then(TransformerRules.Asin<TNumber>());

    /// <summary>Appends the catalogue Acos rule.</summary>
    public static PropertyRule<T, TNumber> Acos<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TransformerRules.Acos<TNumber>());

    /// <summary>Appends the catalogue Acos rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Acos<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ITrigonometricFunctions<TNumber>
        => conversion.Then(TransformerRules.Acos<TNumber>());

    /// <summary>Appends the catalogue Atan rule.</summary>
    public static PropertyRule<T, TNumber> Atan<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TransformerRules.Atan<TNumber>());

    /// <summary>Appends the catalogue Atan rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Atan<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ITrigonometricFunctions<TNumber>
        => conversion.Then(TransformerRules.Atan<TNumber>());

    /// <summary>Appends the catalogue DegreesToRadians rule.</summary>
    public static PropertyRule<T, TNumber> DegreesToRadians<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TransformerRules.DegreesToRadians<TNumber>());

    /// <summary>Appends the catalogue DegreesToRadians rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> DegreesToRadians<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ITrigonometricFunctions<TNumber>
        => conversion.Then(TransformerRules.DegreesToRadians<TNumber>());

    /// <summary>Appends the catalogue RadiansToDegrees rule.</summary>
    public static PropertyRule<T, TNumber> RadiansToDegrees<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TransformerRules.RadiansToDegrees<TNumber>());

    /// <summary>Appends the catalogue RadiansToDegrees rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> RadiansToDegrees<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ITrigonometricFunctions<TNumber>
        => conversion.Then(TransformerRules.RadiansToDegrees<TNumber>());

    /// <summary>Appends the catalogue Sinh rule.</summary>
    public static PropertyRule<T, TNumber> Sinh<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IHyperbolicFunctions<TNumber>
        => rule.Add(TransformerRules.Sinh<TNumber>());

    /// <summary>Appends the catalogue Sinh rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Sinh<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IHyperbolicFunctions<TNumber>
        => conversion.Then(TransformerRules.Sinh<TNumber>());

    /// <summary>Appends the catalogue Cosh rule.</summary>
    public static PropertyRule<T, TNumber> Cosh<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IHyperbolicFunctions<TNumber>
        => rule.Add(TransformerRules.Cosh<TNumber>());

    /// <summary>Appends the catalogue Cosh rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Cosh<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IHyperbolicFunctions<TNumber>
        => conversion.Then(TransformerRules.Cosh<TNumber>());

    /// <summary>Appends the catalogue Tanh rule.</summary>
    public static PropertyRule<T, TNumber> Tanh<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IHyperbolicFunctions<TNumber>
        => rule.Add(TransformerRules.Tanh<TNumber>());

    /// <summary>Appends the catalogue Tanh rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Tanh<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IHyperbolicFunctions<TNumber>
        => conversion.Then(TransformerRules.Tanh<TNumber>());

    /// <summary>Appends the catalogue Asinh rule.</summary>
    public static PropertyRule<T, TNumber> Asinh<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IHyperbolicFunctions<TNumber>
        => rule.Add(TransformerRules.Asinh<TNumber>());

    /// <summary>Appends the catalogue Asinh rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Asinh<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IHyperbolicFunctions<TNumber>
        => conversion.Then(TransformerRules.Asinh<TNumber>());

    /// <summary>Appends the catalogue Acosh rule.</summary>
    public static PropertyRule<T, TNumber> Acosh<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IHyperbolicFunctions<TNumber>
        => rule.Add(TransformerRules.Acosh<TNumber>());

    /// <summary>Appends the catalogue Acosh rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Acosh<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IHyperbolicFunctions<TNumber>
        => conversion.Then(TransformerRules.Acosh<TNumber>());

    /// <summary>Appends the catalogue Atanh rule.</summary>
    public static PropertyRule<T, TNumber> Atanh<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IHyperbolicFunctions<TNumber>
        => rule.Add(TransformerRules.Atanh<TNumber>());

    /// <summary>Appends the catalogue Atanh rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Atanh<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IHyperbolicFunctions<TNumber>
        => conversion.Then(TransformerRules.Atanh<TNumber>());
}
