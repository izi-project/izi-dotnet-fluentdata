namespace Izi.FluentData.Transformer.Tests;

public class TransformerTests
{
    [Fact]
    public async Task Transformer_Then_chains_and_honors_cancellation()
    {
        var pipeline = new Transformer<int, int>(TransformerRules.Add(1))
            .Then(TransformerRules.Multiply(10))
            .Then(TransformerRules.Subtract(3));

        Assert.Equal(17, await pipeline.TransformAsync(1));

        var steps = 0;
        var counting = new Transformer<int, int>(TransformerRules.Map<int, int>(value =>
            {
                steps++;
                return value + 1;
            }))
            .Then(TransformerRules.Multiply(10))
            .Then(TransformerRules.Subtract(3));

        using (var cancelled = new CancellationTokenSource())
        {
            await cancelled.CancelAsync();
            Assert.Throws<OperationCanceledException>(() => counting.TransformAsync(1, cancelled.Token));
        }

        Assert.Equal(0, steps);

        using var during = new CancellationTokenSource();
        var second = new RecordingRule();
        var third = new RecordingRule();
        var partial = new Transformer<int, int>(TransformerRules.Map<int, int>(value =>
            {
                during.Cancel();
                return value + 1;
            }))
            .Then(second)
            .Then(third);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await partial.TransformAsync(4, during.Token).ConfigureAwait(false));
        Assert.Equal(0, second.Calls);
        Assert.Equal(0, third.Calls);
        Assert.Throws<ArgumentNullException>(() => pipeline.Then<int>(null!));
    }

    [Fact]
    public void Transformer_ctor_rejects_null_rule()
    {
        Assert.Throws<ArgumentNullException>(() => new Transformer<string, string>(null!));
    }

    // This step ignores the token. Then must refuse to enter it after an earlier step cancels.
    private sealed class RecordingRule : ITransformerRule<int, int>
    {
        public int Calls { get; private set; }

        public ValueTask<int> TransformAsync(int value, CancellationToken cancellationToken = default)
        {
            Calls++;
            return new ValueTask<int>(value);
        }
    }
}
