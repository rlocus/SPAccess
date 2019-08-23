using SP.Client.Linq.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SP.Client.Linq.Query.ExpressionVisitors
{
  public class WhereClauseExpressionTreeVisitor : SpExpressionVisitor
  {
    private readonly Caml.Query _query = new Caml.Query();
    private readonly Dictionary<string, FieldAttribute> _columnMapping;
    private readonly Type _type;

    public WhereClauseExpressionTreeVisitor(Type type, Dictionary<string, FieldAttribute> columnMapping)
    {
      _type = type;
      _columnMapping = columnMapping;
    }

    public Caml.Clauses.CamlWhere Clause
    {
      get
      {
        if (Operator != null)
        {
          _query.Where = new Caml.Clauses.CamlWhere(Operator);
        }
        return _query.Where;
      }
    }
  }
}
