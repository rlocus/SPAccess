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
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(CustomPropertyItem), new UIPropertyMetadata(false));
        public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register("Category", typeof(string), typeof(CustomPropertyItem), new UIPropertyMetadata(null));
        private bool _isCategoryExpanded = true;
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(CustomPropertyItem), new UIPropertyMetadata(null, new PropertyChangedCallback(CustomPropertyItem.OnValueChanged), new CoerceValueCallback(CustomPropertyItem.OnCoerceValueChanged)));

        private int _categoryOrder;
        public static readonly DependencyProperty PropertyOrderProperty = DependencyProperty.Register("PropertyOrder", typeof(int), typeof(CustomPropertyItem), new UIPropertyMetadata(0));

        public int CategoryOrder
        {
            get
            {
                return this._categoryOrder;
            }
            internal set
            {
                if (this._categoryOrder != value)
                {
                    this._categoryOrder = value;
                    base.RaisePropertyChanged<int>(() => this.CategoryOrder);
                }
            }
        }

        public int PropertyOrder
        {
            get
            {
                return (int)base.GetValue(PropertyOrderProperty);
            }
            set
            {
                base.SetValue(PropertyOrderProperty, value);
            }
        }

        public string Category
        {
            get
            {
                return (string)base.GetValue(CategoryProperty);
            }
            set
            {
                base.SetValue(CategoryProperty, value);
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

        public bool IsReadOnly
        {
            get
            {
                return (bool)base.GetValue(IsReadOnlyProperty);
            }
            set
            {
                base.SetValue(IsReadOnlyProperty, value);
            }
        }

        public CustomPropertyItem()
        {
            base.ContainerHelper = new PropertiesSourceContainerHelper(this);
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
                base.RaiseEvent(new PropertyValueChangedEventArgs(PropertyGridView.PropertyValueChangedEvent, this, oldValue, newValue));
            }
        }
    }
}