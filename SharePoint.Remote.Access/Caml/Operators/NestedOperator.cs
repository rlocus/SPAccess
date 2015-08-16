using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class NestedOperator : Operator
    {
        internal const int OperatorCount = 2;
        internal const int NestedOperatorCount = 1;

        protected NestedOperator(string operatorName, IEnumerable<Operator> operators)
            : base(operatorName)
        {
            InitOperators(operators);
        }

        protected NestedOperator(string operatorName, string existingNestedOperator)
            : base(operatorName, existingNestedOperator)
        {
        }

        protected NestedOperator(string operatorName, XElement existingNestedOperator)
            : base(operatorName, existingNestedOperator)
        {
        }

        public Operator[] Operators { get; private set; }

        private void InitOperators(IEnumerable<Operator> operators)
        {
            Operators = new Operator[OperatorCount];
            var i = 0;
            if (operators != null)
            {
                foreach (var op in operators)
                {
                    Operators[i++] = op;
                    if (i > OperatorCount)
                    {
                        throw new NotSupportedException($"Max count of operators must be {OperatorCount}.");
                    }
                }
                if (Operators.OfType<NestedOperator>().Count() > NestedOperatorCount)
                {
                    throw new NotSupportedException($"Max count of nested operators must be {NestedOperatorCount}.");
                }
            }
        }

        protected override void OnParsing(XElement existingNestedOperator)
        {
            var operators = existingNestedOperator.Elements().Select(GetOperator).Where(op => op != null);
            InitOperators(operators);
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