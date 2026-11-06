using System.Numerics;

namespace Izi.FluentData.Transformer;

public static partial class TransformerRules
{
    /// <summary>Adds <paramref name="value"/> with the type's <c>+</c> operator. Integer addition is unchecked and can wrap.</summary>
    public static TransformerRule<TNumber, TNumber> Add<TNumber>(TNumber value)
        where TNumber : INumberBase<TNumber>
        => Same<TNumber>(source => source + value);

    /// <summary>Subtracts <paramref name="value"/> with the type's <c>-</c> operator. Integer subtraction is unchecked.</summary>
    public static TransformerRule<TNumber, TNumber> Subtract<TNumber>(TNumber value)
        where TNumber : INumberBase<TNumber>
        => Same<TNumber>(source => source - value);

    /// <summary>Multiplies by <paramref name="value"/>. Integer multiplication is unchecked.</summary>
    public static TransformerRule<TNumber, TNumber> Multiply<TNumber>(TNumber value)
        where TNumber : INumberBase<TNumber>
        => Same<TNumber>(source => source * value);

    /// <summary>
    /// Divides by <paramref name="value"/> with the type's <c>/</c> operator.
    /// An integer zero divisor throws <see cref="DivideByZeroException"/> when the value is transformed.
    /// A floating-point zero follows IEEE division.
    /// </summary>
    public static TransformerRule<TNumber, TNumber> Divide<TNumber>(TNumber value)
        where TNumber : INumberBase<TNumber>
        => Same<TNumber>(source => source / value);

    /// <summary>Returns <see cref="INumberBase{TSelf}.Abs"/>. <see cref="int.MinValue"/> throws <see cref="OverflowException"/>.</summary>
    public static TransformerRule<TNumber, TNumber> Abs<TNumber>()
        where TNumber : INumberBase<TNumber>
        => Same<TNumber>(TNumber.Abs);

    /// <summary>
    /// Clamps to the inclusive range. <paramref name="min"/> greater than <paramref name="max"/> throws
    /// <see cref="ArgumentException"/> when the rule is created.
    /// </summary>
    public static TransformerRule<TNumber, TNumber> Clamp<TNumber>(TNumber min, TNumber max)
        where TNumber : INumber<TNumber>
    {
        _ = TNumber.Clamp(min, min, max);
        return Same<TNumber>(source => TNumber.Clamp(source, min, max));
    }

    /// <summary>Negates with the type's unary minus. Integer overflow follows the unchecked operator.</summary>
    public static TransformerRule<TNumber, TNumber> Invert<TNumber>()
        where TNumber : INumberBase<TNumber>
        => Same<TNumber>(source => -source);

    /// <summary>Rounds to the nearest integer with the type's default midpoint policy.</summary>
    public static TransformerRule<TNumber, TNumber> Round<TNumber>()
        where TNumber : IFloatingPoint<TNumber>
        => Same<TNumber>(source => TNumber.Round(source));

    /// <summary>Rounds to <paramref name="digits"/> fractional digits.</summary>
    public static TransformerRule<TNumber, TNumber> Round<TNumber>(int digits)
        where TNumber : IFloatingPoint<TNumber>
    {
        if (digits < 0)
            throw new ArgumentOutOfRangeException(nameof(digits));

        return Same<TNumber>(source => TNumber.Round(source, digits));
    }

    /// <summary>Rounds to the nearest integer with <paramref name="mode"/>.</summary>
    public static TransformerRule<TNumber, TNumber> Round<TNumber>(MidpointRounding mode)
        where TNumber : IFloatingPoint<TNumber>
        => Same<TNumber>(source => TNumber.Round(source, mode));

    /// <summary>Rounds to <paramref name="digits"/> fractional digits with <paramref name="mode"/>.</summary>
    public static TransformerRule<TNumber, TNumber> Round<TNumber>(int digits, MidpointRounding mode)
        where TNumber : IFloatingPoint<TNumber>
    {
        if (digits < 0)
            throw new ArgumentOutOfRangeException(nameof(digits));

        return Same<TNumber>(source => TNumber.Round(source, digits, mode));
    }

