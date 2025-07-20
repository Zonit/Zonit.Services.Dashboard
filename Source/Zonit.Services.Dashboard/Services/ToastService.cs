using MudBlazor;
using Zonit.Extensions.Website;
using Zonit.Extensions.Website.Abstractions.Toasts.Types;

namespace Zonit.Services.Dashboard.Services;

internal class ToastService(ISnackbar snackbar) : IToastProvider
{
    public void Add(string message, ToastType taskType)
    {
        switch(taskType)
        {
            case ToastType.Info:
                snackbar.Add(message, Severity.Info);
                break;
            case ToastType.Success:
                snackbar.Add(message, Severity.Success);
                break;
            case ToastType.Warning:
                snackbar.Add(message, Severity.Warning);
                break;
            case ToastType.Error:
                snackbar.Add(message, Severity.Error);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(taskType), taskType, null);
        }
    }
}
