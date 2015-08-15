using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Caml.Clauses;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml
{
    // var query = new Query()
    //{
    //    Where = new Where(
    //        new Or(
    //            new Eq<string>(fieldName: "ContentType", value: "My Content Type", type: SPFieldType.Text),
    //            new IsNotNull(fieldName: "Description"))),

    //    GroupBy = new GroupBy(fieldName: "Title", collapsed: true),
    //    OrderBy = new OrderBy(fieldName: "_Author").ThenBy(fieldName: "AuthoringDate").ThenBy(fieldName: "AssignedTo")
    //};

    public sealed class Query
    {
        private const string QueryTag = "Query";
        public bool DisableFormatting;
        public Where Where { get; set; }
        public OrderBy OrderBy { get; set; }
        public GroupBy GroupBy { get; set; }
        private SaveOptions SaveOption => DisableFormatting ? SaveOptions.DisableFormatting : SaveOptions.None;

        private string ConvertToString(IEnumerable<XElement> elements, SaveOptions saveOption)
        {
            var sb = new StringBuilder();

            foreach (var element in elements)
            {
                if (DisableFormatting)
                {
                    sb.Append(element.ToString(saveOption));
                }
                else
                {
                    sb.AppendLine(element.ToString(saveOption));
                }
            }

            return sb.ToString();
        }

        public XElement ToCaml()
        {
            var el = new XElement(QueryTag);

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

        public string ToString(bool includeQueryTag)
        {
            var caml = ToCaml();
            var elements = caml.Elements();
            return !includeQueryTag
                ? ConvertToString(elements, SaveOption)
                : caml.ToString(SaveOption);
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public static implicit operator string(Query query)
        {
            return query?.ToString() ?? string.Empty;
        }

        public CamlQuery ToCamlQuery()
        {
            var query = new CamlQuery {ViewXml = ""};
            return query;
        }

        public CamlQuery ToCamlQuery(params string[] viewFields)
        {
            if (viewFields == null) throw new ArgumentNullException(nameof(viewFields));
            var query = ToCamlQuery();
            return query.Include(viewFields);
        }

        public static Query Parse(string existingQuery)
        {
            if (string.IsNullOrEmpty(existingQuery))
            {
                return null;
            }
            var el = XElement.Parse(existingQuery, LoadOptions.None);
            return Parse(el);
        }

        public static Query Parse(XElement existingQuery)
        {
            var query = new Query();

            if (existingQuery != null &&
                (existingQuery.HasElements &&
                 string.Equals(existingQuery.Name.LocalName, QueryTag, StringComparison.InvariantCultureIgnoreCase)))
            {
                var existingWhere =
                    existingQuery.Elements()
                        .SingleOrDefault(
                            el => string.Equals(el.Name.LocalName, "Where", StringComparison.InvariantCultureIgnoreCase));

                if (existingWhere != null) query.Where = new Where(existingWhere);

                var existingOrderBy =
                    existingQuery.Elements()
                        .SingleOrDefault(
                            el =>
                                string.Equals(el.Name.LocalName, "OrderBy", StringComparison.InvariantCultureIgnoreCase));

                if (existingOrderBy != null) query.OrderBy = new OrderBy(existingOrderBy);

                var existingGroupBy =
                    existingQuery.Elements()
                        .SingleOrDefault(
                            el =>
                                string.Equals(el.Name.LocalName, "GroupBy", StringComparison.InvariantCultureIgnoreCase));

                if (existingGroupBy != null) query.GroupBy = new GroupBy(existingGroupBy);
            }

            return query;
        }

        public static Query Combine(Query firstQuery, Query secondQuery)
        {
            return new Query
            {
                Where = Where.Combine(firstQuery.Where, secondQuery.Where),
                OrderBy = OrderBy.Combine(firstQuery.OrderBy, secondQuery.OrderBy),
                GroupBy = GroupBy.Combine(firstQuery.GroupBy, secondQuery.GroupBy)
            };
        }

        public static Query GetFromCamlQuery(CamlQuery camlQuery)
        {
            return camlQuery != null ? Parse(string.Format("<Query>{0}</Query>", camlQuery.ViewXml)) : null;
        }

        public static implicit operator Query(CamlQuery camlQuery)
        {
            return GetFromCamlQuery(camlQuery);
        }
    }
}