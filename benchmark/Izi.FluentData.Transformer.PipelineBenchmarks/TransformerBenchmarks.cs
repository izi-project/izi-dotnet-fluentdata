using System.Globalization;
using BenchmarkDotNet.Attributes;

namespace Izi.FluentData.Transformer.Benchmarks;

/// <summary>
/// Measures <see cref="Transformer{T}.TransformAsync"/> and a few catalogue rules.
/// The synchronous pipelines are the library fast path. Nothing here reimplements it.
/// </summary>
[MemoryDiagnoser]
public class TransformerBenchmarks
{
    private const string RawName = "  Ada  ";
    private const string RawEmail = "  Ada@Example.COM  ";
    private const string RawAgeText = "  42 ";
    private const string ExpectedName = "Ada";
    private const string ExpectedEmail = "ada@example.com";
    private const int ExpectedAge = 42;
    private const double RoundInput = 3.14159;

    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    private readonly Transformer<PersonClass> _noRules = new();
    private readonly PersonClass _noRulesInput = NewClass();
    private readonly Transformer<PersonClass> _syncClass = CreateSyncClass();
    private readonly PersonClass _syncClassInput = NewClass();
    private readonly Transformer<PersonStruct> _syncStruct = CreateSyncStruct();
    private readonly PersonStruct _syncStructInput = NewStruct();
    private readonly Transformer<PersonRecord> _syncRecord = CreateSyncRecord();
    private readonly PersonRecord _syncRecordInput = NewRecord();
    private readonly Transformer<PersonClass> _mixed = CreateMixed();
    private readonly PersonClass _mixedInput = NewClass();
    private readonly TransformerRule<string, string> _trim = TransformerRules.Trim();
    private readonly TransformerRule<string, int> _parse = TransformerRules.Parse<int>(Invariant);
    private readonly TransformerRule<double, double> _round = TransformerRules.Round<double>(2);

    /// <summary>Runs every scenario once and throws if a result is wrong.</summary>
    /// <returns>A completed task when every scenario matches the expected values.</returns>
    public static async Task VerifyAsync()
    {
        var benchmarks = new TransformerBenchmarks();

        var untouched = await benchmarks._noRules.TransformAsync(benchmarks._noRulesInput).ConfigureAwait(false);
        Expect(ReferenceEquals(untouched, benchmarks._noRulesInput), "No-rules transform returned a copy.");
        Expect(untouched.Name == RawName, "No-rules transform changed Name.");

        Reset(benchmarks._syncClassInput);
        var syncClass = await benchmarks._syncClass.TransformAsync(benchmarks._syncClassInput).ConfigureAwait(false);
        ExpectClass(syncClass, sameInstance: benchmarks._syncClassInput);

        var syncStruct = await benchmarks._syncStruct.TransformAsync(benchmarks._syncStructInput).ConfigureAwait(false);
        ExpectValues(syncStruct.Name, syncStruct.Email, syncStruct.AgeText, syncStruct.Age, "struct");
        Expect(benchmarks._syncStructInput.Name == RawName, "Struct transform wrote back to the caller.");

        var syncRecord = await benchmarks._syncRecord.TransformAsync(benchmarks._syncRecordInput).ConfigureAwait(false);
        ExpectValues(syncRecord.Name, syncRecord.Email, syncRecord.AgeText, syncRecord.Age, "record");
        Expect(!ReferenceEquals(syncRecord, benchmarks._syncRecordInput), "Init-only record was mutated in place.");
        Expect(benchmarks._syncRecordInput.Name == RawName, "Init-only record original Name changed.");
        Expect(benchmarks._syncRecordInput.Age == 0, "Init-only record original Age changed.");

        Reset(benchmarks._mixedInput);
        var mixed = await benchmarks._mixed.TransformAsync(benchmarks._mixedInput).ConfigureAwait(false);
        ExpectClass(mixed, sameInstance: benchmarks._mixedInput);

        var trimmed = await benchmarks._trim.TransformAsync(RawName).ConfigureAwait(false);
        Expect(trimmed == ExpectedName, "Trim returned '" + trimmed + "'.");

        var parsed = await benchmarks._parse.TransformAsync("42").ConfigureAwait(false);
        Expect(parsed == ExpectedAge, "Parse returned " + parsed + ".");

        var rounded = await benchmarks._round.TransformAsync(RoundInput).ConfigureAwait(false);
        Expect(rounded == double.Round(RoundInput, 2), "Round returned " + rounded + ".");
        Expect(rounded != RoundInput, "Round left the input unchanged.");
    }

