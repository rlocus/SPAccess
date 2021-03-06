﻿using System;
using System.Xml.Linq;
using SharePoint.Remote.Access.Caml.Interfaces;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class ValueOperator<T> : ComparisonOperator, ICamlValue<T>
    {
        protected ValueOperator(string operatorName, CamlValue<T> value)
            : base(operatorName)
        {
            if (value == null) throw new ArgumentNullException("value");
            Value = value;
        }

        protected ValueOperator(string operatorName, T value, FieldType type)
            : base(operatorName)
        {
            Value = new CamlValue<T>(value, type);
        }

        protected ValueOperator(string operatorName, string existingValueOperator)
            : base(operatorName, existingValueOperator)
        {
        }

        protected ValueOperator(string operatorName, XElement existingValueOperator)
            : base(operatorName, existingValueOperator)
        {
        }

        public CamlValue<T> Value { get; private set; }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            if (Value != null) el.Add(Value.ToXElement());
            return el;
        }

        protected override void OnParsing(XElement existingValueOperator)
        {
            Value = new CamlValue<T>(existingValueOperator);
        }
    }
}