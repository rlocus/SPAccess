using System;
using System.Linq;
using System.Linq.Expressions;
using SP.Client.Linq.Query;

namespace SP.Client.Linq
{
  public interface ISpContext : IDisposable
  {
    string SiteUrl { get; }

    SpEntityQueryable<TListItem> List<TListItem>(string listName) where TListItem : IListItemEntity;
    string GenerateQuery<TListItem>(IQueryable<TListItem> items, bool disableFormatting = false) where TListItem : IListItemEntity;

  }
}
