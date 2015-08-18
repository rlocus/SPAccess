using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharePoint.Remote.Access.Caml;
using SharePoint.Remote.Access.Caml.Clauses;
using SharePoint.Remote.Access.Caml.Operators;

namespace SharePoint.Client.Test
{
    [TestClass]
    public class TestCaml
    {
        [TestMethod]
        public void TestMethod1()
        {
            var query = new Query()
            {
                Where = new CamlWhere(
                    new Or(
                        new Eq<string>("ContentType", "My Content Type", FieldType.Text),
                        new IsNotNull("Description"))),

                GroupBy = new CamlGroupBy(new CamlFieldRef { Name = "Title" }, true),
                OrderBy = new CamlOrderBy(new CamlFieldRef { Name = "_Author" }).ThenBy("AuthoringDate").ThenBy("AssignedTo")
            };

            var caml = new Query()
            {
                Where = new CamlWhere(query.Where.ToString()),
                GroupBy = new CamlGroupBy(query.GroupBy.ToString()),
                OrderBy = new CamlOrderBy(query.OrderBy.ToString())
            };
        }
    }
}
