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
                Where = new Where(
                    new Or(
                        new Eq<string>("ContentType", "My Content Type", FieldType.Text),
                        new IsNotNull("Description"))),

                GroupBy = new GroupBy(new FieldRef { Name = "Title" }, true),
                OrderBy = new OrderBy(new FieldRef { Name = "_Author" }).ThenBy("AuthoringDate").ThenBy("AssignedTo")
            };

            var caml = new Query()
            {
                Where = new Where(query.Where.ToString()),
                GroupBy = new GroupBy(query.GroupBy.ToString()),
                OrderBy = new OrderBy(query.OrderBy.ToString())
            };
        }
    }
}
