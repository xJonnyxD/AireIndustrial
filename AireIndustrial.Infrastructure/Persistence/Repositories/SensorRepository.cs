using AireIndustrial.Domain.Entities;
using AireIndustrial.Domain.Interfaces;

namespace AireIndustrial.Infrastructure.Persistence.Repositories;

public class SensorRepository : BaseRepository<SensorCalidadAire>, ISensorRepository
{
    public SensorRepository(ApplicationDbContext context) : base(context) { }
}
