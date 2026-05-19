using AireIndustrial.Domain.Entities;
using AireIndustrial.Infrastructure.Identity;

namespace AireIndustrial.Infrastructure.Extensions;

public static class UserMappingExtension
{
    public static AppIdentityUser ToIdentityUser(this Usuario usuario)
    {
        return new AppIdentityUser
        {
            UserName = usuario.Email,
            Email = usuario.Email,
            FirstName = usuario.FirstName,
            LastName = usuario.LastName,
            Tel = usuario.Tel,
            PhoneNumber = usuario.Tel
        };
    }

    public static Usuario ToDomainUser(this AppIdentityUser identityUser)
    {
        return new Usuario
        {
            Id = Guid.Parse(identityUser.Id),
            FirstName = identityUser.FirstName,
            LastName = identityUser.LastName,
            Email = identityUser.Email ?? string.Empty,
            Tel = identityUser.Tel
        };
    }
}
