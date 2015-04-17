using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web = Microsoft.SharePoint.Client.Web;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientWeb : Web, IClientObject
    {
        public bool IsApp
        {
            get
            {
                return this.IsAppWeb();
            }
        }

        public bool IsLoaded { get; internal set; }

        public bool IsRoot { get; internal set; }

        public SPClientSite ClientSite { get; internal set; }

        internal SPClientWeb(SPClientContext context, ObjectPath objectPath)
            : base(context, objectPath)
        {
        }

        private IEnumerable<SPClientWeb> LoadSubWebs(WebCollection webs)
        {
            var clientWebs = new List<SPClientWeb>();

            if (webs != null && webs.Count > 0)
            {
                foreach (Web web in webs)
                {
                    SPClientWeb clientWeb = LoadWeb(web);
                    clientWebs.Add(clientWeb);
                    Context.ExecuteQuery();
                }
            }
            return clientWebs;
        }

        private async Task<List<SPClientWeb>> LoadSubWebsAsync(WebCollection webs)
        {
            var clientWebs = new List<SPClientWeb>();
            if (webs != null && webs.Count > 0)
            {
                foreach (Web web in webs)
                {
                    SPClientWeb clientWeb = LoadWeb(web);
                    clientWebs.Add(clientWeb);
                    await Context.ExecuteQueryAsync();
                }
            }
            return clientWebs;
        }

        private IEnumerable<SPClientList> LoadLists(ListCollection lists)
        {
            if (lists == null || lists.Count <= 0) return new SPClientList[0];
            var clientLists = lists.ToList().Select(list => LoadList(list)).ToList();
            this.Context.ExecuteQuery();
            return clientLists;
        }

        private async Task<IEnumerable<SPClientList>> LoadListsAsync(ListCollection lists)
        {
            if (lists == null || lists.Count <= 0) return new SPClientList[0];
            var clientLists = lists.ToList().Select(list => LoadList(list)).ToList();
            await this.Context.ExecuteQueryAsync();
            return clientLists;
        }

        private IEnumerable<ContentType> LoadContentTypes(ContentTypeCollection contentTypes)
        {
            var cts = new List<ContentType>();
            if (contentTypes != null && contentTypes.Count > 0)
            {
                foreach (ContentType contentType in contentTypes)
                {
                    this.Context.Load(contentType);
                    cts.Add(contentType);
                }

                this.Context.ExecuteQuery();
            }
            return cts;
        }

        private async Task<List<ContentType>> LoadContentTypesAsync(ContentTypeCollection contentTypes)
        {
            var cts = new List<ContentType>();
            if (contentTypes != null && contentTypes.Count > 0)
            {
                foreach (ContentType contentType in contentTypes)
                {
                    this.Context.Load(contentType);
                    cts.Add(contentType);
                }

                await this.Context.ExecuteQueryAsync();
            }
            return cts;
        }

        private IEnumerable<SPClientField> LoadFields(FieldCollection fields)
        {
            var clientFields = new List<SPClientField>();
            if (fields != null && fields.Count > 0)
            {
                foreach (Field field in fields)
                {
                    SPClientField clientField = LoadField(field, false);
                    clientFields.Add(clientField);
                }

                this.Context.ExecuteQuery();
            }
            return clientFields;
        }

        private async Task<List<SPClientField>> LoadFieldsAsync(FieldCollection fields)
        {
            var clientFields = new List<SPClientField>();
            if (fields != null && fields.Count > 0)
            {
                foreach (Field field in fields)
                {
                    SPClientField clientField = LoadField(field, false);
                    clientFields.Add(clientField);
                }

                await this.Context.ExecuteQueryAsync();
            }
            return clientFields;
        }

        private SPClientWeb LoadWeb(Web web, bool executeQuery = false)
        {
            SPClientWeb clientWeb = this == web ? this : FromWeb(web);
            this.Context.Load(clientWeb);
            clientWeb.ClientSite = this.ClientSite;
            if (executeQuery)
            {
                this.Context.ExecuteQuery();
                clientWeb.IsLoaded = true;
            }
            return clientWeb;
        }

        //private async Task<SPClientWeb> LoadWebAsync(Web web)
        //{
        //    SPClientWeb clientWeb = this == web ? this : FromWeb(web);
        //    this.Context.Load(clientWeb);
        //    clientWeb.ClientSite = this.ClientSite;
        //    await this.Context.ExecuteQueryAsync();
        //    clientWeb.IsLoaded = true;
        //    return clientWeb;
        //}

        private SPClientList LoadList(List list, bool executeQuery = false)
        {
            SPClientList clientList = SPClientList.FromList(list);
            this.Context.Load(clientList);
            clientList.ClientWeb = this;
            if (executeQuery)
            {
                this.Context.ExecuteQuery();
                clientList.IsLoaded = true;
            }
            return clientList;
        }

        //private async Task<SPClientList> LoadListAsync(List list)
        //{
        //    SPClientList clientList = SPClientList.FromList(list);
        //    this.Context.Load(clientList);
        //    clientList.ClientWeb = this;
        //    await this.Context.ExecuteQueryAsync();
        //    clientList.IsLoaded = true;
        //    return clientList;
        //}

        private SPClientField LoadField(Field field, bool executeQuery = false)
        {
            SPClientField clientField = SPClientField.FromField(field);
            this.Context.Load(clientField);
            clientField.ClientWeb = this;
            clientField.IsSiteField = true;
            if (executeQuery)
            {
                this.Context.ExecuteQuery();
                clientField.IsLoaded = true;
            }
            return clientField;
        }

        internal static SPClientWeb FromWeb(Web web)
        {
            return new SPClientWeb(web.Context as SPClientContext, web.Path);
        }

        public IEnumerable<SPClientWeb> LoadWebs(SubwebQuery query = null)
        {
            int count;
            return LoadWebs(query, out count);
        }

        public IEnumerable<SPClientWeb> LoadWebs(out int count)
        {
            return LoadWebs(null, out count);
        }

        public IEnumerable<SPClientWeb> LoadWebs(SubwebQuery query, out int count)
        {
            WebCollection webs = this.GetWebCollection(query);
            this.Context.ExecuteQuery();
            count = webs.Count;
            return LoadSubWebs(webs).Where(clientWeb => clientWeb != null);
        }

        public async Task<IEnumerable<SPClientWeb>> LoadWebsAsync(SubwebQuery query = null)
        {
            WebCollection webs = await this.GetWebCollectionAsync(query);
            //int count = webs.Count;
            List<SPClientWeb> clientWebs = await LoadSubWebsAsync(webs);
            return clientWebs.Where(clientWeb => clientWeb != null);
        }

        public WebCollection GetWebCollection(SubwebQuery query = null)
        {
            WebCollection webs = this.GetSubwebsForCurrentUser(query);

            if (!webs.AreItemsAvailable)
            {
                this.Context.Load(webs);
            }
            return webs;
        }

        public async Task<WebCollection> GetWebCollectionAsync(SubwebQuery query = null)
        {
            WebCollection webs = this.GetSubwebsForCurrentUser(query);

            if (!webs.AreItemsAvailable)
            {
                this.Context.Load(webs);
                await this.Context.ExecuteQueryAsync();
            }
            return webs;
        }

        public ListCollection GetListCollection()
        {
            ListCollection lists = this.Lists;

            if (!lists.AreItemsAvailable)
            {
                this.Context.Load(lists);
            }
            return lists;
        }

        public async Task<ListCollection> GetListCollectionAsync()
        {
            ListCollection lists = this.Lists;

            if (!lists.AreItemsAvailable)
            {
                this.Context.Load(lists);
                await this.Context.ExecuteQueryAsync();
            }
            return lists;
        }

        public IEnumerable<SPClientList> LoadLists()
        {
            int count;
            return LoadLists(out count);
        }

        public IEnumerable<SPClientList> LoadLists(out int count)
        {
            ListCollection lists = this.GetListCollection();
            this.Context.ExecuteQuery();
            count = lists.Count;
            return LoadLists(lists).Where(clientList => clientList != null);
        }

        public async Task<IEnumerable<SPClientList>> LoadListsAsync()
        {
            ListCollection lists = await this.GetListCollectionAsync();
            var clientLists = await LoadListsAsync(lists);
            return clientLists.Where(clientList => clientList != null);
        }

        public ContentTypeCollection GetContentTypeCollection()
        {
            ContentTypeCollection contentTypes = this.ContentTypes;

            if (!contentTypes.AreItemsAvailable)
            {
                this.Context.Load(contentTypes);
            }
            return contentTypes;
        }

        public async Task<ContentTypeCollection> GetContentTypeCollectionAsync()
        {
            ContentTypeCollection contentTypes = this.ContentTypes;

            if (!contentTypes.AreItemsAvailable)
            {
                this.Context.Load(contentTypes);
                await this.Context.ExecuteQueryAsync();
            }
            return contentTypes;
        }

        public IEnumerable<ContentType> LoadContentTypes(out int count)
        {
            ContentTypeCollection contentTypes = this.GetContentTypeCollection();
            this.Context.ExecuteQuery();
            count = contentTypes.Count;
            return LoadContentTypes(contentTypes).Where(ct => ct != null);
        }

        public async Task<IEnumerable<ContentType>> LoadContentTypesAsync()
        {
            ContentTypeCollection contentTypes = await this.GetContentTypeCollectionAsync();
            List<ContentType> cts = await LoadContentTypesAsync(contentTypes);
            return cts.Where(ct => ct != null);
        }

        public FieldCollection GetFieldCollection()
        {
            FieldCollection fields = this.Fields;
            if (!fields.AreItemsAvailable)
            {
                this.Context.Load(fields);
            }
            return fields;
        }

        public async Task<FieldCollection> GetFieldCollectionAsync()
        {
            FieldCollection fields = this.Fields;

            if (!fields.AreItemsAvailable)
            {
                this.Context.Load(fields);
                await this.Context.ExecuteQueryAsync();
            }
            return fields;
        }

        public IEnumerable<SPClientField> LoadFields()
        {
            int count;
            return LoadFields(out count);
        }

        public IEnumerable<SPClientField> LoadFields(out int count)
        {
            FieldCollection fields = this.GetFieldCollection();
            this.Context.ExecuteQuery();
            count = fields.Count;
            return LoadFields(fields).Where(clientField => clientField != null);
        }

        public async Task<IEnumerable<SPClientField>> LoadFieldsAsync()
        {
            FieldCollection fields = await this.GetFieldCollectionAsync();
            List<SPClientField> clientFields = await LoadFieldsAsync(fields);
            return clientFields.Where(clientField => clientField != null);
        }

        public string GetUrl()
        {
            return Utility.CombineUrls(new Uri(this.Context.Url.ToLower()), this.ServerRelativeUrl.ToLower());
        }

        public string GetSettingsUrl()
        {
            return string.Format("{0}/_layouts/{1}/settings.aspx", this.GetUrl().TrimEnd('/'), this.UIVersion);
        }

        /// <summary>
        /// Returns true, if the current web is an App web
        /// </summary>
        /// <returns></returns>
        public bool IsAppWeb()
        {
            return this.IsPropertyAvailable("AppInstanceId") && this.AppInstanceId != Guid.Empty;
        }

        public string GetRestUrl()
        {
            return string.Format("{0}/_api/web", this.GetUrl().TrimEnd('/'));
        }

        public override void RefreshLoad()
        {
            if (!this.IsLoaded)
            {
                base.RefreshLoad();
            }
        }
    }
}