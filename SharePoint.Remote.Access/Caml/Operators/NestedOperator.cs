﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class NestedOperator : Operator
    {
        internal const int OperatorCount = 2;
        internal const int NestedOperatorCount = 1;

        public NestedOperator Parent { get; private set; }
        internal Operator[] Operators { get; private set; }

        protected NestedOperator(string operatorName, IEnumerable<Operator> operators)
            : base(operatorName)
        {
            InitOperators(operators);
        }

        protected NestedOperator(string operatorName, string existingNestedOperator)
            : base(operatorName, existingNestedOperator)
        {
        }

        protected NestedOperator(string operatorName, XElement existingNestedOperator)
            : base(operatorName, existingNestedOperator)
        {
        }

        internal void InitOperators(IEnumerable<Operator> operators)
        {
            if (operators != null)
            {
                Operators = operators as Operator[] ?? operators.ToArray();
                if (Operators.Length != OperatorCount)
                {
                    throw new NotSupportedException($"Count of operators must be {OperatorCount}.");
                }
                if (Operators.OfType<NestedOperator>().Count() > NestedOperatorCount)
                {
                    throw new NotSupportedException($"Max count of nested operators must be {NestedOperatorCount}.");
                }

                foreach (var @operator in Operators.OfType<NestedOperator>())
                {
                    @operator.Parent = this;
                }
            }
        }

        protected override void OnParsing(XElement existingNestedOperator)
        {
            var operators = existingNestedOperator.Elements().Select(el =>
            {
                Operator op = GetOperator(el);
                var @operator = op as NestedOperator;
                if (@operator != null)
                {
                    @operator.Parent = this;
                }
                return op;
            }).Where(op => op != null);
            InitOperators(operators);
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            foreach (var op in Operators.Where(op => op != null))
            {
                el.Add(op.ToXElement());
            }
            return el;
        }
    }
}