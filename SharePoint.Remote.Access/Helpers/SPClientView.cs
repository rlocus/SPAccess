using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientView
    {
        private bool _executeQuery;
        private ListItemCollection _items;

        internal SPClientView(View view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));
            View = view;
        }

        public View View { get; }
        public bool IsLoaded { get; internal set; }
        public SPClientWeb ClientWeb { get; internal set; }
        public SPClientList ClientList { get; internal set; }

        public string GetRestUrl()
        {
            return null;
        }

        public SPClientView IncludeItems(ListItemCollectionPosition position = null, bool datesInUtc = false,
            string folderServerRelativeUrl = null, params Expression<Func<ListItemCollection, object>>[] retrievals)
        {
            _items = ClientList.List.GetItems(new CamlQuery
            {
                ListItemCollectionPosition = position,
                ViewXml = View.ListViewXml,
                FolderServerRelativeUrl = folderServerRelativeUrl,
                DatesInUtc = datesInUtc
            });
            (View.Context as SPClientContext).Load(_items, retrievals);
            _executeQuery = true;
            return this;
        }

        public ListItem[] GetItems(out ListItemCollectionPosition position)
        {
            if (_items != null && _items.AreItemsAvailable)
            {
                position = _items.ListItemCollectionPosition;
                return _items.ToArray();
            }
            throw new SPAccessException("List Item collection is not available.");
        }

        public void Load()
        {
            if (!IsLoaded)
            {
                (View.Context as SPClientContext).Load(View);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                View.Context.ExecuteQuery();
                IsLoaded = true;
            }
            _executeQuery = false;
        }

        public async Task LoadAsync()
        {
            if (!IsLoaded)
            {
                await (View.Context as SPClientContext).LoadAsync(View);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                await (View.Context as SPClientContext).ExecuteQueryAsync();
                IsLoaded = true;
            }
            _executeQuery = false;
        }

        public void RefreshLoad()
        {
            if (IsLoaded)
            {
                IsLoaded = false;
                View.RefreshLoad();
            }
        }

        internal static SPClientView FromView(View view)
        {
            return new SPClientView(view);
        }
    }
}