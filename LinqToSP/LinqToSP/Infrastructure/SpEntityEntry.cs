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
        private ListItem _item;
        #endregion

        #region Constructors
        public SpEntityEntry([NotNull] TEntity entity, [NotNull] SpQueryArgs<TContext> args)
        {
            EntityId = entity != null ? entity.Id : 0;
            Entity = entity;
            SpQueryArgs = args;
            _manager = new SpQueryManager<TEntity, TContext>(args);
            Context.OnBeforeSaveChanges += Context_OnOnBeforeSaveChanges;
            Context.OnAfterSaveChanges += Context_OnAfterSaveChanges;
            Attach();
        }

        #endregion

        #region Properties
        public TContext Context { get { return SpQueryArgs.Context; } }
        public SpQueryArgs<TContext> SpQueryArgs { get; }
        public TEntity Entity { get; private set; }
        public int EntityId { get; private set; }

        private Dictionary<string, object> CurrentValues { get; set; }

        private Dictionary<string, object> OriginalValues { get; set; }

        public int Version { get; private set; }

        public EntityState State { get; private set; }

        public bool HasChanges => State == EntityState.Added || State == EntityState.Modified || State == EntityState.Deleted;

        #endregion

        #region Methods
        public void Attach()
        {
            CurrentValues = new Dictionary<string, object>();
            OriginalValues = new Dictionary<string, object>();
            State = EntityState.Unchanged;

            foreach (var value in GetValues(Entity))
            {
                if (!SpQueryArgs.FieldMappings.ContainsKey(value.Key)) continue;
                var fieldMapping = SpQueryArgs.FieldMappings[value.Key];
                if (fieldMapping.IsReadOnly) continue;

                if (value.Value != null && fieldMapping.Name.ToLower() == "owshiddenversion")
                {
                    Version = (int)value.Value;
                }
                if (value.Value is ISpEntityLookup)
                {
                    OriginalValues[value.Key] = (value.Value as ISpEntityLookup).EntityId;
                }
                if (value.Value is ISpEntityLookupCollection)
                {
                    OriginalValues[value.Key] = (value.Value as ISpEntityLookupCollection).EntityIds;
                }
                else if (!Equals(default, value.Value))
                {
                    OriginalValues[value.Key] = value.Value;
                }
            }
        }

        public void Detach()
        {
            CurrentValues = new Dictionary<string, object>();
            //requires to reload it after saving item.
            State = EntityState.Detached;
        }

        private void Context_OnOnBeforeSaveChanges(SpSaveArgs args)
        {
            if (HasChanges)
            {
                _item = Save();
                if (_item != null)
                {
                    args.Items.Add(_item);
                    //requires to reload it after saving item.
                    Detach();
                    args.HasChanges = true;
                }
            }
        }
        private void Context_OnAfterSaveChanges(SpSaveArgs args)
        {
            if (_item != null)
            {
                EntityId = _item.Id;
            }
        }
        private ListItem Save()
        {
            switch (State)
            {
                case EntityState.Added:
                case EntityState.Modified:
                    return _manager.Update(Entity.Id, CurrentValues, Version);
                case EntityState.Deleted:
                    return _manager.DeleteItems(new[] { Entity.Id }).FirstOrDefault();
            }
            return null;
        }

        private static IEnumerable<KeyValuePair<string, object>> GetValues(TEntity entity)
        {
            return AttributeHelper.GetFieldValues<TEntity, FieldAttribute>(entity)
              .Concat(AttributeHelper.GetPropertyValues<TEntity, FieldAttribute>(entity));
        }

        public bool DetectChanges()
        {
            if (State == EntityState.Deleted) return false;
            if (State == EntityState.Detached) return false;

            CurrentValues = new Dictionary<string, object>();
            foreach (var value in GetValues(Entity))
            {
                if (!SpQueryArgs.FieldMappings.ContainsKey(value.Key)) continue;
                var fieldMapping = SpQueryArgs.FieldMappings[value.Key];
                if (fieldMapping.IsReadOnly) continue;

                if (value.Value is ISpEntityLookup)
                {
                    if (EntityId <= 0 || (!OriginalValues.ContainsKey(value.Key) || !Equals(OriginalValues[value.Key], (value.Value as ISpEntityLookup).EntityId)))
                    {
                        if (EntityId > 0)
                        {
                            if (!OriginalValues.ContainsKey(value.Key) && Equals(default, value.Value)) continue;
                            CurrentValues[value.Key] = (value.Value as ISpEntityLookup).EntityId;
                        }
                        else
                        {
                            CurrentValues[value.Key] = (value.Value as ISpEntityLookup).EntityId;
                        }
                    }
                }
                else if (value.Value is ISpEntityLookupCollection)
                {
                    if (EntityId <= 0 || (!OriginalValues.ContainsKey(value.Key) || !Equals(OriginalValues[value.Key], (value.Value as ISpEntityLookupCollection).EntityIds)))
                    {
                        if (EntityId > 0)
                        {
                            if (!OriginalValues.ContainsKey(value.Key) && Equals(default, value.Value)) continue;
                            CurrentValues[value.Key] = (value.Value as ISpEntityLookupCollection).EntityIds;
                        }
                        else
                        {
                            CurrentValues[value.Key] = (value.Value as ISpEntityLookupCollection).EntityIds;
                        }
                    }
                }
                else if (EntityId <= 0 || (!OriginalValues.ContainsKey(value.Key) || !Equals(OriginalValues[value.Key], value.Value)))
                {
                    if (EntityId > 0)
                    {
                        if (!OriginalValues.ContainsKey(value.Key) && Equals(default, value.Value)) continue;
                        CurrentValues[value.Key] = value.Value;
                    }
                    else if (!Equals(default, value.Value))
                    {
                        CurrentValues[value.Key] = value.Value;
                    }
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

        public TEntity Reload(bool setOriginalValuesOnly = false)
        {
            if (EntityId > 0 && Context != null && SpQueryArgs != null)
            {
                var originalEntity = Entity;
                Detach();
                var entity = (Context.List<TEntity>(SpQueryArgs as SpQueryArgs<ISpEntryDataContext>) as ISpRepository<TEntity>).Find(EntityId);
                Entity = entity;
                Attach();

                if (setOriginalValuesOnly)
                {
                    Entity = originalEntity;
                }
                return entity;
            }
            return Entity;
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
