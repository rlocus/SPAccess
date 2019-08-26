using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace SP.Client.Linq.Query.Expressions
{
    public class IncludeExpression : Expression
    {
        public IncludeExpression(Expression entityExpression, IEnumerable<Expression> path)
        {
            EntityExpression = entityExpression;
            Type = EntityExpression.Type;
            Path = path;
        }

        public virtual Expression EntityExpression { get; set; }

        public sealed override ExpressionType NodeType => ExpressionType.Extension;
        public override Type Type { get; }
        public IEnumerable<Expression> Path { get; }

        public override bool CanReduce => false;

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var newEntityExpression = visitor.Visit(EntityExpression);
            return Update(newEntityExpression);
        }

        public virtual Expression Update(Expression entityExpression)
        { /*entityExpression != EntityExpression
            ? new IncludeExpression(entityExpression, Path)*/
            return EntityExpression;
        }

        public override string ToString()
        {
            if (Path != null)
            {
                return $"Include({string.Join(", ", Path.Select(p => p.ToString()).ToArray())})";
            }
            return base.ToString();
        }
    }
}
