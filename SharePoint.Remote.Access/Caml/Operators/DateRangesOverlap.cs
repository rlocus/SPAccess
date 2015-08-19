using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public class DateRangesOverlap : ValueMultiFieldOperator<object>
    {
        internal const string DateRangesOverlapTag = "DateRangesOverlap";

        public DateRangesOverlap(IEnumerable<CamlFieldRef> fieldRefs, DateValue value)
            : base(DateRangesOverlapTag, fieldRefs, value, FieldType.DateTime)
        {
        }

        public DateRangesOverlap(IEnumerable<string> fieldNames, DateValue value)
            : base(DateRangesOverlapTag, fieldNames, value, FieldType.DateTime)
        {
        }

        protected DateRangesOverlap(IEnumerable<Guid> fieldIds, DateValue value)
            : base(DateRangesOverlapTag, fieldIds, value, FieldType.DateTime)
        {
        }
        
        public DateRangesOverlap(IEnumerable<CamlFieldRef> fieldRefs, DateTime value)
           : base(DateRangesOverlapTag, fieldRefs, value, FieldType.DateTime)
        {
        }

        public DateRangesOverlap(IEnumerable<string> fieldNames, DateTime value)
            : base(DateRangesOverlapTag, fieldNames, value, FieldType.DateTime)
        {
        }

        protected DateRangesOverlap(IEnumerable<Guid> fieldIds, DateTime value)
            : base(DateRangesOverlapTag, fieldIds, value, FieldType.DateTime)
        {
        }

        public DateRangesOverlap(string existingSingleFieldValueOperator)
            : base(DateRangesOverlapTag, existingSingleFieldValueOperator)
        {
        }

        protected DateRangesOverlap(XElement existingSingleFieldValueOperator)
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