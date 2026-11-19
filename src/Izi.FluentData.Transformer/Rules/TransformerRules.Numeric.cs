using System.Numerics;
using System.Reflection;

namespace Izi.FluentData.Transformer;

public static partial class TransformerRules
{
    /// <summary>
    /// Adds <paramref name="value"/> with the type's <c>+</c> operator. Integer addition is unchecked and
    /// can wrap.
    /// </summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <param name="value">Amount added to the current value. Integer addition is unchecked and can wrap.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Add<TNumber>(TNumber value)
        where TNumber : INumberBase<TNumber>
        => Same<TNumber>(source => source + value);

    /// <summary>
    /// Subtracts <paramref name="value"/> with the type's <c>-</c> operator. Integer subtraction is
    /// unchecked.
    /// </summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <param name="value">Amount subtracted from the current value. Integer subtraction is unchecked.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Subtract<TNumber>(TNumber value)
        where TNumber : INumberBase<TNumber>
        => Same<TNumber>(source => source - value);

    /// <summary>Multiplies by <paramref name="value"/>. Integer multiplication is unchecked.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <param name="value">Factor the current value is multiplied by. Integer multiplication is unchecked.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Multiply<TNumber>(TNumber value)
        where TNumber : INumberBase<TNumber>
        => Same<TNumber>(source => source * value);

    /// <summary>
    /// Divides by <paramref name="value"/> with the type's <c>/</c> operator.
    /// An integer zero divisor throws <see cref="DivideByZeroException"/> when the value is transformed.
    /// A floating-point zero follows IEEE division.
    /// </summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <param name="value">
    /// Divisor. An integer zero throws when the value is transformed. A floating-point zero follows IEEE
    /// division.
    /// </param>
    /// <returns>The rule that applies this transformation.</returns>
    /// <exception cref="DivideByZeroException">
    /// An integer divisor is zero. Thrown when the value is transformed.
    /// </exception>
    public static TransformerRule<TNumber> Divide<TNumber>(TNumber value)
        where TNumber : INumberBase<TNumber>
        => Same<TNumber>(source => source / value);

    /// <summary>
    /// Returns the absolute value. Signed integer <c>MinValue</c> saturates to <c>MaxValue</c>,
    /// the same way unchecked integer addition wraps instead of throwing.
    /// </summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Abs<TNumber>()
        where TNumber : INumberBase<TNumber>
        => Same<TNumber>(AbsDispatch<TNumber>.Apply);

    /// <summary>
    /// Clamps to the inclusive range. <paramref name="min"/> greater than <paramref name="max"/> throws
    /// <see cref="ArgumentException"/> when the rule is created.
    /// </summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <param name="min">
    /// Inclusive lower bound. A minimum greater than the maximum is rejected when the rule is created.
    /// </param>
    /// <param name="max">Inclusive upper bound.</param>
    /// <returns>The rule that applies this transformation.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="min"/> is greater than <paramref name="max"/>. Thrown when the rule is created.
    /// </exception>
    public static TransformerRule<TNumber> Clamp<TNumber>(TNumber min, TNumber max)
        where TNumber : INumber<TNumber>
    {
        // INumber<T>.Clamp throws ArgumentException when min is greater than max.
        // Probe here so a reversed range fails when the rule is created, not on the first value.
        _ = TNumber.Clamp(min, min, max);
        return Same<TNumber>(source => TNumber.Clamp(source, min, max));
    }

    /// <summary>Negates with the type's unary minus. Integer overflow follows the unchecked operator.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Invert<TNumber>()
        where TNumber : INumberBase<TNumber>
        => Same<TNumber>(source => -source);

    /// <summary>Rounds to the nearest integer with the type's default midpoint policy.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Round<TNumber>()
        where TNumber : IFloatingPoint<TNumber>
        => Same<TNumber>(source => TNumber.Round(source));

    /// <summary>Rounds to <paramref name="digits"/> fractional digits.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <param name="digits">Number of fractional digits to keep. A negative count is rejected.</param>
    /// <returns>The rule that applies this transformation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="digits"/> is outside the accepted range.
    /// </exception>
    public static TransformerRule<TNumber> Round<TNumber>(int digits)
        where TNumber : IFloatingPoint<TNumber>
    {
        RequireDigits<TNumber>(digits);
        return Same<TNumber>(source => TNumber.Round(source, digits));
    }

