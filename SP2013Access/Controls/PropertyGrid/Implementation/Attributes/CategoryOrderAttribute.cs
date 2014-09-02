using System;

namespace SP2013Access.Controls.PropertyGrid.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CategoryOrderAttribute : Attribute
    {
        #region Properties

        #region Order

        public int Order
        {
            get;
            set;
        }

        #endregion Order

        #region Category

        public virtual string Category
        {
            get
            {
                return CategoryValue;
            }
        }

        #endregion Category

        #region CategoryValue

        public string CategoryValue
        {
            get;
            private set;
        }

        #endregion CategoryValue

        #endregion Properties

        #region constructor

        public CategoryOrderAttribute()
        {
        }

        public CategoryOrderAttribute(string categoryName, int order)
            : this()
        {
            CategoryValue = categoryName;
            Order = order;
        }

        #endregion constructor
    }
}