    /// <summary>Rounds toward negative infinity.</summary>
    public static TransformerRule<TNumber, TNumber> Floor<TNumber>()
        where TNumber : IFloatingPoint<TNumber>
        => Same<TNumber>(TNumber.Floor);

    /// <summary>Rounds toward positive infinity.</summary>
    public static TransformerRule<TNumber, TNumber> Ceiling<TNumber>()
        where TNumber : IFloatingPoint<TNumber>
        => Same<TNumber>(TNumber.Ceiling);

    /// <summary>Discards the fractional part toward zero. This is the numeric rule, not the string <see cref="Truncate(int)"/>.</summary>
    public static TransformerRule<TNumber, TNumber> Truncate<TNumber>()
        where TNumber : IFloatingPoint<TNumber>
        => Same<TNumber>(TNumber.Truncate);

    /// <summary>
    /// Maps <paramref name="min"/> to 0 and <paramref name="max"/> to 1 with <c>(source - min) / (max - min)</c>.
    /// Equal bounds are rejected when the rule is created. Reversed bounds are kept and produce a reversed scale.
    /// Integer division truncates toward zero.
    /// </summary>
    public static TransformerRule<TNumber, TNumber> Normalize<TNumber>(TNumber min, TNumber max)
        where TNumber : INumber<TNumber>
    {
        if (min == max)
            throw new ArgumentException("Normalize requires distinct bounds.", nameof(max));

        return Same<TNumber>(source => (source - min) / (max - min));
    }

    /// <summary>Square root.</summary>
    public static TransformerRule<TNumber, TNumber> Sqrt<TNumber>()
        where TNumber : IRootFunctions<TNumber>
        => Same<TNumber>(TNumber.Sqrt);

    /// <summary>Cube root.</summary>
    public static TransformerRule<TNumber, TNumber> Cbrt<TNumber>()
        where TNumber : IRootFunctions<TNumber>
        => Same<TNumber>(TNumber.Cbrt);

    /// <summary>Nth root. A zero or negative <paramref name="n"/> follows the BCL and can throw when the value is transformed.</summary>
    public static TransformerRule<TNumber, TNumber> RootN<TNumber>(int n)
        where TNumber : IRootFunctions<TNumber>
        => Same<TNumber>(source => TNumber.RootN(source, n));

    /// <summary>Raises the value to <paramref name="exponent"/>.</summary>
    public static TransformerRule<TNumber, TNumber> Pow<TNumber>(TNumber exponent)
        where TNumber : IPowerFunctions<TNumber>
        => Same<TNumber>(source => TNumber.Pow(source, exponent));

    /// <summary>e raised to the value.</summary>
    public static TransformerRule<TNumber, TNumber> Exp<TNumber>()
        where TNumber : IExponentialFunctions<TNumber>
        => Same<TNumber>(TNumber.Exp);

    /// <summary>2 raised to the value.</summary>
    public static TransformerRule<TNumber, TNumber> Exp2<TNumber>()
        where TNumber : IExponentialFunctions<TNumber>
        => Same<TNumber>(TNumber.Exp2);

    /// <summary>10 raised to the value.</summary>
    public static TransformerRule<TNumber, TNumber> Exp10<TNumber>()
        where TNumber : IExponentialFunctions<TNumber>
        => Same<TNumber>(TNumber.Exp10);

    /// <summary>Natural logarithm.</summary>
    public static TransformerRule<TNumber, TNumber> Log<TNumber>()
        where TNumber : ILogarithmicFunctions<TNumber>
        => Same<TNumber>(source => TNumber.Log(source));

    /// <summary>Logarithm in base <paramref name="newBase"/>.</summary>
    public static TransformerRule<TNumber, TNumber> Log<TNumber>(TNumber newBase)
        where TNumber : ILogarithmicFunctions<TNumber>
        => Same<TNumber>(source => TNumber.Log(source, newBase));

