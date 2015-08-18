using System;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public sealed class BeginsWith : FieldValueOperator<string>
    {
        internal const string BeginsWithTag = "BeginsWith";

        public BeginsWith(CamlFieldRef fieldRef, Value<string> value)
            : base(BeginsWithTag, fieldRef, value)
        {
        }

        public BeginsWith(CamlFieldRef fieldRef, string value)
            : base(BeginsWithTag, fieldRef, value, FieldType.Text)
        {
        }

        public BeginsWith(Guid fieldId, string value)
            : base(BeginsWithTag, fieldId, value, FieldType.Text)
        {
        }

        public BeginsWith(string fieldName, string value)
            : base(BeginsWithTag, fieldName, value, FieldType.Text)
        {
        }

        public BeginsWith(string existingBeginsWithOperator)
            : base(BeginsWithTag, existingBeginsWithOperator)
        {
        }

        public BeginsWith(XElement existingBeginsWithOperator)
            : base(BeginsWithTag, existingBeginsWithOperator)
        {
        }
    }
}