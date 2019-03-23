using SharePoint.Remote.Access.Extensions;
using System;
using System.Xml.Linq;

namespace SharePoint.Remote.Access.Caml
{
    public sealed class CamlRowLimit : CamlElement
    {
        internal const string RowLimitTag = "RowLimit";
        internal const string PagedAttr = "Paged";

        public CamlRowLimit(uint limit = 0, bool? paged = null)
            : base(RowLimitTag)
        {
            Limit = limit;
            Paged = paged;
        }

        public CamlRowLimit(string existingFieldRef)
            : base(RowLimitTag, existingFieldRef)
        {
        }

        public CamlRowLimit(XElement existingFieldRef)
            : base(RowLimitTag, existingFieldRef)
        {
        }

        public bool? Paged { get; set; }

        public uint Limit { get; set; }

        protected override void OnParsing(XElement existingRowLimit)
        {
            var paged = existingRowLimit.AttributeIgnoreCase(PagedAttr);
            if (paged != null)
            {
                Paged = Convert.ToBoolean(paged.Value);
            }
            Limit = Convert.ToUInt32(existingRowLimit.Value);
        }

        public override XElement ToXElement()
        {
            var el = new XElement(RowLimitTag);
            if (Paged.HasValue)
            {
                el.Add(new XAttribute(PagedAttr, Paged.Value));
            }
            el.Add(Limit);
            return el;
        }

        public override string ToString()
        {
            return ToXElement().ToString();
        }
    }
}