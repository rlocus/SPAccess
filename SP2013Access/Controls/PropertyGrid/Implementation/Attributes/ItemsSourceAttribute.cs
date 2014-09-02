using System;

namespace SP2013Access.Controls.PropertyGrid.Attributes
{
    public class ItemsSourceAttribute : Attribute
    {
        public Type Type
        {
            get;
            set;
        }

        public ItemsSourceAttribute(Type type)
        {
            var valueSourceInterface = type.GetInterface(typeof(IItemsSource).FullName);
            if (valueSourceInterface == null)
                throw new ArgumentException("Type must implement the IItemsSource interface.", "type");

            Type = type;
        }
    }
}