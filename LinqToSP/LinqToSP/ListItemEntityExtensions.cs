using System;
using System.Collections;

namespace SP.Client.Linq
{
    public static class ListItemEntityExtensions
    {
        public static bool DateRangesOverlap<TEntity>(this TEntity entity, Func<TEntity, DateTime> startDate, Func<TEntity, DateTime?> endDate, Func<TEntity, string> recurrenceId, DateTime value)
           where TEntity : IEventItemEntity
        {
            //fake method.
            return true;
        }

        public static bool DateRangesOverlap<TEntity>(this TEntity entity, Func<TEntity, DateTime> startDate, Func<TEntity, DateTime?> endDate, Func<TEntity, string> recurrenceId, Caml.CamlValue.DateCamlValue value)
          where TEntity : IEventItemEntity
        {
            //fake method.
            return true;
        }

        public static bool Includes<TEntity, Boolean>(this TEntity entity, Func<TEntity, object> prop, params string[] fieldValues)
         where TEntity : IListItemEntity
        {
            //fake method.
            return true;
        }
        public static bool Includes<TEntity, Boolean>(this TEntity entity, Func<TEntity, object> prop, params int[] fieldIds)
        where TEntity : IListItemEntity
        {
            //fake method.
            return true;
        }

        public static bool LookupIncludes<TEntity, TCollection, Boolean>(this TEntity entity, Func<TEntity, TCollection> prop, string lookupFieldValue)
         where TEntity : IListItemEntity
         where TCollection : ICollection
        {
            //fake method.
            return true;
        }
        public static bool LookupIdIncludes<TEntity, TCollection, Boolean>(this TEntity entity, Func<TEntity, TCollection> prop, int lookupFieldId)
        where TEntity : IListItemEntity
        where TCollection : ICollection
        {
            //fake method.
            return true;
        }

        public static bool LookupNotIncludes<TEntity, TCollection, Boolean>(this TEntity entity, Func<TEntity, TCollection> prop, string lookupFieldValue)
        where TEntity : IListItemEntity
        where TCollection : ICollection
        {
            //fake method.
            return true;
        }
        public static bool LookupIdNotIncludes<TEntity, TCollection, Boolean>(this TEntity entity, Func<TEntity, TCollection> prop, int lookupFieldId)
        where TEntity : IListItemEntity
        where TCollection : ICollection
        {
            //fake method.
            return true;
        }
    }
}
