using System;
using System.Collections.Generic;
using System.Linq;
using SharePoint.Remote.Access.Caml.Clauses;
using SharePoint.Remote.Access.Caml.Operators;

namespace SharePoint.Remote.Access.Caml
{
    public static class CamlExtensions
    {
        public static void WhereAll(this Query query, params Operator[] operators)
        {
            if (operators.Length == 0) return;
            if (query.Where == null)
            {
                query.Where = new CamlWhere(operators.First());
                operators = operators.Skip(1).ToArray();
            }
            query.Where.And(operators);
        }

        public static void WhereAny(this Query query, params Operator[] operators)
        {
            if (operators.Length == 0) return;
            if (query.Where == null)
            {
                query.Where = new CamlWhere(operators.First());
                operators = operators.Skip(1).ToArray();
            }
            query.Where.Or(operators);
        }

        public static CamlWhere And(this CamlWhere firstWhere, CamlWhere secondWhere)
        {
            if (firstWhere == null) throw new ArgumentNullException(nameof(firstWhere));
            if (secondWhere == null) throw new ArgumentNullException(nameof(secondWhere));
            firstWhere = new CamlWhere(firstWhere.Operator);
            firstWhere.Operator.And(secondWhere.Operator);
            return firstWhere;
        }

        public static CamlWhere Or(this CamlWhere where, params Operator[] operators)
        {
            if (where == null) throw new ArgumentNullException(nameof(where));
            if (operators.Length > 0)
            {
                where = new CamlWhere(where.Operator);
                foreach (var @operator in operators)
                {
                    where.Operator.Or(@operator);
                }
            }
            return where;
        }

        public static CamlWhere And(this CamlWhere where, params Operator[] operators)
        {
            if (where == null) throw new ArgumentNullException(nameof(where));
            if (operators.Length > 0)
            {
                where = new CamlWhere(where.Operator);
                foreach (var @operator in operators)
                {
                    where.Operator.And(@operator);
                }
            }
            return where;
        }

        public static CamlWhere Or(this CamlWhere firstWhere, CamlWhere secondWhere)
        {
            if (firstWhere == null) throw new ArgumentNullException(nameof(firstWhere));
            if (secondWhere == null) throw new ArgumentNullException(nameof(secondWhere));
            firstWhere = new CamlWhere(firstWhere.Operator);
            firstWhere.Operator.Or(secondWhere.Operator);
            return firstWhere;
        }

        public static Operator And(this Operator firstOperator, Operator secondOperator)
        {
            if (firstOperator == null) throw new ArgumentNullException(nameof(firstOperator));
            if (secondOperator == null) throw new ArgumentNullException(nameof(secondOperator));
            var logicalJoin = firstOperator as LogicalJoin;
            return logicalJoin != null ? logicalJoin.CombineAnd(secondOperator) : new And(firstOperator, secondOperator);
        }

        public static Operator Or(this Operator firstOperator, Operator secondOperator)
        {
            if (firstOperator == null) throw new ArgumentNullException(nameof(firstOperator));
            if (secondOperator == null) throw new ArgumentNullException(nameof(secondOperator));
            var logicalJoin = firstOperator as LogicalJoin;
            return logicalJoin != null ? logicalJoin.CombineOr(secondOperator) : new Or(firstOperator, secondOperator);
        }

        public static CamlOrderBy ThenBy(this CamlOrderBy orderBy, Guid fieldId, bool? ascending = null)
        {
            return orderBy.ThenBy(new CamlFieldRef {Id = fieldId, Ascending = @ascending});
        }

        public static CamlOrderBy ThenBy(this CamlOrderBy orderBy, string fieldName, bool? ascending = null)
        {
            return orderBy.ThenBy(new CamlFieldRef {Name = fieldName, Ascending = @ascending});
        }

        public static CamlOrderBy ThenBy(this CamlOrderBy orderBy, CamlFieldRef fieldRef)
        {
            if (orderBy == null) throw new ArgumentNullException(nameof(orderBy));
            var fields = new List<CamlFieldRef>(orderBy.FieldRefs) {fieldRef};
            return new CamlOrderBy(fields);
        }

        public static CamlGroupBy ThenBy(this CamlGroupBy groupBy, Guid fieldId)
        {
            return groupBy.ThenBy(new CamlFieldRef {Id = fieldId, Ascending = false});
        }

        public static CamlGroupBy ThenBy(this CamlGroupBy groupBy, string fieldName)
        {
            return groupBy.ThenBy(new CamlFieldRef {Name = fieldName, Ascending = false});
        }

        public static CamlGroupBy ThenBy(this CamlGroupBy groupBy, CamlFieldRef fieldRef)
        {
            if (groupBy == null) throw new ArgumentNullException(nameof(groupBy));
            var fields = new List<CamlFieldRef>(groupBy.FieldRefs) {fieldRef};
            return new CamlGroupBy(fields, groupBy.Collapse);
        }
    }
}