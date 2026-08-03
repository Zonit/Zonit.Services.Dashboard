using MudBlazor;
using Zonit.Extensions.Cultures;
using Zonit.Extensions.Website;
using Zonit.Extensions.Website.Toasts.Models;
using Zonit.Extensions.Website.Toasts.Types;

namespace Zonit.Dashboard.Services;

/// <summary>
/// Dashboard adapter that forwards every <see cref="IToastProvider"/> call to
/// MudBlazor's <see cref="ISnackbar"/> while translating the message through
/// <see cref="ICultureProvider"/>. Replaces the default queue-based
/// <c>Zonit.Extensions.Website.Toasts.Services.ToastService</c> when
/// <c>AddDashboard()</c> registers it.
/// </summary>
/// <remarks>
/// <para>The <see cref="Toasts"/> collection is intentionally always empty — the
/// MudBlazor snackbar widget owns the rendering and lifecycle (auto-dismiss,
/// animations, queue overflow) so the Website-level queue is bypassed. Tests
/// that need to assert "this UI showed a toast" should mock <see cref="ISnackbar"/>
/// rather than reading <see cref="IToastProvider.Toasts"/>.</para>
/// </remarks>
internal sealed class ToastService(ISnackbar snackbar, ICultureProvider culture) : IToastProvider
{
    public IReadOnlyList<ToastEntry> Toasts => Array.Empty<ToastEntry>();

    public event Action? OnChange;

    public void Add(ToastType taskType, string message, params object[]? args)
    {
        ArgumentNullException.ThrowIfNull(message);

        // Culture.Translate handles the params overload itself; double-formatting
        // would corrupt {0}/{1}-style placeholders so we delegate the substitution
        // to the culture provider rather than calling string.Format here.
        var translated = (args is { Length: > 0 })
            ? culture.Translate(message, args)
            : culture.Translate(message);

        var severity = taskType switch
        {
            ToastType.Info => Severity.Info,
            ToastType.Success => Severity.Success,
            ToastType.Warning => Severity.Warning,
            ToastType.Error => Severity.Error,
            ToastType.Normal => Severity.Normal,
            _ => Severity.Normal,
        };

        snackbar.Add(translated, severity);
        OnChange?.Invoke();
    }

    // No-ops — MudSnackbar manages its own dismiss/clear via its UI controls.
    public void Remove(Guid id) { }
    public void Clear() => snackbar.Clear();
}
