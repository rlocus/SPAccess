﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Caml.Interfaces;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class ValueMultiFieldOperator<T> : ValueOperator<T>, ICamlMultiField
    {
        protected ValueMultiFieldOperator(string operatorName, IEnumerable<CamlFieldRef> fieldRefs, T value,
            FieldType type)
            : base(operatorName, value, type)
        {
            if (fieldRefs == null) throw new ArgumentNullException(nameof(fieldRefs));
            FieldRefs = fieldRefs;
        }

        protected ValueMultiFieldOperator(string operatorName, IEnumerable<string> fieldNames, T value, FieldType type)
            : base(operatorName, value, type)
        {
            if (fieldNames == null) throw new ArgumentNullException(nameof(fieldNames));
            var fieldRefs = fieldNames.Select(fieldName => new CamlFieldRef {Name = fieldName});
            FieldRefs = fieldRefs;
        }

        protected ValueMultiFieldOperator(string operatorName, IEnumerable<Guid> fieldIds, T value, FieldType type)
            : base(operatorName, value, type)
        {
            if (fieldIds == null) throw new ArgumentNullException(nameof(fieldIds));
            var fieldRefs = fieldIds.Select(fieldId => new CamlFieldRef {Id = fieldId});
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

        public IEnumerable<CamlFieldRef> FieldRefs { get; private set; }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            el.AddFirst(FieldRefs.Select(fieldRef => fieldRef?.ToXElement()));
            return el;
        }

        protected override void OnParsing(XElement existingMultipleFieldValueOperator)
        {
            var existingFieldRefs = existingMultipleFieldValueOperator.ElementsIgnoreCase(CamlFieldRef.FieldRefTag);
            FieldRefs = existingFieldRefs.Select(existingFieldRef => new CamlFieldRef(existingFieldRef));
            var existingValue = existingMultipleFieldValueOperator.ElementIgnoreCase(CamlValue.ValueTag);
            if (existingValue != null)
            {
                base.OnParsing(existingValue);
            }
        }
    }
}