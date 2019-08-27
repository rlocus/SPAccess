using System.Linq;
using Remotion.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Parsing.Structure;
using SP.Client.Linq.Attributes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Client.Linq.Query
{
  public class SpEntityQueryable<TResult> : QueryableBase<TResult>, IAsyncEnumerable<TResult>
      where TResult : IListItemEntity
  {
    public SpEntityQueryable(SpQueryArgs args)
        : this(QueryParser.CreateDefault(), new SpAsyncQueryExecutor(args))
    {
      foreach (var att in GetFieldAttributes())
      {
        if (!args.FieldMappings.ContainsKey(att.Key))
        {
          args.FieldMappings.Add(att.Key, att.Value);
        }
      }
    }

    internal SpEntityQueryable(IQueryParser queryParser, IAsyncQueryExecutor executor)
        : this(new /*DefaultQueryProvider*/AsyncQueryProvider(typeof(SpEntityQueryable<>), queryParser, executor))
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
      var provider = (this.Provider as QueryProviderBase);
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

    public async Task<IEnumerator<TResult>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
      var result = await (Provider as AsyncQueryProvider).ExecuteAsync<IEnumerable<TResult>>(Expression, cancellationToken);
       return result.GetEnumerator();
    }
  }
}
