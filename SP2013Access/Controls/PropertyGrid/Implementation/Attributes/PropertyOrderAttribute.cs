﻿using System;

namespace SP2013Access.Controls.PropertyGrid.Attributes
{
    public enum UsageContextEnum
    {
        Alphabetical,
        Categorized,
        Both
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class PropertyOrderAttribute : Attribute
    {
        #region Properties

        public int Order
        {
            get;
            set;
        }

        public UsageContextEnum UsageContext
        {
            get;
            set;
        }

        public override object TypeId
        {
            get
            {
                return this;
            }
        }

        #endregion Properties

        #region Initialization

        public PropertyOrderAttribute(int order)
            : this(order, UsageContextEnum.Both)
        {
        }

        public PropertyOrderAttribute(int order, UsageContextEnum usageContext)
        {
            Order = order;
            UsageContext = usageContext;
        }

        #endregion Initialization
    }
}