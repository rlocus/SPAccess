using System.Collections.Generic;
using System.Linq;
using Remotion.Linq;
using System.Data;
using Remotion.Linq.Clauses.ResultOperators;
using SP.Client.Linq.Query.ExpressionVisitors;
using Microsoft.SharePoint.Client;
using View = SP.Client.Caml.View;
using System;
using System.Reflection;
using SP.Client.Helpers;
using System.Diagnostics;

namespace SP.Client.Linq.Query
{
  internal class SpQueryExecutor : IQueryExecutor
  {
    private readonly SpQueryArgs _args;

    public View SpView
    {
      get
      {
        if (_args == null) return null;
        return _args.SpView;
      }
    }

    internal SpQueryArgs SpQueryArgs => _args;

    internal SpQueryExecutor(SpQueryArgs args)
    {
      ValidateArgs(args);
      _args = args;
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

      //foreach (var resultOperator in queryModel.ResultOperators)
      //{
      //  if (resultOperator is LastResultOperator)
      //    return results.LastOrDefault();
      //}

      return (defaultIfEmpty) ? results.FirstOrDefault() : results.First();
    }

    public IEnumerable<TResult> ExecuteCollection<TResult>(QueryModel queryModel)
    {
      if (_args == null) return Enumerable.Empty<TResult>();
      _args.SpView = new View();
      var queryVisitor = new SpGeneratorQueryModelVisitor(_args);
      queryVisitor.VisitQueryModel(queryModel);

      if (SpQueryArgs.SkipResult)
      {
        return Enumerable.Empty<TResult>();
      }

      IEnumerable<TResult> results = GetEntities(_args.SpView, typeof(TResult)).Cast<TResult>();

      //foreach (var resultOperator in queryModel.ResultOperators)
      //{
      //  if (resultOperator is ReverseResultOperator)
      //    results = results.Reverse();
      //}

      return results;
    }

    protected virtual IEnumerable<TResult> GetEntities<TResult>(View view)
    where TResult : ListItemEntity
    {
      return GetEntities(view, typeof(TResult)).Cast<TResult>();
    }

    protected virtual IEnumerable<object> GetEntities(View view, Type type)
    {
      if (!type.IsAssignableFrom(typeof(ListItemEntity)) && !type.IsSubclassOf(typeof(ListItemEntity)))
      {
        throw new Exception($"Entity must be assignable from {typeof(ListItemEntity)}");
      }
      if (_args != null)
      {
        var clientContext = _args.Context;
        if (clientContext != null)
        {
          var list = _args.ListTitle != null ? clientContext.Web.Lists.GetByTitle(_args.ListTitle) :
            (_args.ListUrl != null ? clientContext.Web.GetList(_args.ListUrl)
            : clientContext.Web.Lists.GetById(_args.ListId));

          var items = list.GetItems(new CamlQuery() { ViewXml = view.ToString(true) });
          clientContext.Load(items);

          Debug.WriteLine("# SP Query:");
          Debug.Write(view);

          clientContext.ExecuteQuery();

          var entities = items.Select(item => MapEntity(
            (ListItemEntity)Activator.CreateInstance(type, new object[] {/* item.Id */}), item));
          return entities;
        }
      }
      return Enumerable.Empty<ListItemEntity>();
    }

    protected virtual ListItemEntity MapEntity(ListItemEntity entity, ListItem item)
    {
      if (_args == null || entity == null || item == null) return entity;

      foreach (var column in _args.FieldMappings)
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
}
