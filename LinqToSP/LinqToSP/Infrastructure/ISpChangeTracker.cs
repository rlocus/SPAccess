using System.Collections.Generic;

namespace SP.Client.Linq.Infrastructure
{
    public interface ISpChangeTracker<TEntity>
    where TEntity : class, IListItemEntity
    {
        IEnumerable<SpEntityEntry<TEntity, TContext>> Entries<TContext>()
          where TContext : ISpEntryDataContext;

    }
}

