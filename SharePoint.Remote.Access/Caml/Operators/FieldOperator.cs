﻿using System;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Caml.Interfaces;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class FieldOperator : Operator, ICamlField
    {
        protected FieldOperator(string operatorName, CamlFieldRef fieldRef)
            : base(operatorName)
        {
            if (fieldRef == null) throw new ArgumentNullException(nameof(fieldRef));
            FieldRef = fieldRef;
        }

        protected FieldOperator(string operatorName, string existingFieldOperator)
            : base(operatorName, existingFieldOperator)
        {
        }

        protected FieldOperator(string operatorName, XElement existingFieldOperator)
            : base(operatorName, existingFieldOperator)
        {
        }

        public CamlFieldRef FieldRef { get; private set; }

        protected override void OnParsing(XElement existingFieldValueOperator)
        {
            var existingFieldRef =
                existingFieldValueOperator.Elements(CamlFieldRef.FieldRefTag).SingleOrDefault();
            if (existingFieldRef != null)
            {
                FieldRef = new CamlFieldRef(existingFieldRef);
            }
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            el.AddFirst(FieldRef.ToXElement());
            return el;
        }
    }
}