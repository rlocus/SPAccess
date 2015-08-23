using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Caml.Interfaces;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml
{
    public sealed class ViewFieldsCamlElement : CamlElement, ICamlMultiField
    {
        internal const string ViewFieldsTag = "ViewFields";

        public ViewFieldsCamlElement(IEnumerable<string> viewFields) : base(ViewFieldsTag)
        {
            if (viewFields != null) FieldRefs = viewFields.Select(viewField => new CamlFieldRef {Name = viewField});
        }

        public ViewFieldsCamlElement(string existingViewFields) : base(ViewFieldsTag, existingViewFields)
        {
        }

        public ViewFieldsCamlElement(XElement existingViewFields) : base(ViewFieldsTag, existingViewFields)
        {
        }

        public IEnumerable<CamlFieldRef> FieldRefs { get; private set; }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            if (FieldRefs != null)
            {
                foreach (var fieldRef in FieldRefs)
                {
                    el.Add(fieldRef.ToXElement());
                }
            }
            return el;
        }

        protected override void OnParsing(XElement existingViewFields)
        {
            FieldRefs = existingViewFields.ElementsIgnoreCase(CamlFieldRef.FieldRefTag)
                .Select(existingFieldRef => new CamlFieldRef(existingFieldRef));
        }
    }
}