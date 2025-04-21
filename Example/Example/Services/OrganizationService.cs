using Zonit.Extensions.Organizations;

namespace Example.Services;

internal class OrganizationService : IOrganizationManager
{
    public static IReadOnlyCollection<OrganizationModel>? organizations = [
        new()
        {
            Id = Guid.Parse("96120510-05c1-4aad-97d0-0a834242ed18"),
            Name = "Organization Dog"
        },
        new()
        {
            Id = Guid.Parse("266ac900-8940-4b53-bb71-64a164b5d73e"),
            Name = "Organization Cat"
        }
    ];

    public async Task<OrganizationModel?> GetAsync(Guid id)
    {
        OrganizationModel? model = default;

        model = organizations?.FirstOrDefault(x => x.Id == id);

        return await Task.FromResult(model);
    }
}