    /// <summary>Baseline: <see cref="Transformer{T}.TransformAsync"/> with no rules.</summary>
    /// <returns>The same instance that was passed in.</returns>
    [Benchmark(Baseline = true)]
    public ValueTask<PersonClass> NoRules() => _noRules.TransformAsync(_noRulesInput);

    /// <summary>All-sync class: trim Name, trim and lower Email, trim and parse Age.</summary>
    /// <returns>The mutated class.</returns>
    [Benchmark]
    public ValueTask<PersonClass> SyncClass()
    {
        Reset(_syncClassInput);
        return _syncClass.TransformAsync(_syncClassInput);
    }

    /// <summary>The same all-sync rules on a struct. The returned copy is the result.</summary>
    /// <returns>The updated struct.</returns>
    [Benchmark]
    public ValueTask<PersonStruct> SyncStruct() => _syncStruct.TransformAsync(_syncStructInput);

    /// <summary>The same all-sync rules on a record with init-only properties.</summary>
    /// <returns>A clone with the converted properties.</returns>
    [Benchmark]
    public ValueTask<PersonRecord> SyncRecord() => _syncRecord.TransformAsync(_syncRecordInput);

    /// <summary>The class pipeline with <see cref="Task.Yield"/> between trim and lower on Email.</summary>
    /// <returns>The mutated class.</returns>
    [Benchmark]
    public ValueTask<PersonClass> MixedYield()
    {
        Reset(_mixedInput);
        return _mixed.TransformAsync(_mixedInput);
    }

    /// <summary><see cref="TransformerRules.Trim"/> called directly.</summary>
    /// <returns>The trimmed name.</returns>
    [Benchmark]
    public ValueTask<string> Trim() => _trim.TransformAsync(RawName);

    /// <summary><see cref="TransformerRules.Parse{T}"/> for <see cref="int"/> called directly.</summary>
    /// <returns>The parsed age.</returns>
    [Benchmark]
    public ValueTask<int> ParseInt() => _parse.TransformAsync("42");

    /// <summary><see cref="TransformerRules.Round{TNumber}(int)"/> for <see cref="double"/> called directly.</summary>
    /// <returns>The rounded value.</returns>
    [Benchmark]
    public ValueTask<double> Round() => _round.TransformAsync(RoundInput);

    /// <summary>Class with settable properties. Convert reads <see cref="AgeText"/> and writes <see cref="Age"/>.</summary>
    public sealed class PersonClass
    {
        /// <summary>Display name. Trimmed by the sync and mixed pipelines.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Email. Trimmed, then lowercased with the invariant culture.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Age text. Trimmed and parsed. The pipeline does not write this property back.</summary>
        public string AgeText { get; set; } = string.Empty;

        /// <summary>Parsed age.</summary>
        public int Age { get; set; }
    }

    /// <summary>Struct with the same settable properties as <see cref="PersonClass"/>.</summary>
    public struct PersonStruct
    {
        /// <summary>Display name.</summary>
        public string Name { get; set; }

        /// <summary>Email.</summary>
        public string Email { get; set; }

        /// <summary>Age text. Not written back.</summary>
        public string AgeText { get; set; }

        /// <summary>Parsed age.</summary>
        public int Age { get; set; }
    }

    /// <summary>Record with init-only properties. Transform clones it before writing.</summary>
    public sealed record PersonRecord
    {
        /// <summary>Display name.</summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>Email.</summary>
        public string Email { get; init; } = string.Empty;

