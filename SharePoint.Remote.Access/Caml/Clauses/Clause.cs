using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Caml.Operators;

namespace SharePoint.Remote.Access.Caml.Clauses
{
    public abstract class Clause : QueryElement
    {
        protected Clause(string clauseName, params Operator[] operators)
            : base(clauseName)
        {
            if (operators != null) Operators = operators.AsEnumerable();
        }

        protected Clause(string clauseName, string existingClause)
            : base(clauseName, existingClause)
        {
        }

        protected Clause(string clauseName, XElement existingClause)
            : base(clauseName, existingClause)
        {
        }

        public IEnumerable<Operator> Operators { get; protected set; }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            if (Operators != null)
            {
                foreach (var op in Operators.Where(op => op != null))
                {
                    el.Add(op.ToXElement());
                }
            }
            return el;
        }
    }
}