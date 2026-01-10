using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Zonit.Extensions.Website;
using Zonit.Messaging.Tasks;

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
            if (Workspace.Organization?.Id != null)
            {
                TaskProvider.Publish(new TestModel { 
                    Name = "test"
                }, Workspace.Organization.Id);
            }
            else
            {
                TaskProvider.Publish(new TestModel { 
                    Name = "test"
                });
            }
        }
    }

    public class TestModel
    {
        public string Name { get; set; } = string.Empty;
    }
}