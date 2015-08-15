using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;

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

    public sealed class DateRangesOverlap : MultipleFieldValueOperator<DateTime>
    {
        internal const string DateRangesOverlapTag = "DateRangesOverlap";

        private DateRangesOverlapValue? _enumValue;

        public DateRangesOverlap(IEnumerable<FieldRef> fieldRefs, DateTime value)
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

            var existingValue =
                existingMultipleFieldValueOperator.Elements()
                    .SingleOrDefault(
                        el => string.Equals(el.Name.LocalName, "Value", StringComparison.InvariantCultureIgnoreCase));

            if (existingValue != null && existingValue.HasElements)
            {
                DateRangesOverlapValue[] dateRangesOverlaps =
                {
                    DateRangesOverlapValue.Now,
                    DateRangesOverlapValue.Today,
                    DateRangesOverlapValue.Day,
                    DateRangesOverlapValue.Week,
                    DateRangesOverlapValue.Month,
                    DateRangesOverlapValue.Year
                };

                foreach (var element in existingValue.Elements())
                {
                    if (dateRangesOverlaps.Any(dateRangesOverlap => dateRangesOverlap.ToString() == element.Name.LocalName))
                    {
                        _enumValue = (DateRangesOverlapValue)Enum.Parse(typeof(DateRangesOverlapValue), element.Name.LocalName);
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
                var value = el.Elements("Value").Single();
                value.Value = string.Empty;
                value.Add(new XElement(_enumValue.ToString()));
            }

            return el;
        }
    }
}