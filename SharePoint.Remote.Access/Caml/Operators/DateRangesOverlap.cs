using System;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public class DateRangesOverlap : ValueMultiFieldOperator<object>
    {
        internal const string DateRangesOverlapTag = "DateRangesOverlap";

        public DateRangesOverlap(CamlFieldRef startField, CamlFieldRef endField, CamlFieldRef recurrenceField, DateValue value)
            : base(DateRangesOverlapTag, new[] { startField, endField, recurrenceField }, value, FieldType.DateTime)
        {
        }

        public DateRangesOverlap(string startField, string endField, string recurrenceField, DateValue value)
            : base(DateRangesOverlapTag, new[] { startField, endField, recurrenceField }, value, FieldType.DateTime)
        {
        }

        protected DateRangesOverlap(Guid startField, Guid endField, Guid recurrenceField, DateValue value)
            : base(DateRangesOverlapTag, new[] { startField, endField, recurrenceField }, value, FieldType.DateTime)
        {
        }

        public DateRangesOverlap(CamlFieldRef startField, CamlFieldRef endField, CamlFieldRef recurrenceField, DateTime value)
           : base(DateRangesOverlapTag, new[] { startField, endField, recurrenceField }, value, FieldType.DateTime)
        {
        }

        public DateRangesOverlap(string startField, string endField, string recurrenceField, DateTime value)
            : base(DateRangesOverlapTag, new[] { startField, endField, recurrenceField }, value, FieldType.DateTime)
        {
        }

        protected DateRangesOverlap(Guid startField, Guid endField, Guid recurrenceField, DateTime value)
            : base(DateRangesOverlapTag, new[] { startField, endField, recurrenceField }, value, FieldType.DateTime)
        {
        }

        public DateRangesOverlap(string existingSingleFieldValueOperator)
            : base(DateRangesOverlapTag, existingSingleFieldValueOperator)
        {
        }

        public DateRangesOverlap(XElement existingSingleFieldValueOperator)
            : base(DateRangesOverlapTag, existingSingleFieldValueOperator)
        {
        }

        //protected override void OnParsing(XElement existingMultipleFieldValueOperator)
        //{
        //    base.OnParsing(existingMultipleFieldValueOperator);
        //    var existingValue = existingMultipleFieldValueOperator.ElementsIgnoreCase(CamlValue.ValueTag).SingleOrDefault();
        //    if (existingValue != null && existingValue.HasElements)
        //    {
        //        foreach (XElement element in existingValue.Elements())
        //        {
        //            DateRangesOverlapValue enumValue;
        //            if (Enum.TryParse(element.Name.LocalName, true, out enumValue))
        //            {
        //                _enumValue = enumValue;
        //                break;
        //            }
        //        }
        //    }
        //}

        //public override XElement ToXElement()
        //{
        //    var el = base.ToXElement();
        //    if (_enumValue.HasValue)
        //    {
        //        var value = el.Elements(CamlValue.ValueTag).Single();
        //        value.ReplaceAll(new XElement(_enumValue.ToString()));
        //    }
        //    return el;
        //}
    }
}