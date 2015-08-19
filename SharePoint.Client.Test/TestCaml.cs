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
            string q = GetTestQuery();
            string q1 = GetTestQuery1();
            string q2 = GetTestQuery2();

            string q3 = GetTestQuery1().CombineOr(GetTestQuery2());

            string q4 = GetTestQuery3().CombineAnd(GetTestQuery4());

            string q5 = GetTestQuery6();
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
          return  new CamlWhere(
                new And(new Lt<int>("ProductID", 1000, FieldType.Integer),
                    new Gt<int>("ProductID", 100, FieldType.Integer)));
        }

        public CamlWhere GetTestQuery2()
        {
            return
                new CamlWhere(new Or(new Eq<bool>("IsCompleted", false, FieldType.Boolean), new IsNull("IsCompleted")));

        }

        public CamlWhere GetTestQuery3()
        {
            return
                new CamlWhere(new Eq<bool>("IsCompleted", false, FieldType.Boolean));

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
            return
                new CamlWhere(new DateRangesOverlap(new[] {"Start","End"}, DateValue.Week));

        }
    }
}
