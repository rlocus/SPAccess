using System.Windows;

namespace SP2013Access.Controls.PropertyGrid
{
    /// <summary>
    /// Used when properties are provided using a list source of items (eg. Properties or PropertiesSource).
    ///
    /// An instance of this class can be used as an item to easily customize the
    /// display of the property directly by modifying the values of this class
    /// (e.g., DisplayName, value, Category, etc.).
    /// </summary>
    public class CustomPropertyItem : PropertyItemBase
    {
        public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register("Category", typeof(string), typeof(CustomPropertyItem), new UIPropertyMetadata(null));
        private bool _isCategoryExpanded = true;
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(CustomPropertyItem), new UIPropertyMetadata(null, new PropertyChangedCallback(CustomPropertyItem.OnValueChanged), new CoerceValueCallback(CustomPropertyItem.OnCoerceValueChanged)));

        public string Category
        {
            get
            {
                return (string)base.GetValue(CustomPropertyItem.CategoryProperty);
            }
            set
            {
                base.SetValue(CustomPropertyItem.CategoryProperty, value);
            }
        }

        public bool IsCategoryExpanded
        {
            get
            {
                return this._isCategoryExpanded;
            }
            internal set
            {
                this._isCategoryExpanded = value;
            }
        }

        public object Value
        {
            get
            {
                return base.GetValue(CustomPropertyItem.ValueProperty);
            }
            set
            {
                base.SetValue(CustomPropertyItem.ValueProperty, value);
            }
        }

        public CustomPropertyItem()
        {
            base.ContainerHelper = new PropertiesContainerHelper(this);
        }

        private static object OnCoerceValueChanged(DependencyObject o, object baseValue)
        {
            CustomPropertyItem customPropertyItem = o as CustomPropertyItem;
            if (customPropertyItem != null)
            {
                return customPropertyItem.OnCoerceValueChanged(baseValue);
            }
            return baseValue;
        }

        protected virtual object OnCoerceValueChanged(object baseValue)
        {
            return baseValue;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CustomPropertyItem customPropertyItem = o as CustomPropertyItem;
            if (customPropertyItem != null)
            {
                customPropertyItem.OnValueChanged(e.OldValue, e.NewValue);
            }
        }

        protected virtual void OnValueChanged(object oldValue, object newValue)
        {
            if (base.IsInitialized)
            {
                base.RaiseEvent(new PropertyValueChangedEventArgs(PropertyGrid.PropertyValueChangedEvent, this, oldValue, newValue));
            }
        }
    }
}