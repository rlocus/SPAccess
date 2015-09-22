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
            query.Where = new CamlWhere(query.Where.And(operators).Operator);
        }

        public static void WhereAny(this Query query, params Operator[] operators)
        {
            if (operators.Length == 0) return;
            if (query.Where == null)
            {
                query.Where = new CamlWhere(operators.First());
                operators = operators.Skip(1).ToArray();
            }
            query.Where = new CamlWhere(query.Where.Or(operators).Operator);
        }

        public static CamlWhere And(this CamlWhere firstWhere, CamlWhere secondWhere)
        {
            if (firstWhere == null) throw new ArgumentNullException("firstWhere");
            if (secondWhere == null) throw new ArgumentNullException("secondWhere");
            var logicalJoin = firstWhere.Operator as LogicalJoin;
            var @where = logicalJoin != null
                ? new CamlWhere(logicalJoin.CombineAnd(secondWhere.Operator))
                : new CamlWhere(firstWhere.Operator.And(secondWhere.Operator));
            return where;
        }

        public static CamlWhere Or(this CamlWhere where, params Operator[] operators)
        {
            if (where == null) throw new ArgumentNullException("where");
            if (operators.Length > 0)
            {
                Operator @operator = operators.Where(op => op != null)
                    .Aggregate(@where.Operator, (current, op) => current.Or(op));
                where = new CamlWhere(@operator);
            }
            return where;
        }

        public static CamlWhere And(this CamlWhere where, params Operator[] operators)
        {
            if (where == null) throw new ArgumentNullException(nameof(@where));
            if (operators.Length > 0)
            {
                Operator @operator = operators.Where(op => op != null)
                    .Aggregate(@where.Operator, (current, op) => current.And(op));
                where = new CamlWhere(@operator);
            }
            return where;
        }

        public static CamlWhere Or(this CamlWhere firstWhere, CamlWhere secondWhere)
        {
            if (firstWhere == null) throw new ArgumentNullException(nameof(firstWhere));
            if (secondWhere == null) throw new ArgumentNullException(nameof(secondWhere));
            var logicalJoin = firstWhere.Operator as LogicalJoin;
            var @where = logicalJoin != null
                ? new CamlWhere(logicalJoin.CombineOr(secondWhere.Operator))
                : new CamlWhere(firstWhere.Operator.Or(secondWhere.Operator));
            return where;
        }

        public static Operator And(this Operator @operator, params Operator[] operators)
        {
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            var logicalJoin = @operator as LogicalJoin;
            return logicalJoin != null
                ? logicalJoin.CombineAnd(operators)
                : new And(new List<Operator> { @operator }.Union(operators));
        }

        public static Operator And(this LogicalJoin @operator, params Operator[] operators)
        {
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            return @operator.CombineAnd(operators);
        }

        public static Operator And(this ComparisonOperator @operator, params Operator[] operators)
        {
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            return new And(new List<Operator> { @operator }.Union(operators));
        }

        public static Operator Or(this Operator @operator, params Operator[] operators)
        {
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            var logicalJoin = @operator as LogicalJoin;
            return logicalJoin != null
                ? logicalJoin.CombineOr(operators)
                : new Or(new List<Operator> { @operator }.Union(operators));
        }

        public static Operator Or(this LogicalJoin @operator, params Operator[] operators)
        {
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            return @operator.CombineOr(operators);
        }

        public static Operator Or(this ComparisonOperator @operator, params Operator[] operators)
        {
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            return new Or(new List<Operator> { @operator }.Union(operators));
        }

        public static CamlOrderBy ThenBy(this CamlOrderBy orderBy, Guid fieldId, bool? ascending = null)
        {
            return orderBy.ThenBy(new CamlFieldRef { Id = fieldId, Ascending = @ascending });
        }

        public static CamlOrderBy ThenBy(this CamlOrderBy orderBy, string fieldName, bool? ascending = null)
        {
            return orderBy.ThenBy(new CamlFieldRef { Name = fieldName, Ascending = @ascending });
        }

        public static CamlOrderBy ThenBy(this CamlOrderBy orderBy, CamlFieldRef fieldRef)
        {
            if (orderBy == null)
            {
                orderBy = new CamlOrderBy(fieldRef);
            }
            var fields = new List<CamlFieldRef>(orderBy.FieldRefs) { fieldRef };
            return new CamlOrderBy(fields);
        }

        public static CamlGroupBy ThenBy(this CamlGroupBy groupBy, Guid fieldId, bool? collapse = null, int? limit = null)
        {
            return groupBy.ThenBy(new CamlFieldRef { Id = fieldId, Ascending = false }, collapse, limit);
        }

        public static CamlGroupBy BeforeBy(this CamlGroupBy groupBy, Guid fieldId, bool? collapse = null, int? limit = null)
        {
            return groupBy.BeforeBy(new CamlFieldRef { Id = fieldId, Ascending = false }, collapse, limit);
        }

        public static CamlGroupBy ThenBy(this CamlGroupBy groupBy, string fieldName, bool? collapse = null, int? limit = null)
        {
            return groupBy.ThenBy(new CamlFieldRef { Name = fieldName, Ascending = false }, collapse, limit);
        }

        public static CamlGroupBy BeforeBy(this CamlGroupBy groupBy, string fieldName, bool? collapse = null, int? limit = null)
        {
            return groupBy.BeforeBy(new CamlFieldRef { Name = fieldName, Ascending = false }, collapse, limit);
        }

        public static CamlGroupBy ThenBy(this CamlGroupBy groupBy, CamlFieldRef fieldRef, bool? collapse = null, int? limit = null)
        {
            if (groupBy == null)
            {
                return new CamlGroupBy(fieldRef, collapse, limit);
            }
            if (groupBy.Collapse != null)
            {
                collapse = collapse == null ? groupBy.Collapse.Value : collapse.Value | groupBy.Collapse.Value;
            }
            if (groupBy.Limit != null)
            {
                limit = limit == null ? groupBy.Limit.Value : Math.Max(limit.Value, groupBy.Limit.Value);
            }
            var fields = new List<CamlFieldRef>(groupBy.FieldRefs) { fieldRef };
            return new CamlGroupBy(fields, collapse, limit);
        }

        public static CamlGroupBy BeforeBy(this CamlGroupBy groupBy, CamlFieldRef fieldRef, bool? collapse = null, int? limit = null)
        {
            if (groupBy == null)
            {
                return new CamlGroupBy(fieldRef, collapse, limit);
            }
            if (groupBy.Collapse != null)
            {
                collapse = collapse == null ? groupBy.Collapse.Value : collapse.Value | groupBy.Collapse.Value;
            }
            if (groupBy.Limit != null)
            {
                limit = limit == null ? groupBy.Limit.Value : Math.Max(limit.Value, groupBy.Limit.Value);
            }
            var fields = new List<CamlFieldRef> { fieldRef };
            fields.AddRange(groupBy.FieldRefs);
            return new CamlGroupBy(fields, collapse, limit);
        }

        public static JoinsCamlElement Join(this JoinsCamlElement camlJoins, params Join[] joins)
        {
            if (@joins != null)
            {
                var mergedJoins = new List<Join>();
                if (camlJoins.Joins != null)
                {
                    mergedJoins.AddRange(camlJoins.Joins);
                }
                mergedJoins.AddRange(joins);
                camlJoins.Joins = mergedJoins.ToArray();
            }
            return camlJoins;
        }

        public static ViewFieldsCamlElement View(this ViewFieldsCamlElement camlViewFields, params CamlFieldRef[] viewFields)
        {
            if (@viewFields != null)
            {
                var mergedViewFields = new List<CamlFieldRef>();
                if (camlViewFields.FieldRefs != null)
                {
                    mergedViewFields.AddRange(camlViewFields.FieldRefs);
                }
                mergedViewFields.AddRange(viewFields);
                camlViewFields.FieldRefs = mergedViewFields.ToArray();
            }
            return camlViewFields;
        }

        public static ViewFieldsCamlElement View(this ViewFieldsCamlElement camlViewFields, params string[] viewFields)
        {
            if (@viewFields != null)
            {
                var mergedViewFields = new List<CamlFieldRef>();
                if (camlViewFields.FieldRefs != null)
                {
                    mergedViewFields.AddRange(camlViewFields.FieldRefs);
                }
                mergedViewFields.AddRange(viewFields.Select(viewField => new CamlFieldRef { Name = viewField }));
                camlViewFields.FieldRefs = mergedViewFields.ToArray();
            }
            return camlViewFields;
        }

        public static ProjectedFieldsCamlElement ShowField(this ProjectedFieldsCamlElement camlProjectedFields, params CamlProjectedField[] projectedFields)
        {
            if (projectedFields != null)
            {
                var mergedFields = new List<CamlProjectedField>();
                if (camlProjectedFields.ProjectedFields != null)
                {
                    mergedFields.AddRange(camlProjectedFields.ProjectedFields);
                }
                mergedFields.AddRange(projectedFields);
                camlProjectedFields.ProjectedFields = mergedFields.ToArray();
            }
            return camlProjectedFields;
        }

        public static ProjectedFieldsCamlElement ShowField(this ProjectedFieldsCamlElement camlProjectedFields, string fieldName, string listAlias, string lookupField)
        {
            var mergedFields = new List<CamlProjectedField>();
            if (camlProjectedFields.ProjectedFields != null)
            {
                mergedFields.AddRange(camlProjectedFields.ProjectedFields);
            }
            mergedFields.Add(new CamlProjectedField(fieldName, listAlias, lookupField));
            camlProjectedFields.ProjectedFields = mergedFields.ToArray();

            return camlProjectedFields;
        }
    }
}