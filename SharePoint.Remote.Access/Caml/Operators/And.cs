using System.Xml.Linq;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public sealed class And : NestedOperator
    {
        internal const string AndTag = "And";

        public And(params Operator[] operators)
            : base(AndTag, operators)
        {
        }

        public And(string existingAndOperator)
            : base(AndTag, existingAndOperator)
        {
        }

        public And(XElement existingAndOperator)
            : base(AndTag, existingAndOperator)
        {
        }
    }
}