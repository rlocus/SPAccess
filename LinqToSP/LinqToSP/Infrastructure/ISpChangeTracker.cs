using System.Collections.Generic;

namespace SP.Client.Linq.Infrastructure
{
  public interface ISpChangeTracker<TEntity, TContext>
  where TEntity : class, IListItemEntity
   where TContext : ISpEntryDataContext
  {
    IEnumerable<SpEntityEntry<TEntity, TContext>> Entries();
  }
}

