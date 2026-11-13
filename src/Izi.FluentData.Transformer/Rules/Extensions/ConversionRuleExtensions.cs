using System.Globalization;
using System.Numerics;
using System.Text;

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
    /// <summary>Converts with the catalogue Parse rule for int and the invariant culture.</summary>
    public static PropertyConversion<T, string, int> ParseInt<T>(this PropertyRule<T, string> rule)
        => rule.Convert(TransformerRules.Parse<int>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for int and the invariant culture.</summary>
    public static PropertyConversion<T, TIn, int> ParseInt<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(TransformerRules.Parse<int>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for int and a format provider.</summary>
    public static PropertyConversion<T, string, int> ParseInt<T>(this PropertyRule<T, string> rule, IFormatProvider provider)
        => rule.Convert(TransformerRules.Parse<int>(provider));

    /// <summary>Converts with the catalogue Parse rule for int and a format provider.</summary>
    public static PropertyConversion<T, TIn, int> ParseInt<T, TIn>(this PropertyConversion<T, TIn, string> conversion, IFormatProvider provider)
        => conversion.Then(TransformerRules.Parse<int>(provider));

    /// <summary>Converts with the catalogue Parse rule for long and the invariant culture.</summary>
    public static PropertyConversion<T, string, long> ParseLong<T>(this PropertyRule<T, string> rule)
        => rule.Convert(TransformerRules.Parse<long>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for long and the invariant culture.</summary>
    public static PropertyConversion<T, TIn, long> ParseLong<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(TransformerRules.Parse<long>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for long and a format provider.</summary>
    public static PropertyConversion<T, string, long> ParseLong<T>(this PropertyRule<T, string> rule, IFormatProvider provider)
        => rule.Convert(TransformerRules.Parse<long>(provider));

    /// <summary>Converts with the catalogue Parse rule for long and a format provider.</summary>
    public static PropertyConversion<T, TIn, long> ParseLong<T, TIn>(this PropertyConversion<T, TIn, string> conversion, IFormatProvider provider)
        => conversion.Then(TransformerRules.Parse<long>(provider));

    /// <summary>Converts with the catalogue Parse rule for short and the invariant culture.</summary>
    public static PropertyConversion<T, string, short> ParseShort<T>(this PropertyRule<T, string> rule)
        => rule.Convert(TransformerRules.Parse<short>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for short and the invariant culture.</summary>
    public static PropertyConversion<T, TIn, short> ParseShort<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(TransformerRules.Parse<short>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for short and a format provider.</summary>
    public static PropertyConversion<T, string, short> ParseShort<T>(this PropertyRule<T, string> rule, IFormatProvider provider)
        => rule.Convert(TransformerRules.Parse<short>(provider));

    /// <summary>Converts with the catalogue Parse rule for short and a format provider.</summary>
    public static PropertyConversion<T, TIn, short> ParseShort<T, TIn>(this PropertyConversion<T, TIn, string> conversion, IFormatProvider provider)
        => conversion.Then(TransformerRules.Parse<short>(provider));

    /// <summary>Converts with the catalogue Parse rule for byte and the invariant culture.</summary>
    public static PropertyConversion<T, string, byte> ParseByte<T>(this PropertyRule<T, string> rule)
        => rule.Convert(TransformerRules.Parse<byte>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for byte and the invariant culture.</summary>
    public static PropertyConversion<T, TIn, byte> ParseByte<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(TransformerRules.Parse<byte>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for byte and a format provider.</summary>
    public static PropertyConversion<T, string, byte> ParseByte<T>(this PropertyRule<T, string> rule, IFormatProvider provider)
        => rule.Convert(TransformerRules.Parse<byte>(provider));

    /// <summary>Converts with the catalogue Parse rule for byte and a format provider.</summary>
    public static PropertyConversion<T, TIn, byte> ParseByte<T, TIn>(this PropertyConversion<T, TIn, string> conversion, IFormatProvider provider)
        => conversion.Then(TransformerRules.Parse<byte>(provider));

    /// <summary>Converts with the catalogue Parse rule for double and the invariant culture.</summary>
    public static PropertyConversion<T, string, double> ParseDouble<T>(this PropertyRule<T, string> rule)
        => rule.Convert(TransformerRules.Parse<double>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for double and the invariant culture.</summary>
    public static PropertyConversion<T, TIn, double> ParseDouble<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(TransformerRules.Parse<double>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for double and a format provider.</summary>
    public static PropertyConversion<T, string, double> ParseDouble<T>(this PropertyRule<T, string> rule, IFormatProvider provider)
        => rule.Convert(TransformerRules.Parse<double>(provider));

    /// <summary>Converts with the catalogue Parse rule for double and a format provider.</summary>
    public static PropertyConversion<T, TIn, double> ParseDouble<T, TIn>(this PropertyConversion<T, TIn, string> conversion, IFormatProvider provider)
        => conversion.Then(TransformerRules.Parse<double>(provider));

    /// <summary>Converts with the catalogue Parse rule for float and the invariant culture.</summary>
    public static PropertyConversion<T, string, float> ParseFloat<T>(this PropertyRule<T, string> rule)
        => rule.Convert(TransformerRules.Parse<float>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for float and the invariant culture.</summary>
    public static PropertyConversion<T, TIn, float> ParseFloat<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(TransformerRules.Parse<float>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for float and a format provider.</summary>
    public static PropertyConversion<T, string, float> ParseFloat<T>(this PropertyRule<T, string> rule, IFormatProvider provider)
        => rule.Convert(TransformerRules.Parse<float>(provider));

    /// <summary>Converts with the catalogue Parse rule for float and a format provider.</summary>
    public static PropertyConversion<T, TIn, float> ParseFloat<T, TIn>(this PropertyConversion<T, TIn, string> conversion, IFormatProvider provider)
        => conversion.Then(TransformerRules.Parse<float>(provider));

    /// <summary>Converts with the catalogue Parse rule for decimal and the invariant culture.</summary>
    public static PropertyConversion<T, string, decimal> ParseDecimal<T>(this PropertyRule<T, string> rule)
        => rule.Convert(TransformerRules.Parse<decimal>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for decimal and the invariant culture.</summary>
    public static PropertyConversion<T, TIn, decimal> ParseDecimal<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(TransformerRules.Parse<decimal>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for decimal and a format provider.</summary>
    public static PropertyConversion<T, string, decimal> ParseDecimal<T>(this PropertyRule<T, string> rule, IFormatProvider provider)
        => rule.Convert(TransformerRules.Parse<decimal>(provider));

    /// <summary>Converts with the catalogue Parse rule for decimal and a format provider.</summary>
    public static PropertyConversion<T, TIn, decimal> ParseDecimal<T, TIn>(this PropertyConversion<T, TIn, string> conversion, IFormatProvider provider)
        => conversion.Then(TransformerRules.Parse<decimal>(provider));

    /// <summary>Converts with the catalogue Parse rule for bool and the invariant culture.</summary>
    public static PropertyConversion<T, string, bool> ParseBool<T>(this PropertyRule<T, string> rule)
        => rule.Convert(TransformerRules.Parse<bool>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for bool and the invariant culture.</summary>
    public static PropertyConversion<T, TIn, bool> ParseBool<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(TransformerRules.Parse<bool>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for bool and a format provider.</summary>
    public static PropertyConversion<T, string, bool> ParseBool<T>(this PropertyRule<T, string> rule, IFormatProvider provider)
        => rule.Convert(TransformerRules.Parse<bool>(provider));

    /// <summary>Converts with the catalogue Parse rule for bool and a format provider.</summary>
    public static PropertyConversion<T, TIn, bool> ParseBool<T, TIn>(this PropertyConversion<T, TIn, string> conversion, IFormatProvider provider)
        => conversion.Then(TransformerRules.Parse<bool>(provider));

    /// <summary>Converts with the catalogue Parse rule for DateTime and the invariant culture.</summary>
    public static PropertyConversion<T, string, DateTime> ParseDateTime<T>(this PropertyRule<T, string> rule)
        => rule.Convert(TransformerRules.Parse<DateTime>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for DateTime and the invariant culture.</summary>
    public static PropertyConversion<T, TIn, DateTime> ParseDateTime<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(TransformerRules.Parse<DateTime>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for DateTime and a format provider.</summary>
    public static PropertyConversion<T, string, DateTime> ParseDateTime<T>(this PropertyRule<T, string> rule, IFormatProvider provider)
        => rule.Convert(TransformerRules.Parse<DateTime>(provider));

    /// <summary>Converts with the catalogue Parse rule for DateTime and a format provider.</summary>
    public static PropertyConversion<T, TIn, DateTime> ParseDateTime<T, TIn>(this PropertyConversion<T, TIn, string> conversion, IFormatProvider provider)
        => conversion.Then(TransformerRules.Parse<DateTime>(provider));

    /// <summary>Converts with the catalogue Parse rule for DateTimeOffset and the invariant culture.</summary>
    public static PropertyConversion<T, string, DateTimeOffset> ParseDateTimeOffset<T>(this PropertyRule<T, string> rule)
        => rule.Convert(TransformerRules.Parse<DateTimeOffset>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for DateTimeOffset and the invariant culture.</summary>
    public static PropertyConversion<T, TIn, DateTimeOffset> ParseDateTimeOffset<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(TransformerRules.Parse<DateTimeOffset>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for DateTimeOffset and a format provider.</summary>
    public static PropertyConversion<T, string, DateTimeOffset> ParseDateTimeOffset<T>(this PropertyRule<T, string> rule, IFormatProvider provider)
        => rule.Convert(TransformerRules.Parse<DateTimeOffset>(provider));

    /// <summary>Converts with the catalogue Parse rule for DateTimeOffset and a format provider.</summary>
    public static PropertyConversion<T, TIn, DateTimeOffset> ParseDateTimeOffset<T, TIn>(this PropertyConversion<T, TIn, string> conversion, IFormatProvider provider)
        => conversion.Then(TransformerRules.Parse<DateTimeOffset>(provider));

    /// <summary>Converts with the catalogue Parse rule for DateOnly and the invariant culture.</summary>
    public static PropertyConversion<T, string, DateOnly> ParseDateOnly<T>(this PropertyRule<T, string> rule)
        => rule.Convert(TransformerRules.Parse<DateOnly>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for DateOnly and the invariant culture.</summary>
    public static PropertyConversion<T, TIn, DateOnly> ParseDateOnly<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(TransformerRules.Parse<DateOnly>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for DateOnly and a format provider.</summary>
    public static PropertyConversion<T, string, DateOnly> ParseDateOnly<T>(this PropertyRule<T, string> rule, IFormatProvider provider)
        => rule.Convert(TransformerRules.Parse<DateOnly>(provider));

    /// <summary>Converts with the catalogue Parse rule for DateOnly and a format provider.</summary>
    public static PropertyConversion<T, TIn, DateOnly> ParseDateOnly<T, TIn>(this PropertyConversion<T, TIn, string> conversion, IFormatProvider provider)
        => conversion.Then(TransformerRules.Parse<DateOnly>(provider));

    /// <summary>Converts with the catalogue Parse rule for TimeOnly and the invariant culture.</summary>
    public static PropertyConversion<T, string, TimeOnly> ParseTimeOnly<T>(this PropertyRule<T, string> rule)
        => rule.Convert(TransformerRules.Parse<TimeOnly>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for TimeOnly and the invariant culture.</summary>
    public static PropertyConversion<T, TIn, TimeOnly> ParseTimeOnly<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(TransformerRules.Parse<TimeOnly>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for TimeOnly and a format provider.</summary>
    public static PropertyConversion<T, string, TimeOnly> ParseTimeOnly<T>(this PropertyRule<T, string> rule, IFormatProvider provider)
        => rule.Convert(TransformerRules.Parse<TimeOnly>(provider));

    /// <summary>Converts with the catalogue Parse rule for TimeOnly and a format provider.</summary>
    public static PropertyConversion<T, TIn, TimeOnly> ParseTimeOnly<T, TIn>(this PropertyConversion<T, TIn, string> conversion, IFormatProvider provider)
        => conversion.Then(TransformerRules.Parse<TimeOnly>(provider));

    /// <summary>Converts with the catalogue Parse rule for TimeSpan and the invariant culture.</summary>
    public static PropertyConversion<T, string, TimeSpan> ParseTimeSpan<T>(this PropertyRule<T, string> rule)
        => rule.Convert(TransformerRules.Parse<TimeSpan>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for TimeSpan and the invariant culture.</summary>
    public static PropertyConversion<T, TIn, TimeSpan> ParseTimeSpan<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(TransformerRules.Parse<TimeSpan>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for TimeSpan and a format provider.</summary>
    public static PropertyConversion<T, string, TimeSpan> ParseTimeSpan<T>(this PropertyRule<T, string> rule, IFormatProvider provider)
        => rule.Convert(TransformerRules.Parse<TimeSpan>(provider));

    /// <summary>Converts with the catalogue Parse rule for TimeSpan and a format provider.</summary>
    public static PropertyConversion<T, TIn, TimeSpan> ParseTimeSpan<T, TIn>(this PropertyConversion<T, TIn, string> conversion, IFormatProvider provider)
        => conversion.Then(TransformerRules.Parse<TimeSpan>(provider));

    /// <summary>Converts with the catalogue Parse rule for Guid and the invariant culture.</summary>
    public static PropertyConversion<T, string, Guid> ParseGuid<T>(this PropertyRule<T, string> rule)
        => rule.Convert(TransformerRules.Parse<Guid>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for Guid and the invariant culture.</summary>
    public static PropertyConversion<T, TIn, Guid> ParseGuid<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(TransformerRules.Parse<Guid>(CultureInfo.InvariantCulture));

    /// <summary>Converts with the catalogue Parse rule for Guid and a format provider.</summary>
    public static PropertyConversion<T, string, Guid> ParseGuid<T>(this PropertyRule<T, string> rule, IFormatProvider provider)
        => rule.Convert(TransformerRules.Parse<Guid>(provider));

    /// <summary>Converts with the catalogue Parse rule for Guid and a format provider.</summary>
    public static PropertyConversion<T, TIn, Guid> ParseGuid<T, TIn>(this PropertyConversion<T, TIn, string> conversion, IFormatProvider provider)
        => conversion.Then(TransformerRules.Parse<Guid>(provider));

    /// <summary>Appends the catalogue ParseExactDateTime rule for one format.</summary>
    public static PropertyConversion<T, string, DateTime> ParseExactDateTime<T>(this PropertyRule<T, string> rule, string format, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
        => rule.Convert(TransformerRules.ParseExactDateTime(format, provider, styles));

    /// <summary>Appends the catalogue ParseExactDateTime rule for one format.</summary>
    public static PropertyConversion<T, TIn, DateTime> ParseExactDateTime<T, TIn>(this PropertyConversion<T, TIn, string> conversion, string format, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
        => conversion.Then(TransformerRules.ParseExactDateTime(format, provider, styles));

    /// <summary>Appends the catalogue ParseExactDateTime rule for several formats.</summary>
    public static PropertyConversion<T, string, DateTime> ParseExactDateTime<T>(this PropertyRule<T, string> rule, string[] formats, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
        => rule.Convert(TransformerRules.ParseExactDateTime(formats, provider, styles));

    /// <summary>Appends the catalogue ParseExactDateTime rule for several formats.</summary>
    public static PropertyConversion<T, TIn, DateTime> ParseExactDateTime<T, TIn>(this PropertyConversion<T, TIn, string> conversion, string[] formats, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
        => conversion.Then(TransformerRules.ParseExactDateTime(formats, provider, styles));

    /// <summary>Appends the catalogue ParseExactDateTimeOffset rule for one format.</summary>
    public static PropertyConversion<T, string, DateTimeOffset> ParseExactDateTimeOffset<T>(this PropertyRule<T, string> rule, string format, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
        => rule.Convert(TransformerRules.ParseExactDateTimeOffset(format, provider, styles));

    /// <summary>Appends the catalogue ParseExactDateTimeOffset rule for one format.</summary>
    public static PropertyConversion<T, TIn, DateTimeOffset> ParseExactDateTimeOffset<T, TIn>(this PropertyConversion<T, TIn, string> conversion, string format, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
        => conversion.Then(TransformerRules.ParseExactDateTimeOffset(format, provider, styles));

    /// <summary>Appends the catalogue ParseExactDateTimeOffset rule for several formats.</summary>
    public static PropertyConversion<T, string, DateTimeOffset> ParseExactDateTimeOffset<T>(this PropertyRule<T, string> rule, string[] formats, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
        => rule.Convert(TransformerRules.ParseExactDateTimeOffset(formats, provider, styles));

    /// <summary>Appends the catalogue ParseExactDateTimeOffset rule for several formats.</summary>
    public static PropertyConversion<T, TIn, DateTimeOffset> ParseExactDateTimeOffset<T, TIn>(this PropertyConversion<T, TIn, string> conversion, string[] formats, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
        => conversion.Then(TransformerRules.ParseExactDateTimeOffset(formats, provider, styles));

    /// <summary>Appends the catalogue ParseExactDateOnly rule for one format.</summary>
    public static PropertyConversion<T, string, DateOnly> ParseExactDateOnly<T>(this PropertyRule<T, string> rule, string format, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
        => rule.Convert(TransformerRules.ParseExactDateOnly(format, provider, styles));

    /// <summary>Appends the catalogue ParseExactDateOnly rule for one format.</summary>
    public static PropertyConversion<T, TIn, DateOnly> ParseExactDateOnly<T, TIn>(this PropertyConversion<T, TIn, string> conversion, string format, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
        => conversion.Then(TransformerRules.ParseExactDateOnly(format, provider, styles));

    /// <summary>Appends the catalogue ParseExactDateOnly rule for several formats.</summary>
    public static PropertyConversion<T, string, DateOnly> ParseExactDateOnly<T>(this PropertyRule<T, string> rule, string[] formats, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
        => rule.Convert(TransformerRules.ParseExactDateOnly(formats, provider, styles));

    /// <summary>Appends the catalogue ParseExactDateOnly rule for several formats.</summary>
    public static PropertyConversion<T, TIn, DateOnly> ParseExactDateOnly<T, TIn>(this PropertyConversion<T, TIn, string> conversion, string[] formats, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
        => conversion.Then(TransformerRules.ParseExactDateOnly(formats, provider, styles));

    /// <summary>Appends the catalogue ParseExactTimeOnly rule for one format.</summary>
    public static PropertyConversion<T, string, TimeOnly> ParseExactTimeOnly<T>(this PropertyRule<T, string> rule, string format, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
        => rule.Convert(TransformerRules.ParseExactTimeOnly(format, provider, styles));

    /// <summary>Appends the catalogue ParseExactTimeOnly rule for one format.</summary>
    public static PropertyConversion<T, TIn, TimeOnly> ParseExactTimeOnly<T, TIn>(this PropertyConversion<T, TIn, string> conversion, string format, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
        => conversion.Then(TransformerRules.ParseExactTimeOnly(format, provider, styles));

    /// <summary>Appends the catalogue ParseExactTimeOnly rule for several formats.</summary>
    public static PropertyConversion<T, string, TimeOnly> ParseExactTimeOnly<T>(this PropertyRule<T, string> rule, string[] formats, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
        => rule.Convert(TransformerRules.ParseExactTimeOnly(formats, provider, styles));

    /// <summary>Appends the catalogue ParseExactTimeOnly rule for several formats.</summary>
    public static PropertyConversion<T, TIn, TimeOnly> ParseExactTimeOnly<T, TIn>(this PropertyConversion<T, TIn, string> conversion, string[] formats, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
        => conversion.Then(TransformerRules.ParseExactTimeOnly(formats, provider, styles));

    /// <summary>Appends the catalogue ParseExactTimeSpan rule for one format.</summary>
    public static PropertyConversion<T, string, TimeSpan> ParseExactTimeSpan<T>(this PropertyRule<T, string> rule, string format, IFormatProvider provider, TimeSpanStyles styles = TimeSpanStyles.None)
        => rule.Convert(TransformerRules.ParseExactTimeSpan(format, provider, styles));

    /// <summary>Appends the catalogue ParseExactTimeSpan rule for one format.</summary>
    public static PropertyConversion<T, TIn, TimeSpan> ParseExactTimeSpan<T, TIn>(this PropertyConversion<T, TIn, string> conversion, string format, IFormatProvider provider, TimeSpanStyles styles = TimeSpanStyles.None)
        => conversion.Then(TransformerRules.ParseExactTimeSpan(format, provider, styles));

    /// <summary>Appends the catalogue ParseExactTimeSpan rule for several formats.</summary>
    public static PropertyConversion<T, string, TimeSpan> ParseExactTimeSpan<T>(this PropertyRule<T, string> rule, string[] formats, IFormatProvider provider, TimeSpanStyles styles = TimeSpanStyles.None)
        => rule.Convert(TransformerRules.ParseExactTimeSpan(formats, provider, styles));

    /// <summary>Appends the catalogue ParseExactTimeSpan rule for several formats.</summary>
    public static PropertyConversion<T, TIn, TimeSpan> ParseExactTimeSpan<T, TIn>(this PropertyConversion<T, TIn, string> conversion, string[] formats, IFormatProvider provider, TimeSpanStyles styles = TimeSpanStyles.None)
        => conversion.Then(TransformerRules.ParseExactTimeSpan(formats, provider, styles));

    /// <summary>Appends the catalogue Format rule.</summary>
    public static PropertyConversion<T, TProperty, string> Format<T, TProperty>(this PropertyRule<T, TProperty> rule, string? format, IFormatProvider provider)
        where TProperty : IFormattable
        => rule.Convert(TransformerRules.Format<TProperty>(format, provider));

    /// <summary>Appends the catalogue Format rule.</summary>
    public static PropertyConversion<T, TIn, string> Format<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, string? format, IFormatProvider provider)
        where TProperty : IFormattable
        => conversion.Then(TransformerRules.Format<TProperty>(format, provider));

    /// <summary>Appends the catalogue EnumName rule.</summary>
    public static PropertyConversion<T, TEnum, string> EnumName<T, TEnum>(this PropertyRule<T, TEnum> rule)
        where TEnum : struct, Enum
        => rule.Convert(TransformerRules.EnumName<TEnum>());

    /// <summary>Appends the catalogue EnumName rule.</summary>
    public static PropertyConversion<T, TIn, string> EnumName<T, TIn, TEnum>(this PropertyConversion<T, TIn, TEnum> conversion)
        where TEnum : struct, Enum
        => conversion.Then(TransformerRules.EnumName<TEnum>());

    /// <summary>Appends the catalogue ToDateOnlyFromDateTime rule.</summary>
    public static PropertyConversion<T, DateTime, DateOnly> ToDateOnlyFromDateTime<T>(this PropertyRule<T, DateTime> rule)
        => rule.Convert(TransformerRules.ToDateOnlyFromDateTime());

    /// <summary>Appends the catalogue ToDateOnlyFromDateTime rule.</summary>
    public static PropertyConversion<T, TIn, DateOnly> ToDateOnlyFromDateTime<T, TIn>(this PropertyConversion<T, TIn, DateTime> conversion)
        => conversion.Then(TransformerRules.ToDateOnlyFromDateTime());

    /// <summary>Appends the catalogue ToDateOnlyFromDateTimeOffset rule.</summary>
    public static PropertyConversion<T, DateTimeOffset, DateOnly> ToDateOnlyFromDateTimeOffset<T>(this PropertyRule<T, DateTimeOffset> rule)
        => rule.Convert(TransformerRules.ToDateOnlyFromDateTimeOffset());

    /// <summary>Appends the catalogue ToDateOnlyFromDateTimeOffset rule.</summary>
    public static PropertyConversion<T, TIn, DateOnly> ToDateOnlyFromDateTimeOffset<T, TIn>(this PropertyConversion<T, TIn, DateTimeOffset> conversion)
        => conversion.Then(TransformerRules.ToDateOnlyFromDateTimeOffset());

    /// <summary>Appends the catalogue ToTimeOnlyFromDateTime rule.</summary>
    public static PropertyConversion<T, DateTime, TimeOnly> ToTimeOnlyFromDateTime<T>(this PropertyRule<T, DateTime> rule)
        => rule.Convert(TransformerRules.ToTimeOnlyFromDateTime());

    /// <summary>Appends the catalogue ToTimeOnlyFromDateTime rule.</summary>
    public static PropertyConversion<T, TIn, TimeOnly> ToTimeOnlyFromDateTime<T, TIn>(this PropertyConversion<T, TIn, DateTime> conversion)
        => conversion.Then(TransformerRules.ToTimeOnlyFromDateTime());

    /// <summary>Appends the catalogue ToTimeOnlyFromDateTimeOffset rule.</summary>
    public static PropertyConversion<T, DateTimeOffset, TimeOnly> ToTimeOnlyFromDateTimeOffset<T>(this PropertyRule<T, DateTimeOffset> rule)
        => rule.Convert(TransformerRules.ToTimeOnlyFromDateTimeOffset());

    /// <summary>Appends the catalogue ToTimeOnlyFromDateTimeOffset rule.</summary>
    public static PropertyConversion<T, TIn, TimeOnly> ToTimeOnlyFromDateTimeOffset<T, TIn>(this PropertyConversion<T, TIn, DateTimeOffset> conversion)
        => conversion.Then(TransformerRules.ToTimeOnlyFromDateTimeOffset());

    /// <summary>Appends the catalogue ToTimeOnlyFromTimeSpan rule.</summary>
    public static PropertyConversion<T, TimeSpan, TimeOnly> ToTimeOnlyFromTimeSpan<T>(this PropertyRule<T, TimeSpan> rule)
        => rule.Convert(TransformerRules.ToTimeOnlyFromTimeSpan());

    /// <summary>Appends the catalogue ToTimeOnlyFromTimeSpan rule.</summary>
    public static PropertyConversion<T, TIn, TimeOnly> ToTimeOnlyFromTimeSpan<T, TIn>(this PropertyConversion<T, TIn, TimeSpan> conversion)
        => conversion.Then(TransformerRules.ToTimeOnlyFromTimeSpan());

    /// <summary>Appends the catalogue ToTimeSpan rule.</summary>
    public static PropertyConversion<T, TimeOnly, TimeSpan> ToTimeSpan<T>(this PropertyRule<T, TimeOnly> rule)
        => rule.Convert(TransformerRules.ToTimeSpan());

    /// <summary>Appends the catalogue ToTimeSpan rule.</summary>
    public static PropertyConversion<T, TIn, TimeSpan> ToTimeSpan<T, TIn>(this PropertyConversion<T, TIn, TimeOnly> conversion)
        => conversion.Then(TransformerRules.ToTimeSpan());

    /// <summary>Appends the catalogue ToUnixTimeSeconds rule.</summary>
    public static PropertyConversion<T, DateTimeOffset, long> ToUnixTimeSeconds<T>(this PropertyRule<T, DateTimeOffset> rule)
        => rule.Convert(TransformerRules.ToUnixTimeSeconds());

    /// <summary>Appends the catalogue ToUnixTimeSeconds rule.</summary>
    public static PropertyConversion<T, TIn, long> ToUnixTimeSeconds<T, TIn>(this PropertyConversion<T, TIn, DateTimeOffset> conversion)
        => conversion.Then(TransformerRules.ToUnixTimeSeconds());

    /// <summary>Appends the catalogue ToUnixTimeMilliseconds rule.</summary>
    public static PropertyConversion<T, DateTimeOffset, long> ToUnixTimeMilliseconds<T>(this PropertyRule<T, DateTimeOffset> rule)
        => rule.Convert(TransformerRules.ToUnixTimeMilliseconds());

    /// <summary>Appends the catalogue ToUnixTimeMilliseconds rule.</summary>
    public static PropertyConversion<T, TIn, long> ToUnixTimeMilliseconds<T, TIn>(this PropertyConversion<T, TIn, DateTimeOffset> conversion)
        => conversion.Then(TransformerRules.ToUnixTimeMilliseconds());

    /// <summary>Appends the catalogue FromUnixTimeSeconds rule.</summary>
    public static PropertyConversion<T, long, DateTimeOffset> FromUnixTimeSeconds<T>(this PropertyRule<T, long> rule)
        => rule.Convert(TransformerRules.FromUnixTimeSeconds());

    /// <summary>Appends the catalogue FromUnixTimeSeconds rule.</summary>
    public static PropertyConversion<T, TIn, DateTimeOffset> FromUnixTimeSeconds<T, TIn>(this PropertyConversion<T, TIn, long> conversion)
        => conversion.Then(TransformerRules.FromUnixTimeSeconds());

    /// <summary>Appends the catalogue FromUnixTimeMilliseconds rule.</summary>
    public static PropertyConversion<T, long, DateTimeOffset> FromUnixTimeMilliseconds<T>(this PropertyRule<T, long> rule)
        => rule.Convert(TransformerRules.FromUnixTimeMilliseconds());

    /// <summary>Appends the catalogue FromUnixTimeMilliseconds rule.</summary>
    public static PropertyConversion<T, TIn, DateTimeOffset> FromUnixTimeMilliseconds<T, TIn>(this PropertyConversion<T, TIn, long> conversion)
        => conversion.Then(TransformerRules.FromUnixTimeMilliseconds());

    /// <summary>Appends the catalogue FromBase64 rule.</summary>
    public static PropertyConversion<T, string, byte[]> FromBase64<T>(this PropertyRule<T, string> rule)
        => rule.Convert(TransformerRules.FromBase64());

    /// <summary>Appends the catalogue FromBase64 rule.</summary>
    public static PropertyConversion<T, TIn, byte[]> FromBase64<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(TransformerRules.FromBase64());

    /// <summary>Appends the catalogue FromHex rule.</summary>
    public static PropertyConversion<T, string, byte[]> FromHex<T>(this PropertyRule<T, string> rule)
        => rule.Convert(TransformerRules.FromHex());

    /// <summary>Appends the catalogue FromHex rule.</summary>
    public static PropertyConversion<T, TIn, byte[]> FromHex<T, TIn>(this PropertyConversion<T, TIn, string> conversion)
        => conversion.Then(TransformerRules.FromHex());

    /// <summary>Builds a DateTime from a DateOnly with the catalogue ToDateTime rule.</summary>
    public static PropertyConversion<T, DateOnly, DateTime> ToDateTime<T>(this PropertyRule<T, DateOnly> rule, TimeOnly time, DateTimeKind kind)
        => rule.Convert(TransformerRules.ToDateTime(time, kind));

    /// <summary>Builds a DateTime from a DateOnly with the catalogue ToDateTime rule.</summary>
    public static PropertyConversion<T, TIn, DateTime> ToDateTime<T, TIn>(this PropertyConversion<T, TIn, DateOnly> conversion, TimeOnly time, DateTimeKind kind)
        => conversion.Then(TransformerRules.ToDateTime(time, kind));

    /// <summary>Builds a DateTime from a TimeOnly with the catalogue ToDateTime rule.</summary>
    public static PropertyConversion<T, TimeOnly, DateTime> ToDateTime<T>(this PropertyRule<T, TimeOnly> rule, DateOnly date, DateTimeKind kind)
        => rule.Convert(TransformerRules.ToDateTime(date, kind));

    /// <summary>Builds a DateTime from a TimeOnly with the catalogue ToDateTime rule.</summary>
    public static PropertyConversion<T, TIn, DateTime> ToDateTime<T, TIn>(this PropertyConversion<T, TIn, TimeOnly> conversion, DateOnly date, DateTimeKind kind)
        => conversion.Then(TransformerRules.ToDateTime(date, kind));

    /// <summary>Appends the catalogue ToDateTimeOffset rule.</summary>
    public static PropertyConversion<T, DateTime, DateTimeOffset> ToDateTimeOffset<T>(this PropertyRule<T, DateTime> rule, TimeSpan offset)
        => rule.Convert(TransformerRules.ToDateTimeOffset(offset));

    /// <summary>Appends the catalogue ToDateTimeOffset rule.</summary>
    public static PropertyConversion<T, TIn, DateTimeOffset> ToDateTimeOffset<T, TIn>(this PropertyConversion<T, TIn, DateTime> conversion, TimeSpan offset)
        => conversion.Then(TransformerRules.ToDateTimeOffset(offset));

    /// <summary>Appends the catalogue Encode rule.</summary>
    public static PropertyConversion<T, string, byte[]> Encode<T>(this PropertyRule<T, string> rule, Encoding encoding)
        => rule.Convert(TransformerRules.Encode(encoding));

    /// <summary>Appends the catalogue Encode rule.</summary>
    public static PropertyConversion<T, TIn, byte[]> Encode<T, TIn>(this PropertyConversion<T, TIn, string> conversion, Encoding encoding)
        => conversion.Then(TransformerRules.Encode(encoding));

    /// <summary>Appends the catalogue Decode rule.</summary>
    public static PropertyConversion<T, byte[], string> Decode<T>(this PropertyRule<T, byte[]> rule, Encoding encoding)
        => rule.Convert(TransformerRules.Decode(encoding));

    /// <summary>Appends the catalogue Decode rule.</summary>
    public static PropertyConversion<T, TIn, string> Decode<T, TIn>(this PropertyConversion<T, TIn, byte[]> conversion, Encoding encoding)
        => conversion.Then(TransformerRules.Decode(encoding));

    /// <summary>Appends the catalogue ToBase64 rule.</summary>
    public static PropertyConversion<T, byte[], string> ToBase64<T>(this PropertyRule<T, byte[]> rule, Base64FormattingOptions options = Base64FormattingOptions.None)
        => rule.Convert(TransformerRules.ToBase64(options));

    /// <summary>Appends the catalogue ToBase64 rule.</summary>
    public static PropertyConversion<T, TIn, string> ToBase64<T, TIn>(this PropertyConversion<T, TIn, byte[]> conversion, Base64FormattingOptions options = Base64FormattingOptions.None)
        => conversion.Then(TransformerRules.ToBase64(options));

    /// <summary>Appends the catalogue ToHex rule.</summary>
    public static PropertyConversion<T, byte[], string> ToHex<T>(this PropertyRule<T, byte[]> rule, bool uppercase = true)
        => rule.Convert(TransformerRules.ToHex(uppercase));

    /// <summary>Appends the catalogue ToHex rule.</summary>
    public static PropertyConversion<T, TIn, string> ToHex<T, TIn>(this PropertyConversion<T, TIn, byte[]> conversion, bool uppercase = true)
        => conversion.Then(TransformerRules.ToHex(uppercase));

    /// <summary>
    /// Converts with the catalogue ConvertChecked rule.
    /// Call it as <c>ConvertChecked&lt;T, TFrom, TTo&gt;()</c>.
    /// </summary>
    public static PropertyConversion<T, TFrom, TTo> ConvertChecked<T, TFrom, TTo>(this PropertyRule<T, TFrom> rule)
        where TFrom : INumberBase<TFrom>
        where TTo : INumberBase<TTo>
        => rule.Convert(TransformerRules.ConvertChecked<TFrom, TTo>());

    /// <summary>Continues with the catalogue ConvertChecked rule.</summary>
    public static PropertyConversion<T, TIn, TTo> ConvertChecked<TTo, T, TIn, TFrom>(
        this PropertyConversion<T, TIn, TFrom> conversion)
        where TFrom : INumberBase<TFrom>
        where TTo : INumberBase<TTo>
        => conversion.Then(TransformerRules.ConvertChecked<TFrom, TTo>());

    /// <summary>Converts with the catalogue ConvertSaturating rule.</summary>
    public static PropertyConversion<T, TFrom, TTo> ConvertSaturating<T, TFrom, TTo>(this PropertyRule<T, TFrom> rule)
        where TFrom : INumberBase<TFrom>
        where TTo : INumberBase<TTo>
        => rule.Convert(TransformerRules.ConvertSaturating<TFrom, TTo>());

    /// <summary>Continues with the catalogue ConvertSaturating rule.</summary>
    public static PropertyConversion<T, TIn, TTo> ConvertSaturating<TTo, T, TIn, TFrom>(
        this PropertyConversion<T, TIn, TFrom> conversion)
        where TFrom : INumberBase<TFrom>
        where TTo : INumberBase<TTo>
        => conversion.Then(TransformerRules.ConvertSaturating<TFrom, TTo>());

    /// <summary>Converts with the catalogue ConvertTruncating rule.</summary>
    public static PropertyConversion<T, TFrom, TTo> ConvertTruncating<T, TFrom, TTo>(this PropertyRule<T, TFrom> rule)
        where TFrom : INumberBase<TFrom>
        where TTo : INumberBase<TTo>
        => rule.Convert(TransformerRules.ConvertTruncating<TFrom, TTo>());

    /// <summary>Continues with the catalogue ConvertTruncating rule.</summary>
    public static PropertyConversion<T, TIn, TTo> ConvertTruncating<TTo, T, TIn, TFrom>(
        this PropertyConversion<T, TIn, TFrom> conversion)
        where TFrom : INumberBase<TFrom>
        where TTo : INumberBase<TTo>
        => conversion.Then(TransformerRules.ConvertTruncating<TFrom, TTo>());

    /// <summary>Appends a same-type catalogue Map rule. Use MapTo to change the property type.</summary>
    public static PropertyRule<T, TProperty> Map<T, TProperty>(this PropertyRule<T, TProperty> rule, Func<TProperty, TProperty> map)
        => rule.Add(TransformerRules.Map(map));

    /// <summary>Appends a same-type catalogue Map rule. Use MapTo to change the property type.</summary>
    public static PropertyConversion<T, TIn, TProperty> Map<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, Func<TProperty, TProperty> map)
        => conversion.Then(TransformerRules.Map(map));

    /// <summary>Appends a same-type catalogue MapAsync rule. Use MapToAsync to change the property type.</summary>
    public static PropertyRule<T, TProperty> MapAsync<T, TProperty>(this PropertyRule<T, TProperty> rule, Func<TProperty, CancellationToken, ValueTask<TProperty>> map)
        => rule.Add(TransformerRules.MapAsync(map));

    /// <summary>Appends a same-type catalogue MapAsync rule. Use MapToAsync to change the property type.</summary>
    public static PropertyConversion<T, TIn, TProperty> MapAsync<T, TIn, TProperty>(this PropertyConversion<T, TIn, TProperty> conversion, Func<TProperty, CancellationToken, ValueTask<TProperty>> map)
        => conversion.Then(TransformerRules.MapAsync(map));
}
