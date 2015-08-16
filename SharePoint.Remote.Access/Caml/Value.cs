using System;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;

namespace SharePoint.Remote.Access.Caml
{
    public sealed class Value : Value<object>
    {
        public Value(object value, FieldType type) : base(value, type)
        {
        }

        public Value(string existingValue) : base(existingValue)
        {
        }

        public Value(XElement existingValue) : base(existingValue)
        {
        }
    }

    public class Value<T> : QueryElement
    {
        internal const string ValueTag = "Value";

        public Value(T value, FieldType type)
            : base(ValueTag)
        {
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

        public T Val { get; set; }
        public FieldType Type { get; set; }
        public bool? IncludeTimeValue { get; set; }

        protected override void OnParsing(XElement existingValue)
        {
            var type = existingValue.Attribute("Type");

            if (type != null)
            {
                Type = (FieldType) Enum.Parse(typeof (FieldType), type.Value.Trim(), true);
            }

            var includeTimeValue = existingValue.Attribute("IncludeTimeValue");

            if (includeTimeValue != null)
            {
                IncludeTimeValue = Convert.ToBoolean(includeTimeValue.Value);
            }

            if (!string.IsNullOrEmpty(existingValue.Value))
            {
                if (FieldType.DateTime == Type)
                {
                    //if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(object))
                    //Val = (T)((object)Utility.CreateDateTimeFromISO8601DateTimeString(existingValue.Value));
                }
            }
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            el.Add(new XAttribute("Type", Type));

            if (IncludeTimeValue.HasValue)
            {
                el.Add(new XAttribute("IncludeTimeValue", IncludeTimeValue.Value));
            }

            if (FieldType.DateTime == Type)
            {
                //el.Value = typeof(T) == typeof(DateTime) || typeof(T) == typeof(object)
                //    ? SPUtility.CreateISO8601DateTimeFromSystemDateTime(Convert.ToDateTime(Val))
                //    : Convert.ToString(Val);
            }
            else
            {
                el.Value = Convert.ToString(Val);
            }

            return el;
        }
    }
}