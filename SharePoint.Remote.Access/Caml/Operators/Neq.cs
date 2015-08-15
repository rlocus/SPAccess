using System;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public sealed class Neq<T> : FieldValueOperator<T>
    {
        internal const string NeqTag = "Neq";

        public Neq(FieldRef fieldRef, Value<T> value)
            : base(NeqTag, fieldRef, value)
        {
        }

        public Neq(FieldRef fieldRef, T value, FieldType type)
            : base(NeqTag, fieldRef, value, type)
        {
        }

        public Neq(Guid fieldId, T value, FieldType type)
            : base(NeqTag, fieldId, value, type)
        {
        }

        public Neq(string fieldName, T value, FieldType type)
            : base(NeqTag, fieldName, value, type)
        {
        }

        public Neq(string existingNeqOperator)
            : base(NeqTag, existingNeqOperator)
        {
        }

        public Neq(XElement existingNeqOperator)
            : base(NeqTag, existingNeqOperator)
        {
        }
    }
}