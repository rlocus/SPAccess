using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml
{
    public sealed class ProjectedFieldsCamlElement : CamlElement
    {
        internal const string ProjectedFieldsTag = "ProjectedFields";

        public ProjectedFieldsCamlElement(IEnumerable<CamlProjectedField> projectedFields) : base(ProjectedFieldsTag)
        {
            if (projectedFields != null) ProjectedFields = projectedFields;
        }

        public ProjectedFieldsCamlElement(string existingJoins) : base(ProjectedFieldsTag, existingJoins)
        {
        }

        public ProjectedFieldsCamlElement(XElement existingJoins) : base(ProjectedFieldsTag, existingJoins)
        {
        }

        internal IEnumerable<CamlProjectedField> ProjectedFields { get; set; }

        protected override void OnParsing(XElement existingProjectedFields)
        {
            ProjectedFields =
                existingProjectedFields.ElementsIgnoreCase(CamlProjectedField.FieldTag)
                    .Select(existingProjectedField => new CamlProjectedField(existingProjectedField));
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            if (ProjectedFields != null)
            {
                foreach (var projectedField in ProjectedFields.Where(projectedField => projectedField != null))
                {
                    el.Add(projectedField.ToXElement());
                }
            }
            return el;
        }
    }
}