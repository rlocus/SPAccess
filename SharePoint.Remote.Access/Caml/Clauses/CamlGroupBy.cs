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

        public IEnumerable<CamlFieldRef> FieldRefs { get; private set; }

        public bool? Collapse { get; private set; }

        public CamlGroupBy(IEnumerable<CamlFieldRef> fieldRefs, bool? collapse = null)
            : base(GroupByTag)
        {
            FieldRefs = fieldRefs;
            Collapse = collapse;
        }

        //public GroupBy(Guid fieldId, bool? collapse = null)
        //    : base(GroupByTag)
        //{
        //    FieldRefs = new[] { new FieldRef { FieldId = fieldId } };
        //    Collapse = collapse;
        //}

        //public GroupBy(string fieldName, bool? collapse = null)
        //    : base(GroupByTag)
        //{
        //    FieldRefs = (new[] { new FieldRef { Name = fieldName } }).AsEnumerable();
        //    Collapse = collapse;
        //}

        public CamlGroupBy(CamlFieldRef field, bool? collapse = null)
        : base(GroupByTag)
        {
            FieldRefs = new[] { field }.AsEnumerable();
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
    }
}