using SP.Client.Extensions;
using SP.Client.Linq.Infrastructure;
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
              this IQueryable<TEntity> source, params Expression<Func<TEntity, object>>[] predicates)
               where TEntity : class, IListItemEntity
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicates, nameof(predicates));
            if (source.Provider is IQueryProvider)
            {
                var expression = new IncludeExpression<ISpEntryDataContext>(source.Expression, predicates);
                return new SpEntityQueryable<TEntity>(source.Provider, expression).Concat(new SpEntityQueryable<TEntity>(source.Provider, source.Expression));
            }
            return source;
        }

        public static IQueryable<TEntity> GroupBy<TEntity>(
             this IQueryable<TEntity> source, params Expression<Func<TEntity, object>>[] predicates)
              where TEntity : class, IListItemEntity
        {
            return GroupBy(source, 0, predicates);
        }

        public static IQueryable<TEntity> GroupBy<TEntity>(
            this IQueryable<TEntity> source, int limit, params Expression<Func<TEntity, object>>[] predicates)
             where TEntity : class, IListItemEntity
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicates, nameof(predicates));
            if (source.Provider is IQueryProvider)
            {
                var expression = new GroupByExpression<ISpEntryDataContext>(source.Expression, predicates, limit);
                return new SpEntityQueryable<TEntity>(source.Provider, expression).Concat(new SpEntityQueryable<TEntity>(source.Provider, source.Expression));
            }
            return source;
        }

        public static IEnumerable<SpEntityEntry<TEntity, ISpEntryDataContext>> GetEntries<TEntity>(this IQueryable<TEntity> source)
          where TEntity : class, IListItemEntity
        {
            Check.NotNull(source, nameof(source));
            if (source is SpEntityQueryable<TEntity, ISpEntryDataContext>)
            {
                return (source as SpEntityQueryable<TEntity, ISpEntryDataContext>).Entries();
            }
            return Enumerable.Empty<SpEntityEntry<TEntity, ISpEntryDataContext>>();
        }
    }
}
