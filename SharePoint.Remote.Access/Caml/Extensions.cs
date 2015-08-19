using System;
using System.Collections.Generic;
using SharePoint.Remote.Access.Caml.Clauses;

namespace SharePoint.Remote.Access.Caml
{
    public static class Extensions
    {
        public static CamlWhere CombineAnd(this CamlWhere where, CamlWhere combinedWhere)
        {
            if (where == null) throw new ArgumentNullException(nameof(where));
            if (combinedWhere == null) throw new ArgumentNullException(nameof(combinedWhere));
            where = new CamlWhere(where.Operator);
            where.And(combinedWhere.Operator);
            return where;
        }

        public static CamlWhere CombineOr(this CamlWhere where, CamlWhere combinedWhere)
        {
            if (where == null) throw new ArgumentNullException(nameof(where));
            if (combinedWhere == null) throw new ArgumentNullException(nameof(combinedWhere));
            where = new CamlWhere(where.Operator);
            where.Or(combinedWhere.Operator);
            return where;
        }

        public static CamlOrderBy ThenBy(this CamlOrderBy orderBy, Guid fieldId, bool? ascending = null)
        {
            return orderBy.ThenBy(new CamlFieldRef { FieldId = fieldId, Ascending = @ascending });
        }

        public static CamlOrderBy ThenBy(this CamlOrderBy orderBy, string fieldName, bool? ascending = null)
        {
            return orderBy.ThenBy(new CamlFieldRef { Name = fieldName, Ascending = @ascending });
        }

        public static CamlOrderBy ThenBy(this CamlOrderBy orderBy, CamlFieldRef fieldRef)
        {
            var fields = new List<CamlFieldRef>(orderBy.FieldRefs) { fieldRef };
            return new CamlOrderBy(fields);
        }

        public static CamlGroupBy ThenBy(this CamlGroupBy groupBy, Guid fieldId)
        {
            return groupBy.ThenBy(new CamlFieldRef { FieldId = fieldId, Ascending = false });
        }

        public static CamlGroupBy ThenBy(this CamlGroupBy groupBy, string fieldName)
        {
            return groupBy.ThenBy(new CamlFieldRef { Name = fieldName, Ascending = false });
        }

        public static CamlGroupBy ThenBy(this CamlGroupBy groupBy, CamlFieldRef fieldRef)
        {
            var fields = new List<CamlFieldRef>(groupBy.FieldRefs) { fieldRef };
            return new CamlGroupBy(fields, groupBy.Collapse);
        }
    }
}