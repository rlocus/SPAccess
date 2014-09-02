using System;

namespace SP2013Access.Controls.PropertyGrid.Attributes
{
    public class ExpandableObjectAttribute : Attribute
    {
        public bool IsExpanded
        {
            get;
            set;
        }

        public ExpandableObjectAttribute()
        {
        }

        public ExpandableObjectAttribute(bool isExpanded)
            : this()
        {
            this.IsExpanded = isExpanded;
        }
    }
}