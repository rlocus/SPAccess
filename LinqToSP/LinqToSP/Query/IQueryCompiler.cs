using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Client.Linq.Query
{
    public interface IQueryCompiler
    {
        TResult Execute<TResult>([NotNull] Expression query);

        TResult ExecuteAsync<TResult>([NotNull] Expression query, CancellationToken cancellationToken);
    }
}
