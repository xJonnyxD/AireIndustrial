using AireIndustrial.Application.DTOs;
using AireIndustrial.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace AireIndustrial.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly AuthUseCase _authUseCase;

    public AccountController(AuthUseCase authUseCase)
    {
        _authUseCase = authUseCase;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { mensaje = "Email y contraseña son obligatorios." });

        var token = await _authUseCase.Login(dto);
        if (token is null)
            return Unauthorized(new { mensaje = "Credenciales inválidas." });

        return Ok(new { token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { mensaje = "Email y contraseña son obligatorios." });

        var usuario = await _authUseCase.RegisterUser(dto);
        if (usuario is null)
            return BadRequest(new { mensaje = "No se pudo registrar el usuario. Verifique que el email no esté en uso y que la contraseña cumpla los requisitos." });

        return Ok(new { mensaje = "Usuario registrado exitosamente.", email = usuario.Email, nombre = usuario.FullName });
    }
}
