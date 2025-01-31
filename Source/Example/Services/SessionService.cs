using Zonit.Extensions.Identity;

namespace Example.Services;

public class SessionService : ISessionProvider
{
    public async Task<UserModel?> GetByTokenAsync(string token)
        => new UserModel
        {
            Name = "NoUser",
            Roles = ["User", "TwojaStara"]
        };
}