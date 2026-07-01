using System.Text.RegularExpressions;

namespace Izi.FluentData.Validation.Rules;

/// <summary>
/// Factory methods that create the built-in <see cref="ValidatorRule{T}"/> instances. Each rule has a
/// default-message overload and a custom-message overload, and the returned <see cref="ValidatorRule{T}"/> can be
/// further refined (<c>WithMessage</c>, <c>WithDependent(s)</c>). These are also surfaced as fluent
/// builder methods on <see cref="RuleBuilder{T}"/> (see <c>RuleBuilderExtensions</c>).
/// </summary>
public static partial class ValidatorRules
{
    // =============================
    // Null & Emptiness Checks
    // =============================

    /// <summary>Requires the value to be non-null.</summary>
    public static ValidatorRule<T> NotNull<T>() => NotNull<T>("Value cannot be null.");
    /// <summary>Requires the value to be non-null, with a custom message.</summary>
    public static ValidatorRule<T> NotNull<T>(string message) => new((value, _) => ValueTask.FromResult(value != null), message);

    /// <summary>Requires the value to be null.</summary>
    public static ValidatorRule<T> Null<T>() => Null<T>("Value must be null.");
    /// <summary>Requires the value to be null, with a custom message.</summary>
    public static ValidatorRule<T> Null<T>(string message) => new((value, _) => ValueTask.FromResult(value == null), message);

    /// <summary>Requires a non-empty value (non-blank string or non-empty collection).</summary>
    public static ValidatorRule<T> NotEmpty<T>() => NotEmpty<T>("Value cannot be empty.");
    /// <summary>Requires a non-empty value (non-blank string or non-empty collection), with a custom message.</summary>
    public static ValidatorRule<T> NotEmpty<T>(string message) => new((value, _) =>
    {
        if (value is null) return ValueTask.FromResult(false);
        if (value is string str) return ValueTask.FromResult(!string.IsNullOrWhiteSpace(str));
        if (value is IEnumerable<object> enumerable) return ValueTask.FromResult(enumerable.Any());
        return ValueTask.FromResult(true);
    }, message);

    /// <summary>Requires an empty value (null/blank string or empty collection).</summary>
    public static ValidatorRule<T> Empty<T>() => Empty<T>("Value must be empty.");
    /// <summary>Requires an empty value (null/blank string or empty collection), with a custom message.</summary>
    public static ValidatorRule<T> Empty<T>(string message) => new((value, _) =>
    {
        if (value is null) return ValueTask.FromResult(true);
        if (value is string str) return ValueTask.FromResult(string.IsNullOrWhiteSpace(str));
        if (value is IEnumerable<object> enumerable) return ValueTask.FromResult(!enumerable.Any());
        return ValueTask.FromResult(false);
    }, message);

    // =============================
    // Comparison Checks
    // =============================

    /// <summary>Requires the value to equal <paramref name="expectedValue"/>.</summary>
    public static ValidatorRule<T> Equal<T>(T expectedValue) => Equal(expectedValue, $"Value must be equal to {expectedValue}.");
    /// <summary>Requires the value to equal <paramref name="expectedValue"/>, with a custom message.</summary>
    public static ValidatorRule<T> Equal<T>(T expectedValue, string message) => new((value, _) => ValueTask.FromResult(EqualityComparer<T>.Default.Equals(value, expectedValue)), message);

    /// <summary>Requires the value to differ from <paramref name="expectedValue"/>.</summary>
    public static ValidatorRule<T> NotEqual<T>(T expectedValue) => NotEqual(expectedValue, $"Value must not be equal to {expectedValue}.");
    /// <summary>Requires the value to differ from <paramref name="expectedValue"/>, with a custom message.</summary>
    public static ValidatorRule<T> NotEqual<T>(T expectedValue, string message) => new((value, _) => ValueTask.FromResult(!EqualityComparer<T>.Default.Equals(value, expectedValue)), message);

    /// <summary>Requires the value to be strictly less than <paramref name="threshold"/>.</summary>
    public static ValidatorRule<T> LessThan<T>(T threshold) where T : IComparable<T> => LessThan(threshold, $"Value must be less than {threshold}.");
    /// <summary>Requires the value to be strictly less than <paramref name="threshold"/>, with a custom message.</summary>
    public static ValidatorRule<T> LessThan<T>(T threshold, string message) where T : IComparable<T> => new((value, _) => ValueTask.FromResult(value.CompareTo(threshold) < 0), message);

