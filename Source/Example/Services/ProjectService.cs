using Zonit.Extensions.Projects;

namespace Example.Services;

public class ProjectService : IOrganizationProjectManager
{
    static IReadOnlyCollection<CatalogModel> ExampleData = [
        new CatalogModel
            {
                Project = new()
                {
                    Id = Guid.Parse("e9b21ec4-e493-4786-b105-05680e28288e"),
                    Name = "Warszawa"
                }
            },
            new CatalogModel
            {
                Project = new()
                {
                    Id = Guid.Parse("85ff5a6c-9e85-4d53-b7a5-d83bc6c3ff0b"),
                    Name = "Londyn"
                }
            }
    ];

    public async Task<CatalogModel> InicjalizeAsync()
    {
        var project = ExampleData.Last();

        //await Task.Delay(TimeSpan.FromSeconds(1));
        return await Task.FromResult(project);
    }

    public async Task<IReadOnlyCollection<ProjectModel>?> GetProjectsAsync()
        => await Task.FromResult(ExampleData.Select(x => x.Project).ToList());

    public async Task<CatalogModel?> SwitchProjectAsync(Guid projectId)
    {
        // TODO: Zrób weryfikację czy użytkownik posiada organizację

        var project = ExampleData?.SingleOrDefault(x => x.Project?.Id == projectId);

        if (project is null)
            return null;

        return await Task.FromResult(project);

        //return await Task.FromResult(ExampleData.Where(x => x.Project?.Id == projectId).FirstOrDefault());
    }
}