    /// <summary>Rounds to the nearest integer with <paramref name="mode"/>.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <param name="mode">Policy used when a value is exactly halfway between two results.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Round<TNumber>(MidpointRounding mode)
        where TNumber : IFloatingPoint<TNumber>
        => Same<TNumber>(source => TNumber.Round(source, mode));

    /// <summary>Rounds to <paramref name="digits"/> fractional digits with <paramref name="mode"/>.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <param name="digits">Number of fractional digits to keep. A negative count is rejected.</param>
    /// <param name="mode">Policy used when a value is exactly halfway between two results.</param>
    /// <returns>The rule that applies this transformation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="digits"/> is outside the accepted range.
    /// </exception>
    public static TransformerRule<TNumber> Round<TNumber>(int digits, MidpointRounding mode)
        where TNumber : IFloatingPoint<TNumber>
    {
        RequireDigits<TNumber>(digits);
        return Same<TNumber>(source => TNumber.Round(source, digits, mode));
    }

    /// <summary>Rounds toward negative infinity.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Floor<TNumber>()
        where TNumber : IFloatingPoint<TNumber>
        => Same<TNumber>(TNumber.Floor);

    /// <summary>Rounds toward positive infinity.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Ceiling<TNumber>()
        where TNumber : IFloatingPoint<TNumber>
        => Same<TNumber>(TNumber.Ceiling);

    /// <summary>
    /// Discards the fractional part toward zero. This is the numeric rule, not the string
    /// <see cref="Truncate(int)"/>.
    /// </summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Truncate<TNumber>()
        where TNumber : IFloatingPoint<TNumber>
        => Same<TNumber>(TNumber.Truncate);

    /// <summary>
    /// Maps <paramref name="min"/> to 0 and <paramref name="max"/> to 1.
    /// Equal bounds are rejected when the rule is created. Reversed bounds are kept and produce a reversed scale.
    /// When <c>max - min</c> is not finite, both sides are halved first so the span stays finite.
    /// </summary>
    /// <param name="min">Value that maps to zero. It must differ from <paramref name="max"/>.</param>
    /// <param name="max">Value that maps to one. It must differ from <paramref name="min"/>.</param>
    /// <returns>The rule that applies this transformation.</returns>
    /// <exception cref="ArgumentException">Normalize requires distinct bounds.</exception>
    public static TransformerRule<double> Normalize(double min, double max)
    {
        RequireDistinct(min, max);
        return Same<double>(source => NormalizeFloating(source, min, max));
    }

    /// <summary>
    /// Maps <paramref name="min"/> to 0 and <paramref name="max"/> to 1.
    /// Equal bounds are rejected when the rule is created. Reversed bounds are kept and produce a reversed scale.
    /// When <c>max - min</c> is not finite, both sides are halved first so the span stays finite.
    /// </summary>
    /// <param name="min">Value that maps to zero. It must differ from <paramref name="max"/>.</param>
    /// <param name="max">Value that maps to one. It must differ from <paramref name="min"/>.</param>
    /// <returns>The rule that applies this transformation.</returns>
    /// <exception cref="ArgumentException">Normalize requires distinct bounds.</exception>
    public static TransformerRule<float> Normalize(float min, float max)
    {
        RequireDistinct(min, max);
        return Same<float>(source => NormalizeFloating(source, min, max));
    }

    /// <summary>
    /// Maps <paramref name="min"/> to 0 and <paramref name="max"/> to 1.
    /// Equal bounds are rejected when the rule is created. Reversed bounds are kept and produce a reversed scale.
    /// A span that overflows <see cref="decimal"/> is computed from the halved bounds.
    /// </summary>
    /// <param name="min">Value that maps to zero. It must differ from <paramref name="max"/>.</param>
    /// <param name="max">Value that maps to one. It must differ from <paramref name="min"/>.</param>
    /// <returns>The rule that applies this transformation.</returns>
    /// <exception cref="ArgumentException">Normalize requires distinct bounds.</exception>
    public static TransformerRule<decimal> Normalize(decimal min, decimal max)
    {
        RequireDistinct(min, max);
        return Same<decimal>(source => NormalizeDecimal(source, min, max));
    }

    /// <summary>Square root.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Sqrt<TNumber>()
        where TNumber : IRootFunctions<TNumber>
        => Same<TNumber>(TNumber.Sqrt);

