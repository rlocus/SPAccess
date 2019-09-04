using System.Linq;

namespace SP.Client.Linq.Infrastructure
{
    public interface ISpSet<TEntity> : IQueryable<TEntity>, ISpRepository<TEntity>
       where TEntity : class, IListItemEntity
    {
        
    }
}
