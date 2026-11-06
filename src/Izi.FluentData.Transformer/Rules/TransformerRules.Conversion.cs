using System.Globalization;
using System.Numerics;
using System.Text;

namespace Izi.FluentData.Transformer;

/// <summary>
/// Factories for transformation steps. Each method returns a <see cref="TransformerRule{TIn,TOut}"/>
/// that can be passed to <see cref="Transformer{TIn,TOut}"/> and chained with
/// <see cref="Transformer{TIn,TOut}.Then{TNext}"/>.
/// </summary>
/// <remarks>
/// <para>
/// Same-type steps return <c>TransformerRule&lt;T, T&gt;</c>. Conversion steps change the type.
/// C# cannot overload a method on its return type alone, so names such as <c>ParseExactDateTime</c>
/// and <c>ToDateOnlyFromDateTime</c> carry the type in the method name. Date arithmetic that shares
/// a parameter list is generic, for example <c>AddYears&lt;DateTime&gt;(1)</c>, and rejects an
/// unsupported type when the rule is created.
/// </para>
/// <para>
/// Rules capture their configuration when they are created and do not mutate it. Format arrays are copied.
/// Strings, culture objects, and numeric styles are immutable values. An <see cref="Encoding"/> is stored by
/// reference and used with whatever encoder/decoder fallback it already has; do not mutate a shared encoding.
/// A rule is safe to reuse across threads when the captured delegate and encoding are themselves safe.
/// Stateless numeric rules allocate the rule object at construction. Successful synchronous transforms return
/// a completed <see cref="ValueTask{TResult}"/> and do not build an async state machine.
/// </para>
/// <para>
/// Formatting, base-64, hex, and encoding allocate the resulting string or byte array on every call.
/// <see cref="ParseResult{T}"/> is a struct, so a successful or failed try-parse does not allocate the result
/// object. <see cref="MapAsync{TIn,TOut}"/> returns the delegate's <see cref="ValueTask{TResult}"/> directly:
/// the caller awaits it once. This class does not await that task, so it does not add a second consumption
/// or its own <c>ConfigureAwait</c>.
/// </para>
/// </remarks>
public static partial class TransformerRules
{
    /// <summary>
    /// Parses text with <typeparamref name="T"/>'s <see cref="IParsable{TSelf}.Parse"/>.
    /// Works for the BCL parsable types, including the numeric types, <see cref="bool"/>, <see cref="Guid"/>,
    /// and the date/time types. The provider is required so the culture is chosen by the caller.
    /// A null string throws <see cref="ArgumentNullException"/>. Invalid text throws the parse exception
    /// (<see cref="FormatException"/> or <see cref="OverflowException"/>).
    /// </summary>
    public static TransformerRule<string, T> Parse<T>(IFormatProvider provider)
        where T : IParsable<T>
    {
        ArgumentNullException.ThrowIfNull(provider);
        return new TransformerRule<string, T>((value, _) =>
        {
            ArgumentNullException.ThrowIfNull(value);
            return ValueTask.FromResult(T.Parse(value, provider));
        });
    }