    /// <summary>Requires the value to be less than or equal to <paramref name="threshold"/>.</summary>
    public static ValidatorRule<T> LessThanOrEqual<T>(T threshold) where T : IComparable<T> => LessThanOrEqual(threshold, $"Value must be less than or equal to {threshold}.");
    /// <summary>Requires the value to be less than or equal to <paramref name="threshold"/>, with a custom message.</summary>
    public static ValidatorRule<T> LessThanOrEqual<T>(T threshold, string message) where T : IComparable<T> => new((value, _) => ValueTask.FromResult(value.CompareTo(threshold) <= 0), message);

    /// <summary>Requires the value to be strictly greater than <paramref name="threshold"/>.</summary>
    public static ValidatorRule<T> GreaterThan<T>(T threshold) where T : IComparable<T> => GreaterThan(threshold, $"Value must be greater than {threshold}.");
    /// <summary>Requires the value to be strictly greater than <paramref name="threshold"/>, with a custom message.</summary>
    public static ValidatorRule<T> GreaterThan<T>(T threshold, string message) where T : IComparable<T> => new((value, _) => ValueTask.FromResult(value.CompareTo(threshold) > 0), message);

    /// <summary>Requires the value to be greater than or equal to <paramref name="threshold"/>.</summary>
    public static ValidatorRule<T> GreaterThanOrEqual<T>(T threshold) where T : IComparable<T> => GreaterThanOrEqual(threshold, $"Value must be greater than or equal to {threshold}.");
    /// <summary>Requires the value to be greater than or equal to <paramref name="threshold"/>, with a custom message.</summary>
    public static ValidatorRule<T> GreaterThanOrEqual<T>(T threshold, string message) where T : IComparable<T> => new((value, _) => ValueTask.FromResult(value.CompareTo(threshold) >= 0), message);

    // =============================
    // Length & Range Checks
    // =============================

    /// <summary>Requires a string or collection to have an exact length/count of <paramref name="expectedLength"/>.</summary>
    public static ValidatorRule<T> Length<T>(int expectedLength) => Length<T>(expectedLength, $"Value must have a length of {expectedLength}.");
    /// <summary>Requires a string or collection to have an exact length/count of <paramref name="expectedLength"/>, with a custom message.</summary>
    public static ValidatorRule<T> Length<T>(int expectedLength, string message) => new((value, _) =>
    {
        if (value is null) return ValueTask.FromResult(false);
        if (value is string str) return ValueTask.FromResult(str.Length == expectedLength);
        if (value is IEnumerable<object> enumerable) return ValueTask.FromResult(enumerable.Count() == expectedLength);
        return ValueTask.FromResult(false);
    }, message);

    /// <summary>Requires a string or collection to meet a minimum length/count of <paramref name="minLength"/>.</summary>
    public static ValidatorRule<T> MinLength<T>(int minLength) => MinLength<T>(minLength, $"Value must have a minimum length of {minLength}.");
    /// <summary>Requires a string or collection to meet a minimum length/count of <paramref name="minLength"/>, with a custom message.</summary>
    public static ValidatorRule<T> MinLength<T>(int minLength, string message) => new((value, _) =>
    {
        if (value is null) return ValueTask.FromResult(false);
        if (value is string str) return ValueTask.FromResult(str.Length >= minLength);
        if (value is IEnumerable<object> enumerable) return ValueTask.FromResult(enumerable.Count() >= minLength);
        return ValueTask.FromResult(false);
    }, message);

    /// <summary>Requires a string or collection to stay within a maximum length/count of <paramref name="maxLength"/>.</summary>
    public static ValidatorRule<T> MaxLength<T>(int maxLength) => MaxLength<T>(maxLength, $"Value must have a maximum length of {maxLength}.");
    /// <summary>Requires a string or collection to stay within a maximum length/count of <paramref name="maxLength"/>, with a custom message.</summary>
    public static ValidatorRule<T> MaxLength<T>(int maxLength, string message) => new((value, _) =>
    {
        if (value is null) return ValueTask.FromResult(false);
        if (value is string str) return ValueTask.FromResult(str.Length <= maxLength);
        if (value is IEnumerable<object> enumerable) return ValueTask.FromResult(enumerable.Count() <= maxLength);
        return ValueTask.FromResult(false);
    }, message);

