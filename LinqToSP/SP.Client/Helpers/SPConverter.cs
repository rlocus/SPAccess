using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace SP.Client.Helpers
{
    public static class SpConverter
    {
        public static Type GetValueType(FieldType type, bool allowMultipleValues = false)
        {
            if (type == FieldType.Guid)
            {
                return typeof(Guid);
            }
            if (type == FieldType.Text || type == FieldType.Note || type == FieldType.Choice)
            {
                return typeof(string);
            }
            if (type == FieldType.Number || type == FieldType.Currency)
            {
                return typeof(double);
            }
            if (type == FieldType.MultiChoice)
            {
                return typeof(IEnumerable<string>);
            }
            if (type == FieldType.Boolean || type == FieldType.Recurrence || type == FieldType.Attachments ||
                type == FieldType.AllDayEvent || type == FieldType.CrossProjectLink)
            {
                return typeof(bool);
            }
            if (type == FieldType.Lookup)
            {
                return allowMultipleValues ? typeof(IEnumerable<FieldLookupValue>) : typeof(FieldLookupValue);
            }
            if (type == FieldType.User)
            {
                return allowMultipleValues ? typeof(IEnumerable<FieldUserValue>) : typeof(FieldUserValue);
            }
            if (type == FieldType.URL)
            {
                return typeof(FieldUrlValue);
            }
            if (type == FieldType.DateTime)
            {
                return typeof(DateTime);
            }
            if (type == FieldType.Integer || type == FieldType.Counter || type == FieldType.ModStat ||
                type == FieldType.WorkflowStatus)
            {
                return typeof(int);
            }
            if (type == FieldType.ContentTypeId)
            {
                return typeof(ContentTypeId);
            }

            return null;
        }

        public static object ConvertValue(FieldType type, object value, bool allowMultipleValues = false)
        {
            Type valType = GetValueType(type, allowMultipleValues);
            return ConvertValue(value, valType);
        }

        public static T ConvertValue<T>(object value)
        {
            Type type = typeof(T);
            var convertedValue = ConvertValue(value, type);
            return convertedValue == null ? default(T) : (T)convertedValue;
        }

        public static object ConvertValue(object value, Type type)
        {
            if (value == null || (value.GetType() == type || type == null))
            {
                return value;
            }

            if (type == typeof(Guid))
            {
                value = new Guid(value.ToString());
            }
            else if (type == typeof(IEnumerable<string>))
            {
                value = (IEnumerable<string>)value;
            }
            else if (type == typeof(FieldLookupValue))
            {
                value = (FieldLookupValue)value;
            }
            else if (type == typeof(IEnumerable<FieldLookupValue>))
            {
                value = (IEnumerable<FieldLookupValue>)value;
            }
            else if (type == typeof(FieldUserValue))
            {
                value = (FieldUserValue)value;
            }
            else if (type == typeof(IEnumerable<FieldLookupValue>))
            {
                value = (IEnumerable<FieldLookupValue>)value;
            }
            else if (type == typeof(FieldUrlValue))
            {
                value = (FieldUrlValue)value;
            }
            else if (type == typeof(ContentTypeId))
            {
                value = (ContentTypeId)value;
            }
            else
            {
                var valType = value.GetType();
                if (valType == typeof(FieldLookupValue))
                {
                    if (type.IsNumeric())
                    {
                        value = ((FieldLookupValue)value).LookupId;
                    }
                    else
                    {
                        value = $"{ ((FieldLookupValue)value).LookupId};#{((FieldLookupValue)value).LookupValue}";
                    }
                }
                else if (valType == typeof(FieldUrlValue))
                {
                    value = ((FieldUrlValue)value).Url;
                }
                else if (valType == typeof(FieldUserValue))
                {
                    if (type.IsNumeric())
                    {
                        value = ((FieldUserValue)value).LookupId;
                    }
                    else
                    {
                        value = $"{ ((FieldUserValue)value).LookupId};#{((FieldUserValue)value).LookupValue}";
                    }
                }
                else if (valType == typeof(ContentTypeId))
                {
                    value = ((ContentTypeId)value).StringValue;
                }
                value = Convert(value, type);
            }

            return value;
        }

        internal static T Convert<T>(object value)
        {
            Type type = typeof(T);
            var convertedValue = Convert(value, type);
            return convertedValue == null ? default(T) : (T)convertedValue;
        }

        internal static object Convert(object value, Type type)
        {
            if (value is IConvertible)
            {
                type = Nullable.GetUnderlyingType(type) ?? type;
                value = System.Convert.ChangeType(value, type);
            }
            return value;
        }
    }
}
