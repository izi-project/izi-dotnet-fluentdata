namespace Izi.FluentData.Transformer.Tests;

public class NumericRuleTests
{
    [Fact]
    public async Task Arithmetic_uses_the_type_operator_including_unchecked_integer_wrap()
    {
        Assert.Equal(5, await TransformerRules.Add(2).TransformAsync(3));
        Assert.Equal(int.MinValue, await TransformerRules.Add(1).TransformAsync(int.MaxValue));
        Assert.Equal(7, await TransformerRules.Subtract(3).TransformAsync(10));
        Assert.Equal(42, await TransformerRules.Multiply(7).TransformAsync(6));
        Assert.Equal(4, await TransformerRules.Divide(3).TransformAsync(12));
        Assert.Equal(double.PositiveInfinity, await TransformerRules.Divide(0d).TransformAsync(1d));
        Assert.Throws<DivideByZeroException>(() => TransformerRules.Divide(0).TransformAsync(1));
        Assert.Equal(4, await TransformerRules.Abs<int>().TransformAsync(-4));
        Assert.Throws<OverflowException>(() => TransformerRules.Abs<int>().TransformAsync(int.MinValue));
        Assert.Equal(4, await TransformerRules.Invert<int>().TransformAsync(-4));
        Assert.Equal(3, await TransformerRules.Clamp(1, 3).TransformAsync(9));
        Assert.Equal(1, await TransformerRules.Clamp(1, 3).TransformAsync(0));
        Assert.Throws<ArgumentException>(() => TransformerRules.Clamp(3, 1));
    }

    [Fact]
    public async Task Normalize_rejects_equal_bounds_and_truncates_integers()
    {
        Assert.Equal(0.5d, await TransformerRules.Normalize(0d, 10d).TransformAsync(5d));
        Assert.Equal(0, await TransformerRules.Normalize(0, 10).TransformAsync(5));
        Assert.Equal(-1d, await TransformerRules.Normalize(10d, 0d).TransformAsync(20d));
        Assert.Throws<ArgumentException>(() => TransformerRules.Normalize(2, 2));
    }

    [Fact]
    public async Task Rounding_rules_follow_the_requested_midpoint_policy()
    {
        Assert.Equal(2d, await TransformerRules.Round<double>().TransformAsync(1.5d));
        Assert.Equal(2d, await TransformerRules.Round<double>().TransformAsync(2.5d));
        Assert.Equal(1.2d, await TransformerRules.Round<double>(1).TransformAsync(1.25d));
        Assert.Equal(2d, await TransformerRules.Round<double>(MidpointRounding.AwayFromZero).TransformAsync(1.5d));
        Assert.Equal(-2d, await TransformerRules.Round<double>(MidpointRounding.AwayFromZero).TransformAsync(-1.5d));
        Assert.Equal(1.3d, await TransformerRules.Round<double>(1, MidpointRounding.AwayFromZero).TransformAsync(1.25d));
        Assert.Equal(1d, await TransformerRules.Floor<double>().TransformAsync(1.9d));
        Assert.Equal(-2d, await TransformerRules.Floor<double>().TransformAsync(-1.1d));
        Assert.Equal(2d, await TransformerRules.Ceiling<double>().TransformAsync(1.1d));
        Assert.Equal(-1d, await TransformerRules.Truncate<double>().TransformAsync(-1.9d));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.Round<double>(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.Round<double>(-1, MidpointRounding.ToEven));
    }

    [Fact]
    public async Task Roots_powers_exponents_and_logs_match_the_generic_math_operations()
    {
        Assert.Equal(3d, await TransformerRules.Sqrt<double>().TransformAsync(9d), precision: 10);
        Assert.Equal(2d, await TransformerRules.Cbrt<double>().TransformAsync(8d), precision: 10);
        Assert.Equal(4d, await TransformerRules.RootN<double>(2).TransformAsync(16d), precision: 10);
        Assert.Equal(8d, await TransformerRules.Pow<double>(3d).TransformAsync(2d), precision: 10);
        Assert.Equal(1d, await TransformerRules.Exp<double>().TransformAsync(0d), precision: 10);
        Assert.Equal(8d, await TransformerRules.Exp2<double>().TransformAsync(3d), precision: 10);
        Assert.Equal(100d, await TransformerRules.Exp10<double>().TransformAsync(2d), precision: 10);
        Assert.Equal(0d, await TransformerRules.Log<double>().TransformAsync(1d), precision: 10);
        Assert.Equal(2d, await TransformerRules.Log<double>(10d).TransformAsync(100d), precision: 10);
        Assert.Equal(3d, await TransformerRules.Log2<double>().TransformAsync(8d), precision: 10);
        Assert.Equal(2d, await TransformerRules.Log10<double>().TransformAsync(100d), precision: 10);
    }

    [Fact]
    public async Task Sqrt_of_negative_matches_documented_behaviour()
    {
        // IEEE square root does not throw: a negative, NaN, or negative-infinity input is NaN.
        Assert.True(double.IsNaN(await TransformerRules.Sqrt<double>().TransformAsync(-1d)));
        Assert.True(double.IsNaN(await TransformerRules.Sqrt<double>().TransformAsync(double.NaN)));
        Assert.True(double.IsNaN(await TransformerRules.Sqrt<double>().TransformAsync(double.NegativeInfinity)));
        Assert.True(float.IsNaN(await TransformerRules.Sqrt<float>().TransformAsync(-1f)));
        Assert.True(float.IsNaN(await TransformerRules.Sqrt<float>().TransformAsync(float.NaN)));

        Assert.Equal(2d, await TransformerRules.Sqrt<double>().TransformAsync(4d), precision: 10);
        Assert.Equal(double.PositiveInfinity, await TransformerRules.Sqrt<double>().TransformAsync(double.PositiveInfinity));
    }

    [Fact]
    public async Task Trigonometry_and_hyperbolic_rules_use_radians()
    {
        Assert.Equal(0d, await TransformerRules.Sin<double>().TransformAsync(0d), precision: 10);
        Assert.Equal(1d, await TransformerRules.Cos<double>().TransformAsync(0d), precision: 10);
        Assert.Equal(0d, await TransformerRules.Tan<double>().TransformAsync(0d), precision: 10);
        Assert.Equal(0d, await TransformerRules.Asin<double>().TransformAsync(0d), precision: 10);
        Assert.Equal(0d, await TransformerRules.Acos<double>().TransformAsync(1d), precision: 10);
        Assert.Equal(0d, await TransformerRules.Atan<double>().TransformAsync(0d), precision: 10);
        Assert.Equal(Math.PI, await TransformerRules.DegreesToRadians<double>().TransformAsync(180d), precision: 10);
        Assert.Equal(180d, await TransformerRules.RadiansToDegrees<double>().TransformAsync(Math.PI), precision: 8);
        Assert.Equal(0d, await TransformerRules.Sinh<double>().TransformAsync(0d), precision: 10);
        Assert.Equal(1d, await TransformerRules.Cosh<double>().TransformAsync(0d), precision: 10);
        Assert.Equal(0d, await TransformerRules.Tanh<double>().TransformAsync(0d), precision: 10);
        Assert.Equal(0d, await TransformerRules.Asinh<double>().TransformAsync(0d), precision: 10);
        Assert.Equal(0d, await TransformerRules.Acosh<double>().TransformAsync(1d), precision: 10);
        Assert.Equal(0d, await TransformerRules.Atanh<double>().TransformAsync(0d), precision: 10);
    }
}
