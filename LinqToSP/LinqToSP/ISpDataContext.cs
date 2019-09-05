using System;
using System.Linq;
using Microsoft.SharePoint.Client;
using SP.Client.Linq.Infrastructure;

namespace SP.Client.Linq
{
    public interface ISpDataContext : IDisposable
  {
    string SiteUrl { get; }

    ClientContext Context { get; }

    SpEntityQueryable<TListItem> List<TListItem>(string listName, string query) where TListItem : class, IListItemEntity;

    SpEntityQueryable<TListItem> List<TListItem>(Uri listUrl, string query) where TListItem : class, IListItemEntity;

    SpEntityQueryable<TListItem> List<TListItem>(Guid listId, string query) where TListItem : class, IListItemEntity;

    string GenerateQuery<TListItem>(IQueryable<TListItem> items, bool disableFormatting = false) where TListItem : class, IListItemEntity;

  }
}
