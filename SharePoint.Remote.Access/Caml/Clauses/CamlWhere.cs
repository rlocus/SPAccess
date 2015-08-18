using System;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Caml.Interfaces;
using SharePoint.Remote.Access.Caml.Operators;

namespace SharePoint.Remote.Access.Caml.Clauses
{
    public sealed class CamlWhere : CamlClause
    {
        internal const string WhereTag = "Where";

        internal Operator Operator { get; private set; }

        public CamlWhere(Operator op)
            : base(WhereTag)
        {
            if (op == null) throw new ArgumentNullException(nameof(op));
            Operator = op;
        }

        public CamlWhere(string existingWhere)
            : base(WhereTag, existingWhere)
        {
        }

        public CamlWhere(XElement existingWhere)
            : base(WhereTag, existingWhere)
        {
        }

        internal void And<T>(T op)
            where T : Operator, ICamlField, ICamlMultiField
        {
            if (op == null) throw new ArgumentNullException(nameof(op));

            //var operators = new List<Operator>(Operators) {op};
            //Operators = new[]
            //{
            //    new And(operators.ToArray())
            //};
        }

        internal void Or<T>(T op)
            where T : Operator, ICamlField, ICamlMultiField
        {
            if (op == null) throw new ArgumentNullException(nameof(op));
            //var operators = new List<Operator>(Operators) {op};
            //Operators = new[]
            //{
            //    new Or(operators.ToArray())
            //};
        }

        protected override void OnParsing(XElement existingWhere)
        {
            Operator = existingWhere.Elements().Select(Operator.GetOperator).FirstOrDefault(op => op != null);
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            if (Operator != null)
            {
                el.Add(Operator.ToXElement());
            }
            return el;
        }
    }
}