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
        private const string WhereTag = "Where";

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

            var operators = new List<Operator>(Operators) { op };
            Operators = new[]
            {
                new And(operators.ToArray())
            };
        }

        internal void Or<T>(T op)
            where T : Operator, IFieldOperator, IMultiFieldOperator
        {
            if (op == null) throw new ArgumentNullException(nameof(op));
            var operators = new List<Operator>(Operators) { op };
            Operators = new[]
            {
                new Or(operators.ToArray())
            };
        }

        protected override void OnParsing(XElement existingWhere)
        {
            //Operators = existingWhere.Elements().Select(GetOperator).Where(op => op != null);
        }

        public static Where Combine(Where firstWhere, Where secondWhere)
        {
            Where where = null;
            if (secondWhere != null && firstWhere != null)
            {
                if (secondWhere.Operators.FirstOrDefault() is NestedOperator)
                {
                    var nestedOperator = secondWhere.Operators.FirstOrDefault() as NestedOperator;

                    if (nestedOperator?.Operators != null)
                    {
                        var firstOperator = nestedOperator.Operators.FirstOrDefault(op => op is IFieldOperator);
                        var secondOperator = nestedOperator.Operators.FirstOrDefault(op => !op.Equals(firstOperator));
                        var operators = new List<Operator>();
                        if (firstWhere.Operators != null)
                        {
                            operators.Add(firstWhere.Operators.FirstOrDefault());
                        }
                        operators.Add(firstOperator);
                        nestedOperator.Operators = operators;
                        where = new Where(new And(nestedOperator, secondOperator));
                    }
                    else
                    {
                        if (firstWhere.Operators != null) where = new Where(firstWhere.Operators.FirstOrDefault());
                    }
                }
                else if (secondWhere.Operators?.FirstOrDefault() is IFieldOperator)
                {
                    where = new Where(new And(firstWhere.Operators.FirstOrDefault(), secondWhere.Operators.FirstOrDefault()));
                }
            }
            else if (secondWhere == null && firstWhere != null)
            {
                if (firstWhere.Operators != null)
                {
                    where = new Where(firstWhere.Operators.FirstOrDefault());
                }
            }
            else if (secondWhere?.Operators != null)
            {
                where = new Where(secondWhere.Operators.FirstOrDefault());
            }
            return where;
        }
    }
}