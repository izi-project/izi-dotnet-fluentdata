using System.Numerics;

namespace Izi.FluentData.Transformer;

/// <summary>
/// Fluent wrappers that append a same-type catalogue rule and return the property rule.
/// </summary>

public static partial class TransformerRuleExtensions
{
    /// <summary>Appends the catalogue numeric Add rule.</summary>
    public static PropertyRule<T, TNumber> Add<T, TNumber>(this PropertyRule<T, TNumber> rule, TNumber value)
        where TNumber : INumberBase<TNumber>
        => rule.Add(TransformerRules.Add(value));

    /// <summary>Appends the catalogue numeric Subtract rule.</summary>
    public static PropertyRule<T, TNumber> Subtract<T, TNumber>(this PropertyRule<T, TNumber> rule, TNumber value)
        where TNumber : INumberBase<TNumber>
        => rule.Add(TransformerRules.Subtract(value));

    /// <summary>Appends the catalogue Multiply rule.</summary>
    public static PropertyRule<T, TNumber> Multiply<T, TNumber>(this PropertyRule<T, TNumber> rule, TNumber value)
        where TNumber : INumberBase<TNumber>
        => rule.Add(TransformerRules.Multiply(value));

    /// <summary>Appends the catalogue Divide rule.</summary>
    public static PropertyRule<T, TNumber> Divide<T, TNumber>(this PropertyRule<T, TNumber> rule, TNumber value)
        where TNumber : INumberBase<TNumber>
        => rule.Add(TransformerRules.Divide(value));

    /// <summary>Appends the catalogue Abs rule.</summary>
    public static PropertyRule<T, TNumber> Abs<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : INumberBase<TNumber>
        => rule.Add(NumberCache<TNumber>.Abs);

    /// <summary>Appends the catalogue numeric Clamp rule.</summary>
    public static PropertyRule<T, TNumber> Clamp<T, TNumber>(this PropertyRule<T, TNumber> rule, TNumber min, TNumber max)
        where TNumber : INumber<TNumber>
        => rule.Add(TransformerRules.Clamp(min, max));

    /// <summary>Appends the catalogue Invert rule.</summary>
    public static PropertyRule<T, TNumber> Invert<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : INumberBase<TNumber>
        => rule.Add(NumberCache<TNumber>.Invert);

    /// <summary>Appends the catalogue Round rule to the nearest integer.</summary>
    public static PropertyRule<T, TNumber> Round<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IFloatingPoint<TNumber>
        => rule.Add(FloatingCache<TNumber>.Round);

    /// <summary>Appends the catalogue Round rule for a digit count.</summary>
    public static PropertyRule<T, TNumber> Round<T, TNumber>(this PropertyRule<T, TNumber> rule, int digits)
        where TNumber : IFloatingPoint<TNumber>
        => rule.Add(TransformerRules.Round<TNumber>(digits));

    /// <summary>Appends the catalogue Round rule for a midpoint mode.</summary>
    public static PropertyRule<T, TNumber> Round<T, TNumber>(this PropertyRule<T, TNumber> rule, MidpointRounding mode)
        where TNumber : IFloatingPoint<TNumber>
        => rule.Add(TransformerRules.Round<TNumber>(mode));

    /// <summary>Appends the catalogue Round rule for a digit count and midpoint mode.</summary>
    public static PropertyRule<T, TNumber> Round<T, TNumber>(this PropertyRule<T, TNumber> rule, int digits, MidpointRounding mode)
        where TNumber : IFloatingPoint<TNumber>
        => rule.Add(TransformerRules.Round<TNumber>(digits, mode));

    /// <summary>Appends the catalogue Floor rule.</summary>
    public static PropertyRule<T, TNumber> Floor<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IFloatingPoint<TNumber>
        => rule.Add(FloatingCache<TNumber>.Floor);

    /// <summary>Appends the catalogue Ceiling rule.</summary>
    public static PropertyRule<T, TNumber> Ceiling<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IFloatingPoint<TNumber>
        => rule.Add(FloatingCache<TNumber>.Ceiling);

    /// <summary>Appends the catalogue numeric Truncate rule.</summary>
    public static PropertyRule<T, TNumber> Truncate<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IFloatingPoint<TNumber>
        => rule.Add(FloatingCache<TNumber>.Truncate);

    /// <summary>Appends the catalogue Normalize rule.</summary>
    public static PropertyRule<T, TNumber> Normalize<T, TNumber>(this PropertyRule<T, TNumber> rule, TNumber min, TNumber max)
        where TNumber : INumber<TNumber>
        => rule.Add(TransformerRules.Normalize(min, max));

