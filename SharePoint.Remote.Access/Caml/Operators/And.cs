using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Extensions;

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

        public override void Combine(Operator @operator)
        {
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            var @logicalJoin = Operators.OfType<LogicalJoin>().FirstOrDefault();
            if (@logicalJoin != null)
            {
                @logicalJoin.Combine(@operator);
            }
            else
            {
                var operators = new List<Operator>();
                operators.AddRange(Operators.Where(@op => !(@op is LogicalJoin)).Take(OperatorCount - 1));
                operators.Add(new And(new List<Operator>(Operators.Where(@op => !operators.Contains(@op))) { @operator }.ToArray()));
                InitOperators(operators);
            }
        }
    }
}