using System;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public class DateRangesOverlap : ValueMultiFieldOperator<object>
    {
        internal const string DateRangesOverlapTag = "DateRangesOverlap";

        public DateRangesOverlap(CamlFieldRef startField, CamlFieldRef endField, CamlFieldRef recurrenceField,
            CamlValue.DateCamlValue value)
            : base(DateRangesOverlapTag, new[] { startField, endField, recurrenceField }, value, FieldType.DateTime)
        {
        }

        public DateRangesOverlap(string startField, string endField, string recurrenceField, CamlValue.DateCamlValue value)
            : base(DateRangesOverlapTag, new[] { startField, endField, recurrenceField }, value, FieldType.DateTime)
        {
        }

        protected DateRangesOverlap(Guid startField, Guid endField, Guid recurrenceField, CamlValue.DateCamlValue value)
            : base(DateRangesOverlapTag, new[] { startField, endField, recurrenceField }, value, FieldType.DateTime)
        {
        }

        public DateRangesOverlap(CamlFieldRef startField, CamlFieldRef endField, CamlFieldRef recurrenceField,
            DateTime value)
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
    }
}