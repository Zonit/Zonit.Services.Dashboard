using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Services;
using Zonit.Extensions.Organizations;
using Zonit.Extensions.Website;
using Zonit.Extensions.Website.Abstractions.Navigations.Types;
using Zonit.Services.Dashboard.Abstractions;
using Zonit.Services.EventMessage;

namespace Zonit.Services.Dashboard.Areas.Dashboard.Layouts;

[Authorize]
public partial class MainLayout : LayoutComponentBase, IAsyncDisposable, IBrowserViewportObserver
{
    [Inject]
    IBrowserViewportService BrowserViewportService { get; set; } = null!;

    [Inject]
    private ISettingsProvider settingsProvider { get; set; } = default!;

    [Inject]
    ISettingsManager Settings { get; set; } = default!;

    [Inject]
    IBreadcrumbsProvider BreadcrumbsProvider { get; set; } = default!;

    [Inject]
    ITaskManager TaskManager { get; set; } = default!;

    [Inject]
    IWorkspaceProvider WorkspaceProvider { get; set; } = default;

    AreaType AreaType { get; set; }

    Guid IBrowserViewportObserver.Id { get; } = Guid.NewGuid();

    List<BreadcrumbItem>? _breadcrumbItems = null;

    public bool ProgressTask = false;
    IReadOnlyCollection<TaskEventModel> _activeTasks = new List<TaskEventModel>();

    private bool _disposed = false;

    ResizeOptions IBrowserViewportObserver.ResizeOptions { get; } = new()
    {
        ReportRate = 50,
        NotifyOnBreakpointOnly = false
    };

    private int _width = 0;
    private int _height = 0;

    protected override void OnInitialized()
    {
        Culture.OnChange += () =>
        {
            if (_disposed) return;
            
            if (Culture.GetCulture == "ar-sa")
                RightToLeft = true;
            else
                RightToLeft = false;

            try
            {
                StateHasChanged();
            }
            catch (ObjectDisposedException)
            {
                // Component has been disposed, ignore
            }
            catch (InvalidOperationException)
            {
                // Component is no longer in the render tree, ignore
            }
        };

        if (Settings.Settings.Directory.ToLower() == "manager")
            AreaType = AreaType.Manager;
        else if (Settings.Settings.Directory.ToLower() == "management")
            AreaType = AreaType.Management;
        else if (Settings.Settings.Directory.ToLower() == "diagnostic")
            AreaType = AreaType.Diagnostic;

        BreadcrumbsProvider.OnChange += () => 
        {
            if (_disposed) return;
            
            var g = BreadcrumbsProvider.Get();
            if (g is not null)
            {
                _breadcrumbItems = [];
                foreach (var b in g)
                {
                    _breadcrumbItems.Add(new(b.Text, b.Href, b.Disabled, b.Icon));
                }
            }
            else
            {
                _breadcrumbItems = null;
            }
            
            try
            {
                StateHasChanged();
            }
            catch (ObjectDisposedException)
            {
                // Component has been disposed, ignore
            }
            catch (InvalidOperationException)
            {
                // Component is no longer in the render tree, ignore
            }
        };

        var activeTasks = TaskManager.GetActiveTasks(WorkspaceProvider.Organization?.Id);
        ProgressTask = activeTasks.Count > 0;

        TaskManager.EventOnChange(async task => {
            if (_disposed) return;
            
            if (task.ExtensionId != WorkspaceProvider.Organization?.Id && task.ExtensionId != null)
                return; // Ignoruj taski z innych organizacji

            // Pobierz aktualny stan aktywnych tasków po każdej zmianie
            var activeTasks = TaskManager.GetActiveTasks(WorkspaceProvider.Organization?.Id);
            ProgressTask = activeTasks.Count > 0;
            
            try
            {
                await InvokeAsync(StateHasChanged);
            }
            catch (ObjectDisposedException)
            {
                // Component has been disposed, ignore
            }
            catch (InvalidOperationException)
            {
                // Component is no longer in the render tree, ignore
            }
        });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await BrowserViewportService.SubscribeAsync(this, fireImmediately: true);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        
        Culture.OnChange -= StateHasChanged;
        BreadcrumbsProvider.OnChange -= StateHasChanged;

        await BrowserViewportService.UnsubscribeAsync(this); 
    }

    async Task IBrowserViewportObserver.NotifyBrowserViewportChangeAsync(BrowserViewportEventArgs browserViewportEventArgs)
    {
        if (_disposed) return;
        
        _width = browserViewportEventArgs.BrowserWindowSize.Width;
        _height = browserViewportEventArgs.BrowserWindowSize.Height;

        // TODO: Przy między 900px a 1000px jest błąd z prawą nawigacją, nie powinna a pokazuje się
        // Możliwe że to jest problem przy w standardowym grid

        switch (_width)
        {
            case < 425:
                _drawerOpen = false;
                _drawerOpenMin = false;
                _drawerRightOpen = false;
                _drawerVariant = DrawerVariant.Responsive;
                break;

            case < 1024:
                _drawerOpenMin = true;
                _drawerRightOpen = false;
                _drawerVariant = DrawerVariant.Mini;
                break;

            case < 1366:
                _drawerOpen = true;
                _drawerOpenMin = false;
                _drawerRightOpen = false;
                _drawerVariant = DrawerVariant.Responsive;
                break;

            default:
                _drawerOpen = true;
                _drawerOpenMin = false;
                _drawerRightOpen = true;
                _drawerVariant = DrawerVariant.Responsive;
                break;
        }

        try
        {
            await InvokeAsync(StateHasChanged);
        }
        catch (ObjectDisposedException)
        {
            // Component has been disposed, ignore
        }
        catch (InvalidOperationException)
        {
            // Component is no longer in the render tree, ignore
        }
    }
}