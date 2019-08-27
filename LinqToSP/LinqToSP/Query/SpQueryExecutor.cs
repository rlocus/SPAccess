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

namespace SP.Client.Linq.Query
{
  internal class SpQueryExecutor : IQueryExecutor
  {
    private readonly object _lock = new object();
    internal List<IncludeExpression> IncludeExpressions { get; }

    public SpView SpView
    {
      get
      {
        if (SpQueryArgs == null) return null;
        return SpQueryArgs.SpView;
      }
    }

    internal SpQueryArgs SpQueryArgs { get; }

    internal SpQueryExecutor(SpQueryArgs args)
    {
      ValidateArgs(args);
      SpQueryArgs = args;
      IncludeExpressions = new List<IncludeExpression>();
    }

    private void ValidateArgs(SpQueryArgs args)
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
        SpQueryArgs.SpView = new SpView();
        var queryVisitor = new SpGeneratorQueryModelVisitor(SpQueryArgs);
        queryVisitor.VisitQueryModel(queryModel);
        queryVisitor.VisitIncludeClauses(IncludeExpressions, queryModel);

        if (SpQueryArgs.SpView.ViewFields == null)
        {
          SpQueryArgs.SpView.ViewFields =
          new Caml.ViewFieldsCamlElement(SpQueryArgs.FieldMappings.Select(fieldMapping => fieldMapping.Value.Name));
        }
        else if (SpQueryArgs.SpView.ViewFields.FieldRefs == null || !SpQueryArgs.SpView.ViewFields.FieldRefs.Any())
        {
          SpQueryArgs.SpView.ViewFields.AddViewFields(SpQueryArgs.FieldMappings.Select(fieldMapping => fieldMapping.Value.Name));
        }

        if (SpQueryArgs.SkipResult)
        {
          return Enumerable.Empty<TResult>();
        }

        Debug.WriteLine("# SP Query:");
        Debug.Write(SpQueryArgs.SpView);
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
        if (rowLimit > 0)
        {
          SpQueryArgs.SpView.Limit = Math.Min(rowLimit - itemCount, SpQueryArgs.BatchSize);
        }
        else
        {
          SpQueryArgs.SpView.Limit = SpQueryArgs.BatchSize;
        }
        if (SpQueryArgs.SpView.Limit > 0)
        {
          var items = GetItems(SpQueryArgs, position);
          if (items != null)
          {
            items.Context.ExecuteQuery();
            position = items.ListItemCollectionPosition;
            itemCount += items.Count;
            entities = entities.Concat(MapEntities(items, type));
          }
        }
        else
        {
          position = null;
        }
      }
      while (position != null);

      SpQueryArgs.SpView.Limit = rowLimit;
      return entities;
    }

    protected static void CheckEntityType(Type type)
    {
      if (!type.IsAssignableFrom(typeof(ListItemEntity)) && !type.IsSubclassOf(typeof(ListItemEntity)))
      {
        throw new Exception($"Entity must be assignable from {typeof(ListItemEntity)}");
      }
    }

    protected static List GetList(SpQueryArgs args)
    {
      if (args != null)
      {
        var clientContext = args.Context;
        if (clientContext != null)
        {
          return args.ListTitle != null ? clientContext.Web.Lists.GetByTitle(args.ListTitle) :
              (args.ListUrl != null ? clientContext.Web.GetList(args.ListUrl)
              : clientContext.Web.Lists.GetById(args.ListId));
        }
      }
      return null;
    }

    protected static ListItemCollection GetItems(SpQueryArgs args, ListItemCollectionPosition position)
    {
      var list = GetList(args);
      if (list != null)
      {
        var items = list.GetItems(new CamlQuery() { ViewXml = args.SpView.ToString(true), ListItemCollectionPosition = position });
        items.Context.Load(items);
        return items;
      }
      return null;
    }

    protected virtual IEnumerable<ListItemEntity> MapEntities(ListItemCollection items, Type type)
    {
      return items.Select(item => MapEntity((ListItemEntity)Activator.CreateInstance(type, new object[] {/* item.Id */}), item));
    }

    protected virtual ListItemEntity MapEntity(ListItemEntity entity, ListItem item)
    {
      if (SpQueryArgs == null || entity == null || item == null) return entity;

      foreach (var column in SpQueryArgs.FieldMappings)
      {
        PropertyInfo prop = entity.GetType().GetProperty(column.Key, BindingFlags.Public | BindingFlags.Instance);
        if (null != prop && prop.CanWrite)
        {
          if (item.FieldValues.ContainsKey(column.Value.Name))
          {
            object value = item[column.Value.Name];
            value = SpConverter.ConvertValue(value, prop.PropertyType);
            prop.SetValue(entity, value);
          }
        }
        FieldInfo field = entity.GetType().GetField(column.Key, BindingFlags.Public | BindingFlags.Instance);
        if (null != field)
        {
          if (item.FieldValues.ContainsKey(column.Value.Name))
          {
            object value = item[column.Value.Name];
            value = SpConverter.ConvertValue(value, prop.PropertyType);
            field.SetValue(entity, value);
          }
        }
      }
      return entity;
    }
  }

  internal class SpAsyncQueryExecutor : SpQueryExecutor, IAsyncQueryExecutor
  {
    private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

    internal SpAsyncQueryExecutor(SpQueryArgs args) : base(args)
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
        var queryVisitor = new SpGeneratorQueryModelVisitor(SpQueryArgs);
        queryVisitor.VisitQueryModel(queryModel);
        queryVisitor.VisitIncludeClauses(IncludeExpressions, queryModel);

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
        if (rowLimit > 0)
        {
          SpQueryArgs.SpView.Limit = Math.Min(rowLimit - itemCount, SpQueryArgs.BatchSize);
        }
        else
        {
          SpQueryArgs.SpView.Limit = SpQueryArgs.BatchSize;
        }
        if (SpQueryArgs.SpView.Limit > 0)
        {
          var items = GetItems(SpQueryArgs, position);
          if (items != null)
          {
            await items.Context.ExecuteQueryAsync();
            position = items.ListItemCollectionPosition;
            itemCount += items.Count;
            entities = entities.Concat(MapEntities(items, type));
          }
        }
        else
        {
          position = null;
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
