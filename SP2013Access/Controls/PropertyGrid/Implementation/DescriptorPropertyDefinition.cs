﻿using SP2013Access.Controls.PropertyGrid.Attributes;
using SP2013Access.Controls.PropertyGrid.Editors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup.Primitives;

namespace SP2013Access.Controls.PropertyGrid
{
    internal class DescriptorPropertyDefinition : DescriptorPropertyDefinitionBase
    {
        #region Members

        private readonly object _selectedObject;
        private readonly PropertyDescriptor _propertyDescriptor;
        private readonly DependencyPropertyDescriptor _dpDescriptor;
        private readonly MarkupObject _markupObject;

        #endregion Members

        #region Constructor

        internal DescriptorPropertyDefinition(PropertyDescriptor propertyDescriptor, object selectedObject, bool isPropertyGridCategorized)
            : base(isPropertyGridCategorized)
        {
            if (propertyDescriptor == null)
                throw new ArgumentNullException("propertyDescriptor");

            if (selectedObject == null)
                throw new ArgumentNullException("selectedObject");

            _propertyDescriptor = propertyDescriptor;
            _selectedObject = selectedObject;
            _dpDescriptor = DependencyPropertyDescriptor.FromProperty(propertyDescriptor);
            _markupObject = MarkupWriter.GetMarkupObjectFor(SelectedObject);
        }

        #endregion Constructor

        #region Custom Properties

        internal override PropertyDescriptor PropertyDescriptor
        {
            get
            {
                return _propertyDescriptor;
            }
        }

        private object SelectedObject
        {
            get
            {
                return _selectedObject;
            }
        }

        #endregion Custom Properties

        #region Override Methods

        internal override ObjectContainerHelperBase CreateContainerHelper(IPropertyContainer parent)
        {
            return new ObjectContainerHelper(parent, this.Value);
        }

        internal override void OnValueChanged(object oldValue, object newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            this.RaiseContainerHelperInvalidated();
        }

        protected override BindingBase CreateValueBinding()
        {
            //Bind the value property with the source object.
            var binding = new Binding(PropertyDescriptor.Name)
            {
                Source = SelectedObject,
                Mode = PropertyDescriptor.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay,
                ValidatesOnDataErrors = true,
                ValidatesOnExceptions = true
            };

            return binding;
        }

        protected override bool ComputeIsReadOnly()
        {
            return PropertyDescriptor.IsReadOnly;
        }

        internal override ITypeEditor CreateDefaultEditor()
        {
            return PropertyGridUtilities.CreateDefaultEditor(PropertyDescriptor.PropertyType, PropertyDescriptor.Converter);
        }

        protected override bool ComputeCanResetValue()
        {
            return PropertyDescriptor.CanResetValue(SelectedObject)
              && !PropertyDescriptor.IsReadOnly;
        }

        protected override object ComputeAdvancedOptionsTooltip()
        {
            object tooltip;
            UpdateAdvanceOptionsForItem(_markupObject, SelectedObject as DependencyObject, _dpDescriptor, out tooltip);

            return tooltip;
        }

        protected override string ComputeCategory()
        {
            return PropertyDescriptor.Category;
        }

        protected override string ComputeCategoryValue()
        {
            return PropertyDescriptor.Category;
        }

        protected override bool ComputeExpandableAttribute()
        {
            return (bool)this.ComputeExpandableAttributeForItem(PropertyDescriptor);
        }

        protected override bool ComputeIsExpandable()
        {
            return (this.Value != null);
        }

        protected override IList<Type> ComputeNewItemTypes()
        {
            return (IList<Type>)ComputeNewItemTypesForItem(PropertyDescriptor);
        }

        protected override string ComputeDescription()
        {
            return (string)ComputeDescriptionForItem(PropertyDescriptor);
        }

        protected override int ComputeDisplayOrder(bool isPropertyGridCategorized)
        {
            this.IsPropertyGridCategorized = isPropertyGridCategorized;
            return (int)ComputeDisplayOrderForItem(PropertyDescriptor);
        }

        protected override void ResetValue()
        {
            PropertyDescriptor.ResetValue(SelectedObject);
        }

        internal override ITypeEditor CreateAttributeEditor()
        {
            var editorAttribute = GetAttribute<EditorAttribute>();
            if (editorAttribute != null)
            {
                Type type = Type.GetType(editorAttribute.EditorTypeName);

                // If the editor does not have any public parameterless constructor, forget it.
                if (typeof(ITypeEditor).IsAssignableFrom(type)
                  && (type.GetConstructor(new Type[0]) != null))
                {
                    var instance = Activator.CreateInstance(type) as ITypeEditor;
                    Debug.Assert(instance != null, "Type was expected to be ITypeEditor with public constructor.");
                    return instance;
                }
            }

            var itemsSourceAttribute = GetAttribute<ItemsSourceAttribute>();
            if (itemsSourceAttribute != null)
                return new ItemsSourceAttributeEditor(itemsSourceAttribute);

            return null;
        }

        #endregion Override Methods

        #region Private Methods

        private T GetAttribute<T>() where T : Attribute
        {
            return PropertyGridUtilities.GetAttribute<T>(PropertyDescriptor);
        }

        #endregion Private Methods
    }
}