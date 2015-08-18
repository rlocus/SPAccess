using System;
using System.Xml.Linq;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public sealed class IsNotNull : FieldOperator
    {
        internal const string IsNotNullTag = "IsNotNull";

        public IsNotNull(Guid fieldId)
            : base(IsNotNullTag, new CamlFieldRef {FieldId = fieldId})
        {
        }

        public IsNotNull(string fieldName)
            : base(IsNotNullTag, new CamlFieldRef {Name = fieldName})
        {
        }

        public IsNotNull(XElement existingIsNotNullOperator)
            : base(IsNotNullTag, existingIsNotNullOperator)
        {
        }
    }
}