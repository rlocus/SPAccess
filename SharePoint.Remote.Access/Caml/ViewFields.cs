using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SharePoint.Remote.Access.Caml.Interfaces;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml
{
    public sealed class ViewFields : CamlElement, ICamlMultiField
    {
        internal const string ViewFieldsTag = "ViewFields";

        public ViewFields(IEnumerable<string> viewFields) : base(ViewFieldsTag)
        {
            if (viewFields != null) FieldRefs = viewFields.Select(viewField => new CamlFieldRef {Name = viewField});
        }

        public ViewFields(string existingView) : base(ViewFieldsTag, existingView)
        {
        }

        public ViewFields(XElement existingView) : base(ViewFieldsTag, existingView)
        {
        }

        public IEnumerable<CamlFieldRef> FieldRefs { get; private set; }

        protected override void OnParsing(XElement existingViewFields)
        {
            FieldRefs = existingViewFields.ElementsIgnoreCase(CamlFieldRef.FieldRefTag)
                .Select(existingFieldRef => new CamlFieldRef(existingFieldRef));
        }

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

        public string ToString(bool excludeViewFieldsTag, bool disableFormatting)
        {
            var caml = ToXElement();
            if (excludeViewFieldsTag)
            {
                var sb = new StringBuilder();
                foreach (var element in caml.Elements())
                {
                    if (disableFormatting)
                    {
                        sb.Append(element.ToString(SaveOptions.DisableFormatting));
                    }
                    else
                    {
                        sb.AppendLine(element.ToString(SaveOptions.None));
                    }
                }
                return sb.ToString();
            }
            return ToString(disableFormatting);
        }
    }
}