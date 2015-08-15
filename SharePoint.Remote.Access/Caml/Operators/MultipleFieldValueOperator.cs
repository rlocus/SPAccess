using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Caml.Interfaces;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class MultipleFieldValueOperator<T> : ValueOperator<T>, IMultiFieldOperator
    {
        protected MultipleFieldValueOperator(string operatorName, IEnumerable<FieldRef> fieldRefs, T value,
            FieldType type)
            : base(operatorName, value, type)
        {
            FieldRefs = fieldRefs;
        }

        protected MultipleFieldValueOperator(string operatorName, IEnumerable<string> fieldNames, T value,
            FieldType type)
            : base(operatorName, value, type)
        {
            var fieldRefs = fieldNames.Select(fieldName => new FieldRef {Name = fieldName}) /*.ToList()*/;
            FieldRefs = fieldRefs;
        }

        protected MultipleFieldValueOperator(string operatorName, IEnumerable<Guid> fieldIds, T value, FieldType type)
            : base(operatorName, value, type)
        {
            var fieldRefs = fieldIds.Select(fieldId => new FieldRef {FieldId = fieldId});
            FieldRefs = fieldRefs;
        }

        protected MultipleFieldValueOperator(string operatorName, string existingSingleFieldValueOperator)
            : base(operatorName, existingSingleFieldValueOperator)
        {
        }

        protected MultipleFieldValueOperator(string operatorName, XElement existingSingleFieldValueOperator)
            : base(operatorName, existingSingleFieldValueOperator)
        {
        }

        public IEnumerable<FieldRef> FieldRefs { get; set; }

        protected override void OnParsing(XElement existingMultipleFieldValueOperator)
        {
            var existingFieldRefs =
                existingMultipleFieldValueOperator.Elements()
                    .Where(
                        el => string.Equals(el.Name.LocalName, FieldRef.FieldRefTag, StringComparison.InvariantCultureIgnoreCase));
            FieldRefs = existingFieldRefs.Select(existingFieldRef => new FieldRef(existingFieldRef));
            var existingValue =
                existingMultipleFieldValueOperator.Elements()
                    .SingleOrDefault(
                        el => string.Equals(el.Name.LocalName, Caml.Value.ValueTag, StringComparison.InvariantCultureIgnoreCase));

            if (existingValue != null)
            {
                base.OnParsing(existingValue);
            }
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            el.AddFirst(FieldRefs.Select(fieldRef => fieldRef?.ToXElement()));
            return el;
        }
    }
}