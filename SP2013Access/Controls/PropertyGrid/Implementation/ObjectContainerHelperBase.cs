//using SP2013Access.Controls.PropertyGrid.Attributes;
//using SP2013Access.Controls.PropertyGrid.Editors;
//using SP2013Access.Controls.Utilities;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Windows;
//using System.Windows.Controls.Primitives;
//using System.Windows.Data;
//using System.Windows.Input;
//using System.Windows.Media;

//namespace SP2013Access.Controls.PropertyGrid
//{
//    internal abstract class ObjectContainerHelperBase : ContainerHelperBase
//    {
//        private bool _isPreparingItemFlag;
//        private readonly PropertyItemCollection _propertyItemCollection;

//        public override IList Properties
//        {
//            get
//            {
//                return this._propertyItemCollection;
//            }
//        }

//        private PropertyItem DefaultProperty
//        {
//            get
//            {
//                PropertyItem result = null;
//                string defaultName = this.GetDefaultPropertyName();
//                if (defaultName != null)
//                {
//                    result = this._propertyItemCollection.FirstOrDefault((PropertyItem prop) => object.Equals(defaultName, prop.PropertyDescriptor.Name));
//                }
//                return result;
//            }
//        }

//        protected PropertyItemCollection PropertyItems
//        {
//            get
//            {
//                return this._propertyItemCollection;
//            }
//        }

//        protected ObjectContainerHelperBase(IPropertyContainer propertyContainer)
//            : base(propertyContainer)
//        {
//            this._propertyItemCollection = new PropertyItemCollection(new ObservableCollection<PropertyItem>());
//            this.UpdateFilter();
//            this.UpdateCategorization();
//        }

//        public override PropertyItemBase ContainerFromItem(object item)
//        {
//            if (item == null)
//            {
//                return null;
//            }
//            PropertyItem propertyItem = item as PropertyItem;
//            if (propertyItem != null)
//            {
//                return propertyItem;
//            }
//            string propertyStr = item as string;
//            if (propertyStr != null)
//            {
//                return this.PropertyItems.FirstOrDefault((PropertyItem prop) => propertyStr == prop.PropertyDescriptor.Name);
//            }
//            return null;
//        }

//        public override object ItemFromContainer(PropertyItemBase container)
//        {
//            PropertyItem propertyItem = container as PropertyItem;
//            if (propertyItem == null)
//            {
//                return null;
//            }
//            return propertyItem.PropertyDescriptor.Name;
//        }

//        public override void UpdateValuesFromSource()
//        {
//            foreach (PropertyItem current in this.PropertyItems)
//            {
//                current.DescriptorDefinition.UpdateValueFromSource();
//                current.ContainerHelper.UpdateValuesFromSource();
//            }
//        }

//        public void GenerateProperties()
//        {
//            if (this.PropertyItems.Count == 0)
//            {
//                this.RegenerateProperties();
//            }
//        }

//        protected override void OnFilterChanged()
//        {
//            this.UpdateFilter();
//        }

//        protected override void OnCategorizationChanged()
//        {
//            this.UpdateCategorization();
//        }

//        protected override void OnAutoGeneratePropertiesChanged()
//        {
//            this.RegenerateProperties();
//        }

//        protected override void OnEditorDefinitionsChanged()
//        {
//            this.RegenerateProperties();
//        }

//        protected override void OnPropertyDefinitionsChanged()
//        {
//            this.RegenerateProperties();
//        }

//        protected override void OnCategoryDefinitionsChanged()
//        {
//            this.RegenerateProperties();
//        }

//        public override void NotifyCategoryDefinitionsCollectionChanged()
//        {
//            this.RegenerateProperties();
//        }

//        private void UpdateFilter()
//        {
//            FilterInfo filterInfo = this.PropertyContainer.FilterInfo;
//            this.PropertyItems.FilterPredicate = (filterInfo.Predicate ?? PropertyItemCollection.CreateFilter(filterInfo.InputString));
//        }

