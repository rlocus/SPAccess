using Remotion.Linq.Parsing;
using SP.Client.Extensions;
using SP.Client.Caml.Operators;
using System;
using System.Linq.Expressions;

namespace SP.Client.Linq.Query.ExpressionVisitors
{
  public abstract class SpExpressionVisitor : /*ExpressionVisitorBase*/ ThrowingExpressionVisitor
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
        case ExpressionType.AndAlso:
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
      }
      return op;
    }

    protected override Expression VisitUnary(UnaryExpression exp)
    {
      if (exp.NodeType == ExpressionType.Not)
      {
        throw new NotSupportedException($"Unary type {ExpressionType.Not} is not supported in LinqToSp. Use (a != b) instead of !(a == b).");

      }
      return base.VisitUnary(exp);
    }

    protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
    {
      throw new NotImplementedException(visitMethod + " method is not implemented");
    }
  }

}
