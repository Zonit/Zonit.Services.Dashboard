using Zonit.Extensions.Organizations;

namespace Example.Services;

public class UserOrganization : IUserOrganizationManager
{
    IReadOnlyCollection<WorkspaceModel> ExampleData = [
        new WorkspaceModel
        {
            Organization = new()
            {
                Id = Guid.Parse("96120510-05c1-4aad-97d0-0a834242ed18"),
                Name = "Dog"
            },
            Permissions = ["Kość", "Smycz"],
            Roles = ["Podwórko", "Buda"]
        },
        new WorkspaceModel
        {
            Organization = new()
            {
                Id = Guid.Parse("266ac900-8940-4b53-bb71-64a164b5d73e"),
                Name = "Cat"
            },
            Permissions = ["Mysz", "Miętka"],
            Roles = ["Pudełko", "Stodoła"]
        }
    ];

    public async Task<WorkspaceModel> InicjalizeAsync()
    {
        //await Task.Delay(TimeSpan.FromSeconds(1));
        return await Task.FromResult(ExampleData.Last());
    }

    public async Task<IReadOnlyCollection<OrganizationModel>?> GetOrganizationsAsync()
        => await Task.FromResult(ExampleData.Select(x => x.Organization).ToList());

    public async Task<WorkspaceModel?> SwitchOrganizationAsync(Guid organizationId)
    {
        // TODO: Zrób weryfikację czy użytkownik posiada organizację

        var organization = OrganizationService.organizations?.SingleOrDefault(x => x.Id == organizationId);

        if (organization is null)
            return null;

        return await Task.FromResult(ExampleData.Where(x => x.Organization?.Id == organizationId).FirstOrDefault());
    }
}
