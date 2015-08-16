﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientSite
    {
        private bool _executeQuery = true;

        internal SPClientSite(Site site)
        {
            if (site == null) throw new ArgumentNullException(nameof(site));
            Site = site;
        }

        public Site Site { get; set; }
        public bool IsLoaded { get; private set; }

        internal static SPClientSite FromSite(Site site)
        {
            return new SPClientSite(site);
        }

        public SPClientWeb[] LoadAllWebs()
        {
            var allClientWebs = new List<SPClientWeb>();
            var rootWeb = GetRootWeb();
            Site.Context.Load(rootWeb.Web.Webs);
            Site.Context.ExecuteQuery();
            allClientWebs.Add(rootWeb);
            var webs = rootWeb.Web.Webs.ToList().RecursiveSelect(web => web.Webs);
            foreach (var web in webs)
            {
                Site.Context.Load(web.Webs);
                var clientweb = SPClientWeb.FromWeb(web);
                clientweb.ClientSite = this;
                Site.Context.ExecuteQuery();
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
            Site.Context.Load(rootWeb.Web.Webs);

            var webs = rootWeb.Web.Webs.ToList().RecursiveSelect(web => web.Webs);
            foreach (var web in webs)
            {
                Site.Context.Load(web.Webs);
                var clientweb = SPClientWeb.FromWeb(web);
                clientweb.ClientSite = this;
                await clientweb.LoadAsync();
                allClientWebs.Add(clientweb);
            }
            return allClientWebs.ToArray();
        }

        public SPClientWeb GetRootWeb()
        {
            return new SPClientWeb(Site.RootWeb);
        }

        public SPClientSite IncludeRootWeb()
        {
            Site.Context.Load(Site.RootWeb);
            _executeQuery = true;
            return this;
        }

        public void Load()
        {
            if (!IsLoaded)
            {
                Site.Context.Load(Site);
                _executeQuery = true;
            }
            if (_executeQuery)
            {
                Site.Context.ExecuteQuery();
                IsLoaded = true;
            }
            _executeQuery = false;
        }

        public async Task LoadAsync()
        {
            if (!IsLoaded)
            {
                Site.Context.Load(Site);
                _executeQuery = true;
            }
            if (_executeQuery)
            {
                await Site.Context.ExecuteQueryAsync();
                IsLoaded = true;
            }
            _executeQuery = false;
        }

        public string GetRestUrl()
        {
            return string.Format("{0}/_api/site", Site.Url.TrimEnd('/'));
        }

        public void RefreshLoad()
        {
            if (IsLoaded)
            {
                IsLoaded = false;
                Site.RefreshLoad();
            }
        }
    }
}