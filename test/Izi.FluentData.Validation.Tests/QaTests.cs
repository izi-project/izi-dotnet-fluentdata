using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Izi.FluentData.Validation.Tests;

public class QaTests
{
    private sealed class Person
    {
        public string? Name { get; set; }

        public int Age { get; set; }

        public Address? Address { get; set; }

        public List<string>? Items { get; set; }

        public string[]? Codes { get; set; }

        public IEnumerable<string>? Lazy { get; set; }

        public HashSet<string>? Tags { get; set; }

        public List<Address>? Places { get; set; }

        public IEnumerable<Address>? LazyPlaces { get; set; }

        public Money? Balance { get; set; }

        public string? Cur { get; set; }
    }

    private sealed class Address
    {
        public string? Street { get; set; }

        public string? City { get; set; }
    }

    private sealed class Node
    {
        public string? Name { get; set; }

        public Node? Manager { get; set; }
    }

    private sealed class Money
    {
        public string? Code { get; set; }

        public decimal Amount { get; set; }

        public static bool operator ==(Money? left, Money? right) => left!.Amount == right!.Amount;

        public static bool operator !=(Money? left, Money? right) => !(left == right);

        public override bool Equals(object? obj) => obj is Money other && Amount == other.Amount;

        public override int GetHashCode() => Amount.GetHashCode();
    }

    private sealed class TrackingSequence : IEnumerable<string>
    {
        public int Disposed { get; private set; }

        public IEnumerator<string> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private sealed class Enumerator : IEnumerator<string>
        {
            private readonly TrackingSequence _owner;
            private int _index = -1;

            public Enumerator(TrackingSequence owner) => _owner = owner;

            public string Current => _index == 0 ? "ok" : "";

            object IEnumerator.Current => Current;

            public bool MoveNext() => ++_index < 2;

            public void Reset() => _index = -1;

            public void Dispose() => _owner.Disposed++;
        }
    }

    [Fact]
    public async Task A_regex_timeout_propagates_from_matches_and_not_matches()
    {
        var regex = new Regex("^(a+)+$", RegexOptions.None, TimeSpan.FromMilliseconds(50));
        var hostile = new string('a', 28) + "!";
        var matches = new Validator<Person>();
        matches.RuleFor(person => person.Name).Matches(regex, "pattern");
        await Assert.ThrowsAsync<RegexMatchTimeoutException>(async () =>
            await matches.ValidateAsync(new Person { Name = hostile }).ConfigureAwait(true)).ConfigureAwait(true);

        var rejected = new Validator<Person>();
        rejected.RuleFor(person => person.Name).NotMatches(regex, "pattern");
        await Assert.ThrowsAsync<RegexMatchTimeoutException>(async () =>
            await rejected.ValidateAsync(new Person { Name = hostile }).ConfigureAwait(true)).ConfigureAwait(true);
    }

    [Fact]
    public async Task MustAsync_accepts_an_async_lambda_with_a_cancellation_token()
    {
        var validator = new Validator<Person>();
        validator.RuleFor(person => person.Name).MustAsync(async (string? name, CancellationToken token) =>
        {
            await Task.Yield();
            token.ThrowIfCancellationRequested();
            return name == "Ada";
        }, "Must be Ada.");

        var result = await validator.ValidateAsync(new Person { Name = "Bea" }).ConfigureAwait(true);
        Assert.Equal("Must be Ada.", Assert.Single(Assert.Single(result.Errors).Errors));
    }

    [Fact]
    public async Task A_custom_rule_can_report_a_failure()
    {
        var validator = new Validator<Person>();
        validator.RuleFor(person => person.Name).AddRule(new OddLength());
        var result = await validator.ValidateAsync(new Person { Name = "ab" }).ConfigureAwait(true);
        Assert.Equal("Must have an odd length.", Assert.Single(Assert.Single(result.Errors).Errors));
        Assert.True((await validator.ValidateAsync(new Person { Name = "a" }).ConfigureAwait(true)).IsValid);

        var many = ValidationRuleResult.Failed(ValidationError.From(new[] { "one", "two" }));
        Assert.Equal("one" + Environment.NewLine + "two", many.ToString());
        Assert.Equal("one|two", many.ToString("|"));
        Assert.Equal(string.Empty, ValidationRuleResult.Success.ToString());
        Assert.Equal(string.Empty, ValidationRuleResult.Success.ToString(", "));
        Assert.Equal("only", ValidationRuleResult.Failed("only").ToString());
        Assert.Same(ValidationRuleResult.Success, ValidationRuleResult.Failed(ValidationError.Empty));
        Assert.Equal("one, two", ValidationError.From(new[] { "one", "two" }).ToString());
        Assert.Throws<ArgumentOutOfRangeException>(() => ValidationError.From(new[] { "only" })[1]);
    }