    /// <summary>Base-2 logarithm.</summary>
    public static TransformerRule<TNumber, TNumber> Log2<TNumber>()
        where TNumber : ILogarithmicFunctions<TNumber>
        => Same<TNumber>(TNumber.Log2);

    /// <summary>Base-10 logarithm.</summary>
    public static TransformerRule<TNumber, TNumber> Log10<TNumber>()
        where TNumber : ILogarithmicFunctions<TNumber>
        => Same<TNumber>(TNumber.Log10);

    /// <summary>Sine of an angle in radians.</summary>
    public static TransformerRule<TNumber, TNumber> Sin<TNumber>()
        where TNumber : ITrigonometricFunctions<TNumber>
        => Same<TNumber>(TNumber.Sin);

    /// <summary>Cosine of an angle in radians.</summary>
    public static TransformerRule<TNumber, TNumber> Cos<TNumber>()
        where TNumber : ITrigonometricFunctions<TNumber>
        => Same<TNumber>(TNumber.Cos);

    /// <summary>Tangent of an angle in radians.</summary>
    public static TransformerRule<TNumber, TNumber> Tan<TNumber>()
        where TNumber : ITrigonometricFunctions<TNumber>
        => Same<TNumber>(TNumber.Tan);

    /// <summary>Arcsine, in radians.</summary>
    public static TransformerRule<TNumber, TNumber> Asin<TNumber>()
        where TNumber : ITrigonometricFunctions<TNumber>
        => Same<TNumber>(TNumber.Asin);

    /// <summary>Arccosine, in radians.</summary>
    public static TransformerRule<TNumber, TNumber> Acos<TNumber>()
        where TNumber : ITrigonometricFunctions<TNumber>
        => Same<TNumber>(TNumber.Acos);

    /// <summary>Arctangent, in radians.</summary>
    public static TransformerRule<TNumber, TNumber> Atan<TNumber>()
        where TNumber : ITrigonometricFunctions<TNumber>
        => Same<TNumber>(TNumber.Atan);

    /// <summary>Converts degrees to radians.</summary>
    public static TransformerRule<TNumber, TNumber> DegreesToRadians<TNumber>()
        where TNumber : ITrigonometricFunctions<TNumber>
        => Same<TNumber>(TNumber.DegreesToRadians);

    /// <summary>Converts radians to degrees.</summary>
    public static TransformerRule<TNumber, TNumber> RadiansToDegrees<TNumber>()
        where TNumber : ITrigonometricFunctions<TNumber>
        => Same<TNumber>(TNumber.RadiansToDegrees);

    /// <summary>Hyperbolic sine.</summary>
    public static TransformerRule<TNumber, TNumber> Sinh<TNumber>()
        where TNumber : IHyperbolicFunctions<TNumber>
        => Same<TNumber>(TNumber.Sinh);

    /// <summary>Hyperbolic cosine.</summary>
    public static TransformerRule<TNumber, TNumber> Cosh<TNumber>()
        where TNumber : IHyperbolicFunctions<TNumber>
        => Same<TNumber>(TNumber.Cosh);

    /// <summary>Hyperbolic tangent.</summary>
    public static TransformerRule<TNumber, TNumber> Tanh<TNumber>()
        where TNumber : IHyperbolicFunctions<TNumber>
        => Same<TNumber>(TNumber.Tanh);

    /// <summary>Inverse hyperbolic sine.</summary>
    public static TransformerRule<TNumber, TNumber> Asinh<TNumber>()
        where TNumber : IHyperbolicFunctions<TNumber>
        => Same<TNumber>(TNumber.Asinh);

    /// <summary>Inverse hyperbolic cosine.</summary>
    public static TransformerRule<TNumber, TNumber> Acosh<TNumber>()
        where TNumber : IHyperbolicFunctions<TNumber>
        => Same<TNumber>(TNumber.Acosh);

    /// <summary>Inverse hyperbolic tangent.</summary>
    public static TransformerRule<TNumber, TNumber> Atanh<TNumber>()
        where TNumber : IHyperbolicFunctions<TNumber>
        => Same<TNumber>(TNumber.Atanh);
}
