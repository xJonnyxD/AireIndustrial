using AireIndustrial.Domain.Entities;
using AireIndustrial.Domain.Interfaces;

namespace AireIndustrial.Infrastructure.Persistence.Repositories;

public class AlertaRepository : BaseRepository<AlertaAire>, IAlertaRepository
{
    public AlertaRepository(ApplicationDbContext context) : base(context) { }
}
