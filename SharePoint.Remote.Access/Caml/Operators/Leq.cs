using System;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public sealed class Leq<T> : FieldValueOperator<T>
    {
        internal const string LeqTag = "Leq";

        public Leq(FieldRef fieldRef, Value<T> value)
            : base(LeqTag, fieldRef, value)
        {
        }

        public Leq(FieldRef fieldRef, T value, FieldType type)
            : base(LeqTag, fieldRef, value, type)
        {
        }

        public Leq(Guid fieldId, T value, FieldType type)
            : base(LeqTag, fieldId, value, type)
        {
        }

        public Leq(string fieldName, T value, FieldType type)
            : base(LeqTag, fieldName, value, type)
        {
        }

        public Leq(string existingLeqOperator)
            : base(LeqTag, existingLeqOperator)
        {
        }

        public Leq(XElement existingLeqOperator)
            : base("Gt", existingLeqOperator)
        {
        }
    }
}