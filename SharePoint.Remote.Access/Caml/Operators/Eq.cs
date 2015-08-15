using System;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public sealed class Eq<T> : FieldValueOperator<T>
    {
        internal const string EqTag = "Eq";

        public Eq(FieldRef fieldRef, Value<T> value)
            : base(EqTag, fieldRef, value)
        {
        }

        public Eq(FieldRef fieldRef, T value, FieldType type)
            : base(EqTag, fieldRef, value, type)
        {
        }

        public Eq(Guid fieldId, T value, FieldType type)
            : base(EqTag, fieldId, value, type)
        {
        }

        public Eq(string fieldName, T value, FieldType type)
            : base(EqTag, fieldName, value, type)
        {
        }

        public Eq(string existingEqOperator)
            : base(EqTag, existingEqOperator)
        {
        }

        public Eq(XElement existingEqOperator)
            : base(EqTag, existingEqOperator)
        {
        }
    }
}