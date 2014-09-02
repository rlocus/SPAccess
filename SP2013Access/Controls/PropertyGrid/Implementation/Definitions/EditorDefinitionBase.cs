using System.Windows;

namespace SP2013Access.Controls.PropertyGrid
{
    public abstract class EditorDefinitionBase : PropertyDefinitionBase
    {
        internal FrameworkElement GenerateEditingElementInternal(PropertyItemBase propertyItem)
        {
            return this.GenerateEditingElement(propertyItem);
        }

        protected virtual FrameworkElement GenerateEditingElement(PropertyItemBase propertyItem)
        {
            return null;
        }

        internal void UpdateProperty(FrameworkElement element, DependencyProperty elementProp, DependencyProperty definitionProperty)
        {
            object value = base.GetValue(definitionProperty);
            object localValue = base.ReadLocalValue(definitionProperty);
            if (localValue != DependencyProperty.UnsetValue || value != element.GetValue(elementProp))
            {
                element.SetValue(elementProp, value);
                return;
            }
            element.ClearValue(elementProp);
        }
    }
}