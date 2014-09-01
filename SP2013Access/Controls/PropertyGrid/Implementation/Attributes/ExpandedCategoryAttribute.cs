using System;

namespace SP2013Access.Controls.PropertyGrid.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExpandedCategoryAttribute : Attribute
    {
        public bool IsExpanded
        {
            get;
            set;
        }

        public virtual string Category
        {
            get
            {
                return this.CategoryValue;
            }
        }

        public string CategoryValue
        {
            get;
            private set;
        }

        public ExpandedCategoryAttribute()
        {
        }

        public ExpandedCategoryAttribute(string categoryName, bool isExpanded)
            : this()
        {
            this.CategoryValue = categoryName;
            this.IsExpanded = isExpanded;
        }
    }
}