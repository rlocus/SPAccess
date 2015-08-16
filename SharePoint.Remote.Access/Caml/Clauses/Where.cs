using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Caml.Interfaces;
using SharePoint.Remote.Access.Caml.Operators;

namespace SharePoint.Remote.Access.Caml.Clauses
{
    public sealed class Where : Clause
    {
        internal const string WhereTag = "Where";

        public Where(Operator op)
            : base(WhereTag, op)
        {
            if (op == null) throw new ArgumentNullException(nameof(op));
        }

        public Where(string existingWhere)
            : base(WhereTag, existingWhere)
        {
        }

        public Where(XElement existingWhere)
            : base(WhereTag, existingWhere)
        {
        }

        internal void And<T>(T op)
            where T : Operator, IFieldOperator, IMultiFieldOperator
        {
            if (op == null) throw new ArgumentNullException(nameof(op));

            var operators = new List<Operator>(Operators) {op};
            Operators = new[]
            {
                new And(operators.ToArray())
            };
        }

        internal void Or<T>(T op)
            where T : Operator, IFieldOperator, IMultiFieldOperator
        {
            if (op == null) throw new ArgumentNullException(nameof(op));
            var operators = new List<Operator>(Operators) {op};
            Operators = new[]
            {
                new Or(operators.ToArray())
            };
        }

        protected override void OnParsing(XElement existingWhere)
        {
            Operators = existingWhere.Elements().Select(Operator.GetOperator).Where(op => op != null);
        }
    }
}