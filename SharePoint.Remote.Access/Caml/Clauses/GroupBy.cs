using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Caml.Interfaces;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml.Clauses
{
    public sealed class GroupBy : Clause, IMultiFieldOperator
    {
        internal const string GroupByTag = "GroupBy";
        internal const string CollapseAttr = "Collapse";

        public IEnumerable<FieldRef> FieldRefs { get; private set; }

        public bool? Collapse { get; private set; }

        public GroupBy(IEnumerable<FieldRef> fieldRefs, bool? collapse = null)
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

        public GroupBy(FieldRef field, bool? collapse = null)
        : base(GroupByTag)
        {
            FieldRefs = new[] { field }.AsEnumerable();
            Collapse = collapse;
        }

        public GroupBy(string existingGroupBy)
            : base(GroupByTag, existingGroupBy)
        {
        }

        public GroupBy(XElement existingGroupBy)
            : base(GroupByTag, existingGroupBy)
        {
        }

        protected override void OnParsing(XElement existingGroupBy)
        {
            var existingFieldRefs = existingGroupBy.ElementsIgnoreCase(FieldRef.FieldRefTag);
            FieldRefs = existingFieldRefs.Select(existingFieldRef => new FieldRef(existingFieldRef));
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

        public static GroupBy Combine(GroupBy firstGroupBy, GroupBy secondGroupBy)
        {
            GroupBy groupBy = null;
            var collapse = false;
            var fieldRefs = new List<FieldRef>();
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
                groupBy = new GroupBy(fieldRefs, collapse);
            }
            return groupBy;
        }
    }
}