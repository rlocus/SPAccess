using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SharePoint.Remote.Access.Caml.Clauses
{
    public sealed class GroupBy : Clause
    {
        internal const string GroupByTag = "GroupBy";

        public GroupBy(IEnumerable<FieldRef> fieldRefs, bool collapse)
            : base(GroupByTag)
        {
            FieldRefs = fieldRefs;
            Collapse = collapse;
        }

        public GroupBy(Guid fieldId, bool collapse)
            : base(GroupByTag)
        {
            FieldRefs = new[] {new FieldRef {FieldId = fieldId}};
            Collapse = collapse;
        }

        public GroupBy(string fieldName, bool collapse)
            : base(GroupByTag)
        {
            FieldRefs = (new[] {new FieldRef {Name = fieldName}}).AsEnumerable();
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

        public bool Collapse { get; set; }
        public IEnumerable<FieldRef> FieldRefs { get; set; }

        protected override void OnParsing(XElement existingGroupBy)
        {
            var existingFieldRefs =
                existingGroupBy.Elements()
                    .Where(
                        el => string.Equals(el.Name.LocalName, "FieldRef", StringComparison.InvariantCultureIgnoreCase));
            FieldRefs = existingFieldRefs.Select(existingFieldRef => new FieldRef(existingFieldRef));
            var collaps = existingGroupBy.Attribute("Collapse");
            if (collaps != null)
            {
                Collapse = Convert.ToBoolean(collaps.Value);
            }
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            el.Add(new XAttribute("Collapse", Collapse));
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
            var collaps = false;
            var fieldRefs = new List<FieldRef>();
            if (firstGroupBy?.FieldRefs != null)
            {
                collaps = firstGroupBy.Collapse;
                fieldRefs.AddRange(firstGroupBy.FieldRefs);
            }
            if (secondGroupBy?.FieldRefs != null)
            {
                collaps = collaps | secondGroupBy.Collapse;
                fieldRefs.AddRange(secondGroupBy.FieldRefs);
            }
            if (fieldRefs.Count > 0)
            {
                groupBy = new GroupBy(fieldRefs, collaps);
            }
            return groupBy;
        }
    }
}