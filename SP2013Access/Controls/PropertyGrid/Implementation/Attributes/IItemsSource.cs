using System.Collections.Generic;

namespace SP2013Access.Controls.PropertyGrid.Attributes
{
    public interface IItemsSource
    {
        ItemCollection GetValues();
    }

    public class Item
    {
        public string DisplayName
        {
            get;
            set;
        }

        public object Value
        {
            get;
            set;
        }
    }

    public class ItemCollection : List<Item>
    {public void Add(object value, string displayName)
        {
            base.Add(new Item
            {
                DisplayName = displayName,
                Value = value
            });
        }
    }
}