﻿using System;
using System.Xml.Linq;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public sealed class IsNull : FieldOperator
    {
        internal const string IsNullTag = "IsNull";

        public IsNull(Guid fieldId)
            : base(IsNullTag, new FieldRef {FieldId = fieldId})
        {
        }

        public IsNull(string fieldName)
            : base(IsNullTag, new FieldRef {Name = fieldName})
        {
        }

        public IsNull(XElement existingIsNullOperator)
            : base(IsNullTag, existingIsNullOperator)
        {
        }
    }
}