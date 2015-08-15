using System;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public sealed class Lt<T> : FieldValueOperator<T>
    {
        internal const string LtTag = "Lt";

        public Lt(FieldRef fieldRef, Value<T> value)
            : base(LtTag, fieldRef, value)
        {
        }

        public Lt(FieldRef fieldRef, T value, FieldType type)
            : base(LtTag, fieldRef, value, type)
        {
        }

        public Lt(Guid fieldId, T value, FieldType type)
            : base(LtTag, fieldId, value, type)
        {
        }

        public Lt(string fieldName, T value, FieldType type)
            : base(LtTag, fieldName, value, type)
        {
        }

        public Lt(string existingLtOperator)
            : base(LtTag, existingLtOperator)
        {
        }

        public Lt(XElement existingLtOperator)
            : base(LtTag, existingLtOperator)
        {
        }
    }
}