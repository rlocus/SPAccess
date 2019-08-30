using Microsoft.SharePoint.Client;
using SP.Client.Caml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SP.Client.Linq.Query.ExpressionVisitors
{
    public class SpIncludesExpressionVisitor : SpComparisonExpressionVisitor
    {
        protected IEnumerable<object> FieldValues { get; private set; }

        public SpIncludesExpressionVisitor(SpQueryArgs args) : base(args)
        {
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "Includes" && (node.Method.DeclaringType.IsAssignableFrom(typeof(ListItemEntityExtensions))))
            {
                Visit(node.Object);
                foreach (var arg in node.Arguments)
                {
                    if (arg.NodeType == ExpressionType.Constant || arg.NodeType == ExpressionType.Lambda)
                    {
                        Visit(arg);
                    }
                }

                FieldType dataType;
                CamlFieldRef fieldRef = GetFieldRef(out dataType);
                fieldRef.LookupId = true;
                if (fieldRef == null || FieldValues == null)
                {
                    return node;
                }

                Operator = new Caml.Operators.In(fieldRef, FieldValues, dataType);
                return node;
            }
            throw new NotSupportedException($"{node.NodeType} method is not supported in LinqToSP.");
        }

        protected override Expression VisitConstant(ConstantExpression exp)
        {
            if (exp.Type.IsAssignableFrom(typeof(string[])))
            {
                FieldValues = exp.Value as string[];
            }
            else if (exp.Type.IsAssignableFrom(typeof(int[])))
            {
                FieldValues = (exp.Value as int[]).Select(v => v as object);
            }
            return exp;
        }
    }
}
