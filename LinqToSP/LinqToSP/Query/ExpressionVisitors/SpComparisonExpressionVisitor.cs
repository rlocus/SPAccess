using Microsoft.SharePoint.Client;
using SP.Client.Extensions;
using System;
using System.Linq.Expressions;

namespace SP.Client.Linq.Query.ExpressionVisitors
{
    public class SpComparisonExpressionVisitor : SpExpressionVisitor
    {
        protected string FieldName { get; private set; }
        protected object FieldValue { get; private set; }

        protected override Expression VisitBinary(BinaryExpression exp)
        {
            LeftOperator = ToOperator(exp.Left);
            RightOperator = ToOperator(exp.Right);

            switch (exp.NodeType)
            {
                case ExpressionType.Equal:
                    if (exp.Right.IsNullValue())
                    {
                        Operator = new Client.Caml.Operators.IsNull(FieldName);
                    }
                    else
                    {
                        Operator = new Client.Caml.Operators.Eq(FieldName, FieldValue, FieldType.Text);
                    }
                    break;
                case ExpressionType.NotEqual:
                    if (exp.Right.IsNullValue())
                    {
                        Operator = new Client.Caml.Operators.IsNotNull(FieldName);
                    }
                    else
                    {
                        Operator = new Client.Caml.Operators.Neq(FieldName, FieldValue, FieldType.Text);
                    }
                    break;
                case ExpressionType.GreaterThan:
                    Operator = new Client.Caml.Operators.Gt(FieldName, FieldValue, FieldType.Text);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    Operator = new Client.Caml.Operators.Geq(FieldName, FieldValue, FieldType.Text);
                    break;
                case ExpressionType.LessThan:
                    Operator = new Client.Caml.Operators.Lt(FieldName, FieldValue, FieldType.Text);
                    break;
                case ExpressionType.LessThanOrEqual:
                    Operator = new Client.Caml.Operators.Leq(FieldName, FieldValue, FieldType.Text);
                    break;
                default:
                    throw new NotSupportedException(string.Format("{0} statement is not supported", exp.NodeType.ToString()));
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
