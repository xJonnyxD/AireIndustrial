using System.Net.Http.Json;
using System.Text.Json;
using AireIndustrial.Application.DTOs;
using AireIndustrial.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AireIndustrial.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LecturaController : ControllerBase
{
    private readonly LecturaUseCase _lecturaUseCase;
    private readonly IHttpClientFactory _httpClientFactory;

    public LecturaController(LecturaUseCase lecturaUseCase, IHttpClientFactory httpClientFactory)
    {
        _lecturaUseCase = lecturaUseCase;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] RegisterLecturaDto dto)
    {
        try
        {
            var lectura = await _lecturaUseCase.RegisterLecturaAsync(dto);
            return Ok(lectura);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] LecturaFiltroDto filtro)
    {
        var lecturas = await _lecturaUseCase.GetLecturasFiltradas(filtro);
        return Ok(lecturas);
    }

    [HttpGet("{id}/enriquecida")]
    public async Task<IActionResult> GetEnriquecida(Guid id)
    {
        var lectura = await _lecturaUseCase.GetLecturaById(id);
        if (lectura is null)
            return NotFound(new { mensaje = "Lectura no encontrada." });

        // Coordenadas fijas de referencia. En producción obtener del sensor.
        const double lat = 19.4326;
        const double lon = -99.1332;

        var client = _httpClientFactory.CreateClient();
        var url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current=temperature_2m,relative_humidity_2m";

        JsonElement clima;
        try
        {
            clima = await client.GetFromJsonAsync<JsonElement>(url);
        }
        catch
        {
            return Ok(new { lectura, clima = (object?)null, advertencia = "No se pudo obtener datos climáticos." });
        }

        return Ok(new { lectura, clima });
    }
}
