using AireIndustrial.Domain.Entities;

namespace AireIndustrial.Domain.Interfaces;

public interface IJwtService
{
    string GenerateToken(Usuario usuario, IList<string> roles);
}
