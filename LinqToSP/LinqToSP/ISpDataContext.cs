using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.SharePoint.Client;
using SP.Client.Linq.Query;

namespace SP.Client.Linq
{
  public interface ISpDataContext : IDisposable
  {
    string SiteUrl { get; }

    ClientContext Context { get; }

    SpEntityQueryable<TListItem> List<TListItem>(string listName) where TListItem : IListItemEntity;
    string GenerateQuery<TListItem>(IQueryable<TListItem> items, bool disableFormatting = false) where TListItem : IListItemEntity;

  }
}
