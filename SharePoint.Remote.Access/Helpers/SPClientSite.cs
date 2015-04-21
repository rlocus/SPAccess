using System.Linq;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Site = Microsoft.SharePoint.Client.Site;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientSite : Site, IClientObject
    {
        private bool _executeQuery = true;
        private readonly SPClientWeb _rootWeb;

        internal SPClientSite(SPClientContext context, ObjectPath objectPath)
            : base(context, objectPath)
        {
            _rootWeb = SPClientWeb.FromWeb(this.RootWeb);
            _rootWeb.ClientSite = this;
        }

        internal static SPClientSite FromSite(Site site)
        {
            return new SPClientSite(site.Context as SPClientContext, site.Path);
        }

        public SPClientWeb[] LoadAllWebs()
        {
            var allClientWebs = new List<SPClientWeb>();
            this.Context.Load(_rootWeb.Web.Webs);
            this.Context.ExecuteQuery();
            allClientWebs.Add(_rootWeb);
            var webs = _rootWeb.Web.Webs.ToList().RecursiveSelect(web => web.Webs);
            foreach (Web web in webs)
            {
                this.Context.Load(web.Webs);
                var clientweb = SPClientWeb.FromWeb(web);
                clientweb.ClientSite = this;
                this.Context.ExecuteQuery();
                allClientWebs.Add(clientweb);
            }
            return allClientWebs.ToArray();
        }

        public async Task<SPClientWeb[]> LoadAllWebsAsync()
        {
            var allClientWebs = new List<SPClientWeb>();
            this.Context.Load(_rootWeb.Web.Webs);
            await _rootWeb.LoadAsync();
            allClientWebs.Add(_rootWeb);
            var webs = _rootWeb.Web.Webs.ToList().RecursiveSelect(web => web.Webs);
            foreach (Web web in webs)
            {
                this.Context.Load(web.Webs);
                var clientweb = SPClientWeb.FromWeb(web);
                clientweb.ClientSite = this;
                await clientweb.LoadAsync();
                allClientWebs.Add(clientweb);
            }
            return allClientWebs.ToArray();
        }

        public SPClientWeb GetRootWeb()
        {
            return _rootWeb;
        }

        public SPClientSite IncludeRootWeb()
        {
            this.Context.Load(this.RootWeb);
            _executeQuery = true;
            return this;
        }

        public void Load()
        {
            if (!IsLoaded)
            {
                this.Context.Load(this);
                _executeQuery = true;
            }
            if (_executeQuery)
            {
                this.Context.ExecuteQuery();
                this.IsLoaded = true;
            }
            _executeQuery = false;
        }

        public async Task LoadAsync()
        {
            if (!IsLoaded)
            {
                this.Context.Load(this);
                _executeQuery = true;
            }
            if (_executeQuery)
            {
                await this.Context.ExecuteQueryAsync();
                this.IsLoaded = true;
            }
            _executeQuery = false;
        }

        //public async Task<List<SPClientWeb>> LoadAllWebsAsync()
        //{
        //    var allClientWebs = new List<SPClientWeb>();

        //    if (!_rootWeb.IsLoaded)
        //    {
        //        await LoadRootWebAsync();
        //    }

        //    allClientWebs.Add(_rootWeb);
        //    IEnumerable<SPClientWeb> clientWebs = await _rootWeb.LoadWebsAsync();

        //    var webs = clientWebs.RecursiveSelect(web =>
        //    {
        //        IEnumerable<SPClientWeb> clientSubWebs = null;
        //        new Action(async () =>
        //        {
        //            clientSubWebs = await web.LoadWebsAsync(query);
        //        }).Invoke();
        //        return clientSubWebs;
        //    });

        //    allClientWebs.AddRange(webs);
        //    return allClientWebs;
        //}

        public string GetRestUrl()
        {
            return string.Format("{0}/_api/site", this.Url.TrimEnd('/'));
        }

        public bool IsLoaded { get; private set; }

        public override void RefreshLoad()
        {
            if (this.IsLoaded)
            {
                this.IsLoaded = false;
                base.RefreshLoad();
            }
        }
    }
}