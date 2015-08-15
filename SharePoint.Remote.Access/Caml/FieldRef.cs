using System;
using System.Xml.Linq;

namespace SharePoint.Remote.Access.Caml
{
    public sealed class FieldRef : QueryElement
    {
        internal const string FieldRefTag = "FieldRef";

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
            var name = existingFieldRef.Attribute("Name");

            if (name != null)
            {
                Name = name.Value;
            }

            var id = existingFieldRef.Attribute("ID");

            var guidString = id?.Value.Trim();
            if (guidString?.Length > 0)
            {
                FieldId = new Guid(guidString);
            }
            var ascending = existingFieldRef.Attribute("Ascending");
            if (ascending != null)
            {
                Ascending = Convert.ToBoolean(ascending.Value);
            }
            var nullable = existingFieldRef.Attribute("Nullable");
            if (nullable != null)
            {
                Nullable = Convert.ToBoolean(nullable.Value);
            }
            var lookupId = existingFieldRef.Attribute("LookupId");

            if (lookupId != null)
            {
                LookupId = Convert.ToBoolean(lookupId.Value);
            }
        }

        public override XElement ToXElement()
        {
            var el = new XElement("FieldRef");

            if (FieldId != Guid.Empty)
            {
                el.Add(new XAttribute("ID", FieldId));
            }
            else if (!string.IsNullOrEmpty(Name))
            {
                el.Add(new XAttribute("Name", Name));
            }
            if (Ascending.HasValue)
            {
                el.Add(new XAttribute("Ascending", Ascending.Value));
            }
            if (Nullable.HasValue)
            {
                el.Add(new XAttribute("Nullable", Nullable.Value));
            }
            if (LookupId.HasValue)
            {
                el.Add(new XAttribute("LookupId", LookupId.Value));
            }
            return el;
        }

        public override string ToString()
        {
            return ToXElement().ToString();
        }
    }
}