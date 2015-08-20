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

        public CamlOrderBy(CamlFieldRef fieldRef)
            : this(new[] { fieldRef })
        {
        }

        public CamlOrderBy(IEnumerable<CamlFieldRef> fieldRefs)
            : base(OrderByTag)
        {
            if (fieldRefs == null) throw new ArgumentNullException(nameof(fieldRefs));
            FieldRefs = fieldRefs;
        }

        public CamlOrderBy(IEnumerable<string> fieldNames)
          : base(OrderByTag)
        {
            if (fieldNames == null) throw new ArgumentNullException(nameof(fieldNames));
            FieldRefs = fieldNames.Select(fieldName => new CamlFieldRef { Name = fieldName });
        }

        public CamlOrderBy(IEnumerable<Guid> fieldIds)
         : base(OrderByTag)
        {
            if (fieldIds == null) throw new ArgumentNullException(nameof(fieldIds));
            FieldRefs = fieldIds.Select(fieldId => new CamlFieldRef { Id = fieldId });
        }

        public CamlOrderBy(string existingGroupBy)
            : base(OrderByTag, existingGroupBy)
        {
        }

        public CamlOrderBy(XElement existingOrderBy)
            : base(OrderByTag, existingOrderBy)
        {
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