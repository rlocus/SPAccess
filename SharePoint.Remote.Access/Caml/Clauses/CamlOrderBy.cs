using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Caml.Interfaces;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml.Clauses
{
    public sealed class CamlOrderBy : CamlClause, ICamlMultiField
    {
        internal const string OrderByTag = "OrderBy";

        public IEnumerable<CamlFieldRef> FieldRefs { get; private set; }

        public CamlOrderBy(IEnumerable<CamlFieldRef> fieldRefs)
            : base(OrderByTag)
        {
            FieldRefs = fieldRefs;
        }

        public CamlOrderBy(string existingGroupBy)
            : base(OrderByTag, existingGroupBy)
        {
        }

        public CamlOrderBy(XElement existingOrderBy)
            : base(OrderByTag, existingOrderBy)
        {
        }

        //public OrderBy(Guid fieldId, bool? ascending = null)
        //    : base(OrderByTag)
        //{
        //    FieldRefs = (new[] { new FieldRef { FieldId = fieldId, Ascending = ascending } }).AsEnumerable();
        //}

        //public OrderBy(string fieldName, bool? ascending = null)
        //    : base(OrderByTag)
        //{
        //    FieldRefs = (new[] { new FieldRef { Name = fieldName, Ascending = ascending } }).AsEnumerable();
        //}

        public CamlOrderBy(CamlFieldRef field)
        : base(OrderByTag)
        {
            FieldRefs = new[] { field }.AsEnumerable();
        }

        protected override void OnParsing(XElement existingOrderBy)
        {
            var existingFieldRefs = existingOrderBy.ElementsIgnoreCase(CamlFieldRef.FieldRefTag);
            FieldRefs = existingFieldRefs.Select(existingFieldRef => new CamlFieldRef(existingFieldRef));
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            if (FieldRefs != null)
            {
                foreach (var fieldRef in FieldRefs.Where(fieldRef => fieldRef != null))
                {
                    el.Add(fieldRef.ToXElement());
                }
            }
            return el;
        }

        public static CamlOrderBy Combine(CamlOrderBy firstOrderBy, CamlOrderBy secondOrderBy)
        {
            CamlOrderBy orderBy = null;
            var fieldRefs = new List<CamlFieldRef>();
            if (firstOrderBy?.FieldRefs != null)
            {
                fieldRefs.AddRange(firstOrderBy.FieldRefs);
            }
            if (secondOrderBy?.FieldRefs != null)
            {
                fieldRefs.AddRange(secondOrderBy.FieldRefs);
            }
            if (fieldRefs.Count > 0)
            {
                orderBy = new CamlOrderBy(fieldRefs);
            }
            return orderBy;
        }

        public static CamlOrderBy operator +(CamlOrderBy firstOrderBy, CamlOrderBy secondOrderBy)
        {
            return Combine(firstOrderBy, secondOrderBy);
        }
    }
}