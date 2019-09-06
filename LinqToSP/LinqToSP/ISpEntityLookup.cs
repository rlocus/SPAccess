using SP.Client.Linq.Infrastructure;
using SP.Client.Linq.Query;

namespace SP.Client.Linq
{
    public interface ISpEntityLookup
    {
        SpQueryArgs<ISpEntryDataContext> SpQueryArgs { get; }
        int EntityId { get; set; }
        bool DoesEntryExist();
    }

    public interface ISpEntityLookup<TEntity> : ISpEntityLookup
     where TEntity : class, IListItemEntity
    {
        TEntity GetEntity();
        SpEntityEntry<TEntity, ISpEntryDataContext> GetEntry();
    }
}
