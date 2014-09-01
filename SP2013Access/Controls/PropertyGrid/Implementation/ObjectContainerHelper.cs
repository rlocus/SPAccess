/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using SP2013Access.Controls.PropertyGrid.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SP2013Access.Controls.PropertyGrid
{
    internal class ObjectContainerHelper : ObjectContainerHelperBase
    {
        private object _selectedObject;

        private object SelectedObject
        {
            get
            {
                return this._selectedObject;
            }
        }

        public ObjectContainerHelper(IPropertyContainer propertyContainer, object selectedObject)
            : base(propertyContainer)
        {
            this._selectedObject = selectedObject;
        }

        protected override string GetDefaultPropertyName()
        {
            if (this.SelectedObject == null)
            {
                return null;
            }
            return ObjectContainerHelperBase.GetDefaultPropertyName(this.SelectedObject);
        }

        protected override IEnumerable<PropertyItem> GenerateSubPropertiesCore()
        {
            List<PropertyItem> list = new List<PropertyItem>();
            if (this.SelectedObject != null)
            {
                try
                {
                    List<PropertyDescriptor> propertyDescriptors = ObjectContainerHelperBase.GetPropertyDescriptors(this.SelectedObject);
                    foreach (PropertyDescriptor current in propertyDescriptors)
                    {
                        PropertyDefinition propertyDefinition = base.GetPropertyDefinition(current);
                        bool flag = current.IsBrowsable && this.PropertyContainer.AutoGenerateProperties;
                        if (propertyDefinition != null)
                        {
                            flag = propertyDefinition.IsBrowsable.GetValueOrDefault(flag);
                        }
                        if (flag)
                        {
                            list.Add(this.CreatePropertyItem(current, propertyDefinition));
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            return list;
        }

        private PropertyItem CreatePropertyItem(PropertyDescriptor property, PropertyDefinition propertyDef)
        {
            DescriptorPropertyDefinition descriptorPropertyDefinition = new DescriptorPropertyDefinition(property, this.SelectedObject, this.PropertyContainer.IsCategorized);
            descriptorPropertyDefinition.InitProperties();
            base.InitializeDescriptorDefinition(descriptorPropertyDefinition, propertyDef);
            return new PropertyItem(descriptorPropertyDefinition)
            {
                Instance = this.SelectedObject,
                CategoryOrder = this.GetCategoryOrder(descriptorPropertyDefinition.CategoryValue),
                IsCategoryExpanded = this.GetIsCategoryExpanded(descriptorPropertyDefinition.CategoryValue),
                IsExpanded = base.GetIsExpanded(descriptorPropertyDefinition.PropertyDescriptor)
            };
        }

        private int GetCategoryOrder(object categoryValue)
        {
            if (categoryValue == null)
            {
                return 2147483647;
            }
            int result = 2147483647;
            CategoryDefinition categoryDefinition = base.GetCategoryDefinition(categoryValue);
            if (categoryDefinition != null && categoryDefinition.DisplayOrder.HasValue)
            {
                result = categoryDefinition.DisplayOrder.Value;
            }
            else
            {
                object selectedObject = this.SelectedObject;
                CategoryOrderAttribute[] source = (selectedObject != null) ? ((CategoryOrderAttribute[])selectedObject.GetType().GetCustomAttributes(typeof(CategoryOrderAttribute), true)) : new CategoryOrderAttribute[0];
                CategoryOrderAttribute categoryOrderAttribute = source.FirstOrDefault((CategoryOrderAttribute a) => object.Equals(a.CategoryValue, categoryValue));
                if (categoryOrderAttribute != null)
                {
                    result = categoryOrderAttribute.Order;
                }
            }
            return result;
        }

        private bool GetIsCategoryExpanded(object categoryValue)
        {
            if (categoryValue == null)
            {
                return true;
            }
            object selectedObject = this.SelectedObject;
            ExpandedCategoryAttribute[] source = (selectedObject != null) ? ((ExpandedCategoryAttribute[])selectedObject.GetType().GetCustomAttributes(typeof(ExpandedCategoryAttribute), true)) : new ExpandedCategoryAttribute[0];
            ExpandedCategoryAttribute expandedCategoryAttribute = source.FirstOrDefault((ExpandedCategoryAttribute a) => object.Equals(a.CategoryValue, categoryValue));
            return expandedCategoryAttribute == null || expandedCategoryAttribute.IsExpanded;
        }
    }
}