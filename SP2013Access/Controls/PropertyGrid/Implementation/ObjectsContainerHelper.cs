using SP2013Access.Controls.PropertyGrid.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SP2013Access.Controls.PropertyGrid
{
    internal class ObjectsContainerHelper : ObjectContainerHelperBase
    {
        private readonly IEnumerable _selectedObjects;

        private IEnumerable<object> SelectedObjects
        {
            get
            {
                return this._selectedObjects.Cast<object>();
            }
        }

        public ObjectsContainerHelper(IPropertyContainer propertyContainer, IEnumerable selectedObjects)
            : base(propertyContainer)
        {
            if (selectedObjects == null)
            {
                throw new ArgumentNullException("selectedObjects");
            }
            this._selectedObjects = selectedObjects;
        }

        protected override string GetDefaultPropertyName()
        {
            object obj;
            PropertyGridUtilities.IsSameForAllObject(this.SelectedObjects, ObjectContainerHelperBase.GetDefaultPropertyName, out obj);
            if (obj == null)
            {
                return null;
            }
            return obj as string;
        }

        protected override IEnumerable<PropertyItem> GenerateSubPropertiesCore()
        {
            List<PropertyItem> list = new List<PropertyItem>();
            try
            {
                List<List<PropertyDescriptor>> propertyDescriptorsList = this.GetPropertyDescriptorsList();
                foreach (List<PropertyDescriptor> current in propertyDescriptorsList)
                {
                    object obj;
                    if (PropertyGridUtilities.IsSameForAllObject(current, (object o) => ((PropertyDescriptor)o).PropertyType, out obj))
                    {
                        PropertyDescriptor descriptor = current.First<PropertyDescriptor>();
                        PropertyDefinition propertyDefinition = base.GetPropertyDefinition(descriptor);
                        bool flag = false;
                        if (this.PropertyContainer.AutoGenerateProperties)
                        {
                            object obj2;
                            if (PropertyGridUtilities.IsSameForAllObject(current, (object o) => ((PropertyDescriptor)o).IsBrowsable, out obj2))
                            {
                                flag = (bool)obj2;
                            }
                        }
                        if (propertyDefinition != null)
                        {
                            flag = propertyDefinition.IsBrowsable.GetValueOrDefault(flag);
                        }
                        if (flag)
                        {
                            list.Add(this.CreateCommonPropertyItem(current, propertyDefinition));
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return list;
        }

        private List<List<PropertyDescriptor>> GetPropertyDescriptorsList()
        {
            new List<PropertyDescriptorCollection>();
            List<List<PropertyDescriptor>> list = this.SelectedObjects.Select(new Func<object, List<PropertyDescriptor>>(ObjectContainerHelperBase.GetPropertyDescriptors)).ToList<List<PropertyDescriptor>>();
            List<List<PropertyDescriptor>> list2 = new List<List<PropertyDescriptor>>();
            foreach (PropertyDescriptor pd in list.First<List<PropertyDescriptor>>())
            {
                if (list.All((List<PropertyDescriptor> x) => x.Contains(pd)))
                {
                    list2.Add((
                        from dList in list
                        select dList.Find(new Predicate<PropertyDescriptor>(pd.Equals))).ToList<PropertyDescriptor>());
                }
            }
            return list2;
        }

        private PropertyItem CreateCommonPropertyItem(List<PropertyDescriptor> propertyList, PropertyDefinition propertyDef)
        {
            CommonDescriptorPropertyDefinition commonDescriptorPropertyDefinition = new CommonDescriptorPropertyDefinition(propertyList, this.SelectedObjects.ToList<object>(), this.PropertyContainer.IsCategorized);
            commonDescriptorPropertyDefinition.InitProperties();
            base.InitializeDescriptorDefinition(commonDescriptorPropertyDefinition, propertyDef);
            return new PropertyItem(commonDescriptorPropertyDefinition)
            {
                CategoryOrder = this.GetCategoryOrder(commonDescriptorPropertyDefinition.CategoryValue),
                IsCategoryExpanded = this.GetIsCategoryExpanded(commonDescriptorPropertyDefinition.CategoryValue),
                IsExpanded = this.GetIsExpanded(commonDescriptorPropertyDefinition.PropertyDescriptors)
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
                object obj;
                if (PropertyGridUtilities.IsSameForAllObject(this.SelectedObjects, delegate(object o)
                {
                    CategoryOrderAttribute[] source = (o != null) ? ((CategoryOrderAttribute[])o.GetType().GetCustomAttributes(typeof(CategoryOrderAttribute), true)) : new CategoryOrderAttribute[0];
                    CategoryOrderAttribute categoryOrderAttribute = source.FirstOrDefault((CategoryOrderAttribute a) => object.Equals(a.CategoryValue, categoryValue));
                    return (categoryOrderAttribute != null) ? categoryOrderAttribute.Order : 2147483647;
                }, out obj))
                {
                    result = (int)obj;
                }
            }
            return result;
        }

        private bool GetIsCategoryExpanded(object categoryValue)
        {
            object obj;
            return categoryValue == null || !PropertyGridUtilities.IsSameForAllObject(this.SelectedObjects, delegate(object o)
            {
                ExpandedCategoryAttribute[] source = (o != null) ? ((ExpandedCategoryAttribute[])o.GetType().GetCustomAttributes(typeof(ExpandedCategoryAttribute), true)) : new ExpandedCategoryAttribute[0];
                ExpandedCategoryAttribute expandedCategoryAttribute = source.FirstOrDefault((ExpandedCategoryAttribute a) => object.Equals(a.CategoryValue, categoryValue));
                if (expandedCategoryAttribute != null)
                {
                    return expandedCategoryAttribute.IsExpanded;
                }
                return true;
            }, out obj) || (bool)obj;
        }

        private bool GetIsExpanded(List<PropertyDescriptor> commonDescriptorsLists)
        {
            object obj;
            return PropertyGridUtilities.IsSameForAllObject(commonDescriptorsLists, (o) => base.GetIsExpanded((PropertyDescriptor)o), out obj) && (bool)obj;
        }
    }
}