    /// <summary>Cube root.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Cbrt<TNumber>()
        where TNumber : IRootFunctions<TNumber>
        => Same<TNumber>(TNumber.Cbrt);

    /// <summary>Nth root. Zero is rejected when the rule is created.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <param name="n">Degree of the root. Zero is rejected when the rule is created.</param>
    /// <returns>The rule that applies this transformation.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="n"/> is zero.</exception>
    public static TransformerRule<TNumber> RootN<TNumber>(int n)
        where TNumber : IRootFunctions<TNumber>
    {
        if (n == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n));
        }

        return Same<TNumber>(source => TNumber.RootN(source, n));
    }

    /// <summary>Raises the value to <paramref name="exponent"/>.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <param name="exponent">Power the value is raised to.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Pow<TNumber>(TNumber exponent)
        where TNumber : IPowerFunctions<TNumber>
        => Same<TNumber>(source => TNumber.Pow(source, exponent));

    /// <summary>e raised to the value.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Exp<TNumber>()
        where TNumber : IExponentialFunctions<TNumber>
        => Same<TNumber>(TNumber.Exp);

    /// <summary>2 raised to the value.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Exp2<TNumber>()
        where TNumber : IExponentialFunctions<TNumber>
        => Same<TNumber>(TNumber.Exp2);

    /// <summary>10 raised to the value.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Exp10<TNumber>()
        where TNumber : IExponentialFunctions<TNumber>
        => Same<TNumber>(TNumber.Exp10);

    /// <summary>Natural logarithm.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Log<TNumber>()
        where TNumber : ILogarithmicFunctions<TNumber>
        => Same<TNumber>(source => TNumber.Log(source));

    /// <summary>Logarithm in base <paramref name="newBase"/>.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <param name="newBase">Base of the logarithm.</param>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Log<TNumber>(TNumber newBase)
        where TNumber : ILogarithmicFunctions<TNumber>
        => Same<TNumber>(source => TNumber.Log(source, newBase));

    /// <summary>Base-2 logarithm.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Log2<TNumber>()
        where TNumber : ILogarithmicFunctions<TNumber>
        => Same<TNumber>(TNumber.Log2);

    /// <summary>Base-10 logarithm.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Log10<TNumber>()
        where TNumber : ILogarithmicFunctions<TNumber>
        => Same<TNumber>(TNumber.Log10);

    /// <summary>Sine of an angle in radians.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Sin<TNumber>()
        where TNumber : ITrigonometricFunctions<TNumber>
        => Same<TNumber>(TNumber.Sin);

    /// <summary>Cosine of an angle in radians.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Cos<TNumber>()
        where TNumber : ITrigonometricFunctions<TNumber>
        => Same<TNumber>(TNumber.Cos);

    /// <summary>Tangent of an angle in radians.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Tan<TNumber>()
        where TNumber : ITrigonometricFunctions<TNumber>
        => Same<TNumber>(TNumber.Tan);

    /// <summary>Arcsine, in radians.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Asin<TNumber>()
        where TNumber : ITrigonometricFunctions<TNumber>
        => Same<TNumber>(TNumber.Asin);

    /// <summary>Arccosine, in radians.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Acos<TNumber>()
        where TNumber : ITrigonometricFunctions<TNumber>
        => Same<TNumber>(TNumber.Acos);

    /// <summary>Arctangent, in radians.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Atan<TNumber>()
        where TNumber : ITrigonometricFunctions<TNumber>
        => Same<TNumber>(TNumber.Atan);

    /// <summary>Converts degrees to radians.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> DegreesToRadians<TNumber>()
        where TNumber : ITrigonometricFunctions<TNumber>
        => Same<TNumber>(TNumber.DegreesToRadians);

    /// <summary>Converts radians to degrees.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> RadiansToDegrees<TNumber>()
        where TNumber : ITrigonometricFunctions<TNumber>
        => Same<TNumber>(TNumber.RadiansToDegrees);

    /// <summary>Hyperbolic sine.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Sinh<TNumber>()
        where TNumber : IHyperbolicFunctions<TNumber>
        => Same<TNumber>(TNumber.Sinh);

    /// <summary>Hyperbolic cosine.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Cosh<TNumber>()
        where TNumber : IHyperbolicFunctions<TNumber>
        => Same<TNumber>(TNumber.Cosh);

    /// <summary>Hyperbolic tangent.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Tanh<TNumber>()
        where TNumber : IHyperbolicFunctions<TNumber>
        => Same<TNumber>(TNumber.Tanh);

    /// <summary>Inverse hyperbolic sine.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Asinh<TNumber>()
        where TNumber : IHyperbolicFunctions<TNumber>
        => Same<TNumber>(TNumber.Asinh);

    /// <summary>Inverse hyperbolic cosine.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Acosh<TNumber>()
        where TNumber : IHyperbolicFunctions<TNumber>
        => Same<TNumber>(TNumber.Acosh);

    /// <summary>Inverse hyperbolic tangent.</summary>
    /// <typeparam name="TNumber">The numeric type. It must implement the operation this rule calls.</typeparam>
    /// <returns>The rule that applies this transformation.</returns>
    public static TransformerRule<TNumber> Atanh<TNumber>()
        where TNumber : IHyperbolicFunctions<TNumber>
        => Same<TNumber>(TNumber.Atanh);

    private static void RequireDigits<TNumber>(int digits)
    {
        // float.Round accepts 0..6 on net8.0 and net10.0. 7 throws from the BCL.
        var maximum = 15;
        if (typeof(TNumber) == typeof(float))
        {
            maximum = 6;
        }
        else if (typeof(TNumber) == typeof(decimal))
        {
            maximum = 28;
        }

        if ((uint)digits > (uint)maximum)
        {
            throw new ArgumentOutOfRangeException(nameof(digits));
        }
    }

    private static void RequireDistinct<TNumber>(TNumber min, TNumber max)
        where TNumber : INumber<TNumber>
    {
        if (min == max)
        {
            throw new ArgumentException("Normalize requires distinct bounds.", nameof(max));
        }
    }

    private static double NormalizeFloating(double source, double min, double max)
    {
        var span = max - min;
        if (double.IsFinite(span))
        {
            return (source - min) / span;
        }

        // Halve both sides when the span is not finite. Infinite bounds still produce NaN.
        return ((source / 2d) - (min / 2d)) / ((max / 2d) - (min / 2d));
    }

    private static float NormalizeFloating(float source, float min, float max)
    {
        var span = max - min;
        if (float.IsFinite(span))
        {
            return (source - min) / span;
        }

        // Halve both sides when the span is not finite. Infinite bounds still produce NaN.
        return ((source / 2f) - (min / 2f)) / ((max / 2f) - (min / 2f));
    }

    private static decimal NormalizeDecimal(decimal source, decimal min, decimal max)
    {
        // Halve until max - min fits. One step turns ±decimal.MaxValue into a finite span of MaxValue.
        for (var attempt = 0; attempt < 8; attempt++)
        {
            try
            {
                return (source - min) / (max - min);
            }
            catch (OverflowException)
            {
                source /= 2m;
                min /= 2m;
                max /= 2m;
            }
        }

        return (source - min) / (max - min);
    }

    private static class AbsDispatch<TNumber>
        where TNumber : INumberBase<TNumber>
    {
        public static readonly Func<TNumber, TNumber> Apply = Create();

        private static Func<TNumber, TNumber> Create()
        {
            if (!typeof(IMinMaxValue<>).MakeGenericType(typeof(TNumber)).IsAssignableFrom(typeof(TNumber)))
            {
                return static value => TNumber.Abs(value);
            }

            // Closed once per numeric type. The returned delegate reads MinValue and MaxValue directly.
            var bounded = typeof(BoundedAbs<>).MakeGenericType(typeof(TNumber));
            return (Func<TNumber, TNumber>)bounded
                .GetField(nameof(BoundedAbs<int>.Apply), BindingFlags.Public | BindingFlags.Static)!
                .GetValue(null)!;
        }
    }

    private static class BoundedAbs<TNumber>
        where TNumber : INumberBase<TNumber>, IMinMaxValue<TNumber>
    {
        public static readonly Func<TNumber, TNumber> Apply = Create();

        private static Func<TNumber, TNumber> Create()
        {
            // Probe Abs(MinValue) once per closed type, including Int128. Types that overflow saturate to MaxValue.
            // The chosen delegate is reused for every later call.
            var min = TNumber.MinValue;
            var max = TNumber.MaxValue;
            try
            {
                _ = TNumber.Abs(min);
            }
            catch (OverflowException)
            {
                return value => value == min ? max : TNumber.Abs(value);
            }

            return static value => TNumber.Abs(value);
        }
    }
}
