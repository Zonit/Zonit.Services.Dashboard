using Zonit.Services.Dashboard.Options;
using Zonit.Services.Dashboard.Repositories;

namespace Zonit.Services.Dashboard.Services;

public interface ISettingsProvider
{
    public DashboardOptions Settings { get; }
}

public class SettingsService(ISettingsManager settingsManager) : ISettingsProvider
{
    public DashboardOptions Settings => settingsManager.Settings;
}