//        private void UpdateCategorization()
//        {
//            this._propertyItemCollection.UpdateCategorization(this.ComputeCategoryGroupDescription(), this.PropertyContainer.IsCategorized);
//        }

//        private GroupDescription ComputeCategoryGroupDescription()
//        {
//            if (!this.PropertyContainer.IsCategorized)
//            {
//                return null;
//            }
//            return this.PropertyContainer.CategoryGroupDescription ?? new PropertyGroupDescription(PropertyItemCollection.CategoryPropertyName);
//        }

//        private string GetCategoryGroupingPropertyName()
//        {
//            PropertyGroupDescription propertyGroupDescription = this.ComputeCategoryGroupDescription() as PropertyGroupDescription;
//            if (propertyGroupDescription == null)
//            {
//                return null;
//            }
//            return propertyGroupDescription.PropertyName;
//        }

//        private void OnChildrenPropertyChanged(object sender, PropertyChangedEventArgs e)
//        {
//            if ((ObjectContainerHelperBase.IsItemOrderingProperty(e.PropertyName) || this.GetCategoryGroupingPropertyName() == e.PropertyName) && base.ChildrenItemsControl.ItemContainerGenerator.Status != GeneratorStatus.GeneratingContainers && !this._isPreparingItemFlag)
//            {
//                this.PropertyItems.RefreshView();
//            }
//        }

//        protected abstract string GetDefaultPropertyName();

//        protected abstract IEnumerable<PropertyItem> GenerateSubPropertiesCore();

//        private void RegenerateProperties()
//        {
//            IEnumerable<PropertyItem> enumerable = this.GenerateSubPropertiesCore();
//            foreach (PropertyItem current in enumerable)
//            {
//                this.InitializePropertyItem(current);
//            }
//            foreach (PropertyItem current2 in this.PropertyItems)
//            {
//                current2.PropertyChanged -= new PropertyChangedEventHandler(this.OnChildrenPropertyChanged);
//            }
//            this.PropertyItems.UpdateItems(enumerable);
//            foreach (PropertyItem current3 in this.PropertyItems)
//            {
//                current3.PropertyChanged += new PropertyChangedEventHandler(this.OnChildrenPropertyChanged);
//            }
//            PropertyGrid propertyGrid = this.PropertyContainer as PropertyGrid;
//            if (propertyGrid != null)
//            {
//                propertyGrid.SelectedPropertyItem = this.DefaultProperty;
//            }
//        }

//        protected static List<PropertyDescriptor> GetPropertyDescriptors(object instance)
//        {
//            TypeConverter converter = TypeDescriptor.GetConverter(instance);
//            PropertyDescriptorCollection properties;
//            if (!converter.GetPropertiesSupported())
//            {
//                if (instance is ICustomTypeDescriptor)
//                {
//                    properties = ((ICustomTypeDescriptor)instance).GetProperties();
//                }
//                else
//                {
//                    properties = TypeDescriptor.GetProperties(instance.GetType());
//                }
//            }
//            else
//            {
//                properties = converter.GetProperties(instance);
//            }
//            if (properties == null)
//            {
//                return null;
//            }
//            return properties.Cast<PropertyDescriptor>().ToList<PropertyDescriptor>();
//        }

//        protected bool GetIsExpanded(PropertyDescriptor propertyDescriptor)
//        {
//            if (propertyDescriptor == null)
//            {
//                return false;
//            }
//            ExpandableObjectAttribute attribute = PropertyGridUtilities.GetAttribute<ExpandableObjectAttribute>(propertyDescriptor);
//            return attribute != null && attribute.IsExpanded;
//        }

