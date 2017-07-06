using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientWeb
    {
        private bool _executeQuery;

        internal SPClientWeb(Web web)
        {
            if (web == null) throw new ArgumentNullException(nameof(web));
            Web = web;
            //WebsForCurrentUser = this.Web.GetSubwebsForCurrentUser(null);
        }

        public bool IsApp
        {
            get { return IsAppWeb(); }
        }

        public Web Web { get; }
        public bool IsLoaded { get; private set; }
        public SPClientSite ClientSite { get; internal set; }
        public WebCollection WebsForCurrentUser { get; private set; }

        internal static SPClientWeb FromWeb(Web web)
        {
            return new SPClientWeb(web);
        }

        public SPClientWeb IncludeWebs(params Expression<Func<WebCollection, object>>[] retrievals)
        {
            WebsForCurrentUser = Web.GetSubwebsForCurrentUser(null);
            (Web.Context as SPClientContext).Load(WebsForCurrentUser, retrievals);
            _executeQuery = true;
            return this;
        }

        public SPClientWeb[] GetWebs()
        {
            if (WebsForCurrentUser != null && WebsForCurrentUser.AreItemsAvailable)
            {
                return WebsForCurrentUser.ToList().Select(FromWeb).ToArray();
            }
            throw new SPAccessException("Web collection is not available.");
        }

        public SPClientWeb IncludeLists(params Expression<Func<ListCollection, object>>[] retrievals)
        {
            var lists = Web.Lists;
            (Web.Context as SPClientContext).Load(lists, retrievals);
            _executeQuery = true;
            return this;
        }

        public SPClientList[] GetLists()
        {
            var lists = Web.Lists;
            if (lists != null && lists.AreItemsAvailable)
            {
                var clientLists = new List<SPClientList>(lists.Count);
                foreach (var clientList in lists.ToList().Select(SPClientList.FromList))
                {
                    clientList.ClientWeb = this;
                    clientLists.Add(clientList);
                }
                return clientLists.ToArray();
            }

            throw new SPAccessException("List collection is not available.");
        }

        public SPClientWeb IncludeContentTypes(params Expression<Func<ContentTypeCollection, object>>[] retrievals)
        {
            var contentTypes = Web.ContentTypes;
            (Web.Context as SPClientContext).Load(contentTypes, retrievals);
            _executeQuery = true;
            return this;
        }

        public SPClientContentType[] GetContentTypes()
        {
            var contentTypes = Web.ContentTypes;
            if (contentTypes != null && contentTypes.AreItemsAvailable)
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
            throw new SPAccessException("Content Type collection is not available.");
        }

        public SPClientWeb IncludeFields(params Expression<Func<FieldCollection, object>>[] retrievals)
        {
            var fields = Web.Fields;
            (Web.Context as SPClientContext).Load(fields, retrievals);
            _executeQuery = true;
            return this;
        }

        public SPClientField[] GetFields()
        {
            var fields = Web.Fields;
            if (fields != null && fields.AreItemsAvailable)
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
            throw new SPAccessException("Field collection is not available.");
        }

        public void Load()
        {
            if (!IsLoaded)
            {
                (Web.Context as SPClientContext).Load(Web);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                Web.Context.ExecuteQuery();
                IsLoaded = true;
            }
            _executeQuery = false;
        }

        public async Task LoadAsync()
        {
            if (!IsLoaded)
            {
                //this.Web.RefreshLoad();
                (Web.Context as SPClientContext).Load(Web);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                await (Web.Context as SPClientContext).ExecuteQueryAsync();
                IsLoaded = true;
            }
            _executeQuery = false;
        }

        public string GetUrl()
        {
            return Utility.CombineUrls(new Uri(Web.Context.Url.ToLower()), Web.ServerRelativeUrl.ToLower());
        }

        public string GetSettingsUrl()
        {
            return string.Format("{0}/_layouts/{1}/settings.aspx", GetUrl().TrimEnd('/'), Web.UIVersion);
        }

        /// <summary>
        ///     Returns true, if the current web is an App web
        /// </summary>
        /// <returns></returns>
        public bool IsAppWeb()
        {
            return Web.IsPropertyAvailable("AppInstanceId") && Web.AppInstanceId != Guid.Empty;
        }

        public string GetRestUrl()
        {
            return string.Format("{0}/_api/web", GetUrl().TrimEnd('/'));
        }

        public void RefreshLoad()
        {
            if (IsLoaded)
            {
                IsLoaded = false;
                Web.RefreshLoad();
                //var newCtx = (ClientSite.Context as SPClientContext).Clone();
                //this.Web = newCtx.Site.OpenWebById(this.Web.Id);
                //this.WebsForCurrentUser = this.Web.GetSubwebsForCurrentUser(null);
            }
        }
    }
}