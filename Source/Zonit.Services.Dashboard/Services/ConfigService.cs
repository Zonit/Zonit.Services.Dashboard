using Microsoft.Extensions.Options;
using Zonit.Services.Dashboard.Options;

namespace Zonit.Services.Dashboard.Services;

public class ConfigService
{
    public ConfigService(IOptions<DashboardOptions> options)
    {
        Options = options.Value;
    }

    public DashboardOptions Options { get; }
}