//        internal void InitializeDescriptorDefinition(DescriptorPropertyDefinitionBase descriptorDef, PropertyDefinition propertyDefinition)
//        {
//            if (descriptorDef == null)
//            {
//                throw new ArgumentNullException("descriptorDef");
//            }
//            if (propertyDefinition == null)
//            {
//                return;
//            }
//            if (propertyDefinition != null)
//            {
//                if (propertyDefinition.Category != null)
//                {
//                    descriptorDef.Category = propertyDefinition.Category;
//                    descriptorDef.CategoryValue = propertyDefinition.Category;
//                }
//                if (propertyDefinition.Description != null)
//                {
//                    descriptorDef.Description = propertyDefinition.Description;
//                }
//                if (propertyDefinition.DisplayName != null)
//                {
//                    descriptorDef.DisplayName = propertyDefinition.DisplayName;
//                }
//                if (propertyDefinition.DisplayOrder.HasValue)
//                {
//                    descriptorDef.DisplayOrder = propertyDefinition.DisplayOrder.Value;
//                }
//                if (propertyDefinition.IsExpandable.HasValue)
//                {
//                    descriptorDef.ExpandableAttribute = propertyDefinition.IsExpandable.Value;
//                }
//            }
//        }

//        private void InitializePropertyItem(PropertyItem propertyItem)
//        {
//            DescriptorPropertyDefinitionBase pd = propertyItem.DescriptorDefinition;
//            propertyItem.PropertyDescriptor = pd.PropertyDescriptor;
//            propertyItem.IsReadOnly = pd.IsReadOnly;
//            propertyItem.DisplayName = pd.DisplayName;
//            propertyItem.Description = pd.Description;
//            propertyItem.DefinitionKey = pd.DefinitionKey;
//            propertyItem.Category = pd.Category;
//            propertyItem.PropertyOrder = pd.DisplayOrder;
//            this.SetupDefinitionBinding<bool>(propertyItem, PropertyItemBase.IsExpandableProperty, pd, () => pd.IsExpandable, BindingMode.OneWay);
//            this.SetupDefinitionBinding<ImageSource>(propertyItem, PropertyItemBase.AdvancedOptionsIconProperty, pd, () => pd.AdvancedOptionsIcon, BindingMode.OneWay);
//            this.SetupDefinitionBinding<object>(propertyItem, PropertyItemBase.AdvancedOptionsTooltipProperty, pd, () => pd.AdvancedOptionsTooltip, BindingMode.OneWay);
//            this.SetupDefinitionBinding<object>(propertyItem, CustomPropertyItem.ValueProperty, pd, () => pd.Value, BindingMode.TwoWay);
//            if (pd.CommandBindings != null)
//            {
//                foreach (CommandBinding current in pd.CommandBindings)
//                {
//                    propertyItem.CommandBindings.Add(current);
//                }
//            }
//        }

//        private void SetupDefinitionBinding<T>(PropertyItem propertyItem, DependencyProperty itemProperty, DescriptorPropertyDefinitionBase pd, Expression<Func<T>> definitionProperty, BindingMode bindingMode)
//        {
//            string propertyOrFieldName = ReflectionHelper.GetPropertyOrFieldName<T>(definitionProperty);
//            Binding binding = new Binding(propertyOrFieldName)
//            {
//                Source = pd,
//                Mode = bindingMode
//            };
//            propertyItem.SetBinding(itemProperty, binding);
//        }

