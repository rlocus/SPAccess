using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientList
    {
        private bool _executeQuery;

        internal SPClientList(List list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            List = list;
        }

        public List List { get; }
        public bool IsLoaded { get; internal set; }
        public SPClientWeb ClientWeb { get; internal set; }

        public SPClientList IncludeViews(params Expression<Func<ViewCollection, object>>[] retrievals)
        {
            var views = List.Views;
            List.Context.Load(views, retrievals);
            _executeQuery = true;
            return this;
        }

        public SPClientView[] GetViews()
        {
            var views = List.Views;
            if (views != null && views.AreItemsAvailable)
            {
                return views.ToList().Select(view =>
                {
                    var clientView = SPClientView.FromView(view);
                    clientView.ClientList = this;
                    clientView.ClientWeb = ClientWeb;
                    return clientView;
                }).ToArray();
            }
            throw new SPAccessException("View collection is not available.");
        }

        public SPClientList IncludeContentTypes(params Expression<Func<ContentTypeCollection, object>>[] retrievals)
        {
            var contentTypes = List.ContentTypes;
            List.Context.Load(contentTypes, retrievals);
            _executeQuery = true;
            return this;
        }

        public SPClientContentType[] GetContentTypes()
        {
            var contentTypes = List.ContentTypes;
            if (contentTypes != null && contentTypes.AreItemsAvailable)
            {
                return contentTypes.ToList().Select(ct =>
                {
                    var clientContentType = SPClientContentType.FromContentType(ct);
                    clientContentType.IsSiteContentType = false;
                    clientContentType.ClientList = this;
                    clientContentType.ClientWeb = ClientWeb;
                    return clientContentType;
                }).ToArray();
            }
            throw new SPAccessException("Content Type collection is not available.");
        }

        public SPClientList IncludeFields(params Expression<Func<FieldCollection, object>>[] retrievals)
        {
            var fields = List.Fields;
            List.Context.Load(fields, retrievals);
            _executeQuery = true;
            return this;
        }

        public SPClientField[] GetFields()
        {
            var fields = List.Fields;

            if (fields != null && fields.AreItemsAvailable)
            {
                return fields.ToList().Select(field =>
                {
                    var clientField = SPClientField.FromField(field);
                    clientField.ClientList = this;
                    clientField.ClientWeb = ClientWeb;
                    clientField.IsSiteField = false;
                    return clientField;
                }).ToArray();
            }
            throw new SPAccessException("Field collection is not available.");
        }

        public void Load()
        {
            if (!IsLoaded)
            {
                List.Context.Load(List);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                List.Context.ExecuteQuery();
                IsLoaded = true;
            }
            _executeQuery = false;
        }

        public async Task LoadAsync()
        {
            if (!IsLoaded)
            {
                List.Context.Load(List);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                await List.Context.ExecuteQueryAsync();
                IsLoaded = true;
            }
            _executeQuery = false;
        }

        public string GetListUrl()
        {
            return null; //this.RootFolder.GetFolderUrl();
        }

        public string GetSettingsUrl()
        {
            return string.Format("{0}/_layouts/{1}/listedit.aspx?List={2}", ClientWeb.GetUrl().TrimEnd('/'),
                ClientWeb.Web.UIVersion, List.Id);
        }

        public string GetRestUrl()
        {
            return string.Format("{0}/_api/web/lists(guid'{1}')", ClientWeb.GetUrl().TrimEnd('/'), List.Id);
        }

        public string GetUrl()
        {
            return Utility.CombineUrls(new Uri(List.Context.Url.ToLower()), List.ParentWebUrl.ToLower());
        }

        public FieldCollection GetFieldCollection()
        {
            var fields = List.Fields;

            if (!fields.AreItemsAvailable)
            {
                List.Context.Load(fields);
            }
            return fields;
        }

        internal static SPClientList FromList(List list)
        {
            return new SPClientList(list);
        }

        public void RefreshLoad()
        {
            if (IsLoaded)
            {
                IsLoaded = false;
                List.RefreshLoad();
            }
        }
    }
}