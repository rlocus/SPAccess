using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Site = Microsoft.SharePoint.Client.Site;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientSite : Site, IClientObject
    {
        private readonly SPClientWeb _rootWeb;

        internal SPClientSite(SPClientContext context, ObjectPath objectPath)
            : base(context, objectPath)
        {
            this._rootWeb = SPClientWeb.FromWeb(this.RootWeb);
        }

        internal static SPClientSite FromSite(Site site)
        {
            return new SPClientSite(site.Context as SPClientContext, site.Path);
        }

        public async Task<SPClientWeb> LoadRootWebAsync()
        {
            this.Context.Load(_rootWeb);
            await this.Context.ExecuteQueryAsync();
            _rootWeb.IsLoaded = true;
            _rootWeb.ClientSite = this;
            _rootWeb.IsRoot = true;
            return _rootWeb;
        }

        public SPClientWeb LoadRootWeb()
        {
            this.Context.Load(_rootWeb);
            this.Context.ExecuteQuery();
            _rootWeb.IsLoaded = true;
            _rootWeb.ClientSite = this;
            _rootWeb.IsRoot = true;
            return _rootWeb;
        }

        public IEnumerable<SPClientWeb> LoadAllWebs(SubwebQuery query = null)
        {
            if (!_rootWeb.IsLoaded)
            {
                LoadRootWeb();
            }

            yield return _rootWeb;

            var webs = _rootWeb.LoadWebs(query).RecursiveSelect(web => web.LoadWebs(query));

            foreach (SPClientWeb web in webs)
            {
                yield return web;
            }
        }

        public async Task<List<SPClientWeb>> LoadAllWebsAsync(SubwebQuery query = null)
        {
            List<SPClientWeb> allClientWebs = new List<SPClientWeb>();

            if (!_rootWeb.IsLoaded)
            {
                await LoadRootWebAsync();
            }

            allClientWebs.Add(_rootWeb);
            IEnumerable<SPClientWeb> clientWebs = await _rootWeb.LoadWebsAsync(query);

            var webs = clientWebs.RecursiveSelect(web =>
            {
                IEnumerable<SPClientWeb> clientSubWebs = null;
                new Action(async () =>
                {
                    clientSubWebs = await web.LoadWebsAsync(query);
                }).Invoke();
                return clientSubWebs;
            });

            allClientWebs.AddRange(webs);
            return allClientWebs;
        }

        public string GetRestUrl()
        {
            return string.Format("{0}/_api/site", this.Url.TrimEnd('/'));
        }

        public bool IsLoaded { get; internal set; }

        public override void RefreshLoad()
        {
            if (!this.IsLoaded)
            {
                base.RefreshLoad();
            }
        }
    }
}