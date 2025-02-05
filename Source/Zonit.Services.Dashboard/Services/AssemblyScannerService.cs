using System.Reflection;
using Zonit.Extensions.Website;

namespace Zonit.Services.Dashboard.Services;

public partial class AssemblyScannerService<T> where T : class
{
    public IEnumerable<Assembly> Types()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !string.Equals(a.FullName, "Microsoft.Data.SqlClient, Version=5.0.0.0, Culture=neutral, PublicKeyToken=23ec7fc2d6eaa4a5", StringComparison.OrdinalIgnoreCase))
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && (typeof(T).IsAssignableFrom(t) || typeof(IAreaDashboard).IsAssignableFrom(t)))
            .Select(t => t.Assembly)
            .Distinct();
    }

    public static IEnumerable<Assembly> GetAssemblies()
    {
        var loadedAssemblies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var queue = new Queue<Assembly>();

        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly != null)
        {
            queue.Enqueue(entryAssembly);
            loadedAssemblies.Add(entryAssembly.FullName);
        }

        while (queue.Count > 0)
        {
            var asm = queue.Dequeue();
            yield return asm;

            foreach (var reference in asm.GetReferencedAssemblies())
            {
                if (loadedAssemblies.Add(reference.FullName)) // Sprawdza, czy już było dodane
                {
                    try
                    {
                        var loadedAssembly = Assembly.Load(reference);
                        queue.Enqueue(loadedAssembly);
                    }
                    catch (Exception)
                    {
                        // TODO:: Logowanie błędu
                    }
                }
            }
        }
    }
}
