using JetBrains.Annotations;
using Microsoft.SharePoint.Client;
using SP.Client.Linq.Attributes;
using SP.Client.Linq.Query;
using System.Collections.Generic;
using System.Linq;

namespace SP.Client.Linq.Infrastructure
{
    public sealed class SpEntityEntry<TEntity, TContext>
        where TEntity : class, IListItemEntity
        where TContext : class, ISpEntryDataContext
    {
        private readonly SpQueryManager<TEntity, TContext> _manager;

        public SpEntityEntry([NotNull] TEntity entity, [NotNull] SpQueryArgs<TContext> args)
        {
            Entity = entity;
            Context = args.Context;
            SpQueryArgs = args;
            _manager = new SpQueryManager<TEntity, TContext>(args);
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
                    if (!SpQueryArgs.FieldMappings.ContainsKey(value.Key)) continue;
                    var fieldMapping = SpQueryArgs.FieldMappings[value.Key];
                    if (value.Value != null && fieldMapping.Name.ToLower() == "owshiddenversion")
                    {
                        Version = (int)value.Value;
                    }
                    if (!Equals(default, value.Value))
                    {
                        OriginalValues[value.Key] = value.Value;
                    }
                }
            }
        }

        private void Context_OnSaveChanges(SpSaveArgs args)
        {
            if (HasChanges())
            {
                var item = Save();
                if (item != null)
                {
                    args.Items.Add(item);
                    State = EntityState.Detached;
                }
                args.HasChanges = item != null;
            }
        }

        private ListItem Save()
        {
            switch (State)
            {
                case EntityState.Added:
                case EntityState.Modified:
                    return _manager.Update(Entity.Id, Entity.Id > 0 ? CurrentValues : OriginalValues, Version);
                case EntityState.Deleted:
                    return _manager.DeleteItems(new[] { Entity.Id }).FirstOrDefault();
            }
            return null;
        }

        public TContext Context { get; }
        public SpQueryArgs<TContext> SpQueryArgs { get; }
        public TEntity Entity { get; private set; }

        private Dictionary<string, object> CurrentValues { get; set; }

        private Dictionary<string, object> OriginalValues { get; set; }

        public int Version { get; private set; }

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
                if (!OriginalValues.ContainsKey(value.Key)) continue;
                if (Entity.Id <= 0)
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
