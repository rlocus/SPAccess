using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Caml.Interfaces;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class FieldValueOperator<T> : ValueOperator<T>, IFieldOperator
    {
        protected FieldValueOperator(string operatorName, FieldRef fieldRef, Value<T> value)
            : base(operatorName, value)
        {
            FieldRef = fieldRef;
        }

        protected FieldValueOperator(string operatorName, FieldRef fieldRef, T value, FieldType type)
            : base(operatorName, value, type)
        {
            FieldRef = fieldRef;
        }

        protected FieldValueOperator(string operatorName, Guid fieldId, Value<T> value)
            : base(operatorName, value)
        {
            FieldRef = new FieldRef { FieldId = fieldId };
        }

        protected FieldValueOperator(string operatorName, Guid fieldId, T value, FieldType type)
            : base(operatorName, value, type)
        {
            FieldRef = new FieldRef { FieldId = fieldId };
        }

        protected FieldValueOperator(string operatorName, string fieldName, Value<T> value)
            : base(operatorName, value)
        {
            FieldRef = new FieldRef { Name = fieldName };
        }

        protected FieldValueOperator(string operatorName, string fieldName, T value, FieldType type)
            : base(operatorName, value, type)
        {
            FieldRef = new FieldRef { Name = fieldName };
        }

        protected FieldValueOperator(string operatorName, string existingSingleFieldValueOperator)
            : base(operatorName, existingSingleFieldValueOperator)
        {
        }

        protected FieldValueOperator(string operatorName, XElement existingSingleFieldValueOperator)
            : base(operatorName, existingSingleFieldValueOperator)
        {
        }

        public FieldRef FieldRef { get; set; }

        protected override void OnParsing(XElement existingSingleFieldValueOperator)
        {
            var existingFieldRef =
                existingSingleFieldValueOperator.Elements()
                    .SingleOrDefault(
                        el => string.Equals(el.Name.LocalName, FieldRef.FieldRefTag, StringComparison.InvariantCultureIgnoreCase));

            if (existingFieldRef != null)
            {
                FieldRef = new FieldRef(existingFieldRef);
            }

            var existingValue =
                existingSingleFieldValueOperator.Elements().SingleOrDefault(el => el.Name.LocalName == "Value");

            if (existingValue != null)
            {
                base.OnParsing(existingValue);
            }
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            if (FieldRef != null) el.AddFirst(FieldRef.ToXElement());
            return el;
        }
    }
}