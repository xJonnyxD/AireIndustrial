using AireIndustrial.Domain.Entities;
using AireIndustrial.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AireIndustrial.Infrastructure.Persistence.Repositories;

public class LecturaRepository : BaseRepository<LecturaAire>, ILecturaRepository
{
    public LecturaRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<LecturaAire>> GetByRangoFechasAsync(DateTime inicio, DateTime fin)
    {
        return await _context.LecturasAire
            .Where(l => l.FechaHora >= inicio && l.FechaHora <= fin)
            .ToListAsync();
    }
}