        /// <summary>Age text. Not written back.</summary>
        public string AgeText { get; init; } = string.Empty;

        /// <summary>Parsed age.</summary>
        public int Age { get; init; }
    }

    private static Transformer<PersonClass> CreateSyncClass()
    {
        var transformer = new Transformer<PersonClass>();
        transformer.RuleFor(person => person.Name).Add(TransformerRules.Trim());
        transformer.RuleFor(person => person.Email)
            .Add(TransformerRules.Trim())
            .Add(TransformerRules.ToLower());
        transformer.RuleFor(person => person.AgeText)
            .Add(TransformerRules.Trim())
            .Convert(TransformerRules.Parse<int>(Invariant))
            .Into(person => person.Age);
        return transformer;
    }

    private static Transformer<PersonStruct> CreateSyncStruct()
    {
        var transformer = new Transformer<PersonStruct>();
        transformer.RuleFor(person => person.Name).Add(TransformerRules.Trim());
        transformer.RuleFor(person => person.Email)
            .Add(TransformerRules.Trim())
            .Add(TransformerRules.ToLower());
        transformer.RuleFor(person => person.AgeText)
            .Add(TransformerRules.Trim())
            .Convert(TransformerRules.Parse<int>(Invariant))
            .Into(person => person.Age);
        return transformer;
    }

    private static Transformer<PersonRecord> CreateSyncRecord()
    {
        var transformer = new Transformer<PersonRecord>();
        transformer.RuleFor(person => person.Name).Add(TransformerRules.Trim());
        transformer.RuleFor(person => person.Email)
            .Add(TransformerRules.Trim())
            .Add(TransformerRules.ToLower());
        transformer.RuleFor(person => person.AgeText)
            .Add(TransformerRules.Trim())
            .Convert(TransformerRules.Parse<int>(Invariant))
            .Into(person => person.Age);
        return transformer;
    }

    private static Transformer<PersonClass> CreateMixed()
    {
        var transformer = new Transformer<PersonClass>();
        transformer.RuleFor(person => person.Name).Add(TransformerRules.Trim());
        transformer.RuleFor(person => person.Email)
            .Add(TransformerRules.Trim())
            .Add(static async (email, _) =>
            {
                await Task.Yield();
                return email;
            })
            .Add(TransformerRules.ToLower());
        transformer.RuleFor(person => person.AgeText)
            .Add(TransformerRules.Trim())
            .Convert(TransformerRules.Parse<int>(Invariant))
            .Into(person => person.Age);
        return transformer;
    }

    private static PersonClass NewClass()
    {
        return new PersonClass
        {
            Name = RawName,
            Email = RawEmail,
            AgeText = RawAgeText,
        };
    }

    private static PersonStruct NewStruct()
    {
        return new PersonStruct
        {
            Name = RawName,
            Email = RawEmail,
            AgeText = RawAgeText,
        };
    }

    private static PersonRecord NewRecord()
    {
        return new PersonRecord
        {
            Name = RawName,
            Email = RawEmail,
            AgeText = RawAgeText,
        };
    }

    private static void Reset(PersonClass person)
    {
        person.Name = RawName;
        person.Email = RawEmail;
        person.AgeText = RawAgeText;
        person.Age = 0;
    }

    private static void ExpectClass(PersonClass person, PersonClass sameInstance)
    {
        Expect(ReferenceEquals(person, sameInstance), "Class transform returned a different instance.");
        ExpectValues(person.Name, person.Email, person.AgeText, person.Age, "class");
    }

    private static void ExpectValues(string name, string email, string ageText, int age, string kind)
    {
        Expect(name == ExpectedName, kind + " Name was '" + name + "'.");
        Expect(email == ExpectedEmail, kind + " Email was '" + email + "'.");
        Expect(ageText == RawAgeText, kind + " AgeText was written back as '" + ageText + "'.");
        Expect(age == ExpectedAge, kind + " Age was " + age + ".");
    }

    private static void Expect(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
