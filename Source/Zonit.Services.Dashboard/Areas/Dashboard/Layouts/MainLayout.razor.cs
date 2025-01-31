using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Services;

namespace Zonit.Services.Dashboard.Areas.Dashboard.Layouts;

[Authorize]
public partial class MainLayout : LayoutComponentBase, IAsyncDisposable, IBrowserViewportObserver
{
    [Inject]
    IBrowserViewportService BrowserViewportService { get; set; } = null!;

    Guid IBrowserViewportObserver.Id { get; } = Guid.NewGuid();

    ResizeOptions IBrowserViewportObserver.ResizeOptions { get; } = new()
    {
        ReportRate = 50,
        NotifyOnBreakpointOnly = false
    };

    private int _width = 0;
    private int _height = 0;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await BrowserViewportService.SubscribeAsync(this, fireImmediately: true);
        }
    }

    public async ValueTask DisposeAsync()
        => await BrowserViewportService.UnsubscribeAsync(this);

    async Task IBrowserViewportObserver.NotifyBrowserViewportChangeAsync(BrowserViewportEventArgs browserViewportEventArgs)
    {
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

        await InvokeAsync(StateHasChanged);
    }
}