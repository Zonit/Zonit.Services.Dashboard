using System.Reflection;

namespace Zonit.Services.Dashboard 
{
    public interface ISettingsManager
    {
        public Assembly[] Assemblies { get; }
        public void SetAssemblies(Assembly[] assemblies);

        public DashboardOptions Settings { get; }
        public void SetSettings(DashboardOptions settings);
    }
}

namespace Zonit.Services.Dashboard.Repositories
{
    public class SettingsRepository : ISettingsManager
    {
        public Assembly[] Assemblies { get; private set; } = [];

        public DashboardOptions Settings { get; private set; } = new();

        public void SetSettings(DashboardOptions settings)
            => Settings = settings;
        
        public void SetAssemblies(Assembly[] assemblies)
            => Assemblies = assemblies;
    }
}