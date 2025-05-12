namespace Zonit.Services.Dashboard 
{
    public interface ISettingsManager
    {
        public DashboardOptions Settings { get; }
        public void SetSettings(DashboardOptions settings);

        public Type Area { get; }
        public void SetArea(Type area);
    }
}

namespace Zonit.Services.Dashboard.Repositories
{
    public class SettingsRepository : ISettingsManager
    {
        DashboardOptions _settings = null!;
        public DashboardOptions Settings => _settings;

        public void SetSettings(DashboardOptions settings)
        {
            _settings = settings;
        }

        private Type _area = null!;
        public Type Area => _area;

        public void SetArea(Type areaType)
        {
            ArgumentNullException.ThrowIfNull(areaType);

            if (!areaType.IsInterface)
                throw new ArgumentException("Type must be an interface.", nameof(areaType));

            _area = areaType;
        }
    }
}