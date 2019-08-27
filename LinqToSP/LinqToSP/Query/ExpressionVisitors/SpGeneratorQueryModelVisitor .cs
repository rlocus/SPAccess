using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using SP.Client.Linq.Query.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace SP.Client.Linq.Query.ExpressionVisitors
{
  internal class SpGeneratorQueryModelVisitor : QueryModelVisitorBase
  {
    private readonly SpQueryArgs _args;

    internal SpGeneratorQueryModelVisitor(SpQueryArgs args)
    {
      _args = args;
    }

    public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
    {
    }

    public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
    {
    }

    public override void VisitQueryModel(QueryModel queryModel)
    {
      queryModel.SelectClause.Accept(this, queryModel);
      queryModel.MainFromClause.Accept(this, queryModel);
      VisitBodyClauses(queryModel.BodyClauses, queryModel);
      VisitResultOperators(queryModel.ResultOperators, queryModel);
    }

    public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
    {
      if (fromClause.FromExpression is IncludeExpression)
      {
        VisitIncludeClause(fromClause.FromExpression as IncludeExpression, queryModel);
      }
      else
      {
        base.VisitMainFromClause(fromClause, queryModel);
      }
    }

    public void VisitIncludeClauses(IEnumerable<IncludeExpression> includeExpressions, QueryModel queryModel)
    {
      if (includeExpressions != null)
      {
        foreach (var includeExpression in includeExpressions)
        {
          VisitIncludeClause(includeExpression, queryModel);
        }
      }
    }

    private void VisitIncludeClause(IncludeExpression expression, QueryModel queryModel)
    {
      var includeVisitor = new IncludeExpressionVisitor(_args);
      includeVisitor.Visit(expression);
      _args.SpView.ViewFields = includeVisitor.ViewFields;
    }

    public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
    {
      var where = new WhereClauseExpressionTreeVisitor(_args);
      where.Visit(whereClause.Predicate);
      if (_args.SpView.Query.Where == null)
      {
        _args.SpView.Query.Where = where.Clause;
      }
      else
      {
        if (where.Clause != null)
        {
          _args.SpView.Query.Where = Caml.CamlExtensions.And(_args.SpView.Query.Where, where.Clause);
        }
      }
      base.VisitWhereClause(whereClause, queryModel, index);
    }

    public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
    {
      if (resultOperator is TakeResultOperator)
      {
        var take = resultOperator as TakeResultOperator;
        _args.SpView.Limit = Convert.ToInt32(take.Count.ToString());
      }
      else if (resultOperator is CountResultOperator)
      {

      }
      else if (resultOperator is LongCountResultOperator)
      {

      }
      else if (resultOperator is FirstResultOperator)
      {
        _args.SpView.Limit = 1;
      }

      //Not supported result operators
      else if (resultOperator is SkipResultOperator)
        throw new NotSupportedException("Method Skip() is not supported in LinqToSp.");
      else if (resultOperator is ContainsResultOperator)
        throw new NotSupportedException("Method Contains() is not supported in LinqToSp.");
      else if (resultOperator is DefaultIfEmptyResultOperator)
        throw new NotSupportedException("Method DefaultIfEmpty() is not supported in LinqToSp.");
      else if (resultOperator is ExceptResultOperator)
        throw new NotSupportedException("Method Except() is not supported in LinqToSp.");
      //else if (resultOperator is GroupResultOperator)
      //    throw new NotSupportedException("Method Group() is not supported in LinqToSp.");
      else if (resultOperator is IntersectResultOperator)
        throw new NotSupportedException("Method Intersect() is not supported in LinqToSp.");
      else if (resultOperator is OfTypeResultOperator)
        throw new NotSupportedException("Method OfType() is not supported in LinqToSp.");
      else if (resultOperator is SingleResultOperator)
        throw new NotSupportedException("Method Single() is not supported in LinqToSp. Use First() method instead.");
      else if (resultOperator is UnionResultOperator)
        throw new NotSupportedException("Method Union() is not supported in LinqToSp.");
      else if (resultOperator is AverageResultOperator)
        throw new NotSupportedException("Method Average() is not supported in LinqToSp.");
      else if (resultOperator is MinResultOperator)
        throw new NotSupportedException("Method Min() is not supported in LinqToSp.");
      else if (resultOperator is MaxResultOperator)
        throw new NotSupportedException("Method Max() is not supported in LinqToSp.");
      else if (resultOperator is SumResultOperator)
        throw new NotSupportedException("Method Sum() is not supported in LinqToSp.");
      else if (resultOperator is DistinctResultOperator)
        throw new NotSupportedException("Method Distinct() is not supported in LinqToSp.");

      base.VisitResultOperator(resultOperator, queryModel, index);
    }

    public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
    {
      base.VisitAdditionalFromClause(fromClause, queryModel, index);
    }

    protected override void VisitBodyClauses(ObservableCollection<IBodyClause> bodyClauses, QueryModel queryModel)
    {
      if (_args == null) return;

      _args.SpView.Query.OrderBy = new Caml.Clauses.CamlOrderBy();
      foreach (var orderClause in bodyClauses.OfType<OrderByClause>())
      {
        if (orderClause != null)
        {
          foreach (var ordering in orderClause.Orderings)
          {
            var exp = ordering.Expression;
            if (exp is MemberExpression)
            {
              string fieldName = (exp as MemberExpression).Member.Name;
              if (_args.FieldMappings.ContainsKey(fieldName))
              {
                var fieldMap = _args.FieldMappings[fieldName];
                _args.SpView.Query.OrderBy.AddField(fieldMap.Name, ordering.OrderingDirection == OrderingDirection.Asc);
              }
            }
            else if (exp is MethodCallExpression)
            {

            }
          }
        }
        base.VisitBodyClauses(bodyClauses, queryModel);
      }
    }
  }
}