    [Fact]
    public async Task NaN_fails_ordering_and_range_rules_and_is_rejected_as_a_bound()
    {
        await Fail(ValidatorRules.LessThan<double>(10), double.NaN);
        await Fail(ValidatorRules.LessThanOrEqual<double>(10), double.NaN);
        await Fail(ValidatorRules.GreaterThan<double>(10), double.NaN);
        await Fail(ValidatorRules.GreaterThanOrEqual<double>(10), double.NaN);
        await Fail(ValidatorRules.Range<double>(0, 10), double.NaN);
        await Fail(ValidatorRules.NotRange<double>(0, 10), double.NaN);
        await Fail(ValidatorRules.LessThan<float>(10), float.NaN);
        await Fail(ValidatorRules.NotRange<float>(0, 10), float.NaN);
        await Pass(ValidatorRules.GreaterThan<double>(0), double.PositiveInfinity);
        await Pass(ValidatorRules.LessThan<double>(0), double.NegativeInfinity);
        await Pass(ValidatorRules.Range<double>(double.NegativeInfinity, double.PositiveInfinity), 1d);
        Assert.True((await ValidatorRules.Equal<double>(double.NaN).ValidateAsync(double.NaN).ConfigureAwait(true)).IsValid);
        Assert.False((await ValidatorRules.NotEqual<double>(double.NaN).ValidateAsync(double.NaN).ConfigureAwait(true)).IsValid);
        Assert.Throws<ArgumentOutOfRangeException>(() => ValidatorRules.LessThan<double>(double.NaN));
        Assert.Throws<ArgumentOutOfRangeException>(() => ValidatorRules.Range<double>(double.NaN, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => ValidatorRules.NotRange<float>(0, float.NaN));
    }

    [Fact]
    public async Task String_ordering_is_ordinal_in_every_culture()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            foreach (var name in new[] { "en-US", "sv-SE", "tr-TR" })
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(name);
                await Fail(ValidatorRules.LessThan<string>("B"), "a");
                await Fail(ValidatorRules.GreaterThan<string>("ä"), "z");
                await Fail(ValidatorRules.GreaterThanOrEqual<string>("i"), "I");
            }
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }

    [Theory]
    [InlineData("a@b.co\n")]
    [InlineData("a@b.co\r\n")]
    [InlineData(" a@b.co")]
    [InlineData("a@b.co ")]
    public async Task Email_rejects_a_trailing_newline_and_surrounding_space(string value)
        => await Fail(ValidatorRules.Email<string>(), value);

    [Theory]
    [InlineData("4111111111111111\n")]
    [InlineData("4111111111111111\r\n")]
    public async Task CreditCard_rejects_a_trailing_newline(string value)
        => await Fail(ValidatorRules.CreditCard<string>(), value);

    [Fact]
    public async Task ScalePrecision_counts_integer_digits_exactly_and_does_not_allocate_when_valid()
    {
        await Pass(ValidatorRules.ScalePrecision<decimal>(2, 16), 9999999999999999m);
        await Pass(ValidatorRules.ScalePrecision<decimal>(2, 17), 99999999999999999.99m);
        await Pass(ValidatorRules.ScalePrecision<decimal>(2, 28), decimal.Parse(new string('9', 28), CultureInfo.InvariantCulture));
        await Pass(ValidatorRules.ScalePrecision<decimal>(2, 4), 0m);
        await Pass(ValidatorRules.ScalePrecision<decimal>(2, 4), -1.5m);
        await Fail(ValidatorRules.ScalePrecision<decimal>(1, 4), 1.10m);
        await Pass(ValidatorRules.ScalePrecision<decimal?>(2, 4), 1.5m);
        await Fail(ValidatorRules.ScalePrecision<decimal?>(2, 4), null);
        await Fail(ValidatorRules.ScalePrecision<int>(2, 4), 10);

        Assert.Equal(0, MeasureScale());
    }

    private static long MeasureScale()
    {
        var rule = ValidatorRules.ScalePrecision<decimal>(2, 16);
        var sample = 9999999999999999m;
        for (var i = 0; i < 20; i++)
        {
            var warmed = rule.ValidateAsync(sample).GetAwaiter().GetResult();
            if (!warmed.IsValid)
            {
                throw new InvalidOperationException(warmed.ToString());
            }
        }

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 200; i++)
        {
            var result = rule.ValidateAsync(sample).GetAwaiter().GetResult();
            if (!result.IsValid)
            {
                throw new InvalidOperationException(result.ToString());
            }
        }

        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    [Fact]
    public async Task A_parent_null_check_does_not_call_a_user_defined_equality_operator()
    {
        var validator = new Validator<Person>();
        validator.RuleFor(person => person.Balance!.Code).NotEmpty();
        var present = await validator.ValidateAsync(new Person { Balance = new Money { Amount = 1, Code = "" } }).ConfigureAwait(true);
        Assert.Equal("Balance.Code", Assert.Single(present.Errors).PropertyName);
        var missing = await validator.ValidateAsync(new Person()).ConfigureAwait(true);
        Assert.Equal("Balance", Assert.Single(missing.Errors).PropertyName);
        Assert.Equal("Must not be null.", Assert.Single(missing.Errors[0].Errors));
    }

    [Fact]
    public void Cyclic_include_is_rejected_when_the_validator_is_configured()
    {
        var first = new Validator<Person>();
        var second = new Validator<Person>();
        var third = new Validator<Person>();
        first.Include(second);
        second.Include(third);
        Assert.Throws<InvalidOperationException>(() => third.Include(first));
        Assert.Throws<InvalidOperationException>(() => first.Include(first));
    }

    [Fact]
    public async Task Cyclic_child_data_stops_with_invalid_operation_instead_of_overflowing()
    {
        var validator = new Validator<Node>();
        validator.RuleFor(node => node.Name).NotEmpty();
        validator.RuleFor(node => node.Manager!).SetValidator(validator);
        var node = new Node { Name = "Ada" };
        node.Manager = node;
        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await validator.ValidateAsync(node).ConfigureAwait(true)).ConfigureAwait(true);
        Assert.Contains("128", error.Message);
    }

    [Fact]
    public async Task WithMessage_does_not_change_a_run_that_is_already_in_progress()
    {
        var validator = new Validator<Person>();
        var started = new TaskCompletionSource<bool>();
        var release = new TaskCompletionSource<bool>();
        var property = validator.RuleFor(person => person.Name).MustAsync(async (string? _, CancellationToken _) =>
        {
            started.TrySetResult(true);
            await release.Task.ConfigureAwait(true);
            return false;
        }, "Old.");
        var pending = validator.ValidateAsync(new Person { Name = "Ada" });
        await started.Task.ConfigureAwait(true);
        property.WithMessage("Changed.");
        release.SetResult(true);
        var inflight = await pending.ConfigureAwait(true);
        Assert.Equal("Old.", Assert.Single(Assert.Single(inflight.Errors).Errors));
        var next = await validator.ValidateAsync(new Person { Name = "Ada" }).ConfigureAwait(true);
        Assert.Equal("Changed.", Assert.Single(Assert.Single(next.Errors).Errors));
    }

    [Fact]
    public async Task RuleForEach_async_paths_match_the_synchronous_errors()
    {
        var expected = new[] { "Items[1]" };
        await AssertIndexes(new List<string> { "ok", "" }, expected, yield: false);
        await AssertIndexes(new List<string> { "ok", "" }, expected, yield: true);
        await AssertIndexes(new[] { "ok", "" }, new[] { "Codes[1]" }, yield: true, name: "Codes");
        await AssertIndexes(LazyFail(), new[] { "Lazy[1]" }, yield: true, name: "Lazy");
        var tags = new HashSet<string> { "" };
        var tagsValidator = new Validator<Person>();
        tagsValidator.RuleForEach(person => person.Tags!).MustAsync(async (string value, CancellationToken _) =>
        {
            await Task.Yield();
            return !string.IsNullOrEmpty(value);
        }, "Must not be empty.");
        var tagsResult = await tagsValidator.ValidateAsync(new Person { Tags = tags }).ConfigureAwait(true);
        Assert.Equal("Tags[0]", Assert.Single(tagsResult.Errors).PropertyName);
    }

    [Fact]
    public async Task RuleForEach_disposes_the_enumerator_and_keeps_a_yielding_child()
    {
        var sequence = new TrackingSequence();
        var validator = new Validator<Person>();
        validator.RuleForEach(person => person.Lazy!).NotEmpty();
        var result = await validator.ValidateAsync(new Person { Lazy = sequence }).ConfigureAwait(true);
        Assert.Equal("Lazy[1]", Assert.Single(result.Errors).PropertyName);
        Assert.Equal(1, sequence.Disposed);

        var cancelled = new TrackingSequence();
        var stopping = new Validator<Person>();
        stopping.RuleForEach(person => person.Lazy!).MustAsync(async (string _, CancellationToken token) =>
        {
            await Task.Yield();
            throw new OperationCanceledException(token);
        }, "unused");
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await stopping.ValidateAsync(new Person { Lazy = cancelled }).ConfigureAwait(true)).ConfigureAwait(true);
        Assert.Equal(1, cancelled.Disposed);

        var child = new Validator<Address>();
        child.RuleFor(item => item.Street).MustAsync(async (string? text, CancellationToken _) =>
        {
            await Task.Yield();
            return !string.IsNullOrEmpty(text);
        }, "Must not be empty.");
        var nested = new Validator<Person>();
        nested.RuleForEach(person => person.Places!).SetValidator(child);
        var nestedResult = await nested.ValidateAsync(new Person
        {
            Places = [new Address { Street = "" }, new Address { Street = "ok" }],
        }).ConfigureAwait(true);
        Assert.Equal("Places[0].Street", Assert.Single(nestedResult.Errors).PropertyName);

        var lazyChild = new Validator<Address>();
        lazyChild.RuleFor(item => item.Street).MustAsync(async (string? text, CancellationToken _) =>
        {
            await Task.Yield();
            return !string.IsNullOrEmpty(text);
        }, "Must not be empty.");
        var lazyHost = new Validator<Person>();
        var property = lazyHost.RuleForEach(person => person.LazyPlaces!).SetValidator(lazyChild);
        var lazyResult = await property.ValidateAsync(new Person
        {
            LazyPlaces = LazyPlaces(),
        }).ConfigureAwait(true);
        Assert.Equal("LazyPlaces[1].Street", Assert.Single(lazyResult.Errors).PropertyName);
    }

    private static IEnumerable<Address> LazyPlaces()
    {
        yield return new Address { Street = "ok" };
        yield return new Address { Street = "" };
    }

    [Fact]
    public async Task Standalone_property_validation_returns_every_child_failure()
    {
        var child = new Validator<Address>();
        child.RuleFor(item => item.Street).NotEmpty();
        child.RuleFor(item => item.City).NotEmpty();
        var validator = new Validator<Person>();
        var property = validator.RuleFor(person => person.Address!).SetValidator(child);
        var both = await property.ValidateAsync(new Person { Address = new Address() }).ConfigureAwait(true);
        Assert.Equal(new[] { "Address.Street", "Address.City" }, both.Errors.Select(error => error.PropertyName));

        var gated = validator.RuleFor(person => person.Address!).Must(_ => false, "own").SetValidator(child);
        var own = await gated.ValidateAsync(new Person { Address = new Address() }).ConfigureAwait(true);
        Assert.Equal("own", Assert.Single(Assert.Single(own.Errors).Errors));
    }

    [Theory]
    [InlineData("null-not-null")]
    public async Task Null_inputs_follow_the_documented_matrix(string _)
    {
        await Fail(ValidatorRules.NotNull<string?>(), null);
        await Pass(ValidatorRules.Null<string?>(), null);
        await Fail(ValidatorRules.NotEmpty<string?>(), null);
        await Pass(ValidatorRules.Empty<string?>(), null);
        await Pass(ValidatorRules.Equal<string?>(null), null);
        await Fail(ValidatorRules.Equal<string?>("a"), null);
        await Fail(ValidatorRules.NotEqual<string?>(null), null);
        await Pass(ValidatorRules.NotEqual<string?>("a"), null);
        await Fail(ValidatorRules.LessThan<string>("m"), null!);
        await Fail(ValidatorRules.Range<string>("a", "z"), null!);
        await Fail(ValidatorRules.NotRange<string>("a", "z"), null!);
        await Fail(ValidatorRules.Length<string?>(1), null);
        await Fail(ValidatorRules.Matches<string?>(@"^\d+$"), null);
        await Pass(ValidatorRules.NotMatches<string?>(@"^\d+$"), null);
        await Fail(ValidatorRules.Email<string?>(), null);
        await Fail(ValidatorRules.CountryIso2<string?>(), null);

        var child = new Validator<Address>();
        child.RuleFor(item => item.Street).NotEmpty();
        var nested = new Validator<Person>();
        nested.RuleFor(person => person.Address!).SetValidator(child);
        Assert.True((await nested.ValidateAsync(new Person()).ConfigureAwait(true)).IsValid);
        var each = new Validator<Person>();
        each.RuleForEach(person => person.Items!).NotEmpty();
        Assert.True((await each.ValidateAsync(new Person()).ConfigureAwait(true)).IsValid);
    }

    [Fact]
    public async Task Include_sees_a_later_change_and_a_nested_validator()
    {
        var name = new Validator<Person>();
        var property = name.RuleFor(person => person.Name).NotEmpty();
        var both = new Validator<Person>();
        both.Include(name);
        Assert.False((await both.ValidateAsync(new Person()).ConfigureAwait(true)).IsValid);
        property.WithMessage("Required.");
        var changed = await both.ValidateAsync(new Person()).ConfigureAwait(true);
        Assert.Equal("Required.", Assert.Single(Assert.Single(changed.Errors).Errors));

        var inner = new Validator<Person>();
        inner.RuleFor(person => person.Age).GreaterThan(0);
        var middle = new Validator<Person>();
        middle.Include(inner);
        var outer = new Validator<Person>();
        outer.Include(middle);
        Assert.Equal("Age", Assert.Single((await outer.ValidateAsync(new Person()).ConfigureAwait(true)).Errors).PropertyName);

        var throwing = new Validator<Person>();
        throwing.RuleFor(person => person.Name).Must(_ => throw new InvalidOperationException("boom"), "unused");
        var host = new Validator<Person>();
        host.Include(throwing);
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await host.ValidateAsync(new Person { Name = "Ada" }).ConfigureAwait(true)).ConfigureAwait(true);
    }

    [Fact]
    public async Task Equal_messages_use_the_invariant_format()
    {
        await Fail(ValidatorRules.Equal<string?>(null), "x", "Must be equal to null.");
        await Fail(
            ValidatorRules.Equal(new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc)),
            DateTime.MinValue,
            "Must be equal to 01/02/2020 03:04:05.");
        await Fail(ValidatorRules.Equal(1e21d), 0d, "Must be equal to 1E+21.");
        await Fail(ValidatorRules.Equal(1.50m), 0m, "Must be equal to 1.50.");
        await Fail(ValidatorRules.NotEqual(Day.Mon), Day.Mon, "Must not be equal to Mon.");
    }

    [Fact]
    public async Task A_shared_validator_stays_correct_under_concurrent_runs()
    {
        var validator = new Validator<Person>();
        validator.RuleFor(person => person.Name).NotEmpty().MaxLength(8);
        validator.RuleFor(person => person.Age).GreaterThan(0);
        var valid = new Person { Name = "Ada", Age = 30 };
        var invalid = new Person { Name = "", Age = 0 };
        var tasks = Enumerable.Range(0, 8).Select(_ => Task.Run(async () =>
        {
            for (var i = 0; i < 200; i++)
            {
                Assert.True((await validator.ValidateAsync(valid).ConfigureAwait(false)).IsValid);
                Assert.Equal(2, (await validator.ValidateAsync(invalid).ConfigureAwait(false)).Errors.Count);
            }
        })).ToArray();
        await Task.WhenAll(tasks).ConfigureAwait(true);

        var races = Enumerable.Range(0, 40).Select(_ => Task.Run(async () =>
        {
            var fresh = new Validator<Person>();
            fresh.RuleFor(person => person.Name).NotEmpty();
            var pending = Enumerable.Range(0, 4).Select(_ => fresh.ValidateAsync(valid).AsTask());
            var results = await Task.WhenAll(pending).ConfigureAwait(false);
            Assert.All(results, result => Assert.True(result.IsValid));
        })).ToArray();
        await Task.WhenAll(races).ConfigureAwait(true);
    }

    [Fact]
    public async Task MustAsync_accepts_every_task_delegate_shape()
    {
        var validator = new Validator<Person>();
        validator.RuleFor(person => person.Name).MustAsync(async (string? name, CancellationToken token) =>
        {
            await Task.Yield();
            token.ThrowIfCancellationRequested();
            return name == "Ada";
        }, "async-token");
        validator.RuleFor(person => person.Name).MustAsync(
            (string? name, CancellationToken token) => ExistsAsync(name, token),
            "forward");
        validator.RuleFor(person => person.Name).MustAsync(
            (string? _, CancellationToken _) => Task.FromResult(true),
            "from-result");
        validator.RuleFor(person => person.Name).MustAsync(ExistsAsync, "group-token");
        validator.RuleFor(person => person.Name).MustAsync(Exists, "group");
        validator.RuleFor(person => person.Name).MustAsync(async (string? name) =>
        {
            await Task.Yield();
            return name == "Ada";
        }, "async-one");
        validator.RuleFor(person => person.Name).AddRule(
            new ValidatorRule<string?>((value, _) => new ValueTask<bool>(value == "Ada"), "value-task"));

        Assert.True((await validator.ValidateAsync(new Person { Name = "Ada" }).ConfigureAwait(true)).IsValid);
        Assert.False((await validator.ValidateAsync(new Person { Name = "Bea" }).ConfigureAwait(true)).IsValid);
    }

    [Fact]
    public async Task A_wrapper_around_cyclic_data_stops_for_sync_and_async_rules()
    {
        var sync = new WrappedNode(yield: false);
        var node = new Node { Name = "Ada" };
        node.Manager = node;
        var syncError = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await sync.ValidateAsync(node).ConfigureAwait(true)).ConfigureAwait(true);
        Assert.Contains("128", syncError.Message);

        var asyncValidator = new WrappedNode(yield: true);
        var asyncError = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await asyncValidator.ValidateAsync(node).ConfigureAwait(true)).ConfigureAwait(true);
        Assert.Contains("128", asyncError.Message);
    }

    [Fact]
    public async Task An_include_cycle_through_a_wrapper_stops_at_run_time()
    {
        var root = new Validator<Person>();
        var wrapped = new WrappedPerson();
        root.Include(wrapped);
        wrapped.Inner.Include(root);
        root.RuleFor(person => person.Name).NotEmpty();
        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await root.ValidateAsync(new Person { Name = "Ada" }).ConfigureAwait(true)).ConfigureAwait(true);
        Assert.Contains("128", error.Message);
    }

    [Fact]
    public async Task Depth_128_passes_and_129_throws()
    {
        Assert.True((await Chain(128).ValidateAsync(Links(128)).ConfigureAwait(true)).IsValid);
        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await Chain(129).ValidateAsync(Links(129)).ConfigureAwait(true)).ConfigureAwait(true);
        Assert.Contains("128", error.Message);
    }

    [Fact]
    public async Task ScalePrecision_rejects_one_integer_digit_past_a_large_bound()
        => await Fail(ValidatorRules.ScalePrecision<decimal>(2, 16), 10000000000000000m);

    [Fact]
    public async Task A_null_item_skips_its_child_and_a_failing_item_rule_skips_the_child()
    {
        var calls = 0;
        var child = new Validator<Address>();
        child.RuleFor(item => item.Street).Must(_ =>
        {
            calls++;
            return false;
        }, "child");
        var validator = new Validator<Person>();
        validator.RuleForEach(person => person.Places!).SetValidator(child);
        var skipped = await validator.ValidateAsync(new Person
        {
            Places = [null!, new Address { Street = "ok" }],
        }).ConfigureAwait(true);
        Assert.Equal("Places[1].Street", Assert.Single(skipped.Errors).PropertyName);
        Assert.Equal(1, calls);

        calls = 0;
        var gated = new Validator<Person>();
        gated.RuleForEach(person => person.Places!).Must(_ => false, "own").SetValidator(child);
        var own = await gated.ValidateAsync(new Person
        {
            Places = [new Address { Street = "ok" }],
        }).ConfigureAwait(true);
        Assert.Equal("own", Assert.Single(Assert.Single(own.Errors).Errors));
        Assert.Equal(0, calls);

        calls = 0;
        var asyncOwn = new Validator<Person>();
        asyncOwn.RuleForEach(person => person.Places!).MustAsync(async (Address _, CancellationToken _) =>
        {
            await Task.Yield();
            return false;
        }, "async-own").SetValidator(child);
        var asyncResult = await asyncOwn.ValidateAsync(new Person
        {
            Places = [new Address { Street = "ok" }],
        }).ConfigureAwait(true);
        Assert.Equal("async-own", Assert.Single(Assert.Single(asyncResult.Errors).Errors));
        Assert.Equal(0, calls);
    }

    [Fact]
    public void ValidationError_From_copies_the_messages_and_rejects_null()
    {
        var source = new List<string> { "one", "two" };
        var error = ValidationError.From(source);
        source.Add("three");
        source[0] = "changed";
        Assert.Equal(new[] { "one", "two" }, error);
        Assert.Throws<ArgumentException>(() => ValidationError.From(new string[] { null! }));
        Assert.Same(ValidationError.Empty, ValidationError.From(Array.Empty<string>()));
    }

    [Fact]
    public async Task An_async_wrapper_does_not_leak_depth_across_runs()
    {
        var wrapper = new YieldingAddress(fault: false);
        var root = new Validator<Person>();
        root.RuleFor(person => person.Address!).SetValidator(wrapper);
        var person = new Person { Address = new Address { Street = "ok" } };
        for (var i = 0; i < 200; i++)
        {
            Assert.True((await root.ValidateAsync(person).ConfigureAwait(true)).IsValid);
        }

        wrapper.Fault = true;
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await root.ValidateAsync(person).ConfigureAwait(true)).ConfigureAwait(true);
        wrapper.Fault = false;
        for (var i = 0; i < 200; i++)
        {
            Assert.True((await root.ValidateAsync(person).ConfigureAwait(true)).IsValid);
        }

        Assert.Equal(-1, RunSync(root, person));
        var tasks = Enumerable.Range(0, 8).Select(_ => Task.Run(async () =>
        {
            for (var i = 0; i < 40; i++)
            {
                Assert.True((await root.ValidateAsync(person).ConfigureAwait(false)).IsValid);
            }
        })).ToArray();
        await Task.WhenAll(tasks).ConfigureAwait(true);
    }

    [Fact]
    public async Task Concurrent_includes_do_not_deadlock_and_keep_one_cycle_closed()
    {
        for (var iteration = 0; iteration < 50; iteration++)
        {
            var first = new Validator<Person>();
            var second = new Validator<Person>();
            var third = new Validator<Person>();
            var fourth = new Validator<Person>();
            second.Include(third);
            fourth.Include(first);
            using var barrier = new Barrier(2);
            var left = Task.Run(() =>
            {
                barrier.SignalAndWait();
                return TryInclude(first, second);
            });
            var right = Task.Run(() =>
            {
                barrier.SignalAndWait();
                return TryInclude(third, fourth);
            });
            var both = Task.WhenAll(left, right);
            var finished = await Task.WhenAny(both, Task.Delay(TimeSpan.FromSeconds(5))).ConfigureAwait(true);
            Assert.Same(both, finished);
            var results = await both.ConfigureAwait(true);
            Assert.Equal(1, results[0] + results[1]);
        }
    }

    private static int RunSync(Validator<Person> validator, Person person)
    {
        for (var i = 0; i < 200; i++)
        {
            if (!validator.ValidateAsync(person).GetAwaiter().GetResult().IsValid)
            {
                return i;
            }
        }

        return -1;
    }

    private static int TryInclude(Validator<Person> host, Validator<Person> child)
    {
        try
        {
            host.Include(child);
            return 0;
        }
        catch (InvalidOperationException)
        {
            return 1;
        }
    }

    private sealed class YieldingAddress : IValidator<Address>
    {
        internal YieldingAddress(bool fault)
        {
            Fault = fault;
            Inner.RuleFor(item => item.Street).NotEmpty();
        }

        internal bool Fault { get; set; }

        private Validator<Address> Inner { get; } = new();

        public async ValueTask<ValidationResult> ValidateAsync(Address instance, CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            if (Fault)
            {
                throw new InvalidOperationException("boom");
            }

            return await Inner.ValidateAsync(instance, cancellationToken).ConfigureAwait(false);
        }
    }

    private static Task<bool> ExistsAsync(string? name, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        return Task.FromResult(name == "Ada");
    }

    private static Task<bool> Exists(string? name) => Task.FromResult(name == "Ada");

    private static Validator<Link> Chain(int hops)
    {
        var validators = new Validator<Link>[hops + 1];
        for (var i = 0; i <= hops; i++)
        {
            validators[i] = new Validator<Link>();
        }

        for (var i = 0; i < hops; i++)
        {
            var child = validators[i + 1];
            validators[i].RuleFor(link => link.Next!).SetValidator(child);
        }

        return validators[0];
    }

    private static Link Links(int hops)
    {
        var root = new Link();
        var cursor = root;
        for (var i = 0; i < hops; i++)
        {
            cursor.Next = new Link();
            cursor = cursor.Next;
        }

        return root;
    }

    private sealed class Link
    {
        public Link? Next { get; set; }
    }

    private sealed class WrappedNode : IValidator<Node>
    {
        internal WrappedNode(bool yield)
        {
            if (yield)
            {
                Inner.RuleFor(node => node.Name).MustAsync(async (string? _, CancellationToken _) =>
                {
                    await Task.Yield();
                    return true;
                }, "yield");
            }

            Inner.RuleFor(node => node.Manager!).SetValidator(this);
        }

        private Validator<Node> Inner { get; } = new();

        public ValueTask<ValidationResult> ValidateAsync(Node instance, CancellationToken cancellationToken = default)
            => Inner.ValidateAsync(instance, cancellationToken);
    }

    private sealed class WrappedPerson : IValidator<Person>
    {
        internal Validator<Person> Inner { get; } = new();

        public ValueTask<ValidationResult> ValidateAsync(Person instance, CancellationToken cancellationToken = default)
            => Inner.ValidateAsync(instance, cancellationToken);
    }

    private static async Task AssertIndexes(IEnumerable<string> values, IReadOnlyList<string> names, bool yield, string name = "Items")
    {
        var person = new Person();
        if (name == "Items")
        {
            person.Items = values as List<string> ?? values.ToList();
        }
        else if (name == "Codes")
        {
            person.Codes = values.ToArray();
        }
        else
        {
            person.Lazy = values;
        }

        var validator = new Validator<Person>();
        if (name == "Codes")
        {
            AddEach(validator.RuleForEach(person => person.Codes!), yield);
        }
        else if (name == "Lazy")
        {
            AddEach(validator.RuleForEach(person => person.Lazy!), yield);
        }
        else
        {
            AddEach(validator.RuleForEach(person => person.Items!), yield);
        }

        var result = await validator.ValidateAsync(person).ConfigureAwait(true);
        Assert.Equal(names, result.Errors.Select(error => error.PropertyName));
    }

    private static void AddEach(PropertyValidator<Person, string> property, bool yield)
    {
        if (!yield)
        {
            property.NotEmpty();
            return;
        }

        property.MustAsync(async (string value, CancellationToken _) =>
        {
            await Task.Yield();
            return !string.IsNullOrEmpty(value);
        }, "Must not be empty.");
    }

    private static IEnumerable<string> LazyFail()
    {
        yield return "ok";
        yield return "";
    }

    private static async Task Pass<T>(IValidatorRule<T> rule, T value)
        => Assert.True((await rule.ValidateAsync(value).ConfigureAwait(true)).IsValid);

    private static async Task Fail<T>(IValidatorRule<T> rule, T value)
        => Assert.False((await rule.ValidateAsync(value).ConfigureAwait(true)).IsValid);

    private static async Task Fail<T>(IValidatorRule<T> rule, T value, string message)
        => Assert.Equal(message, Assert.Single((await rule.ValidateAsync(value).ConfigureAwait(true)).Errors));

    private sealed class OddLength : IValidatorRule<string?>
    {
        public ValueTask<ValidationRuleResult> ValidateAsync(string? value, CancellationToken cancellationToken = default)
            => new((value?.Length ?? 0) % 2 == 1
                ? ValidationRuleResult.Success
                : ValidationRuleResult.Failed("Must have an odd length."));
    }

    private enum Day
    {
        Mon,
        Tue,
    }
}
