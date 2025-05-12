using Example.Presentation.Manager.Pages;
using Zonit.Services.EventMessage;

namespace Example.Events;

public class TestEvent : TaskBase<TestModel>
{
    protected override async Task HandleAsync(TestModel data, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(20), cancellationToken);
    }
}
