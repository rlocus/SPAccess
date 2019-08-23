using System;
using System.Linq.Expressions;

namespace SP.Client.Linq.Query.ExpressionVisitors
{
    public class SpConditionalExpressionVisitor : SpExpressionVisitor
    {
        protected override Expression VisitBinary(BinaryExpression exp)
        {
            LeftOperator = ToOperator(exp.Left);
            RightOperator = ToOperator(exp.Right);

            if (LeftOperator == null)
            {
                Operator = RightOperator;
                return exp;
            }
            else if (RightOperator == null)
            {
                Operator = LeftOperator;
                return exp;
            }

            switch (exp.NodeType)
            {
                case ExpressionType.AndAlso:
                    Operator = Client.Caml.CamlExtensions.And(LeftOperator, RightOperator);
                    break;
                case ExpressionType.OrElse:
                    Operator = Client.Caml.CamlExtensions.Or(LeftOperator, RightOperator);
                    break;
                default:
                    throw new NotSupportedException(string.Format("{0} statement is not supported", exp.NodeType.ToString()));
            }
            return exp;
        }      
    }

}
