using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Caml.Interfaces;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class CamlMultiValue<T> : ComparisonOperator, ICamlMultiValue<T>
    {
        internal const string ValuesTag = "Values";

        protected CamlMultiValue(string operatorName, IEnumerable<T> values, FieldType type)
            : base(operatorName)
        {
            if (values != null) Values = values.Select(val => new CamlValue<T>(val, type));
        }

        protected CamlMultiValue(string operatorName, IEnumerable<CamlValue<T>> values)
            : base(operatorName)
        {
            Values = values;
        }

        protected CamlMultiValue(string operatorName, string existingMultipleValueOperator)
            : base(operatorName, existingMultipleValueOperator)
        {
        }

        protected CamlMultiValue(string operatorName, XElement existingMultipleValueOperator)
            : base(operatorName, existingMultipleValueOperator)
        {
        }

        public IEnumerable<CamlValue<T>> Values { get; set; }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            if (Values != null)
            {
                el.Add(new XElement(ValuesTag, Values.Select(val => val?.ToXElement())));
            }
            return el;
        }

        protected override void OnParsing(XElement existingValuesOperator)
        {
            var existingValues = existingValuesOperator.ElementsIgnoreCase(CamlValue.ValueTag);
            Values = existingValues.Select(val => new CamlValue<T>(val));
        }
    }
}