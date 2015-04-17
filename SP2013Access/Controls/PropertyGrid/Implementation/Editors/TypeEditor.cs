using System;
using System.Windows;
using System.Windows.Data;

namespace SP2013Access.Controls.PropertyGrid.Editors
{
    public abstract class TypeEditor<T> : ITypeEditor where T : FrameworkElement, new()
    {
        protected T Editor
        {
            get;
            set;
        }
        protected DependencyProperty ValueProperty
        {
            get;
            set;
        }
        public virtual FrameworkElement ResolveEditor(PropertyItemBase propertyItem)
        {
            this.Editor = this.CreateEditor();
            this.SetValueDependencyProperty();
            this.SetControlProperties();
            this.ResolveValueBinding((CustomPropertyItem)propertyItem);
            return this.Editor;
        }
        protected virtual T CreateEditor()
        {
            return Activator.CreateInstance<T>();
        }
        protected virtual IValueConverter CreateValueConverter()
        {
            return null;
        }
        protected virtual void ResolveValueBinding(PropertyItemBase propertyItem)
        {
            Binding binding = new Binding("Value");
            binding.Source = propertyItem;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.Default;
            binding.Mode = ((CustomPropertyItem) propertyItem).IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            binding.Converter = this.CreateValueConverter();
            BindingOperations.SetBinding(this.Editor, this.ValueProperty, binding);
        }
        protected virtual void SetControlProperties()
        {
        }
        protected abstract void SetValueDependencyProperty();
    }
}