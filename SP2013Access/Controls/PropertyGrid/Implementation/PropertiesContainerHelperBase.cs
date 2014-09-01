using SP2013Access.Controls.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace SP2013Access.Controls.PropertyGrid
{
    internal abstract class PropertiesContainerHelperBase : ContainerHelperBase
    {
        private PropertiesCollectionView _propertyItems;

        protected PropertiesCollectionView CollectionView
        {
            get
            {
                return this._propertyItems;
            }
            set
            {
                this._propertyItems = value;
                this.UpdateCategorization();
            }
        }

        public override IList Properties
        {
            get
            {
                return this._propertyItems;
            }
        }

        protected PropertiesContainerHelperBase(IPropertyContainer propertyContainer)
            : base(propertyContainer)
        {
        }

        public override void PrepareChildrenPropertyItem(PropertyItemBase propertyItem, object item)
        {
            base.PrepareChildrenPropertyItem(propertyItem, item);
            CustomPropertyItem customPropertyItem = propertyItem as CustomPropertyItem;
            if (customPropertyItem != null)
            {
                this.PrepareCustomPropertyItemCore(customPropertyItem);
                return;
            }
            this.PrepareChildrenPropertyItemCore(propertyItem, item);
        }

        public override void ClearChildrenPropertyItem(PropertyItemBase propertyItem, object item)
        {
            Binding binding = BindingOperations.GetBinding(propertyItem, PropertyItemBase.DisplayNameProperty);
            if (binding != null && binding == this.PropertyContainer.PropertyNameBinding)
            {
                BindingOperations.ClearBinding(propertyItem, PropertyItemBase.DisplayNameProperty);
            }
            if (propertyItem.Editor != null && ContainerHelperBase.GetIsGenerated(propertyItem.Editor))
            {
                propertyItem.ClearValue(PropertyItemBase.EditorProperty);
            }
            base.ClearChildrenPropertyItem(propertyItem, item);
        }

        public override Binding CreateChildrenDefaultBinding(PropertyItemBase propertyItem)
        {
            CustomPropertyItem customPropertyItem = propertyItem as CustomPropertyItem;
            if (customPropertyItem != null)
            {
                return this.CreateCustomPropertyBinding(customPropertyItem);
            }
            return this.PropertyContainer.PropertyValueBinding;
        }

        protected override void OnFilterChanged()
        {
            this.UpdateFilter();
        }

        protected override void OnCategorizationChanged()
        {
            this.UpdateCategorization();
        }

        protected override void OnEditorDefinitionsChanged()
        {
            this.ReprepareItems();
        }

        protected override void OnPropertyDefinitionsChanged()
        {
            this.ReprepareItems();
        }

        public override void NotifyEditorDefinitionsCollectionChanged()
        {
            this.ReprepareItems();
        }

        public override void NotifyPropertyDefinitionsCollectionChanged()
        {
        }

        public override void UpdateValuesFromSource()
        {
            throw new InvalidOperationException("This operation is not supported when using a list property source (e.g., PropertiesSource).");
        }

        private void ReprepareItems()
        {
            this.CollectionView.Refresh();
        }

        public override PropertyItemBase ContainerFromItem(object item)
        {
            if (base.ChildrenItemsControl == null)
            {
                return null;
            }
            return (PropertyItemBase)base.ChildrenItemsControl.ItemContainerGenerator.ContainerFromItem(item);
        }

        public override object ItemFromContainer(PropertyItemBase container)
        {
            if (base.ChildrenItemsControl == null)
            {
                return null;
            }
            return base.ChildrenItemsControl.ItemContainerGenerator.ItemFromContainer(container);
        }

        private void PrepareChildrenPropertyItemCore(PropertyItemBase propertyItem, object item)
        {
            //object obj = null;
            if (propertyItem.DisplayName == null && this.PropertyContainer.PropertyNameBinding != null)
            {
                BindingOperations.SetBinding(propertyItem, PropertyItemBase.DisplayNameProperty, this.PropertyContainer.PropertyNameBinding);
                //obj = propertyItem.DisplayName;
            }
            if (propertyItem.Editor == null)
            {
                //object definitionKey = propertyItem.DefinitionKey;
                //Type type = definitionKey as Type;
                //if (propertyItem.Editor == null && definitionKey != null)
                //{
                //    propertyItem.Editor = base.GenerateCustomEditingElement(definitionKey, propertyItem);
                //}
                //if (propertyItem.Editor == null && type != null)
                //{
                //    propertyItem.Editor = (base.GenerateCustomEditingElement(type, propertyItem) ?? this.GenerateSystemDefaultEditingElement(type, propertyItem));
                //}
                //if (propertyItem.Editor == null && definitionKey == null)
                //{
                //    if (obj == null && this.PropertyContainer.PropertyNameBinding != null && item != null)
                //    {
                //        obj = GeneralUtilities.GetBindingValue(item, this.PropertyContainer.PropertyNameBinding);
                //    }
                //    if (obj != null)
                //    {
                //        propertyItem.Editor = base.GenerateCustomEditingElement(obj, propertyItem);
                //    }
                //}
                //if (propertyItem.Editor == null && type == null)
                //{
                //    if (item != null && this.PropertyContainer.PropertyValueBinding != null)
                //    {
                //        object bindingValue = GeneralUtilities.GetBindingValue(item, this.CreateChildrenDefaultBinding(propertyItem));
                //        if (bindingValue != null)
                //        {
                //            Type type2 = bindingValue.GetType();
                //            propertyItem.Editor = (base.GenerateCustomEditingElement(type2, propertyItem) ?? this.GenerateSystemDefaultEditingElement(type2, propertyItem));
                //        }
                //    }
                //    else
                //    {
                //        propertyItem.Editor = this.GenerateDefaultEditingElement(propertyItem);
                //    }
                //}
                //if (propertyItem.Editor == null)
                //{
                //    propertyItem.Editor = this.GenerateDefaultEditingElement(propertyItem);
                //}
                //if (propertyItem.Editor == null)
                //{
                //    propertyItem.Editor = this.GenerateSystemDefaultEditingElement(propertyItem);
                //}
                //ContainerHelperBase.SetIsGenerated(propertyItem.Editor, true);
            }
        }

        private void PrepareCustomPropertyItemCore(CustomPropertyItem customProperty)
        {
            if (customProperty.Editor == null)
            {
                object obj = customProperty.DefinitionKey;
                Type type = obj as Type;
                if (obj == null && customProperty.Value != null)
                {
                    type = customProperty.Value.GetType();
                    obj = type;
                }
                if (obj != null)
                {
                    customProperty.Editor = base.GenerateCustomEditingElement(obj, customProperty);
                }
                if (customProperty.Editor == null && type != null)
                {
                    customProperty.Editor = base.GenerateCustomEditingElement(type, customProperty);
                }
                if (customProperty.Editor == null && type != null)
                {
                    //customProperty.Editor = this.GenerateSystemDefaultEditingElement(type, customProperty);
                }
                if (customProperty.Editor == null)
                {
                    customProperty.Editor = this.GenerateDefaultEditingElement(customProperty);
                }
                if (customProperty.Editor == null)
                {
                    //customProperty.Editor = this.GenerateSystemDefaultEditingElement(customProperty);
                }
                if (customProperty.Editor != null)
                {
                    ContainerHelperBase.SetIsGenerated(customProperty.Editor, true);
                }
            }
        }

        private Binding CreateCustomPropertyBinding(CustomPropertyItem customProperty)
        {
            return new Binding
            {
                Source = customProperty,
                Path = new PropertyPath(CustomPropertyItem.ValueProperty),
                Mode = BindingMode.TwoWay
            };
        }

        private void UpdateFilter()
        {
            if (!this.CollectionView.CanFilter)
            {
                return;
            }
            FilterInfo filterInfo = this.PropertyContainer.FilterInfo;
            if (filterInfo.Predicate != null)
            {
                this.CollectionView.Filter = filterInfo.Predicate;
                return;
            }
            Predicate<object> filter = null;
            Binding nameBinding = this.PropertyContainer.PropertyNameBinding;
            if (!string.IsNullOrEmpty(filterInfo.InputString))
            {
                if (nameBinding != null)
                {
                    filter = delegate(object item)
                    {
                        string text = GeneralUtilities.GetBindingValue(item, nameBinding) as string;
                        return text != null && text.StartsWith(filterInfo.InputString, StringComparison.CurrentCultureIgnoreCase);
                    };
                }
                else
                {
                    filter = delegate(object item)
                    {
                        PropertyItemBase propertyItemBase = item as PropertyItemBase;
                        return propertyItemBase != null && propertyItemBase.DisplayName != null && propertyItemBase.DisplayName.StartsWith(filterInfo.InputString, StringComparison.CurrentCultureIgnoreCase);
                    };
                }
            }
            this.CollectionView.Filter = filter;
        }

        private void UpdateCategorization()
        {
            if (!this.CollectionView.CanGroup)
            {
                return;
            }
            if (this.CollectionView.GroupDescriptions.Count > 0)
            {
                this.CollectionView.GroupDescriptions.Clear();
            }
            if (this.CollectionView.SortDescriptions.Count > 0)
            {
                this.CollectionView.SortDescriptions.Clear();
            }
            GroupDescription groupDescription = this.ComputeCategoryGroupDescription();
            if (groupDescription != null)
            {
                this.CollectionView.GroupDescriptions.Add(groupDescription);
                PropertyGroupDescription propertyGroupDescription = groupDescription as PropertyGroupDescription;
                if (propertyGroupDescription != null)
                {
                    this.SortBy(propertyGroupDescription.PropertyName, ListSortDirection.Ascending);
                }
            }
            if (this.PropertyContainer.PropertyNameBinding != null)
            {
                string path = this.PropertyContainer.PropertyNameBinding.Path.Path;
                this.SortBy(path, ListSortDirection.Ascending);
                return;
            }
            this.SortBy(PropertyItemBase.DisplayNameProperty.Name, ListSortDirection.Ascending);
        }

        private GroupDescription ComputeCategoryGroupDescription()
        {
            if (!this.PropertyContainer.IsCategorized)
            {
                return null;
            }
            return this.PropertyContainer.CategoryGroupDescription;
        }

        private void SortBy(string name, ListSortDirection sortDirection)
        {
            this.CollectionView.SortDescriptions.Add(new SortDescription(name, sortDirection));
        }

        protected FrameworkElement GenerateDefaultEditingElement(PropertyItemBase propertyItem)
        {
            if (this.PropertyContainer.DefaultEditorDefinition == null)
            {
                return null;
            }
            return base.CreateCustomEditor(this.PropertyContainer.DefaultEditorDefinition, propertyItem);
        }

        //protected FrameworkElement GenerateSystemDefaultEditingElement(Type type, PropertyItemBase propertyItem)
        //{
        //    return PropertyGridUtilities.GenerateSystemDefaultEditingElement(type, propertyItem);
        //}
        //protected FrameworkElement GenerateSystemDefaultEditingElement(PropertyItemBase propertyItem)
        //{
        //    return PropertyGridUtilities.GenerateSystemDefaultEditingElement(propertyItem);
        //}
    }
}