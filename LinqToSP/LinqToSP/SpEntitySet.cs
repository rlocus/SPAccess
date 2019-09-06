using JetBrains.Annotations;
using SP.Client.Linq.Infrastructure;
using SP.Client.Linq.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SP.Client.Linq
{
    public sealed class SpEntitySet<TEntity> : SpEntityQueryable<TEntity>, ISpEntitySet<TEntity>
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

        public override TEntity Add([NotNull] TEntity entity)
        {
            if (entity.Id > 0)
            {
                //return base.Add(entity);
                return entity;
            }
            var entry = Entry(entity, false);
            if (entry != null)
            {
                entry.Update();
                return entry.Entity;
            }
            return entity;
        }

        public TEntity Add([NotNull] TEntity entity, Action<SpEntityEntry<TEntity, ISpEntryDataContext>> action)
        {
            if (action == null)
            {
                return base.Add(entity);
            }
            var entry = Entry(entity, false);
            if (entry != null)
            {
                action(entry);
                entry.Update();
                return entry.Entity;
            }
            return entity;
        }

        public IEnumerable<TEntity> AddRange([NotNull] IEnumerable<TEntity> entities, Action<SpEntityEntry<TEntity, ISpEntryDataContext>> action)
        {
            if (action == null)
            {
                return base.AddRange(entities);
            }
            return entities.Select(entity => Add(entity, action));
        }

        public override bool Remove(TEntity entity)
        {
            //return base.Remove(entity);
            var entry = Entry(entity, false);
            if (entry != null)
            {
                entry.Delete();
                return entry.Entity != null;
            }
            return false;
        }

        public override int RemoveRange(IEnumerable<TEntity> entities)
        {
            return entities.Select(entity => Remove(entity)).Count(removed => removed == true);
        }
    }
}
