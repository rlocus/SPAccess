using System.Linq.Expressions;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web = Microsoft.SharePoint.Client.Web;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientWeb
    {
        private bool _executeQuery;

        public bool IsApp
        {
            get
            {
                return this.IsAppWeb();
            }
        }

        public Web Web { get; private set; }

        public bool IsLoaded { get; private set; }

        public SPClientSite ClientSite { get; internal set; }

        public WebCollection WebsForCurrentUser { get; private set; }

        internal SPClientWeb(Web web)
        {
            if (web == null) throw new ArgumentNullException("web");
            this.Web = web;
            //WebsForCurrentUser = this.Web.GetSubwebsForCurrentUser(null);
        }

        internal static SPClientWeb FromWeb(Web web)
        {
            return new SPClientWeb(web);
        }

        public SPClientWeb IncludeWebs(params Expression<Func<WebCollection, object>>[] retrievals)
        {
            WebsForCurrentUser = this.Web.GetSubwebsForCurrentUser(null);
            this.Web.Context.Load(WebsForCurrentUser, retrievals);
            _executeQuery = true;
            return this;
        }

        public SPClientWeb[] GetWebs()
        {
            if (WebsForCurrentUser.AreItemsAvailable)
            {
                return WebsForCurrentUser.ToList().Select(FromWeb).ToArray();
            }
            throw new SPAccessException("Web collection are not available.");
        }

        public SPClientWeb IncludeLists(params Expression<Func<ListCollection, object>>[] retrievals)
        {
            ListCollection lists = this.Web.Lists;
            this.Web.Context.Load(lists, retrievals);
            _executeQuery = true;
            return this;
        }

        public SPClientList[] GetLists()
        {
            ListCollection lists = this.Web.Lists;
            if (lists.AreItemsAvailable)
            {
                var clientLists = new List<SPClientList>(lists.Count);
                foreach (SPClientList clientList in lists.ToList().Select(SPClientList.FromList))
                {
                    clientList.ClientWeb = this;
                    clientLists.Add(clientList);
                }
                return clientLists.ToArray();
            }

            throw new SPAccessException("List collection are not available.");
        }

        public SPClientWeb IncludeContentTypes(params Expression<Func<ContentTypeCollection, object>>[] retrievals)
        {
            ContentTypeCollection contentTypes = this.Web.ContentTypes;
            this.Web.Context.Load(contentTypes, retrievals);
            _executeQuery = true;
            return this;
        }

        public SPClientContentType[] GetContentTypes()
        {
            ContentTypeCollection contentTypes = this.Web.ContentTypes;
            if (contentTypes.AreItemsAvailable)
            {
                return contentTypes.ToList().Select(ct =>
                {
                    var clientContentType = SPClientContentType.FromContentType(ct);
                    clientContentType.IsSiteContentType = true;
                    clientContentType.ClientList = null;
                    clientContentType.ClientWeb = this;
                    return clientContentType;
                }).ToArray();
            }
            throw new SPAccessException("Content Type collection are not available.");
        }

        public SPClientWeb IncludeFields(params Expression<Func<FieldCollection, object>>[] retrievals)
        {
            FieldCollection fields = this.Web.Fields;
            this.Web.Context.Load(fields, retrievals);
            _executeQuery = true;
            return this;
        }

        public SPClientField[] GetFields()
        {
            FieldCollection fields = this.Web.Fields;
            if (fields.AreItemsAvailable)
            {
                return fields.ToList().Select(field =>
                {
                    var clientField = SPClientField.FromField(field);
                    clientField.ClientWeb = this;
                    clientField.ClientList = null;
                    clientField.IsSiteField = true;
                    return clientField;
                }).ToArray();
            }
            throw new SPAccessException("Field collection are not available.");
        }

        public void Load()
        {
            if (!IsLoaded)
            {
                this.Web.Context.Load(this.Web);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                this.Web.Context.ExecuteQuery();
                this.IsLoaded = true;
            }
            _executeQuery = false;
        }

        public async Task LoadAsync()
        {
            if (!IsLoaded)
            {
                //this.Web.RefreshLoad();
                this.Web.Context.Load(this.Web);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                await this.Web.Context.ExecuteQueryAsync();
                this.IsLoaded = true;
            }
            _executeQuery = false;
        }

        public string GetUrl()
        {
            return Utility.CombineUrls(new Uri(this.Web.Context.Url.ToLower()), this.Web.ServerRelativeUrl.ToLower());
        }

        public string GetSettingsUrl()
        {
            return string.Format("{0}/_layouts/{1}/settings.aspx", this.GetUrl().TrimEnd('/'), this.Web.UIVersion);
        }

        /// <summary>
        /// Returns true, if the current web is an App web
        /// </summary>
        /// <returns></returns>
        public bool IsAppWeb()
        {
            return this.Web.IsPropertyAvailable("AppInstanceId") && this.Web.AppInstanceId != Guid.Empty;
        }

        public string GetRestUrl()
        {
            return string.Format("{0}/_api/web", this.GetUrl().TrimEnd('/'));
        }

        public void RefreshLoad()
        {
            if (this.IsLoaded)
            {
                this.IsLoaded = false;
                this.Web.RefreshLoad();
                //var newCtx = (ClientSite.Context as SPClientContext).Clone();
                //this.Web = newCtx.Site.OpenWebById(this.Web.Id);
                //this.WebsForCurrentUser = this.Web.GetSubwebsForCurrentUser(null);
            }
        }
    }
}