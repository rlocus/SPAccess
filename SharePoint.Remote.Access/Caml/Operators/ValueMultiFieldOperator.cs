using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Caml.Interfaces;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class ValueMultiFieldOperator<T> : ValueOperator<T>, IMultiFieldOperator
    {
        public IEnumerable<FieldRef> FieldRefs { get; private set; }

        protected ValueMultiFieldOperator(string operatorName, IEnumerable<FieldRef> fieldRefs, T value, FieldType type)
            : base(operatorName, value, type)
        {
            if (fieldRefs == null) throw new ArgumentNullException(nameof(fieldRefs));
            FieldRefs = fieldRefs;
        }

        protected ValueMultiFieldOperator(string operatorName, IEnumerable<string> fieldNames, T value, FieldType type)
            : base(operatorName, value, type)
        {
            if (fieldNames == null) throw new ArgumentNullException(nameof(fieldNames));
            var fieldRefs = fieldNames.Select(fieldName => new FieldRef { Name = fieldName });
            FieldRefs = fieldRefs;
        }

        protected ValueMultiFieldOperator(string operatorName, IEnumerable<Guid> fieldIds, T value, FieldType type)
            : base(operatorName, value, type)
        {
            if (fieldIds == null) throw new ArgumentNullException(nameof(fieldIds));
            var fieldRefs = fieldIds.Select(fieldId => new FieldRef { FieldId = fieldId });
            FieldRefs = fieldRefs;
        }

        protected ValueMultiFieldOperator(string operatorName, string existingSingleFieldValueOperator)
            : base(operatorName, existingSingleFieldValueOperator)
        {
        }

        protected ValueMultiFieldOperator(string operatorName, XElement existingSingleFieldValueOperator)
            : base(operatorName, existingSingleFieldValueOperator)
        {
        }

        protected override void OnParsing(XElement existingMultipleFieldValueOperator)
        {
            IEnumerable<XElement> existingFieldRefs = existingMultipleFieldValueOperator.ElementsIgnoreCase(FieldRef.FieldRefTag);
            FieldRefs = existingFieldRefs.Select(existingFieldRef => new FieldRef(existingFieldRef));
            var existingValue =
                existingMultipleFieldValueOperator.Elements()
                    .SingleOrDefault(
                        el => string.Equals(el.Name.LocalName, Caml.Value.ValueTag, StringComparison.OrdinalIgnoreCase));

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