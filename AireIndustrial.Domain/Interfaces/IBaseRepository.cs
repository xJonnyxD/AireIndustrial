using System.Linq.Expressions;
using AireIndustrial.Domain.Entities;

namespace AireIndustrial.Domain.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<IEnumerable<TEntity>> GetAllAsync(int page, int take);
    Task<TEntity?> FindFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);
    Task AddAsync(TEntity entity);
    Task<bool> DeleteAsync(Guid id);
}
