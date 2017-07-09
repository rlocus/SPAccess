using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharePoint.Remote.Access;
using SharePoint.Remote.Access.Extensions;
using SharePoint.Remote.Access.Helpers;

namespace SharePoint.Client.Test
{
    [TestClass]
    public class TestContext
    {
        [TestMethod]
        public SPClientContext GetClientContext()
        {
            var ctx = new SPClientContext(
                "https://artezio.sharepoint.com/sites/art_dev2",
                AuthType.SharePointOnline,
                "",
                "");
            ctx.Connect();
            return ctx;
            //var webs = ctx.ClientSite.LoadAllWebs();

            // var root = ctx.ClientSite.GetRootWeb();
            // root.IncludeLists().Load();
            // var lists = root.GetLists();

            //lists[0].IncludeContentTypes().Load();
            // var cts = lists[0].GetContentTypes();

        }

        [TestMethod]
        public void LoadWebContent()
        {
            var ctx = GetClientContext();
            ctx.Load(ctx.Site, site => site.Url, site => site.Id, site => site);
            ctx.Load(ctx.Web, web => web.Id, web => web.Title, web => web.ServerRelativeUrl);
            ctx.Load(ctx.Web.Lists);
            ctx.Load(ctx.Web.CurrentUser);
            ctx.Load(ctx.Web.ContentTypes);
            ctx.Load(ctx.Web.AllProperties);
            ctx.Load(ctx.Web.Features);
            ctx.Load(ctx.Web.UserCustomActions);
            ctx.Load(ctx.Web.SiteGroups);
            ctx.Load(ctx.Web.Fields);
            var webs = ctx.LoadQuery(ctx.Web.Webs.Include(web => web.Id, web => web.Title, web => web.ServerRelativeUrl));
            //var webs = ctx.Web.GetSubwebsForCurrentUser(null);
            //ctx.Load(webs);
            ctx.ExecuteQuery();
            foreach (var web in webs.ToList())
            {
                ctx.Load(web.Lists);
                ctx.Load(web.CurrentUser);
                ctx.Load(web.ContentTypes);
                ctx.Load(web.AllProperties);
                ctx.Load(web.Features);
                ctx.Load(web.UserCustomActions);
                ctx.Load(web.SiteGroups);
                ctx.Load(web.Fields);
                ctx.Load(web.AvailableFields);
                ctx.Load(web.AvailableContentTypes);
                ctx.Load(web.Alerts);
                ctx.Load(web.ListTemplates);
                //ctx.ExecuteQuery();
                //count += 12;
                //if (count > 40) break;
            }
            ctx.ExecuteQuery();
        }

        [TestMethod]
        public async Task LoadWebContentAsync()
        {
            var tasks = new List<Task>();
            var ctx = GetClientContext();
            tasks.Add(ctx.LoadAsync(ctx.Site, site => site.Url, site => site.Id, site => site));
            tasks.Add(ctx.LoadAsync(ctx.Web, web => web.Id, web => web.Title, web => web.ServerRelativeUrl));
            tasks.Add(ctx.LoadAsync(ctx.Web.Lists));
            tasks.Add(ctx.LoadAsync(ctx.Web.CurrentUser));
            tasks.Add(ctx.LoadAsync(ctx.Web.ContentTypes));
            tasks.Add(ctx.LoadAsync(ctx.Web.AllProperties));
            tasks.Add(ctx.LoadAsync(ctx.Web.Features));
            tasks.Add(ctx.LoadAsync(ctx.Web.UserCustomActions));
            tasks.Add(ctx.LoadAsync(ctx.Web.SiteGroups));
            tasks.Add(ctx.LoadAsync(ctx.Web.Fields));
            var webs = ctx.LoadQuery(ctx.Web.Webs.Include(web => web.Id, web => web.Title, web => web.ServerRelativeUrl));
            //var webs = ctx.Web.GetSubwebsForCurrentUser(null);
            //await ctx.LoadAsync(webs);
            tasks.Add(ctx.ExecuteQueryAsync());
            await Task.WhenAll(tasks);
            tasks.Clear();
            foreach (var web in webs.ToList())
            {
                tasks.Add(ctx.LoadAsync(web.Lists));
                tasks.Add(ctx.LoadAsync(web.CurrentUser));
                tasks.Add(ctx.LoadAsync(web.ContentTypes));
                tasks.Add(ctx.LoadAsync(web.AllProperties));
                tasks.Add(ctx.LoadAsync(web.Features));
                tasks.Add(ctx.LoadAsync(web.UserCustomActions));
                tasks.Add(ctx.LoadAsync(web.SiteGroups));
                tasks.Add(ctx.LoadAsync(web.Fields));
                tasks.Add(ctx.LoadAsync(web.AvailableFields));
                tasks.Add(ctx.LoadAsync(web.AvailableContentTypes));
                tasks.Add(ctx.LoadAsync(web.Alerts));
                tasks.Add(ctx.LoadAsync(web.ListTemplates));
                //ctx.ExecuteQuery();
                //count += 12;
                //if (count > 40) break;
            }
            tasks.Add(ctx.ExecuteQueryAsync());
            await Task.WhenAll(tasks);
        }
    }
}

