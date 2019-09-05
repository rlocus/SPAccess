﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace SP.Client.Linq.Query.Expressions
{
    public class GroupByExpression<TContext> : Expression
               where TContext : ISpDataContext
    {
        public GroupByExpression(Expression entityExpression, IEnumerable<Expression> predicates, int limit)
        {
            EntityExpression = entityExpression;
            Type = EntityExpression.Type;
            Predicates = predicates;
            Limit = limit;
        }

        public virtual Expression EntityExpression { get; set; }

        public sealed override ExpressionType NodeType => ExpressionType.Extension;
        public override Type Type { get; }
        public IEnumerable<Expression> Predicates { get; }
        public int Limit { get; }

        public override bool CanReduce => false;

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var result = visitor.Visit(EntityExpression);
            if (result != EntityExpression)
                return new GroupByExpression<TContext>(result, Predicates, Limit);
            return this;
        }

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            return base.Accept(visitor);
        }

        public override string ToString()
        {
            if (Predicates != null)
            {
                return $"GroupBy({string.Join(", ", Predicates.Select(p => p.ToString()).ToArray())})";
            }
            return base.ToString();
        }
    }
}
