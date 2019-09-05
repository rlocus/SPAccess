using JetBrains.Annotations;
using SP.Client.Linq.Attributes;
using SP.Client.Linq.Query;
using System.Collections.Generic;
using System.Linq;

namespace SP.Client.Linq.Infrastructure
{
    public sealed class SpEntityEntry<TEntity, TContext>
        where TEntity : class, IListItemEntity
        where TContext : ISpEntryDataContext
    {
        public SpEntityEntry([NotNull] TEntity entity, [NotNull] SpQueryArgs<TContext> args)
        {
            Entity = entity;
            Context = args.Context;
            SpQueryArgs = args;

            Context.OnSaveChanges += Context_OnSaveChanges;

            Init();
        }

        private void Init()
        {
            CurrentValues = new Dictionary<string, object>();
            OriginalValues = new Dictionary<string, object>();

            if (Entity != null)
            {
                State = Entity.Id > 0 ? EntityState.Unchanged : EntityState.Detached;
                foreach (var value in GetValues())
                {
                    OriginalValues[value.Key] = value.Value;
                }
            }
        }

        private void Context_OnSaveChanges(SpSaveArgs args)
        {
            if (HasChanges())
            {
                args.ItemCount++;
                args.HasChanges = Save();
            }
        }

        private bool Save()
        {
            return false;
        }

        public TContext Context { get; }
        public SpQueryArgs<TContext> SpQueryArgs { get; }
        public TEntity Entity { get; private set; }

        private Dictionary<string, object> CurrentValues { get; set; }

        private Dictionary<string, object> OriginalValues { get; set; }

        public EntityState State { get; private set; }

        public bool IsValueChanged(string propertyName)
        {
            if (CurrentValues != null)
            {
                return CurrentValues.ContainsKey(propertyName);
            }
            return false;
        }

        public bool Reload()
        {
            if (Entity != null && Entity.Id > 0 && Context != null && SpQueryArgs != null)
            {
                Entity = Context.List<TEntity>(SpQueryArgs as SpQueryArgs<ISpEntryDataContext>).Find(Entity.Id);
                Init();
                return Entity != null;
            }
            return false;
        }

        public void Update()
        {
            if (DetectChanges())
            {
                State = Entity.Id > 0 ? EntityState.Added : EntityState.Modified;
            }
        }

        public void Delete()
        {
            State = Entity.Id > 0 ? EntityState.Deleted : EntityState.Detached;
        }

        public bool DetectChanges()
        {
            CurrentValues = new Dictionary<string, object>();
            foreach (var value in GetValues())
            {
                if (State == EntityState.Added)
                {
                    CurrentValues[value.Key] = value.Value;
                }
                else if (!Equals(OriginalValues[value.Key], value.Value))
                {
                    CurrentValues[value.Key] = value.Value;
                }
            }
            return CurrentValues.Count > 0;
        }

        public bool HasChanges()
        {
            return this.State == EntityState.Added || this.State == EntityState.Modified || this.State == EntityState.Deleted;
        }

        private IEnumerable<KeyValuePair<string, object>> GetValues()
        {
            return AttributeHelper.GetFieldValues<TEntity, FieldAttribute>(Entity)
              .Concat(AttributeHelper.GetPropertyValues<TEntity, FieldAttribute>(Entity));
        }
    }
}
