using SP2013Access.Controls.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace SP2013Access.Controls.PropertyGrid
{
    internal abstract class ContainerHelperBase
    {
        protected readonly IPropertyContainer PropertyContainer;
        internal static readonly DependencyProperty IsGeneratedProperty = DependencyProperty.RegisterAttached("IsGenerated", typeof(bool), typeof(ContainerHelperBase), new PropertyMetadata(false));

        public abstract IList Properties
        {
            get;
        }

        internal ItemsControl ChildrenItemsControl
        {
            get;
            set;
        }

        public ContainerHelperBase(IPropertyContainer propertyContainer)
        {
            if (propertyContainer == null)
            {
                throw new ArgumentNullException("propertyContainer");
            }
            this.PropertyContainer = propertyContainer;
            INotifyPropertyChanged notifyPropertyChanged = propertyContainer as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
            {
                notifyPropertyChanged.PropertyChanged += new PropertyChangedEventHandler(this.OnPropertyContainerPropertyChanged);
            }
        }

        internal static bool GetIsGenerated(DependencyObject obj)
        {
            return (bool)obj.GetValue(ContainerHelperBase.IsGeneratedProperty);
        }

        internal static void SetIsGenerated(DependencyObject obj, bool value)
        {
            obj.SetValue(ContainerHelperBase.IsGeneratedProperty, value);
        }

        public virtual void ClearHelper()
        {
            INotifyPropertyChanged notifyPropertyChanged = this.PropertyContainer as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
            {
                notifyPropertyChanged.PropertyChanged -= new PropertyChangedEventHandler(this.OnPropertyContainerPropertyChanged);
            }
            if (this.ChildrenItemsControl != null)
            {
                ((IItemContainerGenerator)this.ChildrenItemsControl.ItemContainerGenerator).RemoveAll();
            }
        }

        public virtual void PrepareChildrenPropertyItem(PropertyItemBase propertyItem, object item)
        {
            propertyItem.ParentNode = this.PropertyContainer;
            PropertyGrid.RaisePreparePropertyItemEvent((UIElement)this.PropertyContainer, propertyItem, item);
        }

        public virtual void ClearChildrenPropertyItem(PropertyItemBase propertyItem, object item)
        {
            propertyItem.ParentNode = null;
            PropertyGrid.RaiseClearPropertyItemEvent((UIElement)this.PropertyContainer, propertyItem, item);
        }

        protected FrameworkElement GenerateCustomEditingElement(Type definitionKey, PropertyItemBase propertyItem)
        {
            if (this.PropertyContainer.EditorDefinitions == null)
            {
                return null;
            }
            return this.CreateCustomEditor(this.PropertyContainer.EditorDefinitions.GetRecursiveBaseTypes(definitionKey), propertyItem);
        }

        protected FrameworkElement GenerateCustomEditingElement(object definitionKey, PropertyItemBase propertyItem)
        {
            if (this.PropertyContainer.EditorDefinitions == null)
            {
                return null;
            }
            return this.CreateCustomEditor(this.PropertyContainer.EditorDefinitions[definitionKey], propertyItem);
        }

        protected FrameworkElement CreateCustomEditor(EditorDefinitionBase customEditor, PropertyItemBase propertyItem)
        {
            if (customEditor == null)
            {
                return null;
            }
            return customEditor.GenerateEditingElementInternal(propertyItem);
        }

        protected virtual void OnPropertyContainerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            IPropertyContainer ps = null;
            if (propertyName == ReflectionHelper.GetPropertyOrFieldName<FilterInfo>(() => ps.FilterInfo))
            {
                this.OnFilterChanged();
                return;
            }
            if (propertyName == ReflectionHelper.GetPropertyOrFieldName<bool>(() => ps.IsCategorized))
            {
                this.OnCategorizationChanged();
                return;
            }
            if (this.PropertyContainer.IsCategorized && propertyName == ReflectionHelper.GetPropertyOrFieldName<GroupDescription>(() => ps.CategoryGroupDescription))
            {
                this.OnCategorizationChanged();
                return;
            }
            //if (propertyName == ReflectionHelper.GetPropertyOrFieldName<CategoryDefinitionCollection>(() => ps.CategoryDefinitions))
            //{
            //    this.OnCategoryDefinitionsChanged();
            //    return;
            //}
            if (propertyName == ReflectionHelper.GetPropertyOrFieldName<bool>(() => ps.AutoGenerateProperties))
            {
                this.OnAutoGeneratePropertiesChanged();
                return;
            }
            if (propertyName == ReflectionHelper.GetPropertyOrFieldName<EditorDefinitionCollection>(() => ps.EditorDefinitions))
            {
                this.OnEditorDefinitionsChanged();
                return;
            }
            if (propertyName == ReflectionHelper.GetPropertyOrFieldName<PropertyDefinitionCollection>(() => ps.PropertyDefinitions))
            {
                this.OnPropertyDefinitionsChanged();
            }
        }

        protected virtual void OnCategorizationChanged()
        {
        }

        protected virtual void OnFilterChanged()
        {
        }

        protected virtual void OnAutoGeneratePropertiesChanged()
        {
        }

        protected virtual void OnEditorDefinitionsChanged()
        {
        }

        protected virtual void OnPropertyDefinitionsChanged()
        {
        }

        protected virtual void OnCategoryDefinitionsChanged()
        {
        }

        public virtual void OnEndInit()
        {
        }

        public abstract PropertyItemBase ContainerFromItem(object item);

        public abstract object ItemFromContainer(PropertyItemBase container);

        public abstract Binding CreateChildrenDefaultBinding(PropertyItemBase propertyItem);

        public virtual void NotifyEditorDefinitionsCollectionChanged()
        {
        }

        public virtual void NotifyPropertyDefinitionsCollectionChanged()
        {
        }

        public virtual void NotifyCategoryDefinitionsCollectionChanged()
        {
        }

        public abstract void UpdateValuesFromSource();
    }
}