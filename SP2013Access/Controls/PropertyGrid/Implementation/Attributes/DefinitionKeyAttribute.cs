using System;

namespace SP2013Access.Controls.PropertyGrid.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DefinitionKeyAttribute : Attribute
    {
        public object Key
        {
            get;
            set;
        }

        public DefinitionKeyAttribute()
        {
        }

        public DefinitionKeyAttribute(object key)
        {
            this.Key = key;
        }
    }
}