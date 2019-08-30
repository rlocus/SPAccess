using SP.Client.Linq.Query.Expressions;
using System.Linq.Expressions;

namespace SP.Client.Linq.Query.ExpressionVisitors
{
    public class GroupByExpressionVisitor : SpExpressionVisitor
    {
        public GroupByExpressionVisitor(SpQueryArgs args) : base(args)
        {
            Clause = new Caml.Clauses.CamlGroupBy();
        }

        public Caml.Clauses.CamlGroupBy Clause { get; }

        public override Expression Visit(Expression expression)
        {
            if (expression is IncludeExpression)
            {
                foreach (var path in (expression as IncludeExpression).Path)
                {
                    Visit(path);
                }
                return expression;
            }
            else if (expression is GroupByExpression)
            {
                Clause.Limit = (expression as GroupByExpression).Limit > 0 ?
                    (expression as GroupByExpression).Limit
                    : (int?)null;

                foreach (var path in (expression as GroupByExpression).Path)
                {
                    Visit(path);
                }
                return expression;
            }
            else
            {
                return base.Visit(expression);
            }
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            string fieldName = node.Member.Name;
            if (SpQueryArgs != null)
            {
                if (SpQueryArgs.FieldMappings.ContainsKey(fieldName))
                {
                    var fieldMap = SpQueryArgs.FieldMappings[fieldName];
                    Clause.AddField(fieldMap.Name);
                }
            }
            return node;
            //return base.VisitMember(node);
        }
    }
}
