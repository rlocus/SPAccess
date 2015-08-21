using System;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Caml.Operators;

namespace SharePoint.Remote.Access.Caml.Clauses
{
    public sealed class CamlWhere : CamlClause
    {
        internal const string WhereTag = "Where";

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

        internal Operator Operator { get; private set; }

        internal void And<T>(T op)
            where T : Operator
        {
            if (op == null) throw new ArgumentNullException(nameof(op));
            var @operator = Operator as LogicalJoin;
            if (@operator != null && op is LogicalJoin)
            {
                @operator.CombineAnd(op as LogicalJoin);
            }
            else
            {
                Operator = new And(Operator, op);
            }
        }

        internal void Or<T>(T op)
            where T : Operator
        {
            if (op == null) throw new ArgumentNullException(nameof(op));
            var @operator = Operator as LogicalJoin;
            if (@operator != null && op is LogicalJoin)
            {
                @operator.CombineOr(op as LogicalJoin);
            }
            else
            {
                Operator = new Or(Operator, op);
            }
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