using System;

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

        public static bool Includes<TEntity, Boolean>(this TEntity entity, Func<TEntity, object> prop, params string[] terms)
         where TEntity : IListItemEntity
        {
            //fake method.
            return true;
        }
    }
}
