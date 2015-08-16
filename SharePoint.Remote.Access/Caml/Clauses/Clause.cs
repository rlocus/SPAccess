using System.Xml.Linq;

namespace SharePoint.Remote.Access.Caml.Clauses
{
    public abstract class Clause : QueryElement
    {
        protected Clause(string clauseName)
            : base(clauseName)
        {
        }

        protected Clause(string clauseName, string existingClause)
            : base(clauseName, existingClause)
        {
        }

        protected Clause(string clauseName, XElement existingClause)
            : base(clauseName, existingClause)
        {
        }
    }
}