﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Caml.Interfaces;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class MultiValueOperator<T> : Operator, IMultiValueOperator<T>
    {
        protected MultiValueOperator(string operatorName, IEnumerable<T> values, FieldType type)
            : base(operatorName)
        {
            if (values != null) Values = values.Select(val => new Value<T>(val, type));
        }

        protected MultiValueOperator(string operatorName, IEnumerable<Value<T>> values)
            : base(operatorName)
        {
            Values = values;
        }

        protected MultiValueOperator(string operatorName, string existingMultipleValueOperator)
            : base(operatorName, existingMultipleValueOperator)
        {
        }

        protected MultiValueOperator(string operatorName, XElement existingMultipleValueOperator)
            : base(operatorName, existingMultipleValueOperator)
        {
        }

        public IEnumerable<Value<T>> Values { get; set; }

        protected override void OnParsing(XElement existingValuesOperator)
        {
            var existingValues = existingValuesOperator.Elements()
                .Where(el => string.Equals(el.Name.LocalName, "Value", StringComparison.InvariantCultureIgnoreCase));
            Values = existingValues.Select(val => new Value<T>(val));
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            if (Values != null)
            {
                el.Add(new XElement("Values", Values.Select(val => val?.ToXElement())));
            }
            return el;
        }
    }
}