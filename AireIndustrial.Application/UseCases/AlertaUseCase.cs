using AireIndustrial.Domain.Entities;
using AireIndustrial.Domain.Interfaces;

namespace AireIndustrial.Application.UseCases;

public class AlertaUseCase
{
    private readonly IAlertaRepository _alertaRepository;

    public AlertaUseCase(IAlertaRepository alertaRepository)
    {
        _alertaRepository = alertaRepository;
    }

    public async Task<IEnumerable<AlertaAire>> GetAlertasAsync(int page, int take)
    {
        return await _alertaRepository.GetAllAsync(page, take);
    }
}
