using Microsoft.AspNetCore.Components;
using Zonit.Services.Dashboard.Abstractions;

namespace Zonit.Services.Dashboard.Areas.Dashboard;

public partial class App : ComponentBase
{
    [Inject]
    ISettingsProvider SettingsProvider { get; set; } = default!;

    [Inject]
    ISettingsManager SettingsManager { get; set; } = default!;

    public string[] AssemblyNames { get; private set; } = [];
    public MarkupString CustomSnippet { get; private set; } = new MarkupString();

    protected override void OnInitialized()
    {
        AssemblyNames = SettingsManager.Assemblies?.Select(a => a.GetName().Name!).ToArray() ?? [];
        CustomSnippet = new MarkupString(SettingsProvider.Settings.CustomSnippet ?? "");
    }
}
