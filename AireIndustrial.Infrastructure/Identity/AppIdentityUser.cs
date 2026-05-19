using Microsoft.AspNetCore.Identity;

namespace AireIndustrial.Infrastructure.Identity;

public class AppIdentityUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Tel { get; set; } = string.Empty;
}
