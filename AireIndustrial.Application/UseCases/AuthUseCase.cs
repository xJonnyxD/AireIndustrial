using AireIndustrial.Application.DTOs;
using AireIndustrial.Domain.Entities;
using AireIndustrial.Domain.Interfaces;

namespace AireIndustrial.Application.UseCases;

public class AuthUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthUseCase(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<string?> Login(LoginDto dto)
    {
        var usuario = await _userRepository.GetUserByEmail(dto.Email);
        if (usuario is null) return null;

        var passwordValida = await _userRepository.CheckPassword(usuario, dto.Password);
        if (!passwordValida) return null;

        var roles = await _userRepository.GetRoles(usuario);
        return _jwtService.GenerateToken(usuario, roles);
    }

    public async Task<Usuario?> RegisterUser(RegisterUserDto dto)
    {
        var usuario = new Usuario
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Tel = dto.Tel,
            Password = dto.Password
        };

        return await _userRepository.CreateUser(usuario);
    }
}
