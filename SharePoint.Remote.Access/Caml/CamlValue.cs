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

    public sealed class CamlValue : CamlValue<object>
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

    public class CamlValue<T> : CamlElement
    {
        internal const string ValueTag = "Value";
        internal const string TypeAttr = "Type";
        internal const string IncludeTimeValueAttr = "IncludeTimeValue";

        public const string UserId = "UserID";

        public CamlValue(T value, FieldType type)
            : base(ValueTag)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            Value = value;
            Type = type;
        }

        public CamlValue(string existingValue)
            : base(ValueTag, existingValue)
        {
        }

        public CamlValue(XElement existingValue)
            : base(ValueTag, existingValue)
        {
        }

        public T Value { get; set; }
        public FieldType Type { get; set; }
        public bool? IncludeTimeValue { get; set; }

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
            var type = existingValue.AttributeIgnoreCase(TypeAttr);
            if (type != null)
            {
                Type = (FieldType)Enum.Parse(typeof(FieldType), type.Value.Trim(), true);
            }
            if (FieldType.DateTime == Type)
            {
                var includeTimeValue = existingValue.AttributeIgnoreCase(IncludeTimeValueAttr);
                if (includeTimeValue != null)
                {
                    IncludeTimeValue = Convert.ToBoolean(includeTimeValue.Value);
                }
                if (existingValue.HasElements)
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
                    var existingDateValue =
                        dateValues.Select(dateValue => existingValue.ElementIgnoreCase(dateValue))
                            .FirstOrDefault(val => val != null);
                    if (existingDateValue != null)
                    {
                        Value = (T)Enum.Parse(typeof(DateValue), existingDateValue.Name.LocalName, true);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(existingValue.Value))
                    {
                        var date = DateTime.Parse(existingValue.Value);
                        Value = (T)(object)date;
                    }
                }
                return;
            }
            if (FieldType.Integer == Type)
            {
                if (existingValue.HasElements)
                {
                    XElement userId = existingValue.ElementIgnoreCase(UserId);
                    if (userId != null)
                    {
                        Value = (T)(object)UserId;
                        return;
                    }
                }
            }
            if (!string.IsNullOrEmpty(existingValue.Value))
            {
                Value = (T)Convert.ChangeType(existingValue.Value, GetValueType());
            }
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            el.Add(new XAttribute(TypeAttr, Type));

            if (FieldType.DateTime == Type)
            {
                if (IncludeTimeValue.HasValue)
                {
                    el.Add(new XAttribute(IncludeTimeValueAttr, IncludeTimeValue.Value));
                }
                if (Value is DateTime)
                {
                    el.Value = string.Concat(Convert.ToDateTime(Value).ToString("s"), "Z");
                }
                else if (Value is DateValue)
                {
                    el.Add(new XElement(Convert.ToString(Value)));
                }
                else
                {
                    el.Value = Convert.ToString(Value);
                }
            }
            else
            {
                el.Value = Convert.ToString(Value);
            }
            return el;
        }
    }
}