using System;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public sealed class Contains : FieldValueOperator<string>
    {
        internal const string ContainsTag = "Contains";

        public Contains(FieldRef fieldRef, Value<string> value)
            : base(ContainsTag, fieldRef, value)
        {
        }

        public Contains(FieldRef fieldRef, string value)
            : base(ContainsTag, fieldRef, value, FieldType.Text)
        {
        }

        public Contains(Guid fieldId, string value)
            : base(ContainsTag, fieldId, value, FieldType.Text)
        {
        }

        public Contains(string fieldName, string value)
            : base(ContainsTag, fieldName, value, FieldType.Text)
        {
        }

        public Contains(string existingContainsOperator)
            : base(ContainsTag, existingContainsOperator)
        {
        }

        public Contains(XElement existingContainsOperator)
            : base(ContainsTag, existingContainsOperator)
        {
        }
    }
}