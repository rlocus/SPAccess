using System;
using System.Linq;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Site = Microsoft.SharePoint.Client.Site;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientSite
    {
        public Site Site { get; set; }
        private bool _executeQuery = true;

        internal SPClientSite(Site site)
        {
            if (site == null) throw new ArgumentNullException("site");
            Site = site;
        }

        internal static SPClientSite FromSite(Site site)
        {
            return new SPClientSite(site);
        }

        public SPClientWeb[] LoadAllWebs()
        {
            var allClientWebs = new List<SPClientWeb>();
            var rootWeb = GetRootWeb();
            this.Site.Context.Load(rootWeb.Web.Webs);
            this.Site.Context.ExecuteQuery();
            allClientWebs.Add(rootWeb);
            var webs = rootWeb.Web.Webs.ToList().RecursiveSelect(web => web.Webs);
            foreach (Web web in webs)
            {
                this.Site.Context.Load(web.Webs);
                var clientweb = SPClientWeb.FromWeb(web);
                clientweb.ClientSite = this;
                this.Site.Context.ExecuteQuery();
                allClientWebs.Add(clientweb);
            }
            return allClientWebs.ToArray();
        }

        public async Task<SPClientWeb[]> LoadAllWebsAsync()
        {
            var allClientWebs = new List<SPClientWeb>();
            var rootWeb = GetRootWeb();
            await rootWeb.LoadAsync();
            allClientWebs.Add(rootWeb);
            this.Site.Context.Load(rootWeb.Web.Webs);
          
            var webs = rootWeb.Web.Webs.ToList().RecursiveSelect(web => web.Webs);
            foreach (Web web in webs)
            {
                this.Site.Context.Load(web.Webs);
                var clientweb = SPClientWeb.FromWeb(web);
                clientweb.ClientSite = this;
                await clientweb.LoadAsync();
                allClientWebs.Add(clientweb);
            }
            return allClientWebs.ToArray();
        }

        public SPClientWeb GetRootWeb()
        {
            return new SPClientWeb(this.Site.RootWeb);
        }

        public SPClientSite IncludeRootWeb()
        {
            this.Site.Context.Load(Site.RootWeb);
            _executeQuery = true;
            return this;
        }

        public void Load()
        {
            if (!IsLoaded)
            {
                this.Site.Context.Load(this.Site);
                _executeQuery = true;
            }
            if (_executeQuery)
            {
                this.Site.Context.ExecuteQuery();
                this.IsLoaded = true;
            }
            _executeQuery = false;
        }

        public async Task LoadAsync()
        {
            if (!IsLoaded)
            {
                this.Site.Context.Load(this.Site);
                _executeQuery = true;
            }
            if (_executeQuery)
            {
                await this.Site.Context.ExecuteQueryAsync();
                this.IsLoaded = true;
            }
            _executeQuery = false;
        }

        public string GetRestUrl()
        {
            return string.Format("{0}/_api/site", this.Site.Url.TrimEnd('/'));
        }

        public bool IsLoaded { get; private set; }

        public void RefreshLoad()
        {
            if (this.IsLoaded)
            {
                this.IsLoaded = false;
                this.Site.RefreshLoad();
            }
        }
    }
}