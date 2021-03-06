﻿using SP.Client.Extensions;
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

    public static SpEntityEntry<TEntity, ISpEntryDataContext> GetEntry<TEntity>(this IQueryable<TEntity> source, TEntity entity, bool reload = false)
     where TEntity : class, IListItemEntity
    {
      Check.NotNull(source, nameof(source));
      if (source is SpEntityQueryable<TEntity, ISpEntryDataContext>)
      {
        return (source as SpEntityQueryable<TEntity, ISpEntryDataContext>).Entry(entity, reload);
      }
      return null;
    }

    public static SpEntityEntry<TEntity, ISpEntryDataContext> NewEntry<TEntity>(this IQueryable<TEntity> source)
     where TEntity : class, IListItemEntity, new()
    {
      Check.NotNull(source, nameof(source));
      if (source is SpEntityQueryable<TEntity, ISpEntryDataContext>)
      {
        return (source as SpEntityQueryable<TEntity, ISpEntryDataContext>).Entry(new TEntity(), false);
      }
      return null;
    }

    public static string Caml<TEntity>(this IQueryable<TEntity> source, bool disableFormatting = false, bool queryOnly = false)
          where TEntity : class, IListItemEntity
    {
      Check.NotNull(source, nameof(source));
      if (source is SpEntityQueryable<TEntity, ISpEntryDataContext>)
      {
        return (source as SpEntityQueryable<TEntity, ISpEntryDataContext>).GenerateCaml(disableFormatting, queryOnly);
      }
      return null;
    }

    public static SpEntityEntry<TEntity, ISpEntryDataContext> AddOrUpdate<TEntity>(this IQueryable<TEntity> source, TEntity entity)
      where TEntity : class, IListItemEntity, new()
    {
      var entry = GetEntry(source, entity, true);
      if (entry != null)
      {
        entry.Update();
      }
      return entry;
    }

    public static SpEntityEntry<TEntity, ISpEntryDataContext> AddOrUpdate<TEntity>(this IQueryable<TEntity> source, TEntity entity, int entityId)
     where TEntity : class, IListItemEntity, new()
    {
      var entry = GetEntry(source, entity, false);
      if (entry != null)
      {
        entry.EntityId = entityId;
        entry.Reload(true);
        entry.Update();
      }
      return entry;
    }

    public static bool DeleteAll<TEntity>(this IQueryable<TEntity> source)
     where TEntity : class, IListItemEntity, new()
    {
      bool deleted = false;
      foreach (var entry in GetEntries(source))
      {
        entry.Delete();
        deleted = true;
      }
      return deleted;
    }
  }
}
