/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SP2013Access.Controls.PropertyGrid
{
    [TemplatePart(Name = "content", Type = typeof(ContentControl))]
    public class PropertyItem : CustomPropertyItem
    {
        private int _categoryOrder;
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(PropertyItem), new UIPropertyMetadata(false));
        public static readonly DependencyProperty PropertyOrderProperty = DependencyProperty.Register("PropertyOrder", typeof(int), typeof(PropertyItem), new UIPropertyMetadata(0));

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

        public bool IsReadOnly
        {
            get
            {
                return (bool)base.GetValue(PropertyItem.IsReadOnlyProperty);
            }
            set
            {
                base.SetValue(PropertyItem.IsReadOnlyProperty, value);
            }
        }

        public int PropertyOrder
        {
            get
            {
                return (int)base.GetValue(PropertyItem.PropertyOrderProperty);
            }
            set
            {
                base.SetValue(PropertyItem.PropertyOrderProperty, value);
            }
        }

        public PropertyDescriptor PropertyDescriptor
        {
            get;
            internal set;
        }

        public Type PropertyType
        {
            get
            {
                if (this.PropertyDescriptor == null)
                {
                    return null;
                }
                return this.PropertyDescriptor.PropertyType;
            }
        }

        internal DescriptorPropertyDefinitionBase DescriptorDefinition
        {
            get;
            private set;
        }

        public object Instance
        {
            get;
            internal set;
        }

        protected override void OnIsExpandedChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                ObjectContainerHelperBase objectContainerHelperBase = base.ContainerHelper as ObjectContainerHelperBase;
                if (objectContainerHelperBase != null)
                {
                    objectContainerHelperBase.GenerateProperties();
                }
            }
        }

        protected override void OnEditorChanged(FrameworkElement oldValue, FrameworkElement newValue)
        {
            if (oldValue != null)
            {
                oldValue.DataContext = null;
            }
            if (newValue != null)
            {
                newValue.DataContext = this;
            }
        }

        protected override object OnCoerceValueChanged(object baseValue)
        {
            BindingExpression bindingExpression = base.GetBindingExpression(CustomPropertyItem.ValueProperty);
            if (bindingExpression != null && bindingExpression.DataItem is DescriptorPropertyDefinitionBase)
            {
                DescriptorPropertyDefinitionBase element = bindingExpression.DataItem as DescriptorPropertyDefinitionBase;
                if (System.Windows.Controls.Validation.GetHasError(element))
                {
                    ReadOnlyObservableCollection<ValidationError> errors = System.Windows.Controls.Validation.GetErrors(element);
                    System.Windows.Controls.Validation.MarkInvalid(bindingExpression, errors[0]);
                }
            }
            return baseValue;
        }

        protected override void OnValueChanged(object oldValue, object newValue)
        {
            base.OnValueChanged(oldValue, newValue);
        }

        private void OnDefinitionContainerHelperInvalidated(object sender, EventArgs e)
        {
            ObjectContainerHelperBase objectContainerHelperBase = this.DescriptorDefinition.CreateContainerHelper(this);
            base.ContainerHelper = objectContainerHelperBase;
            if (base.IsExpanded)
            {
                objectContainerHelperBase.GenerateProperties();
            }
        }

        internal PropertyItem(DescriptorPropertyDefinitionBase definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }
            this.DescriptorDefinition = definition;
            base.ContainerHelper = definition.CreateContainerHelper(this);
            definition.ContainerHelperInvalidated += new EventHandler(this.OnDefinitionContainerHelperInvalidated);
        }
    }
}