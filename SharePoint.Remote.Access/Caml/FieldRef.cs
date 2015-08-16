using System;
using System.Xml.Linq;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml
{
    public sealed class FieldRef : QueryElement
    {
        internal const string FieldRefTag = "FieldRef";
        internal const string NameAttr = "Name";
        internal const string IDAttr = "ID";
        internal const string AscendingAttr = "Ascending";
        internal const string NullableAttr = "Nullable";
        internal const string LookupIdAttr = "LookupId";

        public FieldRef()
            : base(FieldRefTag)
        {
        }

        public FieldRef(string existingFieldRef)
            : base(FieldRefTag, existingFieldRef)
        {
        }

        public FieldRef(XElement existingFieldRef)
            : base(FieldRefTag, existingFieldRef)
        {
        }

        public Guid FieldId { get; set; }
        public string Name { get; set; }
        public bool? Ascending { get; set; }
        public bool? Nullable { get; set; }
        public bool? LookupId { get; set; }

        protected override void OnParsing(XElement existingFieldRef)
        {
            var name = existingFieldRef.AttributeIgnoreCase(NameAttr);

            if (name != null)
            {
                Name = name.Value;
            }

            var id = existingFieldRef.AttributeIgnoreCase(IDAttr);

            var guidString = id?.Value.Trim();
            if (guidString?.Length > 0)
            {
                FieldId = new Guid(guidString);
            }
            var ascending = existingFieldRef.AttributeIgnoreCase(AscendingAttr);
            if (ascending != null)
            {
                Ascending = Convert.ToBoolean(ascending.Value);
            }
            var nullable = existingFieldRef.AttributeIgnoreCase(NullableAttr);
            if (nullable != null)
            {
                Nullable = Convert.ToBoolean(nullable.Value);
            }
            var lookupId = existingFieldRef.AttributeIgnoreCase(LookupIdAttr);

            if (lookupId != null)
            {
                LookupId = Convert.ToBoolean(lookupId.Value);
            }
        }

        public override XElement ToXElement()
        {
            var el = new XElement(FieldRefTag);

            if (FieldId != Guid.Empty)
            {
                el.Add(new XAttribute(IDAttr, FieldId));
            }
            else if (!string.IsNullOrEmpty(Name))
            {
                el.Add(new XAttribute(NameAttr, Name));
            }
            if (Ascending.HasValue)
            {
                el.Add(new XAttribute(AscendingAttr, Ascending.Value));
            }
            if (Nullable.HasValue)
            {
                el.Add(new XAttribute(NullableAttr, Nullable.Value));
            }
            if (LookupId.HasValue)
            {
                el.Add(new XAttribute(LookupIdAttr, LookupId.Value));
            }
            return el;
        }

        public override string ToString()
        {
            return ToXElement().ToString();
        }
    }
}