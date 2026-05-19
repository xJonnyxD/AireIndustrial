using AireIndustrial.Application.DTOs;
using AireIndustrial.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AireIndustrial.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SensorController : ControllerBase
{
    private readonly SensorUseCase _sensorUseCase;

    public SensorController(SensorUseCase sensorUseCase)
    {
        _sensorUseCase = sensorUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int take = 10)
    {
        if (page < 1 || take < 1)
            return BadRequest(new { mensaje = "Los parámetros 'page' y 'take' deben ser mayores a 0." });

        var sensores = await _sensorUseCase.GetSensoresAsync(page, take);
        return Ok(sensores);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateSensorDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Ubicacion))
            return BadRequest(new { mensaje = "La ubicación del sensor es obligatoria." });

        if (string.IsNullOrWhiteSpace(dto.TipoGas))
            return BadRequest(new { mensaje = "El tipo de gas del sensor es obligatorio." });

        var sensor = await _sensorUseCase.CreateSensorAsync(dto);
        return Ok(sensor);
    }
}
