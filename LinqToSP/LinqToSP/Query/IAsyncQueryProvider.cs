using JetBrains.Annotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Client.Linq.Query
{
    public interface IAsyncQueryProvider : IQueryProvider
    {
       Task<TResult> ExecuteAsync<TResult>([NotNull] Expression expression, CancellationToken cancellationToken = default);
    }
}
