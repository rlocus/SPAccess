using System;
using System.Collections.Generic;
using SharePoint.Remote.Access.Caml.Clauses;
using SharePoint.Remote.Access.Caml.Interfaces;
using SharePoint.Remote.Access.Caml.Operators;

namespace SharePoint.Remote.Access.Caml
{
    public static class Extensions
    {
        public static Where CombineAnd<T>(this Where where, T op)
            where T : Operator, IFieldOperator, IMultiFieldOperator
        {
            if (where != null)
            {
                where.And(op);
            }
            else
            {
                where = new Where(op);
            }

            return where;
        }

        public static Where CombineOr<T>(this Where where, T op)
            where T : Operator, IFieldOperator, IMultiFieldOperator
        {
            if (where != null)
            {
                where.Or(op);
            }
            else
            {
                where = new Where(op);
            }

            return where;
        }
        
        public static OrderBy ThenBy(this OrderBy orderBy, Guid fieldId, bool? ascending = null)
        {
            return orderBy.ThenBy(new FieldRef { FieldId = fieldId, Ascending = @ascending });
        }

        public static OrderBy ThenBy(this OrderBy orderBy, string fieldName, bool? ascending = null)
        {
            return orderBy.ThenBy(new FieldRef { Name = fieldName, Ascending = @ascending });
        }

        public static OrderBy ThenBy(this OrderBy orderBy, FieldRef fieldRef)
        {
            var fields = new List<FieldRef>(orderBy.FieldRefs) { fieldRef };
            return new OrderBy(fields);
        }

        public static GroupBy ThenBy(this GroupBy groupBy, Guid fieldId)
        {
            return groupBy.ThenBy(new FieldRef { FieldId = fieldId, Ascending = false });
        }

        public static GroupBy ThenBy(this GroupBy groupBy, string fieldName)
        {
            return groupBy.ThenBy(new FieldRef { Name = fieldName, Ascending = false });
        }

        public static GroupBy ThenBy(this GroupBy groupBy, FieldRef fieldRef)
        {
            var fields = new List<FieldRef>(groupBy.FieldRefs) { fieldRef };
            return new GroupBy(fields, groupBy.Collapse);
        }
    }
}