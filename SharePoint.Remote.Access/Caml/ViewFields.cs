using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Caml.Interfaces;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml
{
    public sealed class ViewFields : CamlElement, ICamlMultiField
    {
        internal const string ViewFieldsTag = "ViewFields";

        public IEnumerable<CamlFieldRef> FieldRefs { get; private set; }

        public ViewFields(params string[] viewFields) : base(ViewFieldsTag)
        {
            FieldRefs = viewFields.Select(viewField => new CamlFieldRef { Name = viewField });
        }

        public ViewFields(string existingView) : base(ViewFieldsTag, existingView)
        {
        }

        public ViewFields(XElement existingView) : base(ViewFieldsTag, existingView)
        {
        }

        protected override void OnParsing(XElement existingViewFields)
        {
            FieldRefs = existingViewFields.ElementsIgnoreCase(CamlFieldRef.FieldRefTag)
                    .Select(existingFieldRef => new CamlFieldRef(existingFieldRef));
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            foreach (CamlFieldRef fieldRef in FieldRefs)
            {
                el.Add(fieldRef.ToXElement());
            }
            return el;
        }
    }
}