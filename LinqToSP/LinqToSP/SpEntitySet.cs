using SP.Client.Linq.Infrastructure;
using SP.Client.Linq.Query;
using System;

namespace SP.Client.Linq
{
    public class SpEntitySet<TEntity> : SpEntityQueryable<TEntity>, ISpEntitySet<TEntity>
       where TEntity : class, IListItemEntity
    {
        public SpQueryArgs<ISpEntryDataContext> SpQueryArgs { get; }

        public SpEntitySet(string listTitle)
            : this(null, listTitle)
        {
        }

        public SpEntitySet(Uri listUrl)
         : this(null, listUrl)
        {
        }

        public SpEntitySet(Guid listId)
          : this(null, listId)
        {
        }

        public SpEntitySet(ISpEntryDataContext context, string listTitle)
           : this(new SpQueryArgs<ISpEntryDataContext>(context, listTitle, null, default, null))
        {
        }

        public SpEntitySet(ISpEntryDataContext context, Uri listUrl)
         : this(new SpQueryArgs<ISpEntryDataContext>(context, null, Convert.ToString(listUrl), default, null))
        {
        }

        public SpEntitySet(ISpEntryDataContext context, Guid listId)
          : this(new SpQueryArgs<ISpEntryDataContext>(context, null, null, listId, null))
        {
        }

        internal SpEntitySet(SpQueryArgs<ISpEntryDataContext> args)
             : base(args)
        {
            SpQueryArgs = args;
        }
    }
}
