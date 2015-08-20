using System;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Caml.Interfaces;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class FieldOperator : Operator, ICamlField
    {
        public CamlFieldRef FieldRef { get; private set; }

        protected FieldOperator(string operatorName, CamlFieldRef fieldRef)
            : base(operatorName)
        {
            if (fieldRef == null) throw new ArgumentNullException(nameof(fieldRef));
            FieldRef = fieldRef;
        }

        protected FieldOperator(string operatorName, string existingFieldOperator)
            : base(operatorName, existingFieldOperator)
        {
        }

        protected FieldOperator(string operatorName, XElement existingFieldOperator)
            : base(operatorName, existingFieldOperator)
        {
        }

        protected override void OnParsing(XElement existingFieldValueOperator)
        {
            XElement existingFieldRef = existingFieldValueOperator.ElementIgnoreCase(CamlFieldRef.FieldRefTag);
            if (existingFieldRef != null)
            {
                FieldRef = new CamlFieldRef(existingFieldRef);
            }
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            el.AddFirst(FieldRef.ToXElement());
            return el;
        }
    }
}