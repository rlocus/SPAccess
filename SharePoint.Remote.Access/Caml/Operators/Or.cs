using System.Xml.Linq;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public sealed class Or : LogicalJoin
    {
        internal const string OrTag = "Or";

        public Or(params Operator[] operators)
            : base(OrTag, operators)
        {
        }

        public Or(Operator firstOperator, Operator secondOperator)
          : base(OrTag, new[] { firstOperator, secondOperator })
        {
        }

        public Or(string existingOrOperator)
            : base(OrTag, existingOrOperator)
        {
        }

        public Or(XElement existingOrOperator)
            : base(OrTag, existingOrOperator)
        {
        }
    }
}