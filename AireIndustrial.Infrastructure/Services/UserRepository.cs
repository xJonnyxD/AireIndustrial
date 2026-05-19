using AireIndustrial.Domain.Entities;
using AireIndustrial.Domain.Interfaces;
using AireIndustrial.Infrastructure.Extensions;
using AireIndustrial.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace AireIndustrial.Infrastructure.Services;

public class UserRepository : IUserRepository
{
    private readonly UserManager<AppIdentityUser> _userManager;

    public UserRepository(UserManager<AppIdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Usuario?> GetUserByEmail(string email)
    {
        var identityUser = await _userManager.FindByEmailAsync(email);
        return identityUser?.ToDomainUser();
    }

    public async Task<Usuario?> CreateUser(Usuario usuario)
    {
        var identityUser = usuario.ToIdentityUser();
        var result = await _userManager.CreateAsync(identityUser, usuario.Password);
        if (!result.Succeeded) return null;
        usuario.Id = Guid.Parse(identityUser.Id);
        return usuario;
    }

    public async Task<bool> CheckPassword(Usuario usuario, string password)
    {
        var identityUser = await _userManager.FindByEmailAsync(usuario.Email);
        if (identityUser is null) return false;
        return await _userManager.CheckPasswordAsync(identityUser, password);
    }

    public async Task<IList<string>> GetRoles(Usuario usuario)
    {
        var identityUser = await _userManager.FindByEmailAsync(usuario.Email);
        if (identityUser is null) return new List<string>();
        return await _userManager.GetRolesAsync(identityUser);
    }

    public async Task<bool> AddToRoleAsync(Usuario usuario, string roleName)
    {
        var identityUser = await _userManager.FindByEmailAsync(usuario.Email);
        if (identityUser is null) return false;
        var result = await _userManager.AddToRoleAsync(identityUser, roleName);
        return result.Succeeded;
    }
}
