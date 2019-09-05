using SP.Client.Linq.Infrastructure;
using SP.Client.Linq.Query;
using System;

namespace SP.Client.Linq
{
  public interface ISpEntryDataContext : ISpDataContext
  {
    bool HasChanges { get; }

    event Action<SpSaveArgs> OnSaveChanges;
    void SaveChanges();

    SpEntityQueryable<TListItem> List<TListItem>(SpQueryArgs<ISpEntryDataContext> args) where TListItem : class, IListItemEntity;
  }
}
