using MudBlazor;
using Zonit.Extensions.Cultures;
using Zonit.Extensions.Website;
using Zonit.Extensions.Website.Abstractions.Toasts.Types;

namespace Zonit.Services.Dashboard.Services;

internal class ToastService(
        ISnackbar snackbar,
        ICultureProvider culture
    ) : IToastProvider
{
    public void Add(string message, ToastType taskType)
    {
        switch (taskType)
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

    public void Add(ToastType taskType, string message, params object?[] objects)
    {
        var translate = culture.Translate(message, objects);

        switch (taskType)
        {
            case ToastType.Info:
                snackbar.Add(translate, Severity.Info);
                break;
            case ToastType.Success:
                snackbar.Add(translate, Severity.Success);
                break;
            case ToastType.Warning:
                snackbar.Add(translate, Severity.Warning);
                break;
            case ToastType.Error:
                snackbar.Add(translate, Severity.Error);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(taskType), taskType, null);
        }
    }
}