//        private FrameworkElement GenerateChildrenEditorElement(PropertyItem propertyItem)
//        {
//            FrameworkElement frameworkElement = null;
//            DescriptorPropertyDefinitionBase descriptorDefinition = propertyItem.DescriptorDefinition;
//            object definitionKey = propertyItem.DefinitionKey;
//            Type type = definitionKey as Type;
//            ITypeEditor typeEditor = null;
//            if (descriptorDefinition.IsReadOnly)
//            {
//                typeEditor = new TextBlockEditor();
//            }
//            if (typeEditor == null)
//            {
//                typeEditor = descriptorDefinition.CreateAttributeEditor();
//            }
//            if (typeEditor != null)
//            {
//                frameworkElement = typeEditor.ResolveEditor(propertyItem);
//            }
//            if (frameworkElement == null && definitionKey != null)
//            {
//                frameworkElement = base.GenerateCustomEditingElement(definitionKey, propertyItem);
//            }
//            if (frameworkElement == null && type != null)
//            {
//                frameworkElement = base.GenerateCustomEditingElement(type, propertyItem);
//            }
//            if (frameworkElement == null && definitionKey == null)
//            {
//                frameworkElement = base.GenerateCustomEditingElement(propertyItem.PropertyDescriptor.Name, propertyItem);
//            }
//            if (frameworkElement == null && type == null)
//            {
//                frameworkElement = base.GenerateCustomEditingElement(propertyItem.PropertyType, propertyItem);
//            }
//            if (frameworkElement == null)
//            {
//                if (typeEditor == null)
//                {
//                    typeEditor = ((type != null) ? PropertyGridUtilities.CreateDefaultEditor(type, null) : descriptorDefinition.CreateDefaultEditor());
//                }
//                frameworkElement = typeEditor.ResolveEditor(propertyItem);
//            }
//            return frameworkElement;
//        }

//        internal PropertyDefinition GetPropertyDefinition(PropertyDescriptor descriptor)
//        {
//            PropertyDefinition propertyDefinition = null;
//            PropertyDefinitionCollection propertyDefinitions = this.PropertyContainer.PropertyDefinitions;
//            if (propertyDefinitions != null)
//            {
//                propertyDefinition = propertyDefinitions[descriptor.Name];
//                if (propertyDefinition == null)
//                {
//                    propertyDefinition = propertyDefinitions.GetRecursiveBaseTypes(descriptor.PropertyType);
//                }
//            }
//            return propertyDefinition;
//        }

//        internal CategoryDefinition GetCategoryDefinition(object categoryValue)
//        {
//            CategoryDefinitionCollection categoryDefinitions = this.PropertyContainer.CategoryDefinitions;
//            if (categoryDefinitions == null)
//            {
//                return null;
//            }
//            return categoryDefinitions[categoryValue];
//        }

//        public override void PrepareChildrenPropertyItem(PropertyItemBase propertyItem, object item)
//        {
//            this._isPreparingItemFlag = true;
//            base.PrepareChildrenPropertyItem(propertyItem, item);
//            if (propertyItem.Editor == null)
//            {
//                FrameworkElement frameworkElement = this.GenerateChildrenEditorElement((PropertyItem)propertyItem);
//                if (frameworkElement != null)
//                {
//                    ContainerHelperBase.SetIsGenerated(frameworkElement, true);
//                    propertyItem.Editor = frameworkElement;
//                }
//            }
//            this._isPreparingItemFlag = false;
//        }

//        public override void ClearChildrenPropertyItem(PropertyItemBase propertyItem, object item)
//        {
//            if (propertyItem.Editor != null && ContainerHelperBase.GetIsGenerated(propertyItem.Editor))
//            {
//                propertyItem.Editor = null;
//            }
//            base.ClearChildrenPropertyItem(propertyItem, item);
//        }

//        public override Binding CreateChildrenDefaultBinding(PropertyItemBase propertyItem)
//        {
//            return new Binding("Value")
//            {
//                Mode = ((PropertyItem)propertyItem).IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay
//            };
//        }

//        protected static string GetDefaultPropertyName(object instance)
//        {
//            AttributeCollection attributes = TypeDescriptor.GetAttributes(instance);
//            DefaultPropertyAttribute defaultPropertyAttribute = (DefaultPropertyAttribute)attributes[typeof(DefaultPropertyAttribute)];
//            if (defaultPropertyAttribute == null)
//            {
//                return null;
//            }
//            return defaultPropertyAttribute.Name;
//        }

//        private static bool IsItemOrderingProperty(string propertyName)
//        {
//            return string.Equals(propertyName, PropertyItemCollection.DisplayNamePropertyName) || string.Equals(propertyName, PropertyItemCollection.CategoryOrderPropertyName) || string.Equals(propertyName, PropertyItemCollection.PropertyOrderPropertyName);
//        }
//    }
//}