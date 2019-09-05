using System.Linq;
using Remotion.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Parsing.Structure;
using SP.Client.Linq.Attributes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using JetBrains.Annotations;
using SP.Client.Linq.Query;

namespace SP.Client.Linq.Infrastructure
{
  public class SpEntityQueryable<TEntity> : SpEntityQueryable<TEntity, ISpEntryDataContext>
  where TEntity : class, IListItemEntity
  {
    public SpEntityQueryable(SpQueryArgs<ISpEntryDataContext> args)
        : base(args)
    {
      
    }

    internal SpEntityQueryable(IQueryParser queryParser, IAsyncQueryExecutor executor)
        : base(new AsyncQueryProvider<TEntity, ISpEntryDataContext>(typeof(SpEntityQueryable<>), queryParser, executor))
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
  }


  public class SpEntityQueryable<TEntity, TContext> : QueryableBase<TEntity>, IAsyncEnumerable<TEntity>, ISpRepository<TEntity>, ISpChangeTrackable<TEntity, TContext>
    where TEntity : class, IListItemEntity
    where TContext : class, ISpEntryDataContext
  {
    public SpEntityQueryable(SpQueryArgs<TContext> args)
        : this(QueryParser.CreateDefault(), new SpAsyncQueryExecutor<TEntity, TContext>(args))
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
        : this(new /*DefaultQueryProvider*/AsyncQueryProvider<TEntity, ISpEntryDataContext>(typeof(SpEntityQueryable<,>), queryParser, executor))
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
      return AttributeHelper.GetFieldAttributes<TEntity, FieldAttribute>()
        .Concat(AttributeHelper.GetPropertyAttributes<TEntity, FieldAttribute>());
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

    internal SpQueryExecutor<TEntity, TContext> GetExecutor()
    {
      var provider = (this.Provider as QueryProviderBase);
      if (provider != null)
      {
        return (SpQueryExecutor<TEntity, TContext>)provider.Executor;
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

    public async Task<IEnumerator<TEntity>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
      var result = await (Provider as AsyncQueryProvider<TEntity, TContext>).ExecuteAsync(Expression, cancellationToken);
      return result.GetEnumerator();
    }

    public TEntity Add([NotNull] TEntity entity)
    {
      if (entity != null)
      {
        var executor = GetExecutor();
        if (executor != null && executor.SpQueryArgs != null && executor.SpQueryArgs.FieldMappings != null)
        {
          if (entity is ListItemEntity)
          {
            return executor.InsertOrUpdateEntity(entity);
          }
        }
      }
      return null;
    }

    public int Delete([NotNull] params int[] itemIds)
    {
      if (itemIds != null)
      {
        var executor = GetExecutor();
        if (executor != null && executor.SpQueryArgs != null && executor.SpQueryArgs.FieldMappings != null)
        {
          return executor.Delete(itemIds);
        }
      }
      return 0;
    }

    public TEntity Find(int itemId)
    {
      return this.FirstOrDefault(i => i.Id == itemId);
    }

    public IQueryable<TEntity> FindAll([NotNull] params int[] itemIds)
    {
      return this.Where(i => i.Includes(x => x.Id, itemIds));
    }

    public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
    {
      if (entities != null)
      {
        var executor = GetExecutor();
        if (executor != null && executor.SpQueryArgs != null && executor.SpQueryArgs.FieldMappings != null)
        {
          return executor.InsertOrUpdateEntities(entities).Cast<TEntity>();
        }
      }
      return null;
    }

    public bool Remove(TEntity entity)
    {
      if (entity != null && entity.Id > 0)
      {
        return Delete(entity.Id) > 0;
      }
      return false;
    }

    public int RemoveRange(IEnumerable<TEntity> entities)
    {
      return Delete(entities.Where(entity => entity != null && entity.Id > 0).Select(entity => entity.Id).ToArray());
    }

    public void DetectChanges()
    {
      throw new NotImplementedException();
    }

    public bool HasChanges()
    {
      throw new NotImplementedException();
    }

    public IEnumerable<SpEntityEntry<TEntity, TContext>> Entries()
    {
      var executor = GetExecutor();
      if (executor != null && executor.SpQueryArgs != null && executor.SpQueryArgs.FieldMappings != null)
      {
        return this.ToList().Select(entity => new SpEntityEntry<TEntity, TContext>(entity, executor.SpQueryArgs));
      }
      return Enumerable.Empty<SpEntityEntry<TEntity, TContext>>();
    }
  }
}