    /// <summary>Requires the value to fall within the inclusive range <c>[<paramref name="minValue"/>, <paramref name="maxValue"/>]</c>.</summary>
    public static ValidatorRule<T> Range<T>(T minValue, T maxValue) where T : IComparable<T> => Range(minValue, maxValue, $"Value must be between {minValue} and {maxValue}.");
    /// <summary>Requires the value to fall within the inclusive range <c>[<paramref name="minValue"/>, <paramref name="maxValue"/>]</c>, with a custom message.</summary>
    public static ValidatorRule<T> Range<T>(T minValue, T maxValue, string message) where T : IComparable<T> => new((value, _) => ValueTask.FromResult(value.CompareTo(minValue) >= 0 && value.CompareTo(maxValue) <= 0), message);

    /// <summary>Requires the value to fall outside the inclusive range <c>[<paramref name="minValue"/>, <paramref name="maxValue"/>]</c>.</summary>
    public static ValidatorRule<T> NotRange<T>(T minValue, T maxValue) where T : IComparable<T> => NotRange(minValue, maxValue, $"Value must not be between {minValue} and {maxValue}.");
    /// <summary>Requires the value to fall outside the inclusive range <c>[<paramref name="minValue"/>, <paramref name="maxValue"/>]</c>, with a custom message.</summary>
    public static ValidatorRule<T> NotRange<T>(T minValue, T maxValue, string message) where T : IComparable<T> => new((value, _) => ValueTask.FromResult(value.CompareTo(minValue) < 0 || value.CompareTo(maxValue) > 0), message);

    // =============================
    // Format & Pattern Matching
    // =============================

    /// <summary>Requires a decimal value to respect a maximum scale and precision.</summary>
    public static ValidatorRule<T> ScalePrecision<T>(int maxScale, int maxPrecision) => ScalePrecision<T>(maxScale, maxPrecision, $"Value must have a maximum scale of {maxScale} and a maximum precision of {maxPrecision}.");
    /// <summary>Requires a decimal value to respect a maximum scale and precision, with a custom message.</summary>
    public static ValidatorRule<T> ScalePrecision<T>(int maxScale, int maxPrecision, string message) => new((value, _) =>
    {
        if (value is decimal dec)
        {
            var scale = BitConverter.GetBytes(decimal.GetBits(dec)[3])[2];
            var precision = (int)Math.Floor(Math.Log10((double)dec)) + 1;
            return ValueTask.FromResult(scale <= maxScale && precision <= maxPrecision);
        }
        return ValueTask.FromResult(false);
    }, message);

    /// <summary>Requires a string to match the regular expression <paramref name="pattern"/>.</summary>
    public static ValidatorRule<T> Matches<T>(string pattern) => Matches<T>(pattern, $"Value must match the pattern: {pattern}.");
    /// <summary>Requires a string to match the regular expression <paramref name="pattern"/>, with a custom message.</summary>
    public static ValidatorRule<T> Matches<T>(string pattern, string message) => Matches<T>(new Regex(pattern), message);
    /// <summary>Requires a string to match the precompiled regular expression <paramref name="regex"/>, with a custom message.</summary>
    public static ValidatorRule<T> Matches<T>(Regex regex, string message) => new((value, _) => ValueTask.FromResult(value is string str && regex.IsMatch(str)), message);

    /// <summary>Requires a string to not match the regular expression <paramref name="pattern"/>.</summary>
    public static ValidatorRule<T> NotMatches<T>(string pattern) => NotMatches<T>(pattern, $"Value must not match the pattern: {pattern}.");
    /// <summary>Requires a string to not match the regular expression <paramref name="pattern"/>, with a custom message.</summary>
    public static ValidatorRule<T> NotMatches<T>(string pattern, string message) => NotMatches<T>(new Regex(pattern), message);
    /// <summary>Requires a string to not match the precompiled regular expression <paramref name="regex"/>, with a custom message.</summary>
    public static ValidatorRule<T> NotMatches<T>(Regex regex, string message) => new((value, _) => ValueTask.FromResult(value is not string str || !regex.IsMatch(str)), message);

    /// <summary>Requires a string to be a valid email address.</summary>
    public static ValidatorRule<T> Email<T>() => Email<T>("Value must be a valid email address.");
    /// <summary>Requires a string to be a valid email address, with a custom message.</summary>
    public static ValidatorRule<T> Email<T>(string message) => Matches<T>(ValidationRegex.Email(), message);

