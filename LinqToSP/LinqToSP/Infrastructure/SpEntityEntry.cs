using JetBrains.Annotations;
using SP.Client.Linq.Attributes;
using SP.Client.Linq.Query;
using System.Collections.Generic;
using System.Linq;

namespace SP.Client.Linq.Infrastructure
{
    public sealed class SpEntityEntry<TEntity, TContext>
        where TEntity : class, IListItemEntity
        where TContext: ISpEntryDataContext
    {
        public SpEntityEntry([NotNull] TEntity entity, [NotNull] SpQueryArgs<TContext> args)
        {
            Entity = entity;
            Context = args.Context;

            Context.OnSaveChanges += Context_OnSaveChanges;

            State = entity.Id > 0 ? EntityState.Unchanged : EntityState.Detached;
            OriginalValues = new Dictionary<string, object>();

            foreach (var value in GetValues())
            {
                OriginalValues[value.Key] = value.Value;
            }
        }

        private void Context_OnSaveChanges(SpSaveArgs args)
        {
            if (HasChanges())
            {
                args.HasChanges = true;
            }
            args.HasChanges = true;
        }

        public TContext Context { get; }

        public TEntity Entity { get; private set; }

        private Dictionary<string, object> CurrentValues { get; set; }

        private Dictionary<string, object> OriginalValues { get; }

        public EntityState State { get; private set; }

        public bool IsValueChanged(string propertyName)
        {
            if (CurrentValues != null)
            {
                return CurrentValues.ContainsKey(propertyName);
            }
            return false;
        }

        public bool Reload(string listName, string query = null)
        {
            if(Entity != null && Context != null)
            {
                Entity = Context.List<TEntity>(listName, query).FirstOrDefault();
                return true;
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
                else if (OriginalValues[value.Key] != value.Value)
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
