using System.Xml.Linq;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public sealed class And : LogicalJoin
    {
        internal const string AndTag = "And";

        internal And(params Operator[] operators)
            : base(AndTag, operators)
        {
        }

        public And(Operator firstOperator, Operator secondOperator)
          : base(AndTag, new[] { firstOperator, secondOperator })
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