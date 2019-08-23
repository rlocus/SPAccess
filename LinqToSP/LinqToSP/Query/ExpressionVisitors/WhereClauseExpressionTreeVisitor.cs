namespace SP.Client.Linq.Query.ExpressionVisitors
{
  public class WhereClauseExpressionTreeVisitor : SpExpressionVisitor
  {
    private readonly Caml.Query _query = new Caml.Query();

    public WhereClauseExpressionTreeVisitor(SpQueryArgs args): base(args)
    {
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
