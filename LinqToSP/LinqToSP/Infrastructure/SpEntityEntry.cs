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
        #region Fields
        private readonly SpQueryManager<TEntity, TContext> _manager;
        #endregion

        #region Constructors
        public SpEntityEntry([NotNull] TEntity entity, [NotNull] SpQueryArgs<TContext> args)
        {
            Entity = entity;
            SpQueryArgs = args;
            _manager = new SpQueryManager<TEntity, TContext>(args);
            Context.OnSaveChanges += Context_OnSaveChanges;

            Init();
        }

        #endregion

        #region Properties
        public TContext Context { get { return SpQueryArgs.Context; } }
        public SpQueryArgs<TContext> SpQueryArgs { get; }
        public TEntity Entity { get; private set; }

        private Dictionary<string, object> CurrentValues { get; set; }

        private Dictionary<string, object> OriginalValues { get; set; }

        public int Version { get; private set; }

        public EntityState State { get; private set; }

        public bool HasChanges => State == EntityState.Added || State == EntityState.Modified || State == EntityState.Deleted;

        #endregion

        #region Methods
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
            if (HasChanges)
            {
                var item = Save();
                if (item != null)
                {
                    args.Items.Add(item);

                    CurrentValues = new Dictionary<string, object>();
                    //requires to reload it after saving item.
                    State = EntityState.Detached;
                    args.HasChanges = true;
                }
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

        private IEnumerable<KeyValuePair<string, object>> GetValues()
        {
            return AttributeHelper.GetFieldValues<TEntity, FieldAttribute>(Entity)
              .Concat(AttributeHelper.GetPropertyValues<TEntity, FieldAttribute>(Entity));
        }

        private bool DetectChanges()
        {
            if (State == EntityState.Deleted) return false;
            if (State == EntityState.Added) return true;
            if (State == EntityState.Detached && Entity.Id > 0) return false;

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
                State = Entity.Id > 0 ? EntityState.Modified : EntityState.Added;
            }
        }

        public void Delete()
        {
            State = Entity.Id > 0 ? EntityState.Deleted : EntityState.Detached;
        }

        #endregion
    }
}
