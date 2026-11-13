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
        => rule.Add(NumberCache<TNumber>.Abs);

    /// <summary>Appends the catalogue Abs rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Abs<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : INumberBase<TNumber>
        => conversion.Then(NumberCache<TNumber>.Abs);

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
        => rule.Add(NumberCache<TNumber>.Invert);

    /// <summary>Appends the catalogue Invert rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Invert<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : INumberBase<TNumber>
        => conversion.Then(NumberCache<TNumber>.Invert);

    /// <summary>Appends the catalogue Round rule to the nearest integer.</summary>
    public static PropertyRule<T, TNumber> Round<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IFloatingPoint<TNumber>
        => rule.Add(FloatingCache<TNumber>.Round);

    /// <summary>Appends the catalogue Round rule to the nearest integer.</summary>
    public static PropertyConversion<T, TIn, TNumber> Round<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IFloatingPoint<TNumber>
        => conversion.Then(FloatingCache<TNumber>.Round);

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
        => rule.Add(FloatingCache<TNumber>.Floor);

    /// <summary>Appends the catalogue Floor rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Floor<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IFloatingPoint<TNumber>
        => conversion.Then(FloatingCache<TNumber>.Floor);

    /// <summary>Appends the catalogue Ceiling rule.</summary>
    public static PropertyRule<T, TNumber> Ceiling<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IFloatingPoint<TNumber>
        => rule.Add(FloatingCache<TNumber>.Ceiling);

    /// <summary>Appends the catalogue Ceiling rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Ceiling<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IFloatingPoint<TNumber>
        => conversion.Then(FloatingCache<TNumber>.Ceiling);

    /// <summary>Appends the catalogue numeric Truncate rule.</summary>
    public static PropertyRule<T, TNumber> Truncate<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IFloatingPoint<TNumber>
        => rule.Add(FloatingCache<TNumber>.Truncate);

    /// <summary>Appends the catalogue numeric Truncate rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Truncate<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IFloatingPoint<TNumber>
        => conversion.Then(FloatingCache<TNumber>.Truncate);

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
        => rule.Add(RootCache<TNumber>.Sqrt);

    /// <summary>Appends the catalogue Sqrt rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Sqrt<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IRootFunctions<TNumber>
        => conversion.Then(RootCache<TNumber>.Sqrt);

    /// <summary>Appends the catalogue Cbrt rule.</summary>
    public static PropertyRule<T, TNumber> Cbrt<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IRootFunctions<TNumber>
        => rule.Add(RootCache<TNumber>.Cbrt);

    /// <summary>Appends the catalogue Cbrt rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Cbrt<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IRootFunctions<TNumber>
        => conversion.Then(RootCache<TNumber>.Cbrt);

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
        => rule.Add(ExpCache<TNumber>.Exp);

    /// <summary>Appends the catalogue Exp rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Exp<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IExponentialFunctions<TNumber>
        => conversion.Then(ExpCache<TNumber>.Exp);

    /// <summary>Appends the catalogue Exp2 rule.</summary>
    public static PropertyRule<T, TNumber> Exp2<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IExponentialFunctions<TNumber>
        => rule.Add(ExpCache<TNumber>.Exp2);

    /// <summary>Appends the catalogue Exp2 rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Exp2<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IExponentialFunctions<TNumber>
        => conversion.Then(ExpCache<TNumber>.Exp2);

    /// <summary>Appends the catalogue Exp10 rule.</summary>
    public static PropertyRule<T, TNumber> Exp10<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IExponentialFunctions<TNumber>
        => rule.Add(ExpCache<TNumber>.Exp10);

    /// <summary>Appends the catalogue Exp10 rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Exp10<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IExponentialFunctions<TNumber>
        => conversion.Then(ExpCache<TNumber>.Exp10);

    /// <summary>Appends the catalogue natural Log rule.</summary>
    public static PropertyRule<T, TNumber> Log<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ILogarithmicFunctions<TNumber>
        => rule.Add(LogCache<TNumber>.Log);

    /// <summary>Appends the catalogue natural Log rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Log<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ILogarithmicFunctions<TNumber>
        => conversion.Then(LogCache<TNumber>.Log);

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
        => rule.Add(LogCache<TNumber>.Log2);

    /// <summary>Appends the catalogue Log2 rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Log2<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ILogarithmicFunctions<TNumber>
        => conversion.Then(LogCache<TNumber>.Log2);

    /// <summary>Appends the catalogue Log10 rule.</summary>
    public static PropertyRule<T, TNumber> Log10<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ILogarithmicFunctions<TNumber>
        => rule.Add(LogCache<TNumber>.Log10);

    /// <summary>Appends the catalogue Log10 rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Log10<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ILogarithmicFunctions<TNumber>
        => conversion.Then(LogCache<TNumber>.Log10);

    /// <summary>Appends the catalogue Sin rule.</summary>
    public static PropertyRule<T, TNumber> Sin<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TrigCache<TNumber>.Sin);

    /// <summary>Appends the catalogue Sin rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Sin<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ITrigonometricFunctions<TNumber>
        => conversion.Then(TrigCache<TNumber>.Sin);

    /// <summary>Appends the catalogue Cos rule.</summary>
    public static PropertyRule<T, TNumber> Cos<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TrigCache<TNumber>.Cos);

    /// <summary>Appends the catalogue Cos rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Cos<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ITrigonometricFunctions<TNumber>
        => conversion.Then(TrigCache<TNumber>.Cos);

    /// <summary>Appends the catalogue Tan rule.</summary>
    public static PropertyRule<T, TNumber> Tan<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TrigCache<TNumber>.Tan);

    /// <summary>Appends the catalogue Tan rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Tan<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ITrigonometricFunctions<TNumber>
        => conversion.Then(TrigCache<TNumber>.Tan);

    /// <summary>Appends the catalogue Asin rule.</summary>
    public static PropertyRule<T, TNumber> Asin<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TrigCache<TNumber>.Asin);

    /// <summary>Appends the catalogue Asin rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Asin<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ITrigonometricFunctions<TNumber>
        => conversion.Then(TrigCache<TNumber>.Asin);

    /// <summary>Appends the catalogue Acos rule.</summary>
    public static PropertyRule<T, TNumber> Acos<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TrigCache<TNumber>.Acos);

    /// <summary>Appends the catalogue Acos rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Acos<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ITrigonometricFunctions<TNumber>
        => conversion.Then(TrigCache<TNumber>.Acos);

    /// <summary>Appends the catalogue Atan rule.</summary>
    public static PropertyRule<T, TNumber> Atan<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TrigCache<TNumber>.Atan);

    /// <summary>Appends the catalogue Atan rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Atan<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ITrigonometricFunctions<TNumber>
        => conversion.Then(TrigCache<TNumber>.Atan);

    /// <summary>Appends the catalogue DegreesToRadians rule.</summary>
    public static PropertyRule<T, TNumber> DegreesToRadians<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TrigCache<TNumber>.DegreesToRadians);

    /// <summary>Appends the catalogue DegreesToRadians rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> DegreesToRadians<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ITrigonometricFunctions<TNumber>
        => conversion.Then(TrigCache<TNumber>.DegreesToRadians);

    /// <summary>Appends the catalogue RadiansToDegrees rule.</summary>
    public static PropertyRule<T, TNumber> RadiansToDegrees<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TrigCache<TNumber>.RadiansToDegrees);

    /// <summary>Appends the catalogue RadiansToDegrees rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> RadiansToDegrees<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : ITrigonometricFunctions<TNumber>
        => conversion.Then(TrigCache<TNumber>.RadiansToDegrees);

    /// <summary>Appends the catalogue Sinh rule.</summary>
    public static PropertyRule<T, TNumber> Sinh<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IHyperbolicFunctions<TNumber>
        => rule.Add(HypCache<TNumber>.Sinh);

    /// <summary>Appends the catalogue Sinh rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Sinh<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IHyperbolicFunctions<TNumber>
        => conversion.Then(HypCache<TNumber>.Sinh);

    /// <summary>Appends the catalogue Cosh rule.</summary>
    public static PropertyRule<T, TNumber> Cosh<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IHyperbolicFunctions<TNumber>
        => rule.Add(HypCache<TNumber>.Cosh);

    /// <summary>Appends the catalogue Cosh rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Cosh<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IHyperbolicFunctions<TNumber>
        => conversion.Then(HypCache<TNumber>.Cosh);

    /// <summary>Appends the catalogue Tanh rule.</summary>
    public static PropertyRule<T, TNumber> Tanh<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IHyperbolicFunctions<TNumber>
        => rule.Add(HypCache<TNumber>.Tanh);

    /// <summary>Appends the catalogue Tanh rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Tanh<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IHyperbolicFunctions<TNumber>
        => conversion.Then(HypCache<TNumber>.Tanh);

    /// <summary>Appends the catalogue Asinh rule.</summary>
    public static PropertyRule<T, TNumber> Asinh<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IHyperbolicFunctions<TNumber>
        => rule.Add(HypCache<TNumber>.Asinh);

    /// <summary>Appends the catalogue Asinh rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Asinh<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IHyperbolicFunctions<TNumber>
        => conversion.Then(HypCache<TNumber>.Asinh);

    /// <summary>Appends the catalogue Acosh rule.</summary>
    public static PropertyRule<T, TNumber> Acosh<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IHyperbolicFunctions<TNumber>
        => rule.Add(HypCache<TNumber>.Acosh);

    /// <summary>Appends the catalogue Acosh rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Acosh<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IHyperbolicFunctions<TNumber>
        => conversion.Then(HypCache<TNumber>.Acosh);

    /// <summary>Appends the catalogue Atanh rule.</summary>
    public static PropertyRule<T, TNumber> Atanh<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IHyperbolicFunctions<TNumber>
        => rule.Add(HypCache<TNumber>.Atanh);

    /// <summary>Appends the catalogue Atanh rule.</summary>
    public static PropertyConversion<T, TIn, TNumber> Atanh<T, TIn, TNumber>(this PropertyConversion<T, TIn, TNumber> conversion)
        where TNumber : IHyperbolicFunctions<TNumber>
        => conversion.Then(HypCache<TNumber>.Atanh);

    private static class NumberCache<TNumber>
        where TNumber : INumberBase<TNumber>
    {
        public static readonly TransformerRule<TNumber, TNumber> Abs = TransformerRules.Abs<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Invert = TransformerRules.Invert<TNumber>();
    }

    private static class FloatingCache<TNumber>
        where TNumber : IFloatingPoint<TNumber>
    {
        public static readonly TransformerRule<TNumber, TNumber> Round = TransformerRules.Round<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Floor = TransformerRules.Floor<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Ceiling = TransformerRules.Ceiling<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Truncate = TransformerRules.Truncate<TNumber>();
    }

    private static class RootCache<TNumber>
        where TNumber : IRootFunctions<TNumber>
    {
        public static readonly TransformerRule<TNumber, TNumber> Sqrt = TransformerRules.Sqrt<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Cbrt = TransformerRules.Cbrt<TNumber>();
    }

    private static class ExpCache<TNumber>
        where TNumber : IExponentialFunctions<TNumber>
    {
        public static readonly TransformerRule<TNumber, TNumber> Exp = TransformerRules.Exp<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Exp2 = TransformerRules.Exp2<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Exp10 = TransformerRules.Exp10<TNumber>();
    }

    private static class LogCache<TNumber>
        where TNumber : ILogarithmicFunctions<TNumber>
    {
        public static readonly TransformerRule<TNumber, TNumber> Log = TransformerRules.Log<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Log2 = TransformerRules.Log2<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Log10 = TransformerRules.Log10<TNumber>();
    }

    private static class TrigCache<TNumber>
        where TNumber : ITrigonometricFunctions<TNumber>
    {
        public static readonly TransformerRule<TNumber, TNumber> Sin = TransformerRules.Sin<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Cos = TransformerRules.Cos<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Tan = TransformerRules.Tan<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Asin = TransformerRules.Asin<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Acos = TransformerRules.Acos<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Atan = TransformerRules.Atan<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> DegreesToRadians = TransformerRules.DegreesToRadians<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> RadiansToDegrees = TransformerRules.RadiansToDegrees<TNumber>();
    }

    private static class HypCache<TNumber>
        where TNumber : IHyperbolicFunctions<TNumber>
    {
        public static readonly TransformerRule<TNumber, TNumber> Sinh = TransformerRules.Sinh<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Cosh = TransformerRules.Cosh<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Tanh = TransformerRules.Tanh<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Asinh = TransformerRules.Asinh<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Acosh = TransformerRules.Acosh<TNumber>();
        public static readonly TransformerRule<TNumber, TNumber> Atanh = TransformerRules.Atanh<TNumber>();
    }
}
