using Remotion.Linq;
using SP.Client.Extensions;
using SP.Client.Linq.Infrastructure;
using SP.Client.Linq.Query;
using SP.Client.Linq.Query.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SP.Client.Linq
{
    public static class QueryableExtensions
    {
        public static IQueryable<TEntity> Include<TEntity>(
             this IQueryable<TEntity> source, params Expression<Func<TEntity, object>>[] path)
              where TEntity : class, IListItemEntity
        {
            Check.NotNull(source, "source");
            Check.NotNull(path, "path");
            return source.Provider is QueryProviderBase
              ? source.Provider.CreateQuery<TEntity>(new IncludeExpression(source.Expression, path))
            : source;
        }

        public static IQueryable<TEntity> GroupBy<TEntity>(
        this IQueryable<TEntity> source, params Expression<Func<TEntity, object>>[] path)
         where TEntity : class, IListItemEntity
        {
            Check.NotNull(source, "source");
            Check.NotNull(path, "path");
            return source.Provider is QueryProviderBase
              ? source.Provider.CreateQuery<TEntity>(new GroupByExpression(source.Expression, path, 0))
            : source;
        }
        public static IQueryable<TEntity> GroupBy<TEntity>(
        this IQueryable<TEntity> source, int limit, params Expression<Func<TEntity, object>>[] path)
         where TEntity : class, IListItemEntity
        {
            Check.NotNull(source, "source");
            Check.NotNull(path, "path");
            return source.Provider is QueryProviderBase
              ? source.Provider.CreateQuery<TEntity>(new GroupByExpression(source.Expression, path, limit))
            : source;
        }

        public static IEnumerable<SpEntityEntry<TEntity, ISpEntryDataContext>> GetEntries<TEntity>(this IQueryable<TEntity> source)
          where TEntity : class, IListItemEntity
        {
            if (source != null)
            {
                return (source as SpEntityQueryable<TEntity>).Entries<ISpEntryDataContext>();
            }
            return Enumerable.Empty<SpEntityEntry<TEntity, ISpEntryDataContext>>();
        }
    }
}
