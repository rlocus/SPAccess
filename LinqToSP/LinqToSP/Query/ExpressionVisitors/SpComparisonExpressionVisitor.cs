using Microsoft.SharePoint.Client;
using SP.Client.Caml;
using SP.Client.Extensions;
using SP.Client.Linq.Attributes;
using System;
using System.Linq.Expressions;

namespace SP.Client.Linq.Query.ExpressionVisitors
{
    public class SpComparisonExpressionVisitor : SpExpressionVisitor
    {
        public SpComparisonExpressionVisitor(SpQueryArgs args) : base(args)
        {
        }

        protected string FieldName { get; private set; }
        protected object FieldValue { get; private set; }

        protected override Expression VisitBinary(BinaryExpression exp)
        {
            LeftOperator = ToOperator(exp.Left);
            RightOperator = ToOperator(exp.Right);
            FieldType dataType;
            CamlFieldRef fieldRef;
            CamlValue value = null;
            if (SpQueryArgs != null)
            {
                if (SpQueryArgs.FieldMappings.ContainsKey(FieldName))
                {
                    var fieldMap = SpQueryArgs.FieldMappings[FieldName];
                    fieldRef = new CamlFieldRef() { Name = fieldMap.Name };
                    dataType = fieldMap.DataType;
                    if (fieldMap is LookupFieldAttribute)
                    {
                        fieldRef.LookupId = ((LookupFieldAttribute)fieldMap).IsLookupId;
                    }
                    if (FieldValue != null)
                    {
                        value = new CamlValue(FieldValue, dataType);
                        if (dataType == FieldType.DateTime)
                        {
                            value.IncludeTimeValue = true;
                        }
                    }
                }
                else
                {
                    throw new Exception($"Cannot find '{FieldName}' mapping field. Check '{typeof(FieldAttribute)}'.");
                }
            }
            else
            {
                return exp;
            }
            switch (exp.NodeType)
            {
                case ExpressionType.Equal:
                    if (exp.Right.IsNullValue())
                    {
                        Operator = new Caml.Operators.IsNull(fieldRef);
                    }
                    else
                    {
                        Operator = new Caml.Operators.Eq(fieldRef, value);
                    }
                    break;
                case ExpressionType.NotEqual:
                    if (exp.Right.IsNullValue())
                    {
                        Operator = new Caml.Operators.IsNotNull(fieldRef);
                    }
                    else
                    {
                        Operator = new Caml.Operators.Neq(fieldRef, value);
                    }
                    break;
                case ExpressionType.GreaterThan:
                    Operator = new Caml.Operators.Gt(fieldRef, value);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    Operator = new Caml.Operators.Geq(fieldRef, value);
                    break;
                case ExpressionType.LessThan:
                    Operator = new Caml.Operators.Lt(fieldRef, value);
                    break;
                case ExpressionType.LessThanOrEqual:
                    Operator = new Caml.Operators.Leq(fieldRef, value);
                    break;
                //case ExpressionType.Convert:
                //    Visit(exp);
                //    break;
                default:
                    throw new NotSupportedException($"{exp.NodeType} operator is not supported in LinqToSP.");
            }
            return exp;
        }

        protected override Expression VisitMember(MemberExpression exp)
        {
            FieldName = exp.Member.Name;
            return exp;
        }

        protected override Expression VisitConstant(ConstantExpression exp)
        {
            FieldValue = exp.Value;
            return exp;
        }
    }

}
