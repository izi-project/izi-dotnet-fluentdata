namespace Izi.FluentData.Transformer.Tests;

public class CompiledSnapshotTests
{
    [Fact]
    public async Task A_config_change_after_a_run_is_used_on_the_next_run()
    {
        var transformer = new Transformer<Customer>();
        var name = transformer.RuleFor(customer => customer.Name);
        var first = await transformer.TransformAsync(new Customer { Name = "  ada  ", Age = 1 });
        Assert.Equal("  ada  ", first.Name);
        Assert.Equal(1, first.Age);

        name.Trim().ToUpper();
        transformer.RuleFor(customer => customer.Age).Add(1);
        var second = await transformer.TransformAsync(new Customer { Name = "  ada  ", Age = 1 });
        Assert.Equal("ADA", second.Name);
        Assert.Equal(2, second.Age);
    }

    [Fact]
    public async Task A_run_in_flight_keeps_its_compiled_snapshot()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var transformer = new Transformer<Customer>();
        var name = transformer.RuleFor(customer => customer.Name);
        name.Add(async (value, _) =>
        {
            entered.TrySetResult();
            await release.Task.ConfigureAwait(true);
            return value.Trim();
        });

        var running = transformer.TransformAsync(new Customer { Name = "  ada  ", Age = 1 });
        await entered.Task.ConfigureAwait(true);
        name.ToUpper();
        transformer.RuleFor(customer => customer.Age).Add(1);
        release.SetResult();

        // This run already holds the rules from before ToUpper and Age were registered.
        var updated = await running.ConfigureAwait(true);
        Assert.Equal("ada", updated.Name);
        Assert.Equal(1, updated.Age);

        var next = await transformer.TransformAsync(new Customer { Name = "  ada  ", Age = 1 });
        Assert.Equal("ADA", next.Name);
        Assert.Equal(2, next.Age);
    }

    [Fact]
    public async Task Concurrent_runs_while_rules_are_added_do_not_throw()
    {
        var transformer = new Transformer<Customer>();
        var name = transformer.RuleFor(customer => customer.Name);
        name.Trim();
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var errors = 0;

        async Task Run()
        {
            await started.Task.ConfigureAwait(true);
            for (var i = 0; i < 100; i++)
            {
                try
                {
                    await transformer.TransformAsync(new Customer { Name = "  ada  ", Age = 1 }).ConfigureAwait(true);
                }
                catch (Exception)
                {
                    Interlocked.Increment(ref errors);
                }
            }
        }

        var runs = new[] { Run(), Run(), Run(), Run() };
        var adds = Task.Run(async () =>
        {
            await started.Task.ConfigureAwait(true);
            name.ToUpper();
            transformer.RuleFor(customer => customer.Age).Add(1);
            name.Add(static (value, _) => new ValueTask<string>(value));
        });

        started.SetResult();
        await Task.WhenAll(runs[0], runs[1], runs[2], runs[3], adds).ConfigureAwait(true);
        Assert.Equal(0, errors);
    }

    public sealed class Customer
    {
        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }
    }
}
