using Remotion.Linq.Parsing;
using SP.Client.Linq.Query.Expressions;
using System.Linq.Expressions;

namespace SP.Client.Linq.Query.ExpressionVisitors
{
  public class IncludeExpressionVisitor : SpExpressionVisitor
  {
    private readonly Caml.ViewFieldsCamlElement _viewFields;

    public IncludeExpressionVisitor(SpQueryArgs args) : base(args)
    {
      _viewFields = new Caml.ViewFieldsCamlElement();
    }

    public Caml.ViewFieldsCamlElement ViewFields
    {
      get
      {
        return _viewFields;
      }
    }

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
          _viewFields.Add(fieldMap.Name);
        }
      }
      return node;
      //return base.VisitMember(node);
    }
  }
}