    /// <summary>Requires a string to be a valid credit-card number.</summary>
    public static ValidatorRule<T> CreditCard<T>() => CreditCard<T>("Value must be a valid credit card number.");
    /// <summary>Requires a string to be a valid credit-card number, with a custom message.</summary>
    public static ValidatorRule<T> CreditCard<T>(string message) => Matches<T>(ValidationRegex.CreditCard(), message);

    // =============================
    // ISO Code Checks
    // =============================

    /// <summary>Requires a string to be a valid ISO 3166-1 alpha-2 country code (e.g. <c>US</c>); case-insensitive.</summary>
    public static ValidatorRule<T> CountryIso2<T>() => CountryIso2<T>("Value must be a valid ISO 3166-1 alpha-2 country code.");
    /// <summary>Requires a string to be a valid ISO 3166-1 alpha-2 country code, with a custom message.</summary>
    public static ValidatorRule<T> CountryIso2<T>(string message) => new((value, _) => ValueTask.FromResult(value is string str && IsoCodes.CountryAlpha2.Contains(str)), message);

    /// <summary>Requires a string to be a valid ISO 3166-1 alpha-3 country code (e.g. <c>USA</c>); case-insensitive.</summary>
    public static ValidatorRule<T> CountryIso3<T>() => CountryIso3<T>("Value must be a valid ISO 3166-1 alpha-3 country code.");
    /// <summary>Requires a string to be a valid ISO 3166-1 alpha-3 country code, with a custom message.</summary>
    public static ValidatorRule<T> CountryIso3<T>(string message) => new((value, _) => ValueTask.FromResult(value is string str && IsoCodes.CountryAlpha3.Contains(str)), message);

    /// <summary>Requires a string to be a valid ISO 3166-1 numeric country code (e.g. <c>840</c>; leading zeros preserved).</summary>
    public static ValidatorRule<T> CountryIsoNumeric<T>() => CountryIsoNumeric<T>("Value must be a valid ISO 3166-1 numeric country code.");
    /// <summary>Requires a string to be a valid ISO 3166-1 numeric country code, with a custom message.</summary>
    public static ValidatorRule<T> CountryIsoNumeric<T>(string message) => new((value, _) => ValueTask.FromResult(value is string str && IsoCodes.CountryNumeric.Contains(str)), message);

    /// <summary>Requires a string to be a valid ISO 4217 currency code (e.g. <c>USD</c>); case-insensitive.</summary>
    public static ValidatorRule<T> CurrencyIso<T>() => CurrencyIso<T>("Value must be a valid ISO 4217 currency code.");
    /// <summary>Requires a string to be a valid ISO 4217 currency code, with a custom message.</summary>
    public static ValidatorRule<T> CurrencyIso<T>(string message) => new((value, _) => ValueTask.FromResult(value is string str && IsoCodes.Currency.Contains(str)), message);
}

// [GeneratedRegex] cannot be declared in a generic type, so the compile-time
// patterns live in this non-generic holder and are reused by the rules above.

/// <summary>Holds the source-generated regular expressions reused by the format rules.</summary>
internal static partial class ValidationRegex
{
    /// <summary>A simple email-shaped pattern.</summary>
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    public static partial Regex Email();

    /// <summary>A pattern matching the common credit-card number formats.</summary>
    [GeneratedRegex(@"^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|3[47][0-9]{13}|6(?:011|5[0-9]{2})[0-9]{12}|(?:2131|1800|35\d{3})\d{11})$")]
    public static partial Regex CreditCard();
}

