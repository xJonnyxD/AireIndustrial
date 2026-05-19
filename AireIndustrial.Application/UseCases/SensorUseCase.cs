using AireIndustrial.Application.DTOs;
using AireIndustrial.Domain.Entities;
using AireIndustrial.Domain.Interfaces;

namespace AireIndustrial.Application.UseCases;

public class SensorUseCase
{
    private readonly ISensorRepository _sensorRepository;

    public SensorUseCase(ISensorRepository sensorRepository)
    {
        _sensorRepository = sensorRepository;
    }

    public async Task<IEnumerable<SensorCalidadAire>> GetSensoresAsync(int page, int take)
    {
        return await _sensorRepository.GetAllAsync(page, take);
    }

    public async Task<SensorCalidadAire> CreateSensorAsync(CreateSensorDto dto)
    {
        var sensor = new SensorCalidadAire
        {
            Ubicacion = dto.Ubicacion,
            TipoGas = dto.TipoGas,
            Estado = dto.Estado
        };
        await _sensorRepository.AddAsync(sensor);
        return sensor;
    }
}
