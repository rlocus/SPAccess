using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Caml.Interfaces;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class FieldMultipleValueOperator<T> : MultipleValueOperator<T>, IFieldOperator
    {
        protected FieldMultipleValueOperator(string operatorName, Guid fieldId, IEnumerable<Value<T>> values)
            : base(operatorName, values)
        {
            FieldRef = new FieldRef {FieldId = fieldId};
        }

        protected FieldMultipleValueOperator(string operatorName, Guid fieldId, IEnumerable<T> values,
            FieldType type)
            : base(operatorName, values, type)
        {
            FieldRef = new FieldRef {FieldId = fieldId};
        }

        protected FieldMultipleValueOperator(string operatorName, string fieldName, IEnumerable<T> values,
            FieldType type)
            : base(operatorName, values, type)
        {
            FieldRef = new FieldRef {Name = fieldName};
        }

        protected FieldMultipleValueOperator(string operatorName, string fieldName, IEnumerable<Value<T>> values)
            : base(operatorName, values)
        {
            FieldRef = new FieldRef {Name = fieldName};
        }

        protected FieldMultipleValueOperator(string operatorName, FieldRef fieldRef, IEnumerable<T> values,
            FieldType type)
            : base(operatorName, values, type)
        {
            FieldRef = fieldRef;
        }

        protected FieldMultipleValueOperator(string operatorName, FieldRef fieldRef, IEnumerable<Value<T>> values)
            : base(operatorName, values)
        {
            FieldRef = fieldRef;
        }

        protected FieldMultipleValueOperator(string operatorName, string existingSingleFieldMultipleValueOperator)
            : base(operatorName, existingSingleFieldMultipleValueOperator)
        {
        }

        protected FieldMultipleValueOperator(string operatorName,
            XElement existingSingleFieldMultipleValueOperator)
            : base(operatorName, existingSingleFieldMultipleValueOperator)
        {
        }

        public FieldRef FieldRef { get; set; }

        protected override void OnParsing(XElement existingSingleFieldMultipleValueOperator)
        {
            var existingFieldRef =
                existingSingleFieldMultipleValueOperator.Elements()
                    .SingleOrDefault(
                        el => string.Equals(el.Name.LocalName, "FieldRef", StringComparison.InvariantCultureIgnoreCase));

            if (existingFieldRef != null)
            {
                FieldRef = new FieldRef(existingFieldRef);
            }
            var existingValues =
                existingSingleFieldMultipleValueOperator.Elements()
                    .SingleOrDefault(
                        el => string.Equals(el.Name.LocalName, "Values", StringComparison.InvariantCultureIgnoreCase));

            if (existingValues != null)
            {
                base.OnParsing(existingValues);
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