using System.Linq;
using Remotion.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Parsing.Structure;
using SP.Client.Linq.Attributes;
using System;
using System.Collections.Generic;

namespace SP.Client.Linq.Query
{
  public class SpEntityQueryable<TResult> : QueryableBase<TResult>
      where TResult : IListItemEntity
  {
    public SpEntityQueryable(SpQueryArgs args)
        : this(QueryParser.CreateDefault(), new SpQueryExecutor(args))
    {
      foreach (var att in GetFieldAttributes())
      {
        if (!args.ColumnMappings.ContainsKey(att.Key))
        {
          args.ColumnMappings.Add(att.Key, att.Value);
        }
      }
    }

    internal SpEntityQueryable(IQueryParser queryParser, IQueryExecutor executor)
        : this(new DefaultQueryProvider(typeof(SpEntityQueryable<>), queryParser, executor))
    {

    }

    public SpEntityQueryable(IQueryProvider provider, Expression expression)
        : base(provider, expression)
    {

    }

    internal SpEntityQueryable(IQueryProvider provider)
      : base(provider)
    {

    }

    private static IEnumerable<KeyValuePair<string, FieldAttribute>> GetFieldAttributes()
    {
      return AttributeHelper.GetFieldAttributes<TResult, FieldAttribute>()
        .Concat(AttributeHelper.GetPopertyAttributes<TResult, FieldAttribute>());
    }

    public string GetQuery(bool disableFormatting)
    {
      var executor = GetExecutor();
      if (executor != null)
      {
        var view = executor.SpView;
        if (view != null)
        {
          return view.ToString(disableFormatting);
        }
      }
      return null;
    }

    internal SpQueryExecutor GetExecutor()
    {
      var provider = (this.Provider as DefaultQueryProvider);
      if (provider != null)
      {
        return (provider.Executor as SpQueryExecutor);
      }
      return null;
    }

    internal string GetQueryInternal(bool disableFormatting)
    {
      var executor = GetExecutor();
      if (executor != null)
      {
        try
        {
          //fake
          executor.SpQueryArgs.SkipResult = true;
          this.ToList();
        }
        finally
        {
          executor.SpQueryArgs.SkipResult = false;
        }
        var view = executor.SpView;
        if (view != null)
        {
          return view.ToString(disableFormatting);
        }
      }
      return null;
    }

    public override string ToString()
    {
      string q = GetQuery(false);
      if (q != null)
      {
        return q;
      }
      return base.ToString();
    }
  }
}
