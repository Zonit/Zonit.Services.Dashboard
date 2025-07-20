using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Zonit.Extensions.Website;
using Zonit.Services.EventMessage;

namespace Example.Presentation.Manager.Pages
{
    [Authorize]
    [Route("/" + Route)]
    public sealed partial class TestTask : PageBase // IAreaWeb, IAreaManager, IAreaManagement, IAreaDiagnostic
    {
        public const string Route = "TestTask";

        [Inject]
        public ITaskProvider TaskProvider { get; set; } = default!;

        protected override List<BreadcrumbsModel>? Breadcrumbs =>
        [
            new("Home", "Home"),
        ];

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
        }

        public void PublishTask()
        {
            TaskProvider.Publish(new TestModel { 
                Name = "test"
            }, Workspace.Organization?.Id);
        }
    }

    public class TestModel
    {
        public string Name { get; set; } = string.Empty;
    }
}