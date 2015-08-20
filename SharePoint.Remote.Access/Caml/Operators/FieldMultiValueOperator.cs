using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Caml.Interfaces;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class FieldCamlMultiValue<T> : CamlMultiValue<T>, ICamlField
    {
        protected FieldCamlMultiValue(string operatorName, Guid fieldId, IEnumerable<CamlValue<T>> values)
            : base(operatorName, values)
        {
            FieldRef = new CamlFieldRef {Id = fieldId};
        }

        protected FieldCamlMultiValue(string operatorName, Guid fieldId, IEnumerable<T> values,
            FieldType type)
            : base(operatorName, values, type)
        {
            FieldRef = new CamlFieldRef {Id = fieldId};
        }

        protected FieldCamlMultiValue(string operatorName, string fieldName, IEnumerable<T> values,
            FieldType type)
            : base(operatorName, values, type)
        {
            FieldRef = new CamlFieldRef {Name = fieldName};
        }

        protected FieldCamlMultiValue(string operatorName, string fieldName, IEnumerable<CamlValue<T>> values)
            : base(operatorName, values)
        {
            FieldRef = new CamlFieldRef {Name = fieldName};
        }

        protected FieldCamlMultiValue(string operatorName, CamlFieldRef fieldRef, IEnumerable<T> values,
            FieldType type)
            : base(operatorName, values, type)
        {
            FieldRef = fieldRef;
        }

        protected FieldCamlMultiValue(string operatorName, CamlFieldRef fieldRef, IEnumerable<CamlValue<T>> values)
            : base(operatorName, values)
        {
            FieldRef = fieldRef;
        }

        protected FieldCamlMultiValue(string operatorName, string existingSingleFieldMultipleValueOperator)
            : base(operatorName, existingSingleFieldMultipleValueOperator)
        {
        }

        protected FieldCamlMultiValue(string operatorName,
            XElement existingSingleFieldMultipleValueOperator)
            : base(operatorName, existingSingleFieldMultipleValueOperator)
        {
        }

        public CamlFieldRef FieldRef { get; private set; }

        protected override void OnParsing(XElement existingSingleFieldMultipleValueOperator)
        {
            var existingValues =
                existingSingleFieldMultipleValueOperator.ElementsIgnoreCase(ValuesTag).SingleOrDefault();
            if (existingValues != null)
            {
                base.OnParsing(existingValues);
            }
            var existingFieldRef =
                existingSingleFieldMultipleValueOperator.ElementsIgnoreCase(CamlFieldRef.FieldRefTag).SingleOrDefault();
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