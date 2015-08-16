using System;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Caml.Interfaces;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class ValueOperator<T> : Operator, IValueOperator<T>
    {
        public Value<T> Value { get; private set; }

        protected ValueOperator(string operatorName, Value<T> value)
            : base(operatorName)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        protected ValueOperator(string operatorName, T value, FieldType type)
            : base(operatorName)
        {
            Value = new Value<T>(value, type);
        }

        protected ValueOperator(string operatorName, string existingValueOperator)
            : base(operatorName, existingValueOperator)
        {
        }

        protected ValueOperator(string operatorName, XElement existingValueOperator)
            : base(operatorName, existingValueOperator)
        {
        }
        
        protected override void OnParsing(XElement existingValueOperator)
        {
            Value = new Value<T>(existingValueOperator);
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            if (Value != null) el.Add(Value.ToXElement());
            return el;
        }
    }
}