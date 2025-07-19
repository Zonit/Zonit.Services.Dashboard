using Zonit.Services.Dashboard.Abstractions;

namespace Zonit.Services.Dashboard.Services;

public class SettingsService(ISettingsManager settingsManager) : ISettingsProvider
{
    public DashboardOptions Settings => settingsManager.Settings;
}