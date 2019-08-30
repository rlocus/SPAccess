using System.Collections.Generic;
using System.Linq;

namespace SP.Client.Linq.Infrastructure
{
    public interface ISpSet<TEntity> : IQueryable<TEntity>
       where TEntity : class
    {
        TEntity Add(TEntity entity);

        IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities);

        TEntity Remove(TEntity entity);

        IEnumerable<TEntity> RemoveRange(IEnumerable<TEntity> entities);
    }
}
