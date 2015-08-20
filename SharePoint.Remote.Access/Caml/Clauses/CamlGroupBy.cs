using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Caml.Interfaces;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml.Clauses
{
    public sealed class CamlGroupBy : CamlClause, ICamlMultiField
    {
        internal const string GroupByTag = "GroupBy";
        internal const string CollapseAttr = "Collapse";

        public CamlGroupBy(CamlFieldRef fieldRef, bool? collapse = null)
            : this(new[] { fieldRef }, collapse)
        {
        }

        public CamlGroupBy(IEnumerable<CamlFieldRef> fieldRefs, bool? collapse = null)
            : base(GroupByTag)
        {
            if (fieldRefs == null) throw new ArgumentNullException(nameof(fieldRefs));
            FieldRefs = fieldRefs;
            Collapse = collapse;
        }

        public CamlGroupBy(IEnumerable<string> fieldNames, bool? collapse = null)
          : base(GroupByTag)
        {
            if (fieldNames == null) throw new ArgumentNullException(nameof(fieldNames));
            FieldRefs = fieldNames.Select(fieldName => new CamlFieldRef { Name = fieldName });
            Collapse = collapse;
        }

        public CamlGroupBy(IEnumerable<Guid> fieldIds, bool? collapse = null)
         : base(GroupByTag)
        {
            if (fieldIds == null) throw new ArgumentNullException(nameof(fieldIds));
            FieldRefs = fieldIds.Select(fieldId => new CamlFieldRef { Id = fieldId });
            Collapse = collapse;
        }

        public CamlGroupBy(string existingGroupBy)
            : base(GroupByTag, existingGroupBy)
        {
        }

        public CamlGroupBy(XElement existingGroupBy)
            : base(GroupByTag, existingGroupBy)
        {
        }

        public bool? Collapse { get; private set; }
        public IEnumerable<CamlFieldRef> FieldRefs { get; private set; }

        protected override void OnParsing(XElement existingGroupBy)
        {
            var existingFieldRefs = existingGroupBy.ElementsIgnoreCase(CamlFieldRef.FieldRefTag);
            FieldRefs = existingFieldRefs.Select(existingFieldRef => new CamlFieldRef(existingFieldRef));
            var collaps = existingGroupBy.Attribute(CollapseAttr);
            if (collaps != null)
            {
                Collapse = Convert.ToBoolean(collaps.Value);
            }
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            if (Collapse != null) el.Add(new XAttribute(CollapseAttr, Collapse));
            if (FieldRefs != null)
            {
                foreach (var fieldRef in FieldRefs.Where(fieldRef => fieldRef != null))
                {
                    el.Add(fieldRef.ToXElement());
                }
            }
            return el;
        }

        public static CamlGroupBy Combine(CamlGroupBy firstGroupBy, CamlGroupBy secondGroupBy)
        {
            CamlGroupBy groupBy = null;
            var collapse = false;
            var fieldRefs = new List<CamlFieldRef>();
            if (firstGroupBy?.FieldRefs != null)
            {
                if (firstGroupBy.Collapse != null) collapse = firstGroupBy.Collapse.Value;
                fieldRefs.AddRange(firstGroupBy.FieldRefs);
            }
            if (secondGroupBy?.FieldRefs != null)
            {
                if (secondGroupBy.Collapse != null)
                {
                    collapse = collapse | secondGroupBy.Collapse.Value;
                }
                fieldRefs.AddRange(secondGroupBy.FieldRefs);
            }
            if (fieldRefs.Count > 0)
            {
                groupBy = new CamlGroupBy(fieldRefs, collapse);
            }
            return groupBy;
        }

        public static CamlGroupBy operator +(CamlGroupBy firstGroupBy, CamlGroupBy secondGroupBy)
        {
            return Combine(firstGroupBy, secondGroupBy);
        }
    }
}