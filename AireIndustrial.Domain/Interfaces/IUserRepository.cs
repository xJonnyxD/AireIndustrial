using AireIndustrial.Domain.Entities;

namespace AireIndustrial.Domain.Interfaces;

public interface IUserRepository
{
    Task<Usuario?> GetUserByEmail(string email);
    Task<Usuario?> CreateUser(Usuario usuario);
    Task<bool> CheckPassword(Usuario usuario, string password);
    Task<IList<string>> GetRoles(Usuario usuario);
    Task<bool> AddToRoleAsync(Usuario usuario, string roleName);
}
