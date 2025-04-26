using Zonit.Extensions.Organizations;
using Zonit.Extensions.Projects;

namespace Example.Services;

public class ProjectService(IWorkspaceProvider _workspace) : IOrganizationProjectManager
{
    private static readonly Guid DogOrganizationId = Guid.Parse("96120510-05c1-4aad-97d0-0a834242ed18");
    private static readonly Guid CatOrganizationId = Guid.Parse("266ac900-8940-4b53-bb71-64a164b5d73e");

    private static readonly IReadOnlyDictionary<Guid, IReadOnlyCollection<CatalogModel>> _organizationProjects =
        new Dictionary<Guid, IReadOnlyCollection<CatalogModel>>
        {
            [DogOrganizationId] =
            [
                    new()
                    {
                        Project = new()
                        {
                            Id = Guid.Parse("e9b21ec4-e493-4786-b105-05680e28288e"),
                            Name = "Buda"
                        }
                    },
                    new()
                    {
                        Project = new()
                        {
                            Id = Guid.Parse("85ff5a6c-9e85-4d53-b7a5-d83bc6c3ff0b"),
                            Name = "Kość"
                        }
                    }
            ],
            [CatOrganizationId] =
            [
                    new()
                    {
                        Project = new()
                        {
                            Id = Guid.Parse("e9b21ec4-e493-4786-b105-05680e28288e"),
                            Name = "Kuweta"
                        }
                    },
                    new()
                    {
                        Project = new()
                        {
                            Id = Guid.Parse("85ff5a6c-9e85-4d53-b7a5-d83bc6c3ff0b"),
                            Name = "Mysz"
                        }
                    }
            ]
        };

    private IReadOnlyCollection<CatalogModel> GetOrganizationProjects(Guid? organizationId)
    {
        if (organizationId.HasValue && _organizationProjects.TryGetValue(organizationId.Value, out var projects))
        {
            return projects;
        }

        // Domyślnie zwracamy projekty dla organizacji Dog
        return _organizationProjects[DogOrganizationId];
    }

    public Task<CatalogModel> InicjalizeAsync()
    {
        var organizationId = _workspace.Organization?.Id;
        var projects = GetOrganizationProjects(organizationId);

        return Task.FromResult(projects.FirstOrDefault() ?? new CatalogModel());
    }

    public Task<IReadOnlyCollection<ProjectModel>?> GetProjectsAsync()
    {
        var organizationId = _workspace.Organization?.Id;
        var projects = GetOrganizationProjects(organizationId)
            .Select(x => x.Project)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<ProjectModel>?>(projects);
    }

    public Task<CatalogModel?> SwitchProjectAsync(Guid projectId)
    {
        // TODO: Zrób weryfikację czy użytkownik posiada organizację

        var organizationId = _workspace.Organization?.Id;
        var projects = GetOrganizationProjects(organizationId);
        var project = projects.SingleOrDefault(x => x.Project?.Id == projectId);

        return Task.FromResult(project);
    }
}
