using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class LogicalJoin : Operator
    {
        internal const int OperatorCount = 2;
        internal const int NestedOperatorCount = 1;

        public LogicalJoin Parent { get; private set; }
        public Operator[] Operators { get; private set; }

        protected LogicalJoin(string operatorName, IEnumerable<Operator> operators)
            : base(operatorName)
        {
            InitOperators(operators);
        }

        protected LogicalJoin(string operatorName, string existingNestedOperator)
            : base(operatorName, existingNestedOperator)
        {
        }

        protected LogicalJoin(string operatorName, XElement existingNestedOperator)
            : base(operatorName, existingNestedOperator)
        {
        }

        private void InitOperators(IEnumerable<Operator> operators)
        {
            if (operators != null)
            {
                Operators = operators as Operator[] ?? operators.Where(op => op != null).ToArray();
                if (Operators.Length != OperatorCount)
                {
                    throw new NotSupportedException($"Count of operators must be {OperatorCount}.");
                }
                if (Operators.OfType<LogicalJoin>().Count() > NestedOperatorCount)
                {
                    throw new NotSupportedException($"Max count of logical operators must be {NestedOperatorCount}.");
                }

                foreach (var @operator in Operators.OfType<LogicalJoin>())
                {
                    @operator.Parent = this;
                }
            }
        }

        protected override void OnParsing(XElement existingNestedOperator)
        {
            var operators = existingNestedOperator.Elements().Select(el =>
            {
                var op = GetOperator(el);
                var @operator = op as LogicalJoin;
                if (@operator != null)
                {
                    @operator.Parent = this;
                }
                return op;
            }).Where(op => op != null);
            InitOperators(operators);
        }

        public void CombineAnd(LogicalJoin logicalJoin)
        {
            if (logicalJoin == null) throw new ArgumentNullException(nameof(logicalJoin));
            var @operator = this;
            var operators = new List<Operator>();
            var childOperator = @operator.Operators.OfType<LogicalJoin>()
                .FirstOrDefaultFromMany(op => op.Operators.OfType<LogicalJoin>(), op => !op.Operators.OfType<LogicalJoin>().Any());
            if (childOperator != null)
            {
                @operator = childOperator;
            }
            operators.AddRange(@operator.Operators.Where(@op => !(@op is LogicalJoin)).Take(OperatorCount - 1));
            operators.Add(new And(new List<Operator>(@operator.Operators.Where(@op => !operators.Contains(@op))) { logicalJoin }.ToArray()));
            @operator.InitOperators(operators);
        }

        public void CombineOr(LogicalJoin logicalJoin)
        {
            if (logicalJoin == null) throw new ArgumentNullException(nameof(logicalJoin));
            var @operator = this;
            var operators = new List<Operator>();
            var childOperator = @operator.Operators.OfType<LogicalJoin>()
                .FirstOrDefaultFromMany(op => op.Operators.OfType<LogicalJoin>(), op => !op.Operators.OfType<LogicalJoin>().Any());
            if (childOperator != null)
            {
                @operator = childOperator;
            }
            operators.AddRange(@operator.Operators.Where(@op => !(@op is LogicalJoin)).Take(OperatorCount - 1));
            operators.Add(new Or(new List<Operator>(@operator.Operators.Where(@op => !operators.Contains(@op))) { logicalJoin }.ToArray()));
            @operator.InitOperators(operators);
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