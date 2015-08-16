using System;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Caml.Interfaces;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class FieldOperator : Operator, IFieldOperator
    {
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

        public FieldRef FieldRef { get; set; }

        protected override void OnParsing(XElement existingSingleFieldValueOperator)
        {
            var existingFieldRef =
                existingSingleFieldValueOperator.Elements()
                    .SingleOrDefault(
                        el =>
                            string.Equals(el.Name.LocalName, FieldRef.FieldRefTag,
                                StringComparison.InvariantCultureIgnoreCase));

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