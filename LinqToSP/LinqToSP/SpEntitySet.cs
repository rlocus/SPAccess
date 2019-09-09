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
            var entry = Entry(entity, false);
            if (entry != null)
            {
                if (entity.Id > 0)
                {
                    entry.Reload(true);
                }
                entry.Update();
                return entry.Entity;
            }
            return entity;
        }

        public TEntity Add([NotNull] TEntity entity, out SpEntityEntry<TEntity, ISpEntryDataContext> entry)
        {
            entry = Entry(entity, false);
            if (entry != null)
            {
                if (entity.Id > 0)
                {
                    entry.Reload(true);
                }
                entry.Update();
                return entry.Entity;
            }
            return entity;
        }

        public TEntity Add([NotNull] TEntity entity, Action<SpEntityEntry<TEntity, ISpEntryDataContext>> action)
        {
            if (action == null)
            {
                return Add(entity);
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
        public override IEnumerable<TEntity> AddRange([NotNull] IEnumerable<TEntity> entities)
        {
            return entities.Select(entity => Add(entity));
        }

        public IEnumerable<TEntity> AddRange([NotNull] IEnumerable<TEntity> entities, out IEnumerable<SpEntityEntry<TEntity, ISpEntryDataContext>> entries)
        {
            IEnumerable<TEntity> outEntities = Enumerable.Empty<TEntity>();
            entries = Enumerable.Empty<SpEntityEntry<TEntity, ISpEntryDataContext>>();
            foreach (var entity in entities)
            {
                SpEntityEntry<TEntity, ISpEntryDataContext> entry;
                var outEntity = Add(entity, out entry);
                if (outEntity != null)
                {
                    outEntities = outEntities.Concat(new[] { outEntity });
                }
                if (entry != null)
                {
                    entries = entries.Concat(new[] { entry });
                }
            }
            return outEntities;
        }

        public IEnumerable<TEntity> AddRange([NotNull] IEnumerable<TEntity> entities, Action<SpEntityEntry<TEntity, ISpEntryDataContext>> action)
        {
            return entities.Select(entity => Add(entity, action));
        }

        public override bool Remove(TEntity entity)
        {
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
