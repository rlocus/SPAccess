using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class NestedOperator : Operator
    {
        protected NestedOperator(string operatorName, params Operator[] operators)
            : base(operatorName)
        {
            Operators = operators;
        }

        protected NestedOperator(string operatorName, string existingNestedOperator)
            : base(operatorName, existingNestedOperator)
        {
        }

        protected NestedOperator(string operatorName, XElement existingNestedOperator)
            : base(operatorName, existingNestedOperator)
        {
        }

        public IEnumerable<Operator> Operators { get; set; }

        protected override void OnParsing(XElement existingNestedOperator)
        {
            Operators = existingNestedOperator.Elements().Select(GetOperator).Where(op => op != null);
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            foreach (var op in Operators.Where(op => op != null))
            {
                el.Add(op.ToXElement());
            }
            return el;
        }
    }
}