    /// <summary>Appends the catalogue Sqrt rule.</summary>
    public static PropertyRule<T, TNumber> Sqrt<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IRootFunctions<TNumber>
        => rule.Add(RootCache<TNumber>.Sqrt);

    /// <summary>Appends the catalogue Cbrt rule.</summary>
    public static PropertyRule<T, TNumber> Cbrt<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IRootFunctions<TNumber>
        => rule.Add(RootCache<TNumber>.Cbrt);

    /// <summary>Appends the catalogue RootN rule.</summary>
    public static PropertyRule<T, TNumber> RootN<T, TNumber>(this PropertyRule<T, TNumber> rule, int n)
        where TNumber : IRootFunctions<TNumber>
        => rule.Add(TransformerRules.RootN<TNumber>(n));

    /// <summary>Appends the catalogue Pow rule.</summary>
    public static PropertyRule<T, TNumber> Pow<T, TNumber>(this PropertyRule<T, TNumber> rule, TNumber exponent)
        where TNumber : IPowerFunctions<TNumber>
        => rule.Add(TransformerRules.Pow(exponent));

    /// <summary>Appends the catalogue Exp rule.</summary>
    public static PropertyRule<T, TNumber> Exp<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IExponentialFunctions<TNumber>
        => rule.Add(ExpCache<TNumber>.Exp);

    /// <summary>Appends the catalogue Exp2 rule.</summary>
    public static PropertyRule<T, TNumber> Exp2<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IExponentialFunctions<TNumber>
        => rule.Add(ExpCache<TNumber>.Exp2);

    /// <summary>Appends the catalogue Exp10 rule.</summary>
    public static PropertyRule<T, TNumber> Exp10<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IExponentialFunctions<TNumber>
        => rule.Add(ExpCache<TNumber>.Exp10);

    /// <summary>Appends the catalogue natural Log rule.</summary>
    public static PropertyRule<T, TNumber> Log<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ILogarithmicFunctions<TNumber>
        => rule.Add(LogCache<TNumber>.Log);

    /// <summary>Appends the catalogue Log rule for a base.</summary>
    public static PropertyRule<T, TNumber> Log<T, TNumber>(this PropertyRule<T, TNumber> rule, TNumber newBase)
        where TNumber : ILogarithmicFunctions<TNumber>
        => rule.Add(TransformerRules.Log(newBase));

    /// <summary>Appends the catalogue Log2 rule.</summary>
    public static PropertyRule<T, TNumber> Log2<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ILogarithmicFunctions<TNumber>
        => rule.Add(LogCache<TNumber>.Log2);

    /// <summary>Appends the catalogue Log10 rule.</summary>
    public static PropertyRule<T, TNumber> Log10<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ILogarithmicFunctions<TNumber>
        => rule.Add(LogCache<TNumber>.Log10);

