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
        public LogicalJoin Parent { get; private set; }
        public Operator[] Operators { get; private set; }

        protected LogicalJoin(string operatorName, IEnumerable<Operator> operators)
            : base(operatorName)
        {
            InitOperators(operators);
        }

        protected LogicalJoin(string operatorName, string existingLogicalJoin)
            : base(operatorName, existingLogicalJoin)
        {
        }

        protected LogicalJoin(string operatorName, XElement existingLogicalJoin)
            : base(operatorName, existingLogicalJoin)
        {
        }

        protected void InitOperators(IEnumerable<Operator> operators)
        {
            if (operators != null)
            {
                Operators = operators as Operator[] ?? operators.Where(op => op != null).ToArray();
                if (Operators.Length < OperatorCount)
                {
                    throw new NotSupportedException($"Should be at least of {OperatorCount} operators.");
                }
                if (Operators.OfType<LogicalJoin>().Count() == Operators.Length)
                {
                    throw new NotSupportedException("All operators are logical joins.");
                }
                foreach (var @operator in Operators.OfType<LogicalJoin>())
                {
                    @operator.Parent = this;
                }
            }
        }

        protected override void OnParsing(XElement existingLogicalJoin)
        {
            var operators = existingLogicalJoin.Elements().Select(el =>
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

        public abstract void Combine(Operator @operator);

        internal LogicalJoin CombineAnd(Operator combinedOperator)
        {
            if (combinedOperator == null) throw new ArgumentNullException(nameof(combinedOperator));
            var @operator = this;
            var operators = new List<Operator>();
            var childOperator = @operator.Operators.OfType<LogicalJoin>()
                .FirstOrDefaultFromMany(op => op.Operators.OfType<LogicalJoin>(), op => !op.Operators.OfType<LogicalJoin>().Any());
            if (childOperator != null)
            {
                @operator = childOperator;
            }
            operators.AddRange(@operator.Operators.Where(@op => !(@op is LogicalJoin)).Take(OperatorCount - 1));
            var result =
                new And(
                    new List<Operator>(@operator.Operators.Where(@op => !operators.Contains(@op))) { combinedOperator }.ToArray());
            operators.Add(result);
            @operator.InitOperators(operators);
            return result;
        }

        internal LogicalJoin CombineOr(Operator combinedOperator)
        {
            if (combinedOperator == null) throw new ArgumentNullException(nameof(combinedOperator));
            var @operator = this;
            var operators = new List<Operator>();
            var childOperator = @operator.Operators.OfType<LogicalJoin>()
                .FirstOrDefaultFromMany(op => op.Operators.OfType<LogicalJoin>(), op => !op.Operators.OfType<LogicalJoin>().Any());
            if (childOperator != null)
            {
                @operator = childOperator;
            }
            operators.AddRange(@operator.Operators.Where(@op => !(@op is LogicalJoin)).Take(OperatorCount - 1));
            var result =
                new Or(
                    new List<Operator>(@operator.Operators.Where(@op => !operators.Contains(@op))) { combinedOperator }
                        .ToArray());
            operators.Add(result);
            @operator.InitOperators(operators);
            return result;
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