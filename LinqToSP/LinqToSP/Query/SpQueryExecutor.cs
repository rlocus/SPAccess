using System.Collections.Generic;
using System.Linq;
using Remotion.Linq;
using System.Data;
using SP.Client.Linq.Query.ExpressionVisitors;
using Microsoft.SharePoint.Client;
using SpView = SP.Client.Caml.View;
using System;
using System.Reflection;
using SP.Client.Helpers;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using Remotion.Linq.Clauses.ResultOperators;
using SP.Client.Linq.Query.Expressions;
using SP.Client.Linq.Attributes;
using SP.Client.Extensions;
using SP.Client.Caml;

namespace SP.Client.Linq.Query
{
    /// <summary>
    /// 
    /// </summary>
    internal class SpQueryExecutor<TContext> : IQueryExecutor
       where TContext : ISpDataContext
    {
        private readonly object _lock = new object();
        internal List<IncludeExpression> IncludeExpressions { get; }
        internal List<GroupByExpression> GroupByExpressions { get; }

        public SpView SpView
        {
            get
            {
                if (SpQueryArgs == null) return null;
                return SpQueryArgs.SpView;
            }
        }

        internal SpQueryArgs<TContext> SpQueryArgs { get; }

        internal SpQueryExecutor(SpQueryArgs<TContext> args)
        {
            ValidateArgs(args);
            SpQueryArgs = args;
            IncludeExpressions = new List<IncludeExpression>();
            GroupByExpressions = new List<GroupByExpression>();
        }

        private void ValidateArgs(SpQueryArgs<TContext> args)
        {

        }

        public TResult ExecuteScalar<TResult>(QueryModel queryModel)
        {
            return ExecuteSingle<TResult>(queryModel, false);
        }

        public TResult ExecuteSingle<TResult>(QueryModel queryModel, bool defaultIfEmpty)
        {
            var results = ExecuteCollection<TResult>(queryModel);
            foreach (var resultOperator in queryModel.ResultOperators)
            {
                if (resultOperator is LastResultOperator)
                    return results.LastOrDefault();
            }
            return (defaultIfEmpty) ? results.FirstOrDefault() : results.First();
        }

        public IEnumerable<TResult> ExecuteCollection<TResult>(QueryModel queryModel)
        {
            lock (_lock)
            {
                if (SpQueryArgs == null) return Enumerable.Empty<TResult>();
                var spView = new SpView();
                if (!string.IsNullOrEmpty(SpQueryArgs.Query))
                {
                    var q = new Caml.Query(SpQueryArgs.Query);
                    spView.Query.Where = q.Where;
                    spView.Query.OrderBy = q.OrderBy;
                    spView.Query.GroupBy = q.GroupBy;
                }

                SpQueryArgs.SpView = spView;
                var queryVisitor = new SpGeneratorQueryModelVisitor<TContext>(SpQueryArgs);
                queryVisitor.VisitQueryModel(queryModel);
                queryVisitor.VisitIncludeClauses(IncludeExpressions, queryModel);
                queryVisitor.VisitGroupByClauses(GroupByExpressions, queryModel);

                if (spView.ViewFields == null)
                {
                    spView.ViewFields =
                    new ViewFieldsCamlElement(SpQueryArgs.FieldMappings.Select(fieldMapping => fieldMapping.Value.Name));
                }
                else if (!spView.ViewFields.Any())
                {
                    spView.ViewFields.AddRange(SpQueryArgs.FieldMappings.Select(fieldMapping => fieldMapping.Value.Name));
                }

                spView.Joins = new JoinsCamlElement();
                spView.ProjectedFields = new ProjectedFieldsCamlElement();

                foreach (var dependentLookupField in SpQueryArgs.FieldMappings.Values.OfType<DependentLookupFieldAttribute>())
                {
                    if (spView.ViewFields.Any(f => f.Name == dependentLookupField.Name))
                    {
                        if (spView.ProjectedFields == null || !spView.ProjectedFields.Any(f => f.Name == dependentLookupField.Name))
                        {
                            spView.Joins.Join(new LeftJoin(dependentLookupField.LookupFieldName, dependentLookupField.List));
                            spView.ProjectedFields.ShowField(new CamlProjectedField(dependentLookupField.Name, dependentLookupField.List, dependentLookupField.ShowField));
                        }
                    }
                }

                if (SpQueryArgs.SkipResult)
                {
                    return Enumerable.Empty<TResult>();
                }

                Debug.WriteLine("# SP Query:");
                Debug.Write(spView);
                Debug.WriteLine("");

                IEnumerable<TResult> results = GetEntities(typeof(TResult)).Cast<TResult>();

                foreach (var resultOperator in queryModel.ResultOperators)
                {
                    if (resultOperator is ReverseResultOperator)
                        results = results.Reverse();
                }
                return results;
            }
        }

        protected virtual IEnumerable<TResult> GetEntities<TResult>()
        where TResult : ListItemEntity
        {
            return GetEntities(typeof(TResult)).Cast<TResult>();
        }
        protected virtual IEnumerable<object> GetEntities(Type type)
        {
            CheckEntityType(type);
            ListItemCollectionPosition position = null;
            IEnumerable<ListItemEntity> entities = Enumerable.Empty<ListItemEntity>();
            if (SpQueryArgs == null) return entities;

            var rowLimit = SpQueryArgs.SpView.Limit;
            int itemCount = 0;
            do
            {
                if (SpQueryArgs.BatchSize > 0)
                {
                    if (rowLimit > 0)
                    {
                        SpQueryArgs.SpView.Limit = Math.Min(rowLimit - itemCount, SpQueryArgs.BatchSize);
                    }
                    else
                    {
                        SpQueryArgs.SpView.Limit = SpQueryArgs.BatchSize;
                    }
                    if (SpQueryArgs.SpView.Limit == 0)
                    {
                        break;
                    }
                }
                var items = GetItems(SpQueryArgs, position);
                if (items != null)
                {
                    items.Context.ExecuteQuery();
                    if (SpQueryArgs.BatchSize > 0)
                    {
                        position = items.ListItemCollectionPosition;
                    }
                    itemCount += items.Count;
                    entities = entities.Concat(MapEntities(items, type));
                }
            }
            while (position != null);

            SpQueryArgs.SpView.Limit = rowLimit;
            return entities;
        }

        protected static void CheckEntityType(Type type)
        {
            if (!typeof(ListItemEntity).IsAssignableFrom(type) && !type.IsSubclassOf(typeof(ListItemEntity)))
            {
                throw new Exception($"Entity must be assignable from {typeof(ListItemEntity)}");
            }
        }

        protected static List GetList(SpQueryArgs<TContext> args)
        {
            if (args != null)
            {
                var clientContext = args.Context.Context;
                if (clientContext != null)
                {
                    return args.ListTitle != null ? clientContext.Web.Lists.GetByTitle(args.ListTitle) :
                        (args.ListUrl != null ? clientContext.Web.GetList(args.ListUrl)
                        : clientContext.Web.Lists.GetById(args.ListId));
                }
            }
            return null;
        }

        protected static ListItemCollection GetItems(SpQueryArgs<TContext> args, ListItemCollectionPosition position)
        {
            var list = GetList(args);
            if (list != null)
            {
                var items = list.GetItems(new CamlQuery() { ViewXml = args.SpView.ToString(true), ListItemCollectionPosition = position });
                items.Context.Load(items, item => item.Include(i => i.EffectiveBasePermissions));
                items.Context.Load(items, item => item.ListItemCollectionPosition);
                return items;
            }
            return null;
        }

        protected virtual IEnumerable<ListItemEntity> MapEntities(ListItemCollection items, Type type)
        {
            return MapEntities(items.Cast<ListItem>(), type);
        }

        protected virtual IEnumerable<ListItemEntity> MapEntities(IEnumerable<ListItem> items, Type type)
        {
            return items.Select(item => MapEntity((ListItemEntity)Activator.CreateInstance(type, new object[] { }), item));
        }

        protected virtual ListItemEntity MapEntity(ListItem item, Type type)
        {
            return MapEntity((ListItemEntity)Activator.CreateInstance(type, new object[] { }), item);
        }

        protected virtual ListItemEntity MapEntity(ListItemEntity entity, ListItem item)
        {
            if (SpQueryArgs == null || entity == null || item == null) return entity;

            foreach (var fieldMap in SpQueryArgs.FieldMappings)
            {
                PropertyInfo prop = entity.GetType().GetProperty(fieldMap.Key, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanWrite)
                {
                    if (item.FieldValues.ContainsKey(fieldMap.Value.Name))
                    {
                        object value = item[fieldMap.Value.Name];
                        value = GetFieldValue(fieldMap.Value, prop.PropertyType, value);

                        value = SpConverter.ConvertValue(value, prop.PropertyType);
                        prop.SetValue(entity, value);
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
            if (SpQueryArgs.IncludeItemPermissions)
            {
                if (item.IsPropertyAvailable("EffectiveBasePermissions"))
                {
                    entity.EffectiveBasePermissions = item.EffectiveBasePermissions;
                }
            }
            return entity;
        }

        private static object GetFieldValue(FieldAttribute fieldAttr, Type valueType, object value)
        {
            if (value != null)
            {
                if (typeof(LookupFieldAttribute).IsAssignableFrom(fieldAttr.GetType()) || fieldAttr.GetType().IsSubclassOf(typeof(LookupFieldAttribute)))
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

        public IListItemEntity InsertOrUpdateEntity(ListItemEntity entity)
        {
            if (entity == null) return null;

            ListItem listItem = InsertOrUpdateItem(entity);

            if (listItem != null)
            {
                listItem.Context.ExecuteQuery();
                entity = MapEntity(listItem, entity.GetType());
            }
            return entity;
        }

        public IEnumerable<IListItemEntity> InsertOrUpdateEntities(IEnumerable<ListItemEntity> entities)
        {
            if (entities == null || !entities.Any()) return null;

            var items = entities.ToDictionary(entity => entity, entity => InsertOrUpdateItem(entity));
            SpQueryArgs.Context.Context.ExecuteQuery();
            return items.Select(item => MapEntity(item.Value, item.Key.GetType()));
        }

        public ListItem InsertOrUpdateItem(ListItemEntity entity)
        {
            if (entity == null) return null;

            List list = GetList(SpQueryArgs);
            ListItem listItem = entity.Id > 0
                ? list.GetItemById(entity.Id)
                : list.AddItem(new ListItemCreationInformation());

            var fieldMappings = SpQueryArgs.FieldMappings;
            bool fUpdate = false;
            foreach (var fieldMapping in fieldMappings)
            {
                if (fieldMapping.Value.IsReadOnly || typeof(DependentLookupFieldAttribute).IsAssignableFrom(fieldMapping.Value.GetType())) { continue; }

                var prop = entity.GetType().GetProperty(fieldMapping.Key);
                if (prop != null)
                {
                    var value = prop.GetValue(entity);
                    if (entity.Id > 0 || (entity.Id <= 0 && value != prop.PropertyType.GetDefaultValue()))
                    {
                        listItem[fieldMapping.Value.Name] = value;
                        fUpdate = true;
                    }
                }

                var field = entity.GetType().GetField(fieldMapping.Key);
                if (field != null)
                {
                    var value = field.GetValue(entity);
                    if (entity.Id > 0 || (entity.Id <= 0 && value != prop.PropertyType.GetDefaultValue()))
                    {
                        listItem[fieldMapping.Value.Name] = value;
                        fUpdate = true;
                    }
                }
            }

            if (fUpdate)
            {
                listItem.Update();
                listItem.Context.Load(listItem);
                return listItem;
            }
            return null;
        }

        public ListItem DeleteItem(int itemId)
        {
            List list = GetList(SpQueryArgs);
            ListItem listItem = list.GetItemById(itemId);
            listItem.DeleteObject();
            return listItem;
        }

        public int Delete(IEnumerable<int> itemIds)
        {
            var items = DeleteItems(itemIds).ToArray();
            if (items.Length > 0)
            {
                SpQueryArgs.Context.Context.ExecuteQuery();
            }
            return items.Count();
        }

        public IEnumerable<ListItem> DeleteItems(IEnumerable<int> itemIds)
        {
            if (itemIds != null && itemIds.Any())
            {
                List list = GetList(SpQueryArgs);
                foreach (int itemId in itemIds)
                {
                    ListItem listItem = list.GetItemById(itemId);
                    list.Context.Load(listItem);
                    listItem.DeleteObject();
                    yield return listItem;
                }
            }
        }
    }

    internal class SpAsyncQueryExecutor<TContext> : SpQueryExecutor<TContext>, IAsyncQueryExecutor
        where TContext : ISpDataContext
    {
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        internal SpAsyncQueryExecutor(SpQueryArgs<TContext> args) : base(args)
        {
            args.IsAsync = true;
        }

        public async Task<IEnumerable<TResult>> ExecuteCollectionAsync<TResult>(QueryModel queryModel)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (SpQueryArgs == null) return Enumerable.Empty<TResult>();
                SpQueryArgs.SpView = new SpView();
                var queryVisitor = new SpGeneratorQueryModelVisitor<TContext>(SpQueryArgs);
                queryVisitor.VisitQueryModel(queryModel);
                queryVisitor.VisitIncludeClauses(IncludeExpressions, queryModel);
                queryVisitor.VisitGroupByClauses(GroupByExpressions, queryModel);

                if (SpQueryArgs.SkipResult)
                {
                    return Enumerable.Empty<TResult>();
                }

                var results = await GetEntitiesAsync(typeof(TResult));

                foreach (var resultOperator in queryModel.ResultOperators)
                {
                    if (resultOperator is ReverseResultOperator)
                        results = results.Reverse();
                }

                return results.Cast<TResult>();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<TResult> ExecuteScalarAsync<TResult>(QueryModel queryModel)
        {
            var result = await ExecuteSingleAsync<TResult>(queryModel, false);
            return result;
        }

        public async Task<TResult> ExecuteSingleAsync<TResult>(QueryModel queryModel, bool defaultIfEmpty)
        {
            var results = await ExecuteCollectionAsync<TResult>(queryModel);
            foreach (var resultOperator in queryModel.ResultOperators)
            {
                if (resultOperator is LastResultOperator)
                    return results.LastOrDefault();
            }
            return (defaultIfEmpty) ? results.FirstOrDefault() : results.First();
        }

        protected virtual async Task<IEnumerable<TResult>> GetEntitiesAsync<TResult>() where TResult : ListItemEntity
        {
            var entities = await GetEntitiesAsync(typeof(TResult));
            return entities.Cast<TResult>();
        }

        protected virtual async Task<IEnumerable<object>> GetEntitiesAsync(Type type)
        {
            CheckEntityType(type);

            ListItemCollectionPosition position = null;
            IEnumerable<ListItemEntity> entities = Enumerable.Empty<ListItemEntity>();
            if (SpQueryArgs == null) return entities;

            var rowLimit = SpQueryArgs.SpView.Limit;
            int itemCount = 0;
            do
            {
                if (SpQueryArgs.BatchSize > 0)
                {
                    if (rowLimit > 0)
                    {
                        SpQueryArgs.SpView.Limit = Math.Min(rowLimit - itemCount, SpQueryArgs.BatchSize);
                    }
                    else
                    {
                        SpQueryArgs.SpView.Limit = SpQueryArgs.BatchSize;
                    }
                    if (SpQueryArgs.SpView.Limit == 0)
                    {
                        break;
                    }
                }

                var items = GetItems(SpQueryArgs, position);
                if (items != null)
                {
                    await items.Context.ExecuteQueryAsync();
                    if (SpQueryArgs.BatchSize > 0)
                    {
                        position = items.ListItemCollectionPosition;
                    }
                    itemCount += items.Count;
                    entities = entities.Concat(MapEntities(items, type));
                }
            }
            while (position != null);

            SpQueryArgs.SpView.Limit = rowLimit;
            return entities;
        }

        //protected override IEnumerable<object> GetEntities(Type type)
        //{
        //  var task = GetEntitiesAsync(type);
        //  task.Wait();
        //  return task.Result;
        //}
    }
}
