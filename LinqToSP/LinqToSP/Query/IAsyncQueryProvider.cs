using JetBrains.Annotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace SP.Client.Linq.Query
{
    public interface IAsyncQueryProvider : IQueryProvider
    {
        TResult ExecuteAsync<TResult>([NotNull] Expression expression, CancellationToken cancellationToken = default);
    }

}
