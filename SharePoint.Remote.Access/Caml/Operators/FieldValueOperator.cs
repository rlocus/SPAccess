using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Caml.Interfaces;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class FieldValueOperator<T> : ValueOperator<T>, ICamlField
    {
        protected FieldValueOperator(string operatorName, CamlFieldRef fieldRef, CamlValue<T> value)
            : base(operatorName, value)
        {
            FieldRef = fieldRef;
        }

        protected FieldValueOperator(string operatorName, CamlFieldRef fieldRef, T value, FieldType type)
            : base(operatorName, value, type)
        {
            FieldRef = fieldRef;
        }

        protected FieldValueOperator(string operatorName, Guid fieldId, CamlValue<T> value)
            : base(operatorName, value)
        {
            FieldRef = new CamlFieldRef {Id = fieldId};
        }

        protected FieldValueOperator(string operatorName, Guid fieldId, T value, FieldType type)
            : base(operatorName, value, type)
        {
            FieldRef = new CamlFieldRef {Id = fieldId};
        }

        protected FieldValueOperator(string operatorName, string fieldName, CamlValue<T> value)
            : base(operatorName, value)
        {
            FieldRef = new CamlFieldRef {Name = fieldName};
        }

        protected FieldValueOperator(string operatorName, string fieldName, T value, FieldType type)
            : base(operatorName, value, type)
        {
            FieldRef = new CamlFieldRef {Name = fieldName};
        }

        protected FieldValueOperator(string operatorName, string existingSingleFieldValueOperator)
            : base(operatorName, existingSingleFieldValueOperator)
        {
        }

        protected FieldValueOperator(string operatorName, XElement existingSingleFieldValueOperator)
            : base(operatorName, existingSingleFieldValueOperator)
        {
        }

        public CamlFieldRef FieldRef { get; private set; }

        protected override void OnParsing(XElement existingSingleFieldValueOperator)
        {
            var existingValue =
                existingSingleFieldValueOperator.Elements()
                    .SingleOrDefault(
                        el => string.Equals(el.Name.LocalName, CamlValue.ValueTag, StringComparison.OrdinalIgnoreCase));
            if (existingValue != null)
            {
                base.OnParsing(existingValue);
            }
            var existingFieldRef =
                existingSingleFieldValueOperator.Elements(CamlFieldRef.FieldRefTag).SingleOrDefault();
            if (existingFieldRef != null)
            {
                FieldRef = new CamlFieldRef(existingFieldRef);
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