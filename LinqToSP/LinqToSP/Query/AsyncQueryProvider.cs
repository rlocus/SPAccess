using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;

namespace SP.Client.Linq.Query
{
  public class AsyncQueryProvider : QueryProvider, IAsyncQueryProvider
  {
    public AsyncQueryProvider(Type queryableType, [NotNull] IQueryParser queryParser, [NotNull] IAsyncQueryExecutor executor) : base(queryableType, queryParser, executor)
    {
    }
    public async Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
      return (TResult)await ExecuteAsync(expression, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
      {
        return await Task.FromCanceled<object>(cancellationToken);
      }
      try
      {
        QueryModel queryModel = this.GenerateQueryModel(expression);
        return await Task.FromResult(queryModel.Execute(this.Executor).Value);
      }
      catch (Exception ex)
      {
        return await Task.FromException<object>(ex);
      }
    }
  }
}
