﻿using Microsoft.SharePoint.Client;
using SP.Client.Extensions;
using SP.Client.Helpers;
using SP.Client.Linq.Attributes;
using SP.Client.Linq.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SP.Client.Linq
{
    internal sealed class SpQueryManager<TEntity, TContext>
        where TEntity : class, IListItemEntity
        where TContext : class, ISpEntryDataContext
    {
        #region Fields
        private readonly SpQueryArgs<TContext> _args;
        #endregion

        #region Constructors
        public SpQueryManager(SpQueryArgs<TContext> args)
        {
            _args = args;
        }

        #endregion

        #region Methods

        private static void CheckEntityType(Type type)
        {
            if (!typeof(TEntity).IsAssignableFrom(type) && !type.IsSubclassOf(typeof(TEntity)))
            {
                throw new Exception($"Entity must be assignable from {typeof(TEntity)}");
            }
        }

        private static object GetFieldValue(FieldAttribute fieldAttr, Type valueType, object value)
        {
            if (value != null)
            {
                if (fieldAttr.DataType == FieldType.Choice || fieldAttr.DataType == FieldType.MultiChoice)
                {
                    value = EnumExtensions.ParseChoiceValue(valueType, value.ToString());
                }
                else if (fieldAttr.DataType == FieldType.Lookup && (typeof(LookupFieldAttribute).IsAssignableFrom(fieldAttr.GetType()) || fieldAttr.GetType().IsSubclassOf(typeof(LookupFieldAttribute))))
                {
                    var lookupFieldMap = fieldAttr as LookupFieldAttribute;

                    if (lookupFieldMap.Result == LookupItemResult.None) return value;

                    if (value is FieldLookupValue)
                    {
                        if (!typeof(FieldLookupValue).IsAssignableFrom(valueType) && !valueType.IsSubclassOf(typeof(FieldLookupValue)))
                        {
                            value = lookupFieldMap.Result == LookupItemResult.Id
                                ? (object)(value as FieldLookupValue).LookupId
                                : (value as FieldLookupValue).LookupValue;

                            if (valueType.IsArray)
                            {
                                var elType = (valueType.GetElementType()
                                 ?? valueType.GenericTypeArguments.FirstOrDefault())
                                 ?? typeof(object);
                                value = new[] { SpConverter.ConvertValue(value, elType) }.ToArray(elType);
                            }
                        }
                    }
                    else if (value is FieldLookupValue[])
                    {
                        if (!lookupFieldMap.IsMultiple)
                        {
                            var lookupValue = (value as FieldLookupValue[]).FirstOrDefault();
                            if (lookupValue != null)
                            {
                                if (!typeof(FieldLookupValue).IsAssignableFrom(valueType) && !valueType.IsSubclassOf(typeof(FieldLookupValue)))
                                {
                                    value = lookupFieldMap.Result == LookupItemResult.Id ? (object)lookupValue.LookupId : lookupValue.LookupValue;
                                }
                                else
                                {
                                    value = lookupValue;
                                }
                            }
                            else
                            {
                                value = null;
                            }
                        }
                        else
                        {
                            var elType = (valueType.GetElementType()
                                ?? valueType.GenericTypeArguments.FirstOrDefault())
                                ?? typeof(object);
                            if (!typeof(FieldLookupValue).IsAssignableFrom(elType) && !elType.IsSubclassOf(typeof(FieldLookupValue)))
                            {
                                var result = lookupFieldMap.Result == LookupItemResult.Id
                                ? (value as FieldLookupValue[]).Select(v => SpConverter.ConvertValue(v.LookupId, elType))
                                : (value as FieldLookupValue[]).Select(v => SpConverter.ConvertValue(v.LookupValue, elType));
                                if (valueType.IsArray)
                                {
                                    value = result.ToArray(elType);
                                }
                                else
                                {
                                    value = result.ToList(elType);
                                }
                            }
                        }
                    }
                }
            }
            return value;
        }

        private bool SetEntityLookup(Type type, object value, object itemValue)
        {
            if (value is ISpEntityLookup || typeof(ISpEntityLookup).IsAssignableFrom(type))
            {
                var entitySet = (ISpEntityLookup)value;
                if (entitySet != null && itemValue != null && itemValue is FieldLookupValue)
                {
                    int entityId = ((FieldLookupValue)itemValue).LookupId;
                    entitySet.EntityId = entityId;
                    if (entitySet.SpQueryArgs != null)
                    {
                        entitySet.SpQueryArgs.Context = _args.Context;
                    }
                }
                return true;
            }
            else if (value is ISpEntityLookupCollection || typeof(ISpEntityLookupCollection).IsAssignableFrom(type))
            {
                var entitySets = (ISpEntityLookupCollection)value;
                if (entitySets != null && itemValue != null && itemValue is FieldLookupValue[])
                {
                    entitySets.EntityIds = ((FieldLookupValue[])itemValue).Select(lv => lv.LookupId).ToArray();
                    if (entitySets.SpQueryArgs != null)
                    {
                        entitySets.SpQueryArgs.Context = _args.Context;
                    }
                }
                return true;
            }

            return false;
        }

        public List GetList()
        {
            if (_args != null && _args.Context != null)
            {
                var clientContext = _args.Context.Context;
                if (clientContext != null)
                {
                    return !string.IsNullOrEmpty(_args.ListUrl) ? clientContext.Web.GetList($"{_args.Context.SiteUrl.TrimEnd('/')}/{_args.ListUrl.TrimStart('/')}") :
                           (_args.ListId != default ? clientContext.Web.Lists.GetById(_args.ListId) :
                           (!string.IsNullOrEmpty(_args.ListTitle) ? clientContext.Web.Lists.GetByTitle(_args.ListTitle) : null));
                }
            }
            return null;
        }

        public ListItemCollection GetItems(Caml.View spView, ListItemCollectionPosition position)
        {
            var list = GetList();
            if (list != null)
            {
                var items = list.GetItems(new CamlQuery() { ViewXml = spView.ToString(true), ListItemCollectionPosition = position });
                items.Context.Load(items, item => item.Include(i => i.EffectiveBasePermissions));
                items.Context.Load(items, item => item.ListItemCollectionPosition);
                return items;
            }
            return null;
        }

        public IEnumerable<TEntity> GetEntities(Type type, Caml.View spView)
        {
            CheckEntityType(type);
            ListItemCollectionPosition position = null;
            var entities = Enumerable.Empty<TEntity>();
            if (_args == null || spView == null) return entities;

            var rowLimit = spView.Limit;
            int itemCount = 0;
            do
            {
                if (_args.BatchSize > 0)
                {
                    if (rowLimit > 0)
                    {
                        spView.Limit = Math.Min(rowLimit - itemCount, _args.BatchSize);
                    }
                    else
                    {
                        spView.Limit = _args.BatchSize;
                    }
                    if (spView.Limit == 0)
                    {
                        break;
                    }
                }
                var items = GetItems(spView, position);
                if (items != null)
                {
                    items.Context.ExecuteQuery();
                    if (_args.BatchSize > 0)
                    {
                        position = items.ListItemCollectionPosition;
                    }
                    itemCount += items.Count;
                    entities = entities.Concat(MapEntities(items, type));
                }
            }
            while (position != null);

            spView.Limit = rowLimit;
            return entities;
        }

        public async Task<IEnumerable<TEntity>> GetEntitiesAsync(Type type, Caml.View spView)
        {
            CheckEntityType(type);
            ListItemCollectionPosition position = null;
            var entities = Enumerable.Empty<TEntity>();
            if (_args == null || spView == null) return entities;

            var rowLimit = spView.Limit;
            int itemCount = 0;
            do
            {
                if (_args.BatchSize > 0)
                {
                    if (rowLimit > 0)
                    {
                        spView.Limit = Math.Min(rowLimit - itemCount, _args.BatchSize);
                    }
                    else
                    {
                        spView.Limit = _args.BatchSize;
                    }
                    if (spView.Limit == 0)
                    {
                        break;
                    }
                }
                var items = GetItems(spView, position);
                if (items != null)
                {
                    await items.Context.ExecuteQueryAsync();

                    if (_args.BatchSize > 0)
                    {
                        position = items.ListItemCollectionPosition;
                    }
                    itemCount += items.Count;
                    entities = entities.Concat(MapEntities(items, type));
                }
            }
            while (position != null);

            spView.Limit = rowLimit;
            return entities;
        }

        public TEntity MapEntity(TEntity entity, ListItem item)
        {
            if (_args == null || entity == null || item == null) return entity;

            foreach (var fieldMap in _args.FieldMappings)
            {
                PropertyInfo prop = entity.GetType().GetProperty(fieldMap.Key, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop)
                {
                    if (item.FieldValues.ContainsKey(fieldMap.Value.Name))
                    {
                        object value = item[fieldMap.Value.Name];
                        if (!SetEntityLookup(prop.PropertyType, prop.GetValue(entity), value))
                        {
                            if (prop.CanWrite)
                            {
                                value = GetFieldValue(fieldMap.Value, prop.PropertyType, value);
                                value = SpConverter.ConvertValue(value, prop.PropertyType);
                                prop.SetValue(entity, value);
                            }
                        }
                    }
                }
                FieldInfo field = entity.GetType().GetField(fieldMap.Key, BindingFlags.Public | BindingFlags.Instance);
                if (null != field)
                {
                    if (item.FieldValues.ContainsKey(fieldMap.Value.Name))
                    {
                        object value = item[fieldMap.Value.Name];
                        value = GetFieldValue(fieldMap.Value, field.FieldType, value);
                        value = SpConverter.ConvertValue(value, field.FieldType);
                        field.SetValue(entity, value);
                    }
                }
            }
            if (_args.IncludeItemPermissions)
            {
                if (item.IsPropertyAvailable("EffectiveBasePermissions"))
                {
                    if (entity is ListItemEntity)
                    {
                        (entity as ListItemEntity).EffectiveBasePermissions = item.EffectiveBasePermissions;
                    }
                }
            }
            return entity;
        }

        public IEnumerable<ISpEntitySet> GetEntitySets(TEntity entity)
        {
            return AttributeHelper.GetFieldValuesOfType<TEntity, ISpEntitySet>(entity).Select(val => val.Value)
                .Concat(AttributeHelper.GetPropertyValuesOfType<TEntity, ISpEntitySet>(entity).Select(val => val.Value))
                .Cast<ISpEntitySet>();
        }

        public IEnumerable<TEntity> MapEntities(ListItemCollection items, Type type)
        {
            return MapEntities(items.Cast<ListItem>(), type);
        }

        public IEnumerable<TEntity> MapEntities(IEnumerable<ListItem> items, Type type)
        {
            return items.Select(item => MapEntity(item, type));
        }

        public TEntity MapEntity(ListItem item, Type type)
        {
            var entity = MapEntity((TEntity)Activator.CreateInstance(type, new object[] { }), item);
            if (_args != null)
            {
                foreach (var entitySet in GetEntitySets(entity))
                {
                    if (entitySet != null && entitySet.SpQueryArgs != null)
                    {
                        entitySet.SpQueryArgs.Context = _args.Context;
                    }
                }
            }
            return entity;
        }

        public ListItem Update(int itemId, Dictionary<string, object> properties, int version)
        {
            if (properties == null || _args == null) return null;

            List list = GetList();
            ListItem listItem = itemId > 0
                ? list.GetItemById(itemId)
                : list.AddItem(new ListItemCreationInformation());

            var fieldMappings = _args.FieldMappings;
            bool fUpdate = false;
            foreach (var property in properties)
            {
                if (!fieldMappings.ContainsKey(property.Key)) continue;

                var fieldMapping = fieldMappings[property.Key];

                if (fieldMapping.IsReadOnly
                    || typeof(DependentLookupFieldAttribute).IsAssignableFrom(fieldMapping.GetType())
                    || typeof(CalculatedFieldAttribute).IsAssignableFrom(fieldMapping.GetType())
                    || fieldMapping.DataType == FieldType.Calculated)
                {
                    continue;
                }

                var value = property.Value;
                if (itemId > 0 || (itemId <= 0 && !Equals(value, default)))
                {
                    if (value != null)
                    {
                        if (fieldMapping.DataType == FieldType.Choice || fieldMapping.DataType == FieldType.MultiChoice)
                        {
                            Type valueType = value.GetType();
                            value = EnumExtensions.GetChoiceValue(valueType, value);
                        }
                        else if (fieldMapping.DataType == FieldType.Lookup)
                        {
                            if (typeof(LookupFieldAttribute).IsAssignableFrom(fieldMapping.GetType()))
                            {
                                if ((fieldMapping as LookupFieldAttribute).Result == LookupItemResult.Value)
                                {
                                    continue;
                                }
                                else if((fieldMapping as LookupFieldAttribute).IsMultiple)
                                {
                                    //TODO: update lookup field with multiple values.
                                }
                            }
                        }
                    }
                    listItem[fieldMapping.Name] = value;
                    fUpdate = true;
                }
            }

            if (fUpdate)
            {
                if (version > 0)
                {
                    listItem["owshiddenversion"] = version;
                }
                listItem.Update();
                listItem.Context.Load(listItem);
                return listItem;
            }
            return null;
        }

        public IEnumerable<ListItem> DeleteItems(IEnumerable<int> itemIds)
        {
            if (itemIds != null && itemIds.Any())
            {
                List list = GetList();
                foreach (int itemId in itemIds)
                {
                    ListItem listItem = list.GetItemById(itemId);
                    list.Context.Load(listItem);
                    listItem.DeleteObject();
                    yield return listItem;
                }
            }
        }

        #endregion
    }
}
