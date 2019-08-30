using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace SP.Client.Linq.Query.Expressions
{
    public class GroupByExpression : Expression
    {
        public GroupByExpression(Expression entityExpression, IEnumerable<Expression> path, int limit)
        {
            EntityExpression = entityExpression;
            Type = EntityExpression.Type;
            Path = path;
            Limit = limit;
        }

        public virtual Expression EntityExpression { get; set; }

        public sealed override ExpressionType NodeType => ExpressionType.Extension;
        public override Type Type { get; }
        public IEnumerable<Expression> Path { get; }
        public int Limit { get; }

        public override bool CanReduce => false;

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var result = visitor.Visit(EntityExpression);
            if (result != EntityExpression)
                return new GroupByExpression(result, Path, Limit);
            return this;
        }

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            return base.Accept(visitor);
        }

        public override string ToString()
        {
            if (Path != null)
            {
                return $"GroupBy({string.Join(", ", Path.Select(p => p.ToString()).ToArray())})";
            }
            return base.ToString();
        }
    }
}
