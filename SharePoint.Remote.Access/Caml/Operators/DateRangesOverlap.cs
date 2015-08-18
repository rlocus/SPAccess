using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public enum DateRangesOverlapValue
    {
        Now,
        Today,
        Day,
        Week,
        Month,
        Year
    }

    public sealed class DateRangesOverlap : ValueMultiFieldOperator<DateTime>
    {
        internal const string DateRangesOverlapTag = "DateRangesOverlap";
        private DateRangesOverlapValue? _enumValue;

        public DateRangesOverlap(IEnumerable<CamlFieldRef> fieldRefs, DateTime value)
            : base(DateRangesOverlapTag, fieldRefs, value, FieldType.DateTime)
        {
        }

        public DateRangesOverlap(DateTime value, params Guid[] fieldIds)
            : base(DateRangesOverlapTag, fieldIds, value, FieldType.DateTime)
        {
        }

        public DateRangesOverlap(DateTime value, params string[] fieldNames)
            : base(DateRangesOverlapTag, fieldNames, value, FieldType.DateTime)
        {
        }

        public DateRangesOverlap(DateRangesOverlapValue value, params Guid[] fieldIds)
            : base(DateRangesOverlapTag, fieldIds, DateTime.MinValue, FieldType.DateTime)
        {
            _enumValue = value;
        }

        public DateRangesOverlap(DateRangesOverlapValue value, params string[] fieldNames)
            : base(DateRangesOverlapTag, fieldNames, DateTime.MinValue, FieldType.DateTime)
        {
            _enumValue = value;
        }

        public DateRangesOverlap(string existingDateRangesOverlapOperator)
            : base(DateRangesOverlapTag, existingDateRangesOverlapOperator)
        {
        }

        public DateRangesOverlap(XElement existingDateRangesOverlapOperator)
            : base(DateRangesOverlapTag, existingDateRangesOverlapOperator)
        {
        }

        protected override void OnParsing(XElement existingMultipleFieldValueOperator)
        {
            base.OnParsing(existingMultipleFieldValueOperator);
            var existingValue = existingMultipleFieldValueOperator.ElementsIgnoreCase(Caml.CamlValue.ValueTag).SingleOrDefault();
            if (existingValue != null && existingValue.HasElements)
            {
                foreach (XElement element in existingValue.Elements())
                {
                    DateRangesOverlapValue enumValue;
                    if (Enum.TryParse(element.Name.LocalName, true, out enumValue))
                    {
                        _enumValue = enumValue;
                        break;
                    }
                }
            }
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            if (_enumValue.HasValue)
            {
                var value = el.Elements(Caml.CamlValue.ValueTag).Single();
                value.Value = string.Empty;
                value.Add(new XElement(_enumValue.ToString()));
            }
            return el;
        }
    }
}