internal static class IsoCodes
{
    // One row per country — "alpha2 alpha3 numeric" — kept aligned so the three lookup sets
    // below are derived from a single source of truth and cannot drift out of sync.
    private static readonly string[] CountryRows =
    [
        "AF AFG 004", "AX ALA 248", "AL ALB 008", "DZ DZA 012", "AS ASM 016", "AD AND 020",
        "AO AGO 024", "AI AIA 660", "AQ ATA 010", "AG ATG 028", "AR ARG 032", "AM ARM 051",
        "AW ABW 533", "AU AUS 036", "AT AUT 040", "AZ AZE 031", "BS BHS 044", "BH BHR 048",
        "BD BGD 050", "BB BRB 052", "BY BLR 112", "BE BEL 056", "BZ BLZ 084", "BJ BEN 204",
        "BM BMU 060", "BT BTN 064", "BO BOL 068", "BQ BES 535", "BA BIH 070", "BW BWA 072",
        "BV BVT 074", "BR BRA 076", "IO IOT 086", "BN BRN 096", "BG BGR 100", "BF BFA 854",
        "BI BDI 108", "CV CPV 132", "KH KHM 116", "CM CMR 120", "CA CAN 124", "KY CYM 136",
        "CF CAF 140", "TD TCD 148", "CL CHL 152", "CN CHN 156", "CX CXR 162", "CC CCK 166",
        "CO COL 170", "KM COM 174", "CG COG 178", "CD COD 180", "CK COK 184", "CR CRI 188",
        "CI CIV 384", "HR HRV 191", "CU CUB 192", "CW CUW 531", "CY CYP 196", "CZ CZE 203",
        "DK DNK 208", "DJ DJI 262", "DM DMA 212", "DO DOM 214", "EC ECU 218", "EG EGY 818",
        "SV SLV 222", "GQ GNQ 226", "ER ERI 232", "EE EST 233", "SZ SWZ 748", "ET ETH 231",
        "FK FLK 238", "FO FRO 234", "FJ FJI 242", "FI FIN 246", "FR FRA 250", "GF GUF 254",
        "PF PYF 258", "TF ATF 260", "GA GAB 266", "GM GMB 270", "GE GEO 268", "DE DEU 276",
        "GH GHA 288", "GI GIB 292", "GR GRC 300", "GL GRL 304", "GD GRD 308", "GP GLP 312",
        "GU GUM 316", "GT GTM 320", "GG GGY 831", "GN GIN 324", "GW GNB 624", "GY GUY 328",
        "HT HTI 332", "HM HMD 334", "VA VAT 336", "HN HND 340", "HK HKG 344", "HU HUN 348",
        "IS ISL 352", "IN IND 356", "ID IDN 360", "IR IRN 364", "IQ IRQ 368", "IE IRL 372",
        "IM IMN 833", "IL ISR 376", "IT ITA 380", "JM JAM 388", "JP JPN 392", "JE JEY 832",
        "JO JOR 400", "KZ KAZ 398", "KE KEN 404", "KI KIR 296", "KP PRK 408", "KR KOR 410",
        "KW KWT 414", "KG KGZ 417", "LA LAO 418", "LV LVA 428", "LB LBN 422", "LS LSO 426",
        "LR LBR 430", "LY LBY 434", "LI LIE 438", "LT LTU 440", "LU LUX 442", "MO MAC 446",
        "MG MDG 450", "MW MWI 454", "MY MYS 458", "MV MDV 462", "ML MLI 466", "MT MLT 470",
        "MH MHL 584", "MQ MTQ 474", "MR MRT 478", "MU MUS 480", "YT MYT 175", "MX MEX 484",
        "FM FSM 583", "MD MDA 498", "MC MCO 492", "MN MNG 496", "ME MNE 499", "MS MSR 500",
        "MA MAR 504", "MZ MOZ 508", "MM MMR 104", "NA NAM 516", "NR NRU 520", "NP NPL 524",
        "NL NLD 528", "NC NCL 540", "NZ NZL 554", "NI NIC 558", "NE NER 562", "NG NGA 566",
        "NU NIU 570", "NF NFK 574", "MK MKD 807", "MP MNP 580", "NO NOR 578", "OM OMN 512",
        "PK PAK 586", "PW PLW 585", "PS PSE 275", "PA PAN 591", "PG PNG 598", "PY PRY 600",
        "PE PER 604", "PH PHL 608", "PN PCN 612", "PL POL 616", "PT PRT 620", "PR PRI 630",
        "QA QAT 634", "RE REU 638", "RO ROU 642", "RU RUS 643", "RW RWA 646", "BL BLM 652",
        "SH SHN 654", "KN KNA 659", "LC LCA 662", "MF MAF 663", "PM SPM 666", "VC VCT 670",
        "WS WSM 882", "SM SMR 674", "ST STP 678", "SA SAU 682", "SN SEN 686", "RS SRB 688",
        "SC SYC 690", "SL SLE 694", "SG SGP 702", "SX SXM 534", "SK SVK 703", "SI SVN 705",
        "SB SLB 090", "SO SOM 706", "ZA ZAF 710", "GS SGS 239", "SS SSD 728", "ES ESP 724",
        "LK LKA 144", "SD SDN 729", "SR SUR 740", "SJ SJM 744", "SE SWE 752", "CH CHE 756",
        "SY SYR 760", "TW TWN 158", "TJ TJK 762", "TZ TZA 834", "TH THA 764", "TL TLS 626",
        "TG TGO 768", "TK TKL 772", "TO TON 776", "TT TTO 780", "TN TUN 788", "TR TUR 792",
        "TM TKM 795", "TC TCA 796", "TV TUV 798", "UG UGA 800", "UA UKR 804", "AE ARE 784",
        "GB GBR 826", "US USA 840", "UM UMI 581", "UY URY 858", "UZ UZB 860", "VU VUT 548",
        "VE VEN 862", "VN VNM 704", "VG VGB 092", "VI VIR 850", "WF WLF 876", "EH ESH 732",
        "YE YEM 887", "ZM ZMB 894", "ZW ZWE 716",
    ];

