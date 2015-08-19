using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharePoint.Remote.Access;
using SharePoint.Remote.Access.Helpers;

namespace SharePoint.Client.Test
{
    [TestClass]
    public class TestContext
    {
        [TestMethod]
        public void TestMethod1()
        {
            var ctx = new SPClientContext(
                "https://artezio.sharepoint.com/sites/art_dev3",
                AuthType.SharePointOnline,
                "rpohomenko@artezio.com",
                "rlocus1!");
            ctx.Connect();
            //var webs = ctx.ClientSite.LoadAllWebs();

            var root = ctx.ClientSite.GetRootWeb();
            root.IncludeLists().Load();
            var lists = root.GetLists();

           lists[0].IncludeContentTypes().Load();
            var cts = lists[0].GetContentTypes();

        }
    }
}

