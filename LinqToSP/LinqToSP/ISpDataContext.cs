using System;
using System.Linq;
using Microsoft.SharePoint.Client;

namespace SP.Client.Linq
{
    public interface ISpDataContext : IDisposable
    {
        string SiteUrl { get; }

        ClientContext Context { get; }

        IQueryable<TListItem> List<TListItem>(string query) where TListItem : class, IListItemEntity;

        IQueryable<TListItem> List<TListItem>(string listName, string query) where TListItem : class, IListItemEntity;

        IQueryable<TListItem> List<TListItem>(Uri listUrl, string query) where TListItem : class, IListItemEntity;

        IQueryable<TListItem> List<TListItem>(Guid listId, string query) where TListItem : class, IListItemEntity;

        IQueryable<TListItem> Query<TListItem>(string listName, string query) where TListItem : class, IListItemEntity;

        IQueryable<TListItem> Query<TListItem>(Uri listUrl, string query) where TListItem : class, IListItemEntity;

        IQueryable<TListItem> Query<TListItem>(Guid listId, string query) where TListItem : class, IListItemEntity;

        string Query<TListItem>(IQueryable<TListItem> items, bool disableFormatting = false) where TListItem : class, IListItemEntity;
    }
}
