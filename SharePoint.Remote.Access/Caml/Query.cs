using System.Linq;
using System.Text;
using System.Xml.Linq;
using SharePoint.Remote.Access.Caml.Clauses;

namespace SharePoint.Remote.Access.Caml
{
    public sealed class Query : CamlElement
    {
        internal const string QueryTag = "Query";
        public CamlWhere Where { get; set; }
        public CamlOrderBy OrderBy { get; set; }
        public CamlGroupBy GroupBy { get; set; }

        public Query() : base(QueryTag)
        {
        }

        public Query(string existingQuery) : base(QueryTag, existingQuery)
        {
        }

        public Query(XElement existingQuery) : base(QueryTag, existingQuery)
        {
        }

        protected override void OnParsing(XElement existingQuery)
        {
            var existingClauses = existingQuery.Elements().Select(CamlClause.GetClause).ToList();
            Where = existingClauses.OfType<CamlWhere>().FirstOrDefault();
            OrderBy = existingClauses.OfType<CamlOrderBy>().FirstOrDefault();
            GroupBy = existingClauses.OfType<CamlGroupBy>().FirstOrDefault();
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            if (Where != null)
            {
                el.Add(Where.ToXElement());
            }
            if (OrderBy != null)
            {
                el.Add(OrderBy.ToXElement());
            }
            if (GroupBy != null)
            {
                el.Add(GroupBy.ToXElement());
            }
            return el;
        }

        public string ToString(bool includeQueryTag, bool disableFormatting)
        {
            XElement caml = ToXElement();

            if (!includeQueryTag)
            {
                var sb = new StringBuilder();
                foreach (var element in caml.Elements())
                {
                    if (disableFormatting)
                    {
                        sb.Append(element.ToString(SaveOptions.DisableFormatting));
                    }
                    else
                    {
                        sb.AppendLine(element.ToString(SaveOptions.None));
                    }
                }
                return sb.ToString();
            }
            return ToString(disableFormatting);
        }

        public static implicit operator string (Query query)
        {
            return query?.ToString() ?? string.Empty;
        }
        
        public static Query Combine(Query firstQuery, Query secondQuery)
        {
            return new Query
            {
                //Where = CamlWhere.Combine(firstQuery.Where, secondQuery.Where),
                OrderBy = CamlOrderBy.Combine(firstQuery.OrderBy, secondQuery.OrderBy),
                GroupBy = CamlGroupBy.Combine(firstQuery.GroupBy, secondQuery.GroupBy)
            };
        }
        
    }
}