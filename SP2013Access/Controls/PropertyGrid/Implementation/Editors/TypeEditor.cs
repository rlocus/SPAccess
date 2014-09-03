/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System.Windows;
using System.Windows.Data;

namespace SP2013Access.Controls.PropertyGrid.Editors
{
    public abstract class TypeEditor<T> : ITypeEditor where T : FrameworkElement, new()
    {
        #region Properties

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

        #endregion Properties

        #region ITypeEditor Members

        public virtual FrameworkElement ResolveEditor(PropertyItemBase propertyItem)
        {
            Editor = this.CreateEditor();
            SetValueDependencyProperty();
            SetControlProperties();
            ResolveValueBinding(propertyItem);
            return Editor;
        }

        #endregion ITypeEditor Members

        #region Methods

        protected virtual T CreateEditor()
        {
            return new T();
        }

        protected virtual IValueConverter CreateValueConverter()
        {
            return null;
        }

        protected virtual void ResolveValueBinding(PropertyItemBase propertyItem)
        {
            var binding = new Binding("Value");
            binding.Source = propertyItem;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.Default;
            binding.Mode = (propertyItem as CustomPropertyItem).IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            binding.Converter = CreateValueConverter();
            BindingOperations.SetBinding(Editor, ValueProperty, binding);
        }

        protected virtual void SetControlProperties()
        {
            //TODO: implement in derived class
        }

        protected abstract void SetValueDependencyProperty();

        #endregion Methods
    }
}