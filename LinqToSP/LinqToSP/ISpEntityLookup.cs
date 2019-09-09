using SP.Client.Linq.Infrastructure;
using SP.Client.Linq.Query;

namespace SP.Client.Linq
{
    public interface ISpEntityLookup
    {
        SpQueryArgs<ISpEntryDataContext> SpQueryArgs { get; }
        int EntityId { get; set; }
    }

    public interface ISpEntityLookup<TEntity> : ISpEntityLookup
     where TEntity : class, IListItemEntity
    {
        TEntity GetEntity();
        void SetEntity(TEntity entity);

        SpEntityEntry<TEntity, ISpEntryDataContext> GetEntry();
    }
}
