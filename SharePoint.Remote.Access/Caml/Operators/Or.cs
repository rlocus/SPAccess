using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public sealed class Or : LogicalJoin
    {
        internal const string OrTag = "Or";

        public Or(params Operator[] operators)
            : base(OrTag, operators)
        {
        }

        public Or(Operator firstOperator, Operator secondOperator)
            : base(OrTag, new[] {firstOperator, secondOperator})
        {
        }

        public Or(string existingOrOperator)
            : base(OrTag, existingOrOperator)
        {
        }

        public Or(XElement existingOrOperator)
            : base(OrTag, existingOrOperator)
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
                operators.Add(
                    new Or(new List<Operator>(Operators.Where(@op => !operators.Contains(@op))) {@operator}.ToArray()));
                InitOperators(operators);
            }
        }
    }
}