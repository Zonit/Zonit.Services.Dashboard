using Example.Presentation.Manager.Pages;
using Zonit.Messaging.Tasks;

namespace Example.Events;

public class TestEvent : TaskHandler<TestModel>
{
    public override int WorkerCount => 3;
    public override TimeSpan Timeout => TimeSpan.FromMinutes(5);

    public override string? Title => "Import Data";
    public override string? Description => "Importing data from external source";

    // Define steps with estimated durations for smooth progress calculation
    public override TaskProgressStep[]? ProgressSteps =>
    [
        new(TimeSpan.FromSeconds(3), "Initializing task..."),
        new(TimeSpan.FromSeconds(5), "Loading data..."),
        new(TimeSpan.FromSeconds(4), "Processing records..."),
        new(TimeSpan.FromSeconds(6), "Validating results..."),
        new(TimeSpan.FromSeconds(2), "Finalizing...")
    ];

    protected override async Task HandleAsync(TestModel data, ITaskProgressContext progress, CancellationToken cancellationToken)
    {
        // Step 1: Initialize (0% -> ~15%)
        await progress.NextAsync();
        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

        // Step 2: Load data (15% -> ~40%)
        await progress.NextAsync();
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        await progress.SetMessageAsync($"Loading {data.Name} configuration...");
        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

        // Step 3: Process records (40% -> ~60%)
        await progress.NextAsync();
        for (int i = 1; i <= 4; i++)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            await progress.SetMessageAsync($"Processing record {i}/4...");
        }

        // Step 4: Validate (60% -> ~90%)
        await progress.NextAsync();
        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
        await progress.SetMessageAsync("Running validation checks...");
        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

        // Step 5: Finalize (90% -> 100%)
        await progress.NextAsync();
        await progress.SetMessageAsync("Cleaning up resources...");
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }
}
