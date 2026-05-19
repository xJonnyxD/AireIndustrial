namespace AireIndustrial.Application.DTOs;

public record RegisterUserDto(
    string FirstName,
    string LastName,
    string Email,
    string Tel,
    string Password
);
