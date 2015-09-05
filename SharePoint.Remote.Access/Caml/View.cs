using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml
{
    public sealed class View : CamlElement
    {
        internal const string ViewTag = "View";

        public View(uint rowLimit = 0, bool? paged = null) : this(null, null, null, rowLimit, paged)
        {
        }

        public View(IEnumerable<string> viewFields, uint rowLimit = 0, bool? paged = null) : this(viewFields, null, null, rowLimit, paged)
        {
        }

        public View(IEnumerable<Join> joins, IEnumerable<CamlProjectedField> projectedFields, uint rowLimit = 0, bool? paged = null) : this(null, joins, projectedFields, rowLimit, paged)
        {
        }

        public View(IEnumerable<string> viewFields, IEnumerable<Join> joins, IEnumerable<CamlProjectedField> projectedFields, uint rowLimit = 0, bool? paged = null) : base(ViewTag)
        {
            Query = new Query();
            ViewFields = new ViewFieldsCamlElement(viewFields);
            Joins = new JoinsCamlElement(joins);
            ProjectedFields = new ProjectedFieldsCamlElement(projectedFields);
            RowLimit = new CamlRowLimit(rowLimit, paged);
        }

        public View(string existingView) : base(ViewTag, existingView)
        {
        }

        public View(XElement existingView) : base(ViewTag, existingView)
        {
        }

        public Query Query { get; private set; }
        internal CamlRowLimit RowLimit { get; private set; }

        public bool? Paged
        {
            get { return RowLimit.Paged; }
            set { RowLimit.Paged = value; }
        }

        public uint Limit
        {
            get { return RowLimit.Limit; }
            set { RowLimit.Limit = value; }
        }

        public ViewFieldsCamlElement ViewFields { get; private set; }
        public JoinsCamlElement Joins { get; private set; }
        public ProjectedFieldsCamlElement ProjectedFields { get; private set; }

        protected override void OnParsing(XElement existingView)
        {
            var existingQuery = existingView.ElementIgnoreCase(Query.QueryTag);
            if (existingQuery != null)
            {
                Query = new Query(existingQuery);
            }
            var existingRowLimit = existingView.ElementIgnoreCase(CamlRowLimit.RowLimitTag);
            if (existingRowLimit != null)
            {
                RowLimit = new CamlRowLimit(existingRowLimit);
            }
            var existingViewFields = existingView.ElementIgnoreCase(ViewFieldsCamlElement.ViewFieldsTag);
            if (existingViewFields != null)
            {
                ViewFields = new ViewFieldsCamlElement(existingViewFields);
            }
            var existingJoins = existingView.ElementIgnoreCase(JoinsCamlElement.JoinsTag);
            if (existingJoins != null)
            {
                Joins = new JoinsCamlElement(existingJoins);
            }
            var projectedFields = existingView.ElementIgnoreCase(ProjectedFieldsCamlElement.ProjectedFieldsTag);
            if (projectedFields != null)
            {
                ProjectedFields = new ProjectedFieldsCamlElement(projectedFields);
            }
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            var queryElement = Query.ToXElement();
            if (queryElement != null /*&& queryElement.HasElements*/)
            {
                el.Add(queryElement);
            }
            if (ViewFields?.FieldRefs != null && ViewFields.FieldRefs.Any())
            {
                el.Add(ViewFields.ToXElement());
            }
            if (Joins?.Joins != null && Joins.Joins.Any())
            {
                el.Add(Joins.ToXElement());
            }
            if (ProjectedFields?.ProjectedFields != null && ProjectedFields.ProjectedFields.Any())
            {
                el.Add(ProjectedFields.ToXElement());
            }
            if (RowLimit != null && RowLimit.Limit > 0)
            {
                el.Add(RowLimit.ToXElement());
            }
            return el;
        }

        public CamlQuery ToCamlQuery()
        {
            return new CamlQuery { ViewXml = ToString(true) };
        }

        public static View GetView(CamlQuery camlQuery)
        {
            return camlQuery != null ? new View(camlQuery.ViewXml) : null;
        }

        public static implicit operator View(CamlQuery camlQuery)
        {
            return GetView(camlQuery);
        }
    }
}