    /// <summary>Appends the catalogue Sin rule.</summary>
    public static PropertyRule<T, TNumber> Sin<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TrigCache<TNumber>.Sin);

    /// <summary>Appends the catalogue Cos rule.</summary>
    public static PropertyRule<T, TNumber> Cos<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TrigCache<TNumber>.Cos);

    /// <summary>Appends the catalogue Tan rule.</summary>
    public static PropertyRule<T, TNumber> Tan<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TrigCache<TNumber>.Tan);

    /// <summary>Appends the catalogue Asin rule.</summary>
    public static PropertyRule<T, TNumber> Asin<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TrigCache<TNumber>.Asin);

    /// <summary>Appends the catalogue Acos rule.</summary>
    public static PropertyRule<T, TNumber> Acos<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TrigCache<TNumber>.Acos);

    /// <summary>Appends the catalogue Atan rule.</summary>
    public static PropertyRule<T, TNumber> Atan<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TrigCache<TNumber>.Atan);

    /// <summary>Appends the catalogue DegreesToRadians rule.</summary>
    public static PropertyRule<T, TNumber> DegreesToRadians<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TrigCache<TNumber>.DegreesToRadians);

    /// <summary>Appends the catalogue RadiansToDegrees rule.</summary>
    public static PropertyRule<T, TNumber> RadiansToDegrees<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : ITrigonometricFunctions<TNumber>
        => rule.Add(TrigCache<TNumber>.RadiansToDegrees);

    /// <summary>Appends the catalogue Sinh rule.</summary>
    public static PropertyRule<T, TNumber> Sinh<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IHyperbolicFunctions<TNumber>
        => rule.Add(HypCache<TNumber>.Sinh);

    /// <summary>Appends the catalogue Cosh rule.</summary>
    public static PropertyRule<T, TNumber> Cosh<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IHyperbolicFunctions<TNumber>
        => rule.Add(HypCache<TNumber>.Cosh);

    /// <summary>Appends the catalogue Tanh rule.</summary>
    public static PropertyRule<T, TNumber> Tanh<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IHyperbolicFunctions<TNumber>
        => rule.Add(HypCache<TNumber>.Tanh);

    /// <summary>Appends the catalogue Asinh rule.</summary>
    public static PropertyRule<T, TNumber> Asinh<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IHyperbolicFunctions<TNumber>
        => rule.Add(HypCache<TNumber>.Asinh);

    /// <summary>Appends the catalogue Acosh rule.</summary>
    public static PropertyRule<T, TNumber> Acosh<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IHyperbolicFunctions<TNumber>
        => rule.Add(HypCache<TNumber>.Acosh);

    /// <summary>Appends the catalogue Atanh rule.</summary>
    public static PropertyRule<T, TNumber> Atanh<T, TNumber>(this PropertyRule<T, TNumber> rule)
        where TNumber : IHyperbolicFunctions<TNumber>
        => rule.Add(HypCache<TNumber>.Atanh);

    private static class NumberCache<TNumber>
        where TNumber : INumberBase<TNumber>
    {
        public static readonly TransformerRule<TNumber> Abs = TransformerRules.Abs<TNumber>();
        public static readonly TransformerRule<TNumber> Invert = TransformerRules.Invert<TNumber>();
    }

    private static class FloatingCache<TNumber>
        where TNumber : IFloatingPoint<TNumber>
    {
        public static readonly TransformerRule<TNumber> Round = TransformerRules.Round<TNumber>();
        public static readonly TransformerRule<TNumber> Floor = TransformerRules.Floor<TNumber>();
        public static readonly TransformerRule<TNumber> Ceiling = TransformerRules.Ceiling<TNumber>();
        public static readonly TransformerRule<TNumber> Truncate = TransformerRules.Truncate<TNumber>();
    }

    private static class RootCache<TNumber>
        where TNumber : IRootFunctions<TNumber>
    {
        public static readonly TransformerRule<TNumber> Sqrt = TransformerRules.Sqrt<TNumber>();
        public static readonly TransformerRule<TNumber> Cbrt = TransformerRules.Cbrt<TNumber>();
    }

    private static class ExpCache<TNumber>
        where TNumber : IExponentialFunctions<TNumber>
    {
        public static readonly TransformerRule<TNumber> Exp = TransformerRules.Exp<TNumber>();
        public static readonly TransformerRule<TNumber> Exp2 = TransformerRules.Exp2<TNumber>();
        public static readonly TransformerRule<TNumber> Exp10 = TransformerRules.Exp10<TNumber>();
    }

    private static class LogCache<TNumber>
        where TNumber : ILogarithmicFunctions<TNumber>
    {
        public static readonly TransformerRule<TNumber> Log = TransformerRules.Log<TNumber>();
        public static readonly TransformerRule<TNumber> Log2 = TransformerRules.Log2<TNumber>();
        public static readonly TransformerRule<TNumber> Log10 = TransformerRules.Log10<TNumber>();
    }

    private static class TrigCache<TNumber>
        where TNumber : ITrigonometricFunctions<TNumber>
    {
        public static readonly TransformerRule<TNumber> Sin = TransformerRules.Sin<TNumber>();
        public static readonly TransformerRule<TNumber> Cos = TransformerRules.Cos<TNumber>();
        public static readonly TransformerRule<TNumber> Tan = TransformerRules.Tan<TNumber>();
        public static readonly TransformerRule<TNumber> Asin = TransformerRules.Asin<TNumber>();
        public static readonly TransformerRule<TNumber> Acos = TransformerRules.Acos<TNumber>();
        public static readonly TransformerRule<TNumber> Atan = TransformerRules.Atan<TNumber>();
        public static readonly TransformerRule<TNumber> DegreesToRadians = TransformerRules.DegreesToRadians<TNumber>();
        public static readonly TransformerRule<TNumber> RadiansToDegrees = TransformerRules.RadiansToDegrees<TNumber>();
    }

    private static class HypCache<TNumber>
        where TNumber : IHyperbolicFunctions<TNumber>
    {
        public static readonly TransformerRule<TNumber> Sinh = TransformerRules.Sinh<TNumber>();
        public static readonly TransformerRule<TNumber> Cosh = TransformerRules.Cosh<TNumber>();
        public static readonly TransformerRule<TNumber> Tanh = TransformerRules.Tanh<TNumber>();
        public static readonly TransformerRule<TNumber> Asinh = TransformerRules.Asinh<TNumber>();
        public static readonly TransformerRule<TNumber> Acosh = TransformerRules.Acosh<TNumber>();
        public static readonly TransformerRule<TNumber> Atanh = TransformerRules.Atanh<TNumber>();
    }
}
