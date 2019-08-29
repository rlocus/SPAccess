using Remotion.Linq.Parsing;
using SP.Client.Extensions;
using SP.Client.Caml.Operators;
using System;
using System.Linq.Expressions;

namespace SP.Client.Linq.Query.ExpressionVisitors
{
    public abstract class SpExpressionVisitor : ExpressionVisitorBase /*ThrowingExpressionVisitor*/
    {
        protected SpQueryArgs SpQueryArgs { get; }
        protected SpExpressionVisitor(SpQueryArgs args)
        {
            SpQueryArgs = args;
        }

        public Operator Operator
        {
            get; protected set;
        }

        public Operator LeftOperator
        {
            get; protected set;
        }

        public Operator RightOperator
        {
            get; protected set;
        }

        protected override Expression VisitBinary(BinaryExpression exp)
        {
            SpExpressionVisitor expVisitor = GetExpressionVisitor(exp);
            if (expVisitor != null)
            {
                Operator = expVisitor.ToOperator(exp);
                LeftOperator = expVisitor.LeftOperator;
                RightOperator = expVisitor.RightOperator;
            }
            else
            {
                Visit(exp.Left);
                Visit(exp.Right);
            }
            return exp;
        }

        private SpExpressionVisitor GetExpressionVisitor(Expression exp)
        {
            SpExpressionVisitor expVisitor = null;

            switch (exp.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    expVisitor = new SpConditionalExpressionVisitor(SpQueryArgs);
                    break;
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    expVisitor = new SpComparisonExpressionVisitor(SpQueryArgs);
                    break;
            }
            return expVisitor;
        }

        protected virtual Operator ToOperator(Expression exp)
        {
            if (exp == null) return null;
            Operator op = null;
            SpExpressionVisitor expVisitor = GetExpressionVisitor(exp);
            if (expVisitor != null)
            {
                expVisitor.Visit(exp);
                op = expVisitor.Operator;
            }
            else
            {
                Visit(exp);
                op = this.Operator;
            }
            return op;
        }

        protected override Expression VisitUnary(UnaryExpression exp)
        {
            if (exp.NodeType == ExpressionType.Not)
            {
                throw new NotSupportedException($"Unary type {ExpressionType.Not} is not supported in LinqToSp. Use (a != b) instead of !(a == b).");
            }
            else if (exp.NodeType == ExpressionType.Convert || exp.NodeType == ExpressionType.TypeAs)
            {
                //try
                //{
                //  return Visit(Expression.Constant(Expression.Lambda(exp).Compile().DynamicInvoke()));
                //}
                //catch
                //{
                return Visit(exp.Operand);
                //}
            }
            return base.VisitUnary(exp);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Expression expression = node;
            if (node.Method.Name == "Contains")
            {
                var visitor = new SpContainsExpressionVisitor(SpQueryArgs);
                visitor.Visit(expression);
                Operator = visitor.Operator;
            }
            else if (node.Method.Name == "StartsWith")
            {
                var visitor = new SpStartsWithExpressionVisitor(SpQueryArgs);
                visitor.Visit(expression);
                Operator = visitor.Operator;
            }
            else if (node.Method.Name == "DateRangesOverlap" && (node.Method.DeclaringType.IsAssignableFrom(typeof(ListItemEntityExtensions))))
            {
                var visitor = new SpDateRangesOverlapExpressionVisitor(SpQueryArgs);
                visitor.Visit(expression);
                Operator = visitor.Operator;
            }
            else if (node.Method.Name == "Includes" && (node.Method.DeclaringType.IsAssignableFrom(typeof(ListItemEntityExtensions))))
            {
                var visitor = new SpIncludesExpressionVisitor(SpQueryArgs);
                visitor.Visit(expression);
                Operator = visitor.Operator;
            }
            else if ((node.Method.Name == "LookupIncludes" || node.Method.Name == "LookupIdIncludes") && (node.Method.DeclaringType.IsAssignableFrom(typeof(ListItemEntityExtensions))))
            {
                var visitor = new SpLookupIncludesExpressionVisitor(SpQueryArgs);
                visitor.Visit(expression);
                Operator = visitor.Operator;
            }
            else if ((node.Method.Name == "LookupNotIncludes" || node.Method.Name == "LookupIdNotIncludes") && (node.Method.DeclaringType.IsAssignableFrom(typeof(ListItemEntityExtensions))))
            {
                var visitor = new SpLookupNotIncludesExpressionVisitor(SpQueryArgs);
                visitor.Visit(expression);
                Operator = visitor.Operator;
            }
            else
            {
                expression = base.VisitMethodCall(node);
            }
            return expression;
        }

        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            throw new NotImplementedException(visitMethod + " method is not implemented");
        }
    }

}
