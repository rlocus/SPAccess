using System.Xml.Linq;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public sealed class Or : NestedOperator
    {
        internal const string OrTag = "Or";

        public Or(params Operator[] operators)
            : base(OrTag, operators)
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