    /// <summary>
    /// Parses a <see cref="DateTime"/> with an exact format. Null text throws <see cref="ArgumentNullException"/>.
    /// A mismatch throws <see cref="FormatException"/>.
    /// </summary>
    public static TransformerRule<string, DateTime> ParseExactDateTime(string format, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
    {
        RequireFormat(format);
        ArgumentNullException.ThrowIfNull(provider);
        return new TransformerRule<string, DateTime>((value, _) =>
        {
            ArgumentNullException.ThrowIfNull(value);
            return ValueTask.FromResult(DateTime.ParseExact(value, format, provider, styles));
        });
    }

    /// <summary>Parses a <see cref="DateTime"/> against the first matching exact format. The array is copied.</summary>
    public static TransformerRule<string, DateTime> ParseExactDateTime(string[] formats, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
    {
        var captured = CopyFormats(formats);
        ArgumentNullException.ThrowIfNull(provider);
        return new TransformerRule<string, DateTime>((value, _) =>
        {
            ArgumentNullException.ThrowIfNull(value);
            return ValueTask.FromResult(DateTime.ParseExact(value, captured, provider, styles));
        });
    }

    /// <summary>Parses a <see cref="DateTimeOffset"/> with an exact format.</summary>
    public static TransformerRule<string, DateTimeOffset> ParseExactDateTimeOffset(string format, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
    {
        RequireFormat(format);
        ArgumentNullException.ThrowIfNull(provider);
        return new TransformerRule<string, DateTimeOffset>((value, _) =>
        {
            ArgumentNullException.ThrowIfNull(value);
            return ValueTask.FromResult(DateTimeOffset.ParseExact(value, format, provider, styles));
        });
    }

    /// <summary>Parses a <see cref="DateTimeOffset"/> against the first matching exact format. The array is copied.</summary>
    public static TransformerRule<string, DateTimeOffset> ParseExactDateTimeOffset(string[] formats, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
    {
        var captured = CopyFormats(formats);
        ArgumentNullException.ThrowIfNull(provider);
        return new TransformerRule<string, DateTimeOffset>((value, _) =>
        {
            ArgumentNullException.ThrowIfNull(value);
            return ValueTask.FromResult(DateTimeOffset.ParseExact(value, captured, provider, styles));
        });
    }

    /// <summary>Parses a <see cref="DateOnly"/> with an exact format.</summary>
    public static TransformerRule<string, DateOnly> ParseExactDateOnly(string format, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
    {
        RequireFormat(format);
        ArgumentNullException.ThrowIfNull(provider);
        return new TransformerRule<string, DateOnly>((value, _) =>
        {
            ArgumentNullException.ThrowIfNull(value);
            return ValueTask.FromResult(DateOnly.ParseExact(value, format, provider, styles));
        });
    }

    /// <summary>Parses a <see cref="DateOnly"/> against the first matching exact format. The array is copied.</summary>
    public static TransformerRule<string, DateOnly> ParseExactDateOnly(string[] formats, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
    {
        var captured = CopyFormats(formats);
        ArgumentNullException.ThrowIfNull(provider);
        return new TransformerRule<string, DateOnly>((value, _) =>
        {
            ArgumentNullException.ThrowIfNull(value);
            return ValueTask.FromResult(DateOnly.ParseExact(value, captured, provider, styles));
        });
    }

    /// <summary>Parses a <see cref="TimeOnly"/> with an exact format.</summary>
    public static TransformerRule<string, TimeOnly> ParseExactTimeOnly(string format, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
    {
        RequireFormat(format);
        ArgumentNullException.ThrowIfNull(provider);
        return new TransformerRule<string, TimeOnly>((value, _) =>
        {
            ArgumentNullException.ThrowIfNull(value);
            return ValueTask.FromResult(TimeOnly.ParseExact(value, format, provider, styles));
        });
    }

    /// <summary>Parses a <see cref="TimeOnly"/> against the first matching exact format. The array is copied.</summary>
    public static TransformerRule<string, TimeOnly> ParseExactTimeOnly(string[] formats, IFormatProvider provider, DateTimeStyles styles = DateTimeStyles.None)
    {
        var captured = CopyFormats(formats);
        ArgumentNullException.ThrowIfNull(provider);
        return new TransformerRule<string, TimeOnly>((value, _) =>
        {
            ArgumentNullException.ThrowIfNull(value);
            return ValueTask.FromResult(TimeOnly.ParseExact(value, captured, provider, styles));
        });
    }

    /// <summary>
    /// Parses a <see cref="TimeSpan"/> with an exact format and <see cref="TimeSpanStyles"/>.
    /// Time spans use a different styles enum than the date types, so this method is not a generic <c>ParseExact&lt;T&gt;</c>.
    /// </summary>
    public static TransformerRule<string, TimeSpan> ParseExactTimeSpan(string format, IFormatProvider provider, TimeSpanStyles styles = TimeSpanStyles.None)
    {
        RequireFormat(format);
        ArgumentNullException.ThrowIfNull(provider);
        return new TransformerRule<string, TimeSpan>((value, _) =>
        {
            ArgumentNullException.ThrowIfNull(value);
            return ValueTask.FromResult(TimeSpan.ParseExact(value, format, provider, styles));
        });
    }

    /// <summary>Parses a <see cref="TimeSpan"/> against the first matching exact format. The array is copied.</summary>
    public static TransformerRule<string, TimeSpan> ParseExactTimeSpan(string[] formats, IFormatProvider provider, TimeSpanStyles styles = TimeSpanStyles.None)
    {
        var captured = CopyFormats(formats);
        ArgumentNullException.ThrowIfNull(provider);
        return new TransformerRule<string, TimeSpan>((value, _) =>
        {
            ArgumentNullException.ThrowIfNull(value);
            return ValueTask.FromResult(TimeSpan.ParseExact(value, captured, provider, styles));
        });
    }

    /// <summary>
    /// Parses an enum with <see cref="Enum.Parse{TEnum}(string, bool)"/>. This is the normal BCL behavior:
    /// names are accepted, a flags combination such as <c>"Read, Write"</c> is accepted when
    /// <typeparamref name="TEnum"/> has <see cref="FlagsAttribute"/>, and a numeric value is accepted even when
    /// no named member has that value. Unknown names throw <see cref="ArgumentException"/>.
    /// Null or empty text throws. There is no defined-only mode.
    /// </summary>
    public static TransformerRule<string, TEnum> ParseEnum<TEnum>(bool ignoreCase = false)
        where TEnum : struct, Enum
    {
        return new TransformerRule<string, TEnum>((value, _) =>
        {
            ArgumentNullException.ThrowIfNull(value);
            return ValueTask.FromResult(Enum.Parse<TEnum>(value, ignoreCase));
        });
    }

    /// <summary>
    /// Parses with <see cref="IParsable{TSelf}.TryParse"/>. Null text and text that does not parse, including
    /// overflow, produce <paramref name="fallback"/> instead of an exception. A successful parse of a value equal
    /// to <paramref name="fallback"/> is indistinguishable from failure; use <see cref="TryParse{T}"/> when that matters.
    /// </summary>
    public static TransformerRule<string, T> ParseOrDefault<T>(T fallback, IFormatProvider provider)
        where T : IParsable<T>
    {
        ArgumentNullException.ThrowIfNull(provider);
        return new TransformerRule<string, T>((value, _) =>
        {
            if (value is not null && T.TryParse(value, provider, out var parsed))
            {
                return ValueTask.FromResult(parsed);
            }

            return ValueTask.FromResult(fallback);
        });
    }

    /// <summary>
    /// Parses with <see cref="IParsable{TSelf}.TryParse"/> into a <see cref="ParseResult{T}"/>.
    /// Null text and invalid text return <see cref="ParseResult{T}.Failed"/> and do not throw.
    /// <see cref="ParseResult{T}.Value"/> is <c>default</c> on failure, so check <see cref="ParseResult{T}.Success"/>.
    /// </summary>
    public static TransformerRule<string, ParseResult<T>> TryParse<T>(IFormatProvider provider)
        where T : IParsable<T>
    {
        ArgumentNullException.ThrowIfNull(provider);
        return new TransformerRule<string, ParseResult<T>>((value, _) =>
        {
            if (value is not null && T.TryParse(value, provider, out var parsed))
            {
                return ValueTask.FromResult(ParseResult<T>.Succeeded(parsed));
            }

            return ValueTask.FromResult(ParseResult<T>.Failed());
        });
    }

    /// <summary>
    /// Formats with <see cref="IFormattable.ToString(string?, IFormatProvider?)"/>.
    /// A null <paramref name="format"/> asks the value for its general format. An empty format is rejected.
    /// The provider is required. The call allocates the result string.
    /// </summary>
    public static TransformerRule<T, string> Format<T>(string? format, IFormatProvider provider)
        where T : IFormattable
    {
        if (format is { Length: 0 })
        {
            throw new ArgumentException("Format must be non-empty when it is supplied. Pass null for the general format.", nameof(format));
        }

        ArgumentNullException.ThrowIfNull(provider);
        return new TransformerRule<T, string>((value, _) =>
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return ValueTask.FromResult(value.ToString(format, provider));
        });
    }

    /// <summary>
    /// Formats an enum with <see cref="Enum.ToString()"/>. A defined value returns its name.
    /// A flags combination of named bits returns the comma-separated names (<c>"Read, Write"</c>).
    /// An unnamed numeric value, including a flags combination that has no name, returns the decimal digits.
    /// The result string is allocated. The call can box the enum value.
    /// </summary>
    public static TransformerRule<TEnum, string> EnumName<TEnum>()
        where TEnum : struct, Enum
    {
        return new TransformerRule<TEnum, string>((value, _) =>
            ValueTask.FromResult(value.ToString() ?? string.Empty));
    }

    /// <summary>
    /// Converts with <see cref="INumberBase{TSelf}.CreateChecked{TOther}"/>.
    /// Throws <see cref="OverflowException"/> when the value is outside the destination range, including non-finite
    /// floating-point values. On the supported runtimes a fractional value whose truncation toward zero still fits
    /// is accepted (1.9 becomes 1) rather than rejected for having a fraction.
    /// </summary>
    public static TransformerRule<TFrom, TTo> ConvertChecked<TFrom, TTo>()
        where TFrom : INumberBase<TFrom>
        where TTo : INumberBase<TTo>
        => new((value, _) => ValueTask.FromResult(TTo.CreateChecked(value)));

    /// <summary>
    /// Converts with <see cref="INumberBase{TSelf}.CreateSaturating{TOther}"/>.
    /// Finite values outside the destination range become <c>MinValue</c> or <c>MaxValue</c>.
    /// Fractional values that fit are truncated toward zero. Non-finite values follow the runtime's BCL method
    /// and are not given a separate policy here; NaN in particular differs between .NET 8 and .NET 10.
    /// </summary>
    public static TransformerRule<TFrom, TTo> ConvertSaturating<TFrom, TTo>()
        where TFrom : INumberBase<TFrom>
        where TTo : INumberBase<TTo>
        => new((value, _) => ValueTask.FromResult(TTo.CreateSaturating(value)));

    /// <summary>
    /// Converts with <see cref="INumberBase{TSelf}.CreateTruncating{TOther}"/>.
    /// Fractional values are truncated toward zero. Out-of-range values follow the unchecked BCL conversion
    /// (300 as a byte is 44, and -1 as a byte is 255) and do not saturate or throw.
    /// Non-finite values follow the runtime's BCL method.
    /// </summary>
    public static TransformerRule<TFrom, TTo> ConvertTruncating<TFrom, TTo>()
        where TFrom : INumberBase<TFrom>
        where TTo : INumberBase<TTo>
        => new((value, _) => ValueTask.FromResult(TTo.CreateTruncating(value)));

    /// <summary>Reads the calendar date of a <see cref="DateTime"/> as displayed. <see cref="DateTime.Kind"/> is ignored and no UTC conversion is applied.</summary>
    public static TransformerRule<DateTime, DateOnly> ToDateOnlyFromDateTime()
        => new((value, _) => ValueTask.FromResult(DateOnly.FromDateTime(value)));

    /// <summary>
    /// Reads the wall-clock date of a <see cref="DateTimeOffset"/> in its own offset.
    /// This is <see cref="DateTimeOffset.DateTime"/>, not <see cref="DateTimeOffset.UtcDateTime"/>.
    /// </summary>
    public static TransformerRule<DateTimeOffset, DateOnly> ToDateOnlyFromDateTimeOffset()
        => new((value, _) => ValueTask.FromResult(DateOnly.FromDateTime(value.DateTime)));

    /// <summary>Reads the clock time of a <see cref="DateTime"/> as displayed. <see cref="DateTime.Kind"/> is ignored.</summary>
    public static TransformerRule<DateTime, TimeOnly> ToTimeOnlyFromDateTime()
        => new((value, _) => ValueTask.FromResult(TimeOnly.FromDateTime(value)));

    /// <summary>Reads the wall-clock time of a <see cref="DateTimeOffset"/> in its own offset, not the UTC time.</summary>
    public static TransformerRule<DateTimeOffset, TimeOnly> ToTimeOnlyFromDateTimeOffset()
        => new((value, _) => ValueTask.FromResult(TimeOnly.FromTimeSpan(value.TimeOfDay)));

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to <see cref="TimeOnly"/>.
    /// Negative spans and spans of one day or more throw <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    public static TransformerRule<TimeSpan, TimeOnly> ToTimeOnlyFromTimeSpan()
        => new((value, _) => ValueTask.FromResult(TimeOnly.FromTimeSpan(value)));

    /// <summary>Converts a <see cref="TimeOnly"/> to the matching <see cref="TimeSpan"/> within a day.</summary>
    public static TransformerRule<TimeOnly, TimeSpan> ToTimeSpan()
        => new((value, _) => ValueTask.FromResult(value.ToTimeSpan()));

    /// <summary>
    /// Builds a <see cref="DateTime"/> from a <see cref="DateOnly"/> by attaching <paramref name="time"/> and stamping <paramref name="kind"/>.
    /// The kind is stored. The clock value is not shifted into UTC or local time.
    /// </summary>
    public static TransformerRule<DateOnly, DateTime> ToDateTime(TimeOnly time, DateTimeKind kind)
        => new((value, _) => ValueTask.FromResult(value.ToDateTime(time, kind)));

    /// <summary>
    /// Builds a <see cref="DateTime"/> from a <see cref="TimeOnly"/> by attaching <paramref name="date"/> and stamping <paramref name="kind"/>.
    /// The kind is stored. The clock value is not shifted.
    /// </summary>
    public static TransformerRule<TimeOnly, DateTime> ToDateTime(DateOnly date, DateTimeKind kind)
        => new((value, _) => ValueTask.FromResult(date.ToDateTime(value, kind)));

    /// <summary>
    /// Builds a <see cref="DateTimeOffset"/> from a <see cref="DateTime"/> and an offset, using the BCL constructor
    /// so <see cref="DateTime.Kind"/> stays compatible with the offset. Utc requires a zero offset. Local requires
    /// the offset of the local zone for that instant. Unspecified accepts the supplied offset as the wall-clock offset.
    /// An offset that is not a whole number of minutes throws <see cref="ArgumentException"/> when the rule is created.
    /// An offset outside -14 to +14 hours throws <see cref="ArgumentOutOfRangeException"/> when the rule is created.
    /// A kind mismatch throws <see cref="ArgumentException"/> when the value is transformed.
    /// </summary>
    public static TransformerRule<DateTime, DateTimeOffset> ToDateTimeOffset(TimeSpan offset)
    {
        _ = new DateTimeOffset(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Unspecified), offset);
        return new TransformerRule<DateTime, DateTimeOffset>((value, _) => ValueTask.FromResult(new DateTimeOffset(value, offset)));
    }

    /// <summary>Returns the Unix time in seconds. The offset is already accounted for by <see cref="DateTimeOffset"/>.</summary>
    public static TransformerRule<DateTimeOffset, long> ToUnixTimeSeconds()
        => new((value, _) => ValueTask.FromResult(value.ToUnixTimeSeconds()));

    /// <summary>Returns the Unix time in milliseconds.</summary>
    public static TransformerRule<DateTimeOffset, long> ToUnixTimeMilliseconds()
        => new((value, _) => ValueTask.FromResult(value.ToUnixTimeMilliseconds()));

    /// <summary>
    /// Builds a <see cref="DateTimeOffset"/> from Unix seconds. The result uses a zero offset (UTC).
    /// Values outside the BCL range throw <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    public static TransformerRule<long, DateTimeOffset> FromUnixTimeSeconds()
        => new((value, _) => ValueTask.FromResult(DateTimeOffset.FromUnixTimeSeconds(value)));

    /// <summary>
    /// Builds a <see cref="DateTimeOffset"/> from Unix milliseconds. The result uses a zero offset (UTC).
    /// Values outside the BCL range throw <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    public static TransformerRule<long, DateTimeOffset> FromUnixTimeMilliseconds()
        => new((value, _) => ValueTask.FromResult(DateTimeOffset.FromUnixTimeMilliseconds(value)));

    /// <summary>
    /// Encodes text with <paramref name="encoding"/>. Allocates a new byte array.
    /// Invalid input uses that encoding's encoder fallback (replacement or exception); it is not overridden.
    /// A null string throws <see cref="ArgumentNullException"/>.
    /// </summary>
    public static TransformerRule<string, byte[]> Encode(Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(encoding);
        return new TransformerRule<string, byte[]>((value, _) =>
        {
            ArgumentNullException.ThrowIfNull(value);
            return ValueTask.FromResult(encoding.GetBytes(value));
        });
    }

    /// <summary>
    /// Decodes bytes with <paramref name="encoding"/>. Allocates a new string.
    /// Invalid bytes use that encoding's decoder fallback. A null array throws <see cref="ArgumentNullException"/>.
    /// </summary>
    public static TransformerRule<byte[], string> Decode(Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(encoding);
        return new TransformerRule<byte[], string>((value, _) =>
        {
            ArgumentNullException.ThrowIfNull(value);
            return ValueTask.FromResult(encoding.GetString(value));
        });
    }

    /// <summary>Encodes bytes as base-64 text with <see cref="Convert.ToBase64String(byte[], Base64FormattingOptions)"/>. Allocates the string.</summary>
    public static TransformerRule<byte[], string> ToBase64(Base64FormattingOptions options = Base64FormattingOptions.None)
    {
        return new TransformerRule<byte[], string>((value, _) =>
        {
            ArgumentNullException.ThrowIfNull(value);
            return ValueTask.FromResult(Convert.ToBase64String(value, options));
        });
    }

    /// <summary>
    /// Decodes base-64 text with <see cref="Convert.FromBase64String"/>. Allocates the byte array.
    /// Invalid text throws <see cref="FormatException"/>. The BCL's whitespace tolerance is left as-is.
    /// A null string throws <see cref="ArgumentNullException"/>.
    /// </summary>
    public static TransformerRule<string, byte[]> FromBase64()
    {
        return new TransformerRule<string, byte[]>((value, _) =>
        {
            ArgumentNullException.ThrowIfNull(value);
            return ValueTask.FromResult(Convert.FromBase64String(value));
        });
    }

    /// <summary>
    /// Encodes bytes as hexadecimal text. The default is uppercase, matching <see cref="Convert.ToHexString(byte[])"/>.
    /// Pass <c>false</c> for lowercase. Allocates one result string. An empty array returns <see cref="string.Empty"/>.
    /// </summary>
    public static TransformerRule<byte[], string> ToHex(bool uppercase = true)
    {
        return new TransformerRule<byte[], string>((value, _) =>
        {
            ArgumentNullException.ThrowIfNull(value);
            return ValueTask.FromResult(EncodeHex(value, uppercase));
        });
    }

    /// <summary>
    /// Decodes hexadecimal text. Both cases are accepted. An odd length or a non-hex character throws
    /// <see cref="FormatException"/>. A <c>0x</c> prefix is not accepted. An empty string returns an empty array.
    /// Allocates the byte array. A null string throws <see cref="ArgumentNullException"/>.
    /// </summary>
    public static TransformerRule<string, byte[]> FromHex()
    {
        return new TransformerRule<string, byte[]>((value, _) =>
        {
            ArgumentNullException.ThrowIfNull(value);
            return ValueTask.FromResult(DecodeHex(value));
        });
    }

    /// <summary>
    /// Adapts a synchronous function. Exceptions thrown by <paramref name="map"/> propagate from
    /// <see cref="TransformerRule{TIn,TOut}.TransformAsync"/> instead of being stored on the <see cref="ValueTask{TResult}"/>.
    /// </summary>
    public static TransformerRule<TIn, TOut> Map<TIn, TOut>(Func<TIn, TOut> map)
    {
        ArgumentNullException.ThrowIfNull(map);
        return new TransformerRule<TIn, TOut>((value, _) => ValueTask.FromResult(map(value)));
    }

    /// <summary>
    /// Adapts an asynchronous function. The cancellation token is checked once by
    /// <see cref="TransformerRule{TIn,TOut}.TransformAsync"/> and then passed to <paramref name="map"/>.
    /// The returned <see cref="ValueTask{TResult}"/> is the delegate's task, consumed by the caller and not awaited here.
    /// A delegate that waits on the token should use <c>ConfigureAwait(false)</c> when it does not need the caller's context.
    /// </summary>
    public static TransformerRule<TIn, TOut> MapAsync<TIn, TOut>(Func<TIn, CancellationToken, ValueTask<TOut>> map)
    {
        ArgumentNullException.ThrowIfNull(map);
        return new TransformerRule<TIn, TOut>(map);
    }

    private static void RequireFormat(string format)
    {
        ArgumentNullException.ThrowIfNull(format);
        if (format.Length == 0)
        {
            throw new ArgumentException("Format must be non-empty.", nameof(format));
        }
    }

    private static string[] CopyFormats(string[] formats)
    {
        ArgumentNullException.ThrowIfNull(formats);
        if (formats.Length == 0)
        {
            throw new ArgumentException("At least one format is required.", nameof(formats));
        }

        var copy = new string[formats.Length];
        for (var i = 0; i < formats.Length; i++)
        {
            if (string.IsNullOrEmpty(formats[i]))
            {
                throw new ArgumentException("Formats must be non-empty.", nameof(formats));
            }

            copy[i] = formats[i];
        }

        return copy;
    }

    private static string EncodeHex(byte[] bytes, bool uppercase)
    {
        if (bytes.Length == 0)
        {
            return string.Empty;
        }

        return string.Create(bytes.Length * 2, (Bytes: bytes, Uppercase: uppercase), static (destination, state) =>
        {
            ReadOnlySpan<char> alphabet = state.Uppercase ? "0123456789ABCDEF" : "0123456789abcdef";
            var data = state.Bytes;
            for (var i = 0; i < data.Length; i++)
            {
                var value = data[i];
                destination[i * 2] = alphabet[value >> 4];
                destination[(i * 2) + 1] = alphabet[value & 0x0F];
            }
        });
    }

    private static byte[] DecodeHex(string text)
    {
        if ((text.Length & 1) != 0)
        {
            throw new FormatException("Hexadecimal text must contain an even number of characters.");
        }

        var bytes = new byte[text.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            var high = HexValue(text[i * 2]);
            var low = HexValue(text[(i * 2) + 1]);
            bytes[i] = (byte)((high << 4) | low);
        }

        return bytes;
    }

    private static int HexValue(char character) => character switch
    {
        >= '0' and <= '9' => character - '0',
        >= 'A' and <= 'F' => character - 'A' + 10,
        >= 'a' and <= 'f' => character - 'a' + 10,
        _ => throw new FormatException($"'{character}' is not a hexadecimal character."),
    };

    private static TransformerRule<T, T> Same<T>(Func<T, T> map)
        => new((value, _) => ValueTask.FromResult(map(value)));
}
