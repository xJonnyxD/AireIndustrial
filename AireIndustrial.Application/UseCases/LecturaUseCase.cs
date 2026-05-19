using AireIndustrial.Application.DTOs;
using AireIndustrial.Domain.Entities;
using AireIndustrial.Domain.Interfaces;

namespace AireIndustrial.Application.UseCases;

public class LecturaUseCase
{
    private readonly ILecturaRepository _lecturaRepository;
    private readonly IAlertaRepository _alertaRepository;

    public LecturaUseCase(ILecturaRepository lecturaRepository, IAlertaRepository alertaRepository)
    {
        _lecturaRepository = lecturaRepository;
        _alertaRepository = alertaRepository;
    }

    public async Task<LecturaAire> RegisterLecturaAsync(RegisterLecturaDto dto)
    {
        if (dto.PM2_5 < 0 || dto.PM10 < 0 || dto.CO2 < 0)
            throw new ArgumentException("Los valores de PM2.5, PM10 y CO2 deben ser mayores o iguales a 0.");

        var lectura = new LecturaAire
        {
            SensorId = dto.SensorId,
            PM2_5 = dto.PM2_5,
            PM10 = dto.PM10,
            CO2 = dto.CO2,
            FechaHora = dto.FechaHora
        };

        await _lecturaRepository.AddAsync(lectura);

        var alerta = EvaluarAlerta(lectura);
        if (alerta is not null)
            await _alertaRepository.AddAsync(alerta);

        return lectura;
    }

    public async Task<IEnumerable<LecturaAire>> GetLecturasFiltradas(LecturaFiltroDto dto)
    {
        var lecturas = await _lecturaRepository.GetByRangoFechasAsync(dto.FechaInicio, dto.FechaFin);

        if (!string.IsNullOrWhiteSpace(dto.TipoContaminante))
        {
            lecturas = dto.TipoContaminante.ToUpper() switch
            {
                "PM2_5" => lecturas.Where(l => l.PM2_5 > 0),
                "PM10"  => lecturas.Where(l => l.PM10 > 0),
                "CO2"   => lecturas.Where(l => l.CO2 > 0),
                _       => lecturas
            };
        }

        return lecturas;
    }

    public async Task<LecturaAire?> GetLecturaById(Guid id)
    {
        return await _lecturaRepository.FindFirstOrDefaultAsync(
            l => l.Id == id,
            l => l.Sensor
        );
    }

    private static AlertaAire? EvaluarAlerta(LecturaAire lectura)
    {
        string? nivel = null;
        string? mensaje = null;

        if (lectura.CO2 > 5000 || lectura.PM2_5 > 250)
        {
            nivel = "Extrema";
            mensaje = "Nivel de contaminación extremadamente alto. Riesgo severo para la salud.";
        }
        else if (lectura.PM2_5 > 150 || lectura.PM10 > 200)
        {
            nivel = "Critica";
            mensaje = "La calidad del aire es peligrosa. Se recomienda permanecer en interiores y usar mascarilla.";
        }
        else if (lectura.PM2_5 is > 51 and <= 100 || lectura.CO2 > 1000)
        {
            nivel = "Moderada";
            mensaje = "La calidad del aire es poco saludable para grupos sensibles (niños, adultos mayores, personas con enfermedades respiratorias).";
        }
        else if (lectura.PM2_5 is >= 25 and <= 50)
        {
            nivel = "Leve";
            mensaje = "La calidad del aire es moderada, se recomienda reducir actividades al aire libre prolongadas.";
        }

        if (nivel is null) return null;

        return new AlertaAire
        {
            SensorId = lectura.SensorId,
            Nivel = nivel,
            Mensaje = mensaje!,
            FechaHora = lectura.FechaHora
        };
    }
}
