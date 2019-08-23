using System.Collections.Generic;
using System.Linq;
using Remotion.Linq;
using System.Data;
using Remotion.Linq.Clauses.ResultOperators;
using SP.Client.Linq.Query.ExpressionVisitors;
using SP.Client.Caml;

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

    internal bool SkipResult { get; set; }

    internal SpQueryExecutor(SpQueryArgs args)
    {
      ValidateArgs(args);
      _args = args;
    }

    private void ValidateArgs(SpQueryArgs args)
    {

    }

    public T ExecuteScalar<T>(QueryModel queryModel)
    {
      return ExecuteSingle<T>(queryModel, false);
    }

    public T ExecuteSingle<T>(QueryModel queryModel, bool defaultIfEmpty)
    {
      var results = ExecuteCollection<T>(queryModel);

      //foreach (var resultOperator in queryModel.ResultOperators)
      //{
      //  if (resultOperator is LastResultOperator)
      //    return results.LastOrDefault();
      //}

      return (defaultIfEmpty) ? results.FirstOrDefault() : results.First();
    }

    public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
    {
      var queryVisitor = new SpGeneratorQueryModelVisitor(_args);
      queryVisitor.VisitQueryModel(queryModel);

      if (SkipResult)
      {
        return Enumerable.Empty<T>();
      }
      var objectResults = GetDataResults(_args.SpView, queryModel);
      var returnResults = objectResults.Cast<T>();

      //foreach (var resultOperator in queryModel.ResultOperators)
      //{
      //  if (resultOperator is ReverseResultOperator)
      //    returnResults = returnResults.Reverse();
      //}

      return returnResults;
    }

    protected IEnumerable<object> GetDataResults(View view, QueryModel queryModel)
    {
      yield return new ListItemEntity(1) { Title = "Test" };
    }
  }
}
