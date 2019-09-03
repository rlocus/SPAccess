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

namespace SP.Client.Linq.Query
{
    public class SpEntityQueryable<TResult> : QueryableBase<TResult>, IAsyncEnumerable<TResult>, ISpRepository<TResult>
        where TResult : class, IListItemEntity
    {
        public SpEntityQueryable(SpQueryArgs args)
            : this(QueryParser.CreateDefault(), new SpAsyncQueryExecutor(args))
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
            : this(new /*DefaultQueryProvider*/AsyncQueryProvider(typeof(SpEntityQueryable<>), queryParser, executor))
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
            return AttributeHelper.GetFieldAttributes<TResult, FieldAttribute>()
              .Concat(AttributeHelper.GetPopertyAttributes<TResult, FieldAttribute>());
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

        internal SpQueryExecutor GetExecutor()
        {
            var provider = (this.Provider as QueryProviderBase);
            if (provider != null)
            {
                return (provider.Executor as SpQueryExecutor);
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

        public async Task<IEnumerator<TResult>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            var result = await (Provider as AsyncQueryProvider).ExecuteAsync<IEnumerable<TResult>>(Expression, cancellationToken);
            return result.GetEnumerator();
        }

        public TResult Add([NotNull] TResult entity)
        {
            if (entity != null)
            {
                var executor = GetExecutor();
                if (executor != null && executor.SpQueryArgs != null && executor.SpQueryArgs.FieldMappings != null)
                {
                    if (entity is ListItemEntity)
                    {
                        return (TResult)executor.InsertOrUpdateEntity(entity as ListItemEntity);
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

        public TResult Find(int itemId)
        {
            return this.FirstOrDefault(i => i.Id == itemId);
        }

        public IQueryable<TResult> FindAll([NotNull] params int[] itemIds)
        {
            return this.Where(i => i.Includes(x => x.Id, itemIds));
        }

        public IEnumerable<TResult> AddRange(IEnumerable<TResult> entities)
        {
            if (entities != null)
            {
                var executor = GetExecutor();
                if (executor != null && executor.SpQueryArgs != null && executor.SpQueryArgs.FieldMappings != null)
                {
                    return executor.InsertOrUpdateEntities(entities.Cast<ListItemEntity>()).Cast<TResult>();
                }
            }
            return null;
        }

        public bool Remove(TResult entity)
        {
            if (entity != null && entity.Id > 0)
            {
                return Delete(entity.Id) > 0;
            }
            return false;
        }

        public int RemoveRange(IEnumerable<TResult> entities)
        {
            return Delete(entities.Where(entity => entity != null && entity.Id > 0).Select(entity => entity.Id).ToArray());
        }
    }
}
