using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml
{
    public enum DateValue
    {
        Now,
        Today,
        Day,
        Week,
        Month,
        Year
    }

    public sealed class CamlValue : Value<object>
    {
        public CamlValue(object value, FieldType type) : base(value, type)
        {
        }

        public CamlValue(string existingValue) : base(existingValue)
        {
        }

        public CamlValue(XElement existingValue) : base(existingValue)
        {
        }
    }

    public class Value<T> : CamlElement
    {
        internal const string ValueTag = "Value";
        internal const string TypeAttr = "Type";
        internal const string IncludeTimeValueAttr = "IncludeTimeValue";

        public T Val { get; set; }
        public FieldType Type { get; set; }
        public bool? IncludeTimeValue { get; set; }

        public Value(T value, FieldType type)
            : base(ValueTag)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            Val = value;
            Type = type;
        }

        public Value(string existingValue)
            : base(ValueTag, existingValue)
        {
        }

        public Value(XElement existingValue)
            : base(ValueTag, existingValue)
        {
        }

        private Type GetValueType()
        {
            switch (Type)
            {
                case FieldType.Guid:
                    return typeof(Guid);
                case FieldType.Text:
                case FieldType.Note:
                case FieldType.Choice:
                case FieldType.Lookup:
                case FieldType.User:
                case FieldType.URL:
                case FieldType.MultiChoice:
                case FieldType.ContentTypeId:
                    return typeof(string);
                case FieldType.Number:
                case FieldType.Currency:
                    return typeof(double);
                case FieldType.Boolean:
                case FieldType.Recurrence:
                case FieldType.Attachments:
                case FieldType.AllDayEvent:
                case FieldType.CrossProjectLink:
                    return typeof(bool);
                case FieldType.DateTime:
                    return typeof(DateTime);
                case FieldType.Integer:
                case FieldType.Counter:
                case FieldType.ModStat:
                case FieldType.WorkflowStatus:
                    return typeof(int);
            }
            throw new NotSupportedException(nameof(Type));
        }

        protected override void OnParsing(XElement existingValue)
        {
            XAttribute type = existingValue.AttributeIgnoreCase(TypeAttr);
            if (type != null)
            {
                Type = (FieldType)Enum.Parse(typeof(FieldType), type.Value.Trim(), true);
            }

            XAttribute includeTimeValue = existingValue.AttributeIgnoreCase(IncludeTimeValueAttr);
            if (includeTimeValue != null)
            {
                IncludeTimeValue = Convert.ToBoolean(includeTimeValue.Value);
            }
            if (FieldType.DateTime == Type)
            {
                var dateValues = new[]
                {
                    DateValue.Today.ToString(),
                    DateValue.Day.ToString(),
                    DateValue.Month.ToString(),
                    DateValue.Now.ToString(),
                    DateValue.Week.ToString(),
                    DateValue.Year.ToString()
                };
                XElement existingDateValue =
                    dateValues.Select(dateValue => existingValue.ElementIgnoreCase(dateValue)).FirstOrDefault(val => val != null);

                if (existingDateValue != null)
                {
                    Val = (T)Enum.Parse(typeof(DateValue), existingDateValue.Name.LocalName, true);
                }
                else
                {
                    if (!string.IsNullOrEmpty(existingValue.Value))
                    {
                        DateTime date = DateTime.Parse(existingValue.Value);
                        Val = (T)(object)date;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(existingValue.Value))
                {
                    Val = (T)Convert.ChangeType(existingValue.Value, GetValueType());
                }
            }
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            el.Add(new XAttribute(TypeAttr, Type));
            if (IncludeTimeValue.HasValue)
            {
                el.Add(new XAttribute(IncludeTimeValueAttr, IncludeTimeValue.Value));
            }
            if (FieldType.DateTime == Type)
            {
                if (Val is DateTime)
                {
                    el.Value = string.Concat(Convert.ToDateTime(Val).ToString("s"), "Z");
                }
                else if (Val is DateValue)
                {
                    el.Add(new XElement(Convert.ToString(Val)));
                }
                else
                {
                    el.Value = Convert.ToString(Val);
                }
            }
            else
            {
                el.Value = Convert.ToString(Val);
            }
            return el;
        }
    }
}