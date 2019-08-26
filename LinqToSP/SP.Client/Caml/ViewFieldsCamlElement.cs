using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;
using SP.Client.Caml.Interfaces;
using SP.Client.Extensions;

namespace SP.Client.Caml
{
    public sealed class ViewFieldsCamlElement : CamlElement, ICamlMultiField
    {
        internal const string ViewFieldsTag = "ViewFields";

        public ViewFieldsCamlElement(IEnumerable<string> viewFields) : base(ViewFieldsTag)
        {
            if (viewFields != null)
            {
                FieldRefs = viewFields.Select(viewField => new CamlFieldRef { Name = viewField });
            }
        }

        public ViewFieldsCamlElement(string existingViewFields) : base(ViewFieldsTag, existingViewFields)
        {
        }

        public ViewFieldsCamlElement(XElement existingViewFields) : base(ViewFieldsTag, existingViewFields)
        {
        }

        public void AddViewField([NotNull] string fieldName)
        {
            if (FieldRefs == null)
            {
                FieldRefs = new[] { fieldName }.Select(viewField => new CamlFieldRef { Name = viewField });
            }
            else
            {
                if (!FieldRefs.Any(vf => vf.Name == fieldName))
                {
                    FieldRefs = FieldRefs.Concat(new[] { fieldName }.Select(viewField => new CamlFieldRef { Name = viewField }));
                }
            }
        }

        public void AddViewFields([NotNull] IEnumerable<string> fieldNames)
        {
            foreach (string fieldName in fieldNames)
            {
                AddViewField(fieldName);
            }
        }

        public IEnumerable<CamlFieldRef> FieldRefs { get; internal set; }

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