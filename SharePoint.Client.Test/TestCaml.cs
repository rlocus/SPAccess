using System;
using System.Linq;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharePoint.Remote.Access.Caml;
using SharePoint.Remote.Access.Caml.Clauses;
using SharePoint.Remote.Access.Caml.Operators;
using View = SharePoint.Remote.Access.Caml.View;

namespace SharePoint.Client.Test
{
    [TestClass]
    public class TestCaml
    {
        [TestMethod]
        public void TestMethod1()
        {
            //string q = GetTestQuery();
            //string q1 = GetTestQuery1();
            //string q2 = GetTestQuery2();

            //string q3 = GetTestQuery1().CombineOr(GetTestQuery2());
            //string q4 = GetTestQuery2().CombineOr(GetTestQuery1());

            //string q5 = GetTestQuery().CombineAnd(GetTestQuery7());
            //string q6 = new CamlWhere(q5);

            //string q = GetTestQuery7();
            //string s = new CamlWhere(q);
            var view = new View(new[] { "Title", "ID" }, new Join[] { new InnerJoin("Test", "List 1"), new LeftJoin("Test", "List 1", "List 2") }) { Query = { Where = GetTestQuery2() } };

            view.Query.WhereAny(new Eq<bool>("IsCompleted", false, FieldType.Boolean), new Lt<int>("ProductID", 1000, FieldType.Integer));

            view.Query.WhereAny(new Or(new Eq<bool>("IsCompleted", false, FieldType.Boolean), new And(new IsNull("IsCompleted"), new BeginsWith("Title", "test"))));
            view.Query.GroupBy = new CamlGroupBy(new[] { "Title" }, true);
            view.Query.OrderBy = new CamlOrderBy(new[] { new CamlFieldRef { Name = "Title", Ascending = true } });
            
            view.Query.Where.Or(new Eq<int>(new CamlFieldRef { Name = "ID" }, 1, FieldType.Counter)).Or(new Eq<int>(new CamlFieldRef { Name = "ID" }, 2, FieldType.Counter)).And(new Eq<string>(new CamlFieldRef { Name = "Title" }, "", FieldType.Text));

            string q = view;
            string v = new View(q);


            //var q = new Query() { Where = new CamlWhere(new Contains("Title", "e")) };
            //q.WhereAll(new IsNotNull("Title"), new Leq<DateTime>(new CamlFieldRef { Name = "Created" }, DateTime.Now, FieldType.DateTime));

            //string s = q;
        }

        public CamlWhere GetTestQuery()
        {
            //var caml =
            //   Camlex.Query()
            //       .Where(x => ((int)x["ProductID"] < 1000 && (int)x["ProductID"] > 100) ||
            //             ((bool)x["IsCompleted"] == false || x["IsCompleted"] == null))
            //           .ToString();


            return
                  new CamlWhere(
                      new And(new Lt<int>("ProductID", 1000, FieldType.Integer),
                          new Or(new Gt<int>("ProductID", 100, FieldType.Integer),
                              new Or(new Eq<bool>("IsCompleted", false, FieldType.Boolean), new IsNull("IsCompleted")))));

        }

        public CamlWhere GetTestQuery1()
        {
            return new CamlWhere(
                  new And(new Lt<int>("ProductID", 1000, FieldType.Integer),
                      new Gt<int>("ProductID", 100, FieldType.Integer)));
        }

        public CamlWhere GetTestQuery2()
        {
            return
                new CamlWhere(new Or(new Eq<bool>("IsCompleted", false, FieldType.Boolean), new And(new IsNull("IsCompleted"), new BeginsWith("Title", "test"))));

        }

        public CamlWhere GetTestQuery3()
        {
            var eq = new Eq<bool>("IsCompleted", false, FieldType.Boolean);
            eq.FieldRef.Nullable = true;
            return new CamlWhere(eq);

        }

        public CamlWhere GetTestQuery4()
        {
            return
                new CamlWhere(new IsNull("IsCompleted"));

        }

        public CamlWhere GetTestQuery5()
        {
            return
                new CamlWhere(new Eq<bool>("IsCompleted", false, FieldType.Boolean));
        }

        public CamlWhere GetTestQuery6()
        {
            var today = CamlValue.Today;
            today.Offset = -6;
            var dateRangesOverlap = new DateRangesOverlap("Start", "End", "Reccurence", today);
            dateRangesOverlap.Value.IncludeTimeValue = true;
            return new CamlWhere(dateRangesOverlap);

        }

        public CamlWhere GetTestQuery7()
        {
            return
                new CamlWhere(new And(new IsNull("Title"),
                    new Eq(new CamlFieldRef { Name = "User" }, CamlValue.UserId, FieldType.Integer)));

        }


        public CamlWhere GetTestQuery8()
        {
            return
                new CamlWhere(new Membership("Author", MembershipType.WebUsers));

        }
    }
}
