using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using List = Microsoft.SharePoint.Client.List;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientList
    {
        private bool _executeQuery;
        public List List { get; private set; }

        internal SPClientList(List list)
        {
            if (list == null) throw new ArgumentNullException("list");
            this.List = list;
        }

        public bool IsLoaded { get; internal set; }

        public SPClientWeb ClientWeb { get; internal set; }

        public SPClientList IncludeViews(params Expression<Func<ViewCollection, object>>[] retrievals)
        {
            ViewCollection views = this.List.Views;
            this.List.Context.Load(views, retrievals);
            _executeQuery = true;
            return this;
        }

        public SPClientView[] GetViews()
        {
            ViewCollection views = this.List.Views;
            if (views != null && views.AreItemsAvailable)
            {
                return views.ToList().Select(view =>
                {
                    var clientView = SPClientView.FromView(view);
                    clientView.ClientList = this;
                    clientView.ClientWeb = this.ClientWeb;
                    return clientView;
                }).ToArray();
            }
            throw new SPAccessException("View collection is not available.");
        }

        public SPClientList IncludeContentTypes(params Expression<Func<ContentTypeCollection, object>>[] retrievals)
        {
            ContentTypeCollection contentTypes = this.List.ContentTypes;
            this.List.Context.Load(contentTypes, retrievals);
            _executeQuery = true;
            return this;
        }

        public SPClientContentType[] GetContentTypes()
        {
            ContentTypeCollection contentTypes = this.List.ContentTypes;
            if (contentTypes != null && contentTypes.AreItemsAvailable)
            {
                return contentTypes.ToList().Select(ct =>
                {
                    var clientContentType = SPClientContentType.FromContentType(ct);
                    clientContentType.IsSiteContentType = false;
                    clientContentType.ClientList = this;
                    clientContentType.ClientWeb = this.ClientWeb;
                    return clientContentType;
                }).ToArray();
            }
            throw new SPAccessException("Content Type collection is not available.");
        }

        public SPClientList IncludeFields(params Expression<Func<FieldCollection, object>>[] retrievals)
        {
            FieldCollection fields = this.List.Fields;
            this.List.Context.Load(fields, retrievals);
            _executeQuery = true;
            return this;
        }

        public SPClientField[] GetFields()
        {
            FieldCollection fields = this.List.Fields;

            if (fields != null && fields.AreItemsAvailable)
            {
                return fields.ToList().Select(field =>
                {
                    var clientField = SPClientField.FromField(field);
                    clientField.ClientList = this;
                    clientField.ClientWeb = this.ClientWeb;
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
                this.List.Context.Load(this.List);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                this.List.Context.ExecuteQuery();
                this.IsLoaded = true;
            }
            _executeQuery = false;
        }

        public async Task LoadAsync()
        {
            if (!IsLoaded)
            {
                this.List.Context.Load(this.List);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                await this.List.Context.ExecuteQueryAsync();
                this.IsLoaded = true;
            }
            _executeQuery = false;
        }

        public string GetListUrl()
        {
            return null; //this.RootFolder.GetFolderUrl();
        }

        public string GetSettingsUrl()
        {
            return string.Format("{0}/_layouts/{1}/listedit.aspx?List={2}", this.ClientWeb.GetUrl().TrimEnd('/'), this.ClientWeb.Web.UIVersion, this.List.Id);
        }

        public string GetRestUrl()
        {
            return string.Format("{0}/_api/web/lists(guid'{1}')", this.ClientWeb.GetUrl().TrimEnd('/'), this.List.Id);
        }

        public string GetUrl()
        {
            return Utility.CombineUrls(new Uri(this.List.Context.Url.ToLower()), this.List.ParentWebUrl.ToLower());
        }

        public FieldCollection GetFieldCollection()
        {
            FieldCollection fields = this.List.Fields;

            if (!fields.AreItemsAvailable)
            {
                this.List.Context.Load(fields);
            }
            return fields;
        }

        internal static SPClientList FromList(List list)
        {
            return new SPClientList(list);
        }

        public void RefreshLoad()
        {
            if (this.IsLoaded)
            {
                this.IsLoaded = false;
                this.List.RefreshLoad();
            }
        }
    }
}