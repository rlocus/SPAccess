using System;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Caml.Interfaces;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class FieldOperator : Operator, IFieldOperator
    {
        public FieldRef FieldRef { get; private set; }

        protected FieldOperator(string operatorName, FieldRef fieldRef)
            : base(operatorName)
        {
            if (fieldRef == null) throw new ArgumentNullException(nameof(fieldRef));
            FieldRef = fieldRef;
        }

        protected FieldOperator(string operatorName, string existingSingleFieldOperator)
            : base(operatorName, existingSingleFieldOperator)
        {
        }

        protected FieldOperator(string operatorName, XElement existingSingleFieldOperator)
            : base(operatorName, existingSingleFieldOperator)
        {
        }

        protected override void OnParsing(XElement existingSingleFieldValueOperator)
        {
            XElement existingFieldRef =
                existingSingleFieldValueOperator.Elements(FieldRef.FieldRefTag).SingleOrDefault();
            if (existingFieldRef != null)
            {
                FieldRef = new FieldRef(existingFieldRef);
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