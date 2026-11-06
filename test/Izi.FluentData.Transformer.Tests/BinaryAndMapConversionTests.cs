using System.Globalization;
using System.Text;

namespace Izi.FluentData.Transformer.Tests;

public class BinaryAndMapConversionTests
{
    [Fact]
    public async Task Encode_and_decode_use_the_supplied_encoding_and_its_fallback()
    {
        Assert.Equal(new byte[] { 0xE9 }, await TransformerRules.Encode(Encoding.Latin1).TransformAsync("é"));
        Assert.Equal("é", await TransformerRules.Decode(Encoding.Latin1).TransformAsync(new byte[] { 0xE9 }));
        Assert.Equal("\uFFFD", await TransformerRules.Decode(Encoding.UTF8).TransformAsync(new byte[] { 0xFF }));

        var strict = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
        Assert.Throws<DecoderFallbackException>(() =>
            TransformerRules.Decode(strict).TransformAsync(new byte[] { 0xFF }));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.Encode(null!));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.Encode(Encoding.UTF8).TransformAsync(null!));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.Decode(Encoding.UTF8).TransformAsync(null!));
        Assert.Equal(Array.Empty<byte>(), await TransformerRules.Encode(Encoding.UTF8).TransformAsync(""));
    }

    [Fact]
    public async Task Base64_and_hex_round_trip_and_reject_invalid_text()
    {
        var bytes = new byte[] { 0x0A, 0xFF, 0x10 };
        var base64 = await TransformerRules.ToBase64().TransformAsync(bytes);
        Assert.Equal(Convert.ToBase64String(bytes), base64);
        Assert.Equal(bytes, await TransformerRules.FromBase64().TransformAsync(base64));

        var wrapped =
            await TransformerRules.ToBase64(Base64FormattingOptions.InsertLineBreaks).TransformAsync(new byte[80]);
        Assert.Contains("\r\n", wrapped, StringComparison.Ordinal);

        Assert.Equal("0AFF", await TransformerRules.ToHex().TransformAsync(new byte[] { 0x0A, 0xFF }));
        Assert.Equal("0aff", await TransformerRules.ToHex(uppercase: false).TransformAsync(new byte[] { 0x0A, 0xFF }));
        Assert.Equal(bytes, await TransformerRules.FromHex().TransformAsync("0aff10"));
        Assert.Equal(bytes, await TransformerRules.FromHex().TransformAsync("0AFF10"));
        Assert.Equal(string.Empty, await TransformerRules.ToHex().TransformAsync([]));
        Assert.Empty(await TransformerRules.FromHex().TransformAsync(""));

        Assert.Throws<FormatException>(() => TransformerRules.FromBase64().TransformAsync("!!!!"));
        Assert.Throws<FormatException>(() => TransformerRules.FromHex().TransformAsync("abc"));
        Assert.Throws<FormatException>(() => TransformerRules.FromHex().TransformAsync("zz"));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.FromHex().TransformAsync(null!));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.ToHex().TransformAsync(null!));
    }

    [Fact]
    public async Task Map_and_MapAsync_compose_and_observe_cancellation()
    {
        var pipeline = new Transformer<string, int>(TransformerRules.Parse<int>(CultureInfo.InvariantCulture))
            .Then(TransformerRules.Map<int, int>(value => value + 1))
            .Then(TransformerRules.MapAsync<int, string>(async (value, token) =>
            {
                await Task.Yield();
                token.ThrowIfCancellationRequested();
                return value.ToString(CultureInfo.InvariantCulture);
            }));

        Assert.Equal("43", await pipeline.TransformAsync("42"));

        var calls = 0;
        using var cancelled = new CancellationTokenSource();
        await cancelled.CancelAsync();
        var blocked = TransformerRules.MapAsync<int, int>((_, _) =>
        {
            calls++;
            return new ValueTask<int>(1);
        });
        Assert.Throws<OperationCanceledException>(() => blocked.TransformAsync(1, cancelled.Token));
        Assert.Equal(0, calls);

        using var during = new CancellationTokenSource();
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var waiting = TransformerRules.MapAsync<int, int>(async (value, token) =>
        {
            started.TrySetResult();
            await Task.Delay(Timeout.Infinite, token).ConfigureAwait(false);
            return value;
        });
        var pending = waiting.TransformAsync(5, during.Token);

        // xUnit suppresses CA2007 for calls made directly in a test, but not for a stored task.
        // ConfigureAwait(false) here trips xUnit1030, and the default continuation is the one the test needs.
        await started.Task.ConfigureAwait(true);
        await during.CancelAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await pending.ConfigureAwait(false));

        using var supplied = new CancellationTokenSource();
        CancellationToken observed = default;
        var seen = TransformerRules.MapAsync<int, int>((value, token) =>
        {
            observed = token;
            return new ValueTask<int>(value);
        });
        Assert.Equal(9, await seen.TransformAsync(9, supplied.Token));
        Assert.Equal(supplied.Token, observed);

        var boom = TransformerRules.Map<int, int>(_ => throw new InvalidOperationException("boom"));
        var thrown = Assert.Throws<InvalidOperationException>(() => boom.TransformAsync(1));
        Assert.Equal("boom", thrown.Message);
        Assert.Throws<ArgumentNullException>(() => TransformerRules.Map<int, int>(null!));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.MapAsync<int, int>(null!));
    }

    [Fact]
    public void A_shared_parse_rule_can_be_used_from_many_threads()
    {
        var rule = TransformerRules.Parse<int>(CultureInfo.InvariantCulture);
        var mismatches = 0;
        Parallel.For(0, 200, i =>
        {
            var value = rule.TransformAsync(i.ToString(CultureInfo.InvariantCulture)).GetAwaiter().GetResult();
            if (value != i)
            {
                Interlocked.Increment(ref mismatches);
            }
        });

        Assert.Equal(0, mismatches);
    }
}
