using AireIndustrial.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AireIndustrial.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AlertaController : ControllerBase
{
    private readonly AlertaUseCase _alertaUseCase;

    public AlertaController(AlertaUseCase alertaUseCase)
    {
        _alertaUseCase = alertaUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int take = 10)
    {
        if (page < 1 || take < 1)
            return BadRequest(new { mensaje = "Los parámetros 'page' y 'take' deben ser mayores a 0." });

        var alertas = await _alertaUseCase.GetAlertasAsync(page, take);
        return Ok(alertas);
    }
}
