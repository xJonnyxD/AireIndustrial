using AireIndustrial.Domain.Entities;

namespace AireIndustrial.Domain.Interfaces;

public interface ILecturaRepository : IBaseRepository<LecturaAire>
{
    Task<IEnumerable<LecturaAire>> GetByRangoFechasAsync(DateTime inicio, DateTime fin);
}
