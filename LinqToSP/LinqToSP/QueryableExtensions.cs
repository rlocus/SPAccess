using Remotion.Linq;
using SP.Client.Extensions;
using SP.Client.Linq.Query;
using SP.Client.Linq.Query.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SP.Client.Linq
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Include<T>(
           this IQueryable<T> source, params Expression<Func<T, object>>[] path)
            where T: IListItemEntity
        {
            Check.NotNull(source, "source");
            Check.NotNull(path, "path");
            if (source is SpEntityQueryable<T>)
            {
                var provider = source.Provider as DefaultQueryProvider;
                if (provider != null)
                {
                    return provider.CreateQuery<T>(new IncludeExpression(source.Expression, path));
                }
            }
            return source;
        }

    }
}
