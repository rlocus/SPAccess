using System.Windows;
using System.Windows.Data;

namespace SP2013Access.Controls.PropertyGrid
{
    public abstract class EditorBoundDefinition : EditorDefinitionBase
    {
        public static readonly DependencyProperty EditingElementStyleProperty = DependencyProperty.Register("EditingElementStyle", typeof(Style), typeof(EditorBoundDefinition), new UIPropertyMetadata(null));

        public BindingBase Binding
        {
            get;
            set;
        }

        public Style EditingElementStyle
        {
            get
            {
                return (Style)base.GetValue(EditorBoundDefinition.EditingElementStyleProperty);
            }
            set
            {
                base.SetValue(EditorBoundDefinition.EditingElementStyleProperty, value);
            }
        }

        //protected EditorBoundDefinition()
        //{
        //}

        internal void UpdateBinding(FrameworkElement element, DependencyProperty dependencyProperty, PropertyItemBase propertyItem)
        {
            BindingBase bindingBase = this.Binding ?? PropertyGridUtilities.GetDefaultBinding(propertyItem);
            if (bindingBase == null)
            {
                BindingOperations.ClearBinding(element, dependencyProperty);
                return;
            }
            BindingOperations.SetBinding(element, dependencyProperty, bindingBase);
        }

        internal virtual void UpdateStyle(FrameworkElement element)
        {
            if (this.EditingElementStyle != null)
            {
                element.Style = this.EditingElementStyle;
            }
        }
    }
}