    public static readonly HashSet<string> CountryAlpha2;
    public static readonly HashSet<string> CountryAlpha3;
    public static readonly HashSet<string> CountryNumeric;

    static IsoCodes()
    {
        // Alpha codes are matched case-insensitively; numeric codes are exact (3 digits, leading zeros kept).
        CountryAlpha2 = new(CountryRows.Length, StringComparer.OrdinalIgnoreCase);
        CountryAlpha3 = new(CountryRows.Length, StringComparer.OrdinalIgnoreCase);
        CountryNumeric = new(CountryRows.Length, StringComparer.Ordinal);

        foreach (var row in CountryRows)
        {
            var parts = row.Split(' ');
            CountryAlpha2.Add(parts[0]);
            CountryAlpha3.Add(parts[1]);
            CountryNumeric.Add(parts[2]);
        }
    }

    // ISO 4217 active currency codes (alpha-3), including the standard fund/precious-metal X-codes.
    public static readonly HashSet<string> Currency = new(StringComparer.OrdinalIgnoreCase)
    {
        "AED", "AFN", "ALL", "AMD", "ANG", "AOA", "ARS", "AUD", "AWG", "AZN", "BAM", "BBD",
        "BDT", "BGN", "BHD", "BIF", "BMD", "BND", "BOB", "BOV", "BRL", "BSD", "BTN", "BWP",
        "BYN", "BZD", "CAD", "CDF", "CHE", "CHF", "CHW", "CLF", "CLP", "CNY", "COP", "COU",
        "CRC", "CUC", "CUP", "CVE", "CZK", "DJF", "DKK", "DOP", "DZD", "EGP", "ERN", "ETB",
        "EUR", "FJD", "FKP", "GBP", "GEL", "GHS", "GIP", "GMD", "GNF", "GTQ", "GYD", "HKD",
        "HNL", "HTG", "HUF", "IDR", "ILS", "INR", "IQD", "IRR", "ISK", "JMD", "JOD", "JPY",
        "KES", "KGS", "KHR", "KMF", "KPW", "KRW", "KWD", "KYD", "KZT", "LAK", "LBP", "LKR",
        "LRD", "LSL", "LYD", "MAD", "MDL", "MGA", "MKD", "MMK", "MNT", "MOP", "MRU", "MUR",
        "MVR", "MWK", "MXN", "MXV", "MYR", "MZN", "NAD", "NGN", "NIO", "NOK", "NPR", "NZD",
        "OMR", "PAB", "PEN", "PGK", "PHP", "PKR", "PLN", "PYG", "QAR", "RON", "RSD", "RUB",
        "RWF", "SAR", "SBD", "SCR", "SDG", "SEK", "SGD", "SHP", "SLE", "SOS", "SRD", "SSP",
        "STN", "SVC", "SYP", "SZL", "THB", "TJS", "TMT", "TND", "TOP", "TRY", "TTD", "TWD",
        "TZS", "UAH", "UGX", "USD", "USN", "UYI", "UYU", "UYW", "UZS", "VED", "VES", "VND",
        "VUV", "WST", "XAF", "XAG", "XAU", "XBA", "XBB", "XBC", "XBD", "XCD", "XDR", "XOF",
        "XPD", "XPF", "XPT", "XSU", "XTS", "XUA", "XXX", "YER", "ZAR", "ZMW", "ZWG",
    };
}
