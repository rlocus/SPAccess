using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Caml.Interfaces;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class FieldValueOperator<T> : ValueOperator<T>, ICamlField
    {
        public CamlFieldRef FieldRef { get; private set; }

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
            FieldRef = new CamlFieldRef { Id = fieldId };
        }

        protected FieldValueOperator(string operatorName, Guid fieldId, T value, FieldType type)
            : base(operatorName, value, type)
        {
            FieldRef = new CamlFieldRef { Id = fieldId };
        }

        protected FieldValueOperator(string operatorName, string fieldName, CamlValue<T> value)
            : base(operatorName, value)
        {
            FieldRef = new CamlFieldRef { Name = fieldName };
        }

        protected FieldValueOperator(string operatorName, string fieldName, T value, FieldType type)
            : base(operatorName, value, type)
        {
            FieldRef = new CamlFieldRef { Name = fieldName };
        }

        protected FieldValueOperator(string operatorName, string existingSingleFieldValueOperator)
            : base(operatorName, existingSingleFieldValueOperator)
        {
        }

        protected FieldValueOperator(string operatorName, XElement existingSingleFieldValueOperator)
            : base(operatorName, existingSingleFieldValueOperator)
        {
        }

        protected override void OnParsing(XElement existingSingleFieldValueOperator)
        {
            XElement existingValue = existingSingleFieldValueOperator.ElementIgnoreCase(CamlValue.ValueTag);
            if (existingValue != null)
            {
                base.OnParsing(existingValue);
            }
            XElement existingFieldRef = existingSingleFieldValueOperator.ElementIgnoreCase(CamlFieldRef.FieldRefTag);
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