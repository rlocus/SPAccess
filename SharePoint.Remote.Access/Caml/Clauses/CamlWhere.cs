using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
            where T : Operator
        {
            if (op == null) throw new ArgumentNullException(nameof(op));
            var @operator = Operator as NestedOperator;
            if (@operator != null && op is NestedOperator)
            {
                var operators = new List<Operator>();
                operators.AddRange(@operator.Operators.Where(@o => !(@o is NestedOperator)).Take(NestedOperator.OperatorCount - 1));
                operators.Add(new And(new List<Operator>(@operator.Operators.Where(@o => !operators.Contains(@o))) { op }.ToArray()));

                if (@operator is And)
                {
                    Operator = new And(operators.ToArray());
                }
                if (@operator is Or)
                {
                    Operator = new Or(operators.ToArray());
                }
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
            var @operator = Operator as NestedOperator;
            if (@operator != null && op is NestedOperator)
            {
                var operators = new List<Operator>();
                operators.AddRange(@operator.Operators.Where(@o => !(@o is NestedOperator)).Take(NestedOperator.OperatorCount - 1));
                operators.Add(new Or(new List<Operator>(@operator.Operators.Where(@o => !operators.Contains(@o))) { op }.ToArray()));

                if (@operator is And)
                {
                    Operator = new And(operators.ToArray());
                }
                if (@operator is Or)
                {
                    Operator = new Or(operators.ToArray());
                }
            }
            else
            {
                Operator = new And(Operator, op);
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