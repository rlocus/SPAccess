using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using System;
using System.Linq.Expressions;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientView
    {
        private bool _executeQuery;
        private ListItemCollection _items;
        public View View { get; private set; }

        internal SPClientView(View view)
        {
            this.View = view;
        }

        public bool IsLoaded { get; internal set; }

        public SPClientWeb ClientWeb { get; internal set; }

        public SPClientList ClientList { get; internal set; }

        public string GetRestUrl()
        {
            return null;
        }

        public SPClientView IncludeItems(ListItemCollectionPosition position = null, bool datesInUtc = false, string folderServerRelativeUrl = null, params Expression<Func<ListItemCollection, object>>[] retrievals)
        {
            _items = this.ClientList.List.GetItems(new CamlQuery()
             {
                 ListItemCollectionPosition = position,
                 ViewXml = this.View.ListViewXml
             });
            this.View.Context.Load(_items, retrievals);
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
                this.View.Context.Load(this.View);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                this.View.Context.ExecuteQuery();
                this.IsLoaded = true;
            }
            _executeQuery = false;
        }

        public async Task LoadAsync()
        {
            if (!IsLoaded)
            {
                this.View.Context.Load(this.View);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                await this.View.Context.ExecuteQueryAsync();
                this.IsLoaded = true;
            }
            _executeQuery = false;
        }

        public void RefreshLoad()
        {
            if (this.IsLoaded)
            {
                this.IsLoaded = false;
                this.View.RefreshLoad();
            }
        }

        internal static SPClientView FromView(View view)
        {
            return new SPClientView(view);
        }
    }
}