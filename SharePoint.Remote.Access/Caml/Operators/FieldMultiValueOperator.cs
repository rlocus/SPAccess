using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Caml.Interfaces;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class FieldMultiValueOperator<T> : MultiValueOperator<T>, ICamlField
    {
        public CamlFieldRef FieldRef { get; private set; }

        protected FieldMultiValueOperator(string operatorName, Guid fieldId, IEnumerable<Value<T>> values)
            : base(operatorName, values)
        {
            FieldRef = new CamlFieldRef { FieldId = fieldId };
        }

        protected FieldMultiValueOperator(string operatorName, Guid fieldId, IEnumerable<T> values,
            FieldType type)
            : base(operatorName, values, type)
        {
            FieldRef = new CamlFieldRef { FieldId = fieldId };
        }

        protected FieldMultiValueOperator(string operatorName, string fieldName, IEnumerable<T> values,
            FieldType type)
            : base(operatorName, values, type)
        {
            FieldRef = new CamlFieldRef { Name = fieldName };
        }

        protected FieldMultiValueOperator(string operatorName, string fieldName, IEnumerable<Value<T>> values)
            : base(operatorName, values)
        {
            FieldRef = new CamlFieldRef { Name = fieldName };
        }

        protected FieldMultiValueOperator(string operatorName, CamlFieldRef fieldRef, IEnumerable<T> values,
            FieldType type)
            : base(operatorName, values, type)
        {
            FieldRef = fieldRef;
        }

        protected FieldMultiValueOperator(string operatorName, CamlFieldRef fieldRef, IEnumerable<Value<T>> values)
            : base(operatorName, values)
        {
            FieldRef = fieldRef;
        }

        protected FieldMultiValueOperator(string operatorName, string existingSingleFieldMultipleValueOperator)
            : base(operatorName, existingSingleFieldMultipleValueOperator)
        {
        }

        protected FieldMultiValueOperator(string operatorName,
            XElement existingSingleFieldMultipleValueOperator)
            : base(operatorName, existingSingleFieldMultipleValueOperator)
        {
        }

        protected override void OnParsing(XElement existingSingleFieldMultipleValueOperator)
        {
            XElement existingValues = existingSingleFieldMultipleValueOperator.ElementsIgnoreCase(ValuesTag).SingleOrDefault();
            if (existingValues != null)
            {
                base.OnParsing(existingValues);
            }
            XElement existingFieldRef = existingSingleFieldMultipleValueOperator.ElementsIgnoreCase(CamlFieldRef.FieldRefTag).SingleOrDefault();
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