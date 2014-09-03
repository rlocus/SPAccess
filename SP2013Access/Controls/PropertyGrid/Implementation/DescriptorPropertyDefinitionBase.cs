using SP2013Access.Controls.PropertyGrid.Attributes;
using SP2013Access.Controls.PropertyGrid.Commands;
using SP2013Access.Controls.PropertyGrid.Editors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup.Primitives;
using System.Windows.Media;

namespace SP2013Access.Controls.PropertyGrid
{
    internal abstract class DescriptorPropertyDefinitionBase : DependencyObject
    {
        private string _category;
        private string _categoryValue;
        private string _description;
        private string _displayName;
        private object _definitionKey;
        private int _displayOrder;
        private bool _expandableAttribute;
        //private IList<Type> _newItemTypes;
        private IEnumerable<CommandBinding> _commandBindings;
        public static readonly DependencyProperty AdvancedOptionsIconProperty = DependencyProperty.Register("AdvancedOptionsIcon", typeof(ImageSource), typeof(DescriptorPropertyDefinitionBase), new UIPropertyMetadata(null));
        public static readonly DependencyProperty AdvancedOptionsTooltipProperty = DependencyProperty.Register("AdvancedOptionsTooltip", typeof(object), typeof(DescriptorPropertyDefinitionBase), new UIPropertyMetadata(null));
        public static readonly DependencyProperty IsExpandableProperty = DependencyProperty.Register("IsExpandable", typeof(bool), typeof(DescriptorPropertyDefinitionBase), new UIPropertyMetadata(false));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(DescriptorPropertyDefinitionBase), new UIPropertyMetadata(null, new PropertyChangedCallback(DescriptorPropertyDefinitionBase.OnValueChanged)));

        public event EventHandler ContainerHelperInvalidated;

        internal abstract PropertyDescriptor PropertyDescriptor
        {
            get;
        }

        //public ImageSource AdvancedOptionsIcon
        //{
        //    get
        //    {
        //        return (ImageSource)base.GetValue(DescriptorPropertyDefinitionBase.AdvancedOptionsIconProperty);
        //    }
        //    set
        //    {
        //        base.SetValue(DescriptorPropertyDefinitionBase.AdvancedOptionsIconProperty, value);
        //    }
        //}

        public object AdvancedOptionsTooltip
        {
            get
            {
                return base.GetValue(DescriptorPropertyDefinitionBase.AdvancedOptionsTooltipProperty);
            }
            set
            {
                base.SetValue(DescriptorPropertyDefinitionBase.AdvancedOptionsTooltipProperty, value);
            }
        }

        public bool IsExpandable
        {
            get
            {
                return (bool)base.GetValue(DescriptorPropertyDefinitionBase.IsExpandableProperty);
            }
            set
            {
                base.SetValue(DescriptorPropertyDefinitionBase.IsExpandableProperty, value);
            }
        }

        public string Category
        {
            get
            {
                return this._category;
            }
            internal set
            {
                this._category = value;
            }
        }

        public string CategoryValue
        {
            get
            {
                return this._categoryValue;
            }
            internal set
            {
                this._categoryValue = value;
            }
        }

        public IEnumerable<CommandBinding> CommandBindings
        {
            get
            {
                return this._commandBindings;
            }
        }

        public string DisplayName
        {
            get
            {
                return this._displayName;
            }
            internal set
            {
                this._displayName = value;
            }
        }

        public object DefinitionKey
        {
            get
            {
                return this._definitionKey;
            }
        }

        public string Description
        {
            get
            {
                return this._description;
            }
            internal set
            {
                this._description = value;
            }
        }

        public int DisplayOrder
        {
            get
            {
                return this._displayOrder;
            }
            internal set
            {
                this._displayOrder = value;
            }
        }

        public bool IsReadOnly { get; private set; }

        //public IList<Type> NewItemTypes
        //{
        //    get
        //    {
        //        return this._newItemTypes;
        //    }
        //}

        public string PropertyName
        {
            get
            {
                return this.PropertyDescriptor.Name;
            }
        }

        public Type PropertyType
        {
            get
            {
                return this.PropertyDescriptor.PropertyType;
            }
        }

        internal bool ExpandableAttribute
        {
            get
            {
                return this._expandableAttribute;
            }
            set
            {
                this._expandableAttribute = value;
                this.UpdateIsExpandable();
            }
        }

        internal bool IsPropertyGridCategorized
        {
            get;
            set;
        }

        public object Value
        {
            get
            {
                return base.GetValue(DescriptorPropertyDefinitionBase.ValueProperty);
            }
            set
            {
                base.SetValue(DescriptorPropertyDefinitionBase.ValueProperty, value);
            }
        }

        internal DescriptorPropertyDefinitionBase(bool isPropertyGridCategorized)
        {
            this.IsPropertyGridCategorized = isPropertyGridCategorized;
        }

        protected virtual string ComputeCategory()
        {
            return null;
        }

        protected virtual string ComputeCategoryValue()
        {
            return null;
        }

        protected virtual string ComputeDescription()
        {
            return null;
        }

        protected virtual object ComputeDefinitionKey()
        {
            return null;
        }

        protected virtual int ComputeDisplayOrder(bool isPropertyGridCategorized)
        {
            return 2147483647;
        }

        protected virtual bool ComputeExpandableAttribute()
        {
            return false;
        }

        protected abstract bool ComputeIsExpandable();

        protected virtual IList<Type> ComputeNewItemTypes()
        {
            return null;
        }

        protected virtual bool ComputeIsReadOnly()
        {
            return false;
        }

        protected virtual bool ComputeCanResetValue()
        {
            return false;
        }

        protected virtual object ComputeAdvancedOptionsTooltip()
        {
            return null;
        }

        protected virtual void ResetValue()
        {
        }

        protected abstract BindingBase CreateValueBinding();

        internal abstract ContainerHelperBase CreateContainerHelper(IPropertyContainer parent);

        internal void RaiseContainerHelperInvalidated()
        {
            if (this.ContainerHelperInvalidated != null)
            {
                this.ContainerHelperInvalidated(this, EventArgs.Empty);
            }
        }

        internal virtual ITypeEditor CreateDefaultEditor()
        {
            return null;
        }

        internal virtual ITypeEditor CreateAttributeEditor()
        {
            return null;
        }

        internal void UpdateAdvanceOptionsForItem(MarkupObject markupObject, DependencyObject dependencyObject, DependencyPropertyDescriptor dpDescriptor, out object tooltip)
        {
            tooltip = StringConstants.AdvancedProperties;
            bool flag = false;
            bool flag2 = false;
            MarkupProperty markupProperty = (
                from p in markupObject.Properties
                where p.Name == this.PropertyName
                select p).FirstOrDefault<MarkupProperty>();
            if (markupProperty != null && markupProperty.PropertyType != typeof(object) && !markupProperty.PropertyType.IsEnum)
            {
                flag = (markupProperty.Value is Style);
                flag2 = (markupProperty.Value is DynamicResourceExtension);
            }
            if (flag || flag2)
            {
                tooltip = StringConstants.Resource;
                return;
            }
            if (dependencyObject != null && dpDescriptor != null)
            {
                if (BindingOperations.GetBindingExpressionBase(dependencyObject, dpDescriptor.DependencyProperty) != null)
                {
                    tooltip = StringConstants.Databinding;
                    return;
                }
                switch (DependencyPropertyHelper.GetValueSource(dependencyObject, dpDescriptor.DependencyProperty).BaseValueSource)
                {
                    case BaseValueSource.Inherited:
                    case BaseValueSource.DefaultStyle:
                    case BaseValueSource.ImplicitStyleReference:
                        tooltip = StringConstants.Inheritance;
                        return;

                    case BaseValueSource.DefaultStyleTrigger:
                    case BaseValueSource.TemplateTrigger:
                    case BaseValueSource.StyleTrigger:
                    case BaseValueSource.ParentTemplate:
                    case BaseValueSource.ParentTemplateTrigger:
                        break;

                    case BaseValueSource.Style:
                        tooltip = StringConstants.StyleSetter;
                        return;

                    case BaseValueSource.Local:
                        tooltip = StringConstants.Local;
                        break;

                    default:
                        return;
                }
            }
        }

        internal void UpdateAdvanceOptions()
        {
            this.AdvancedOptionsTooltip = this.ComputeAdvancedOptionsTooltip();
        }

        internal void UpdateIsExpandable()
        {
            this.IsExpandable = (this.ExpandableAttribute && this.ComputeIsExpandable());
        }

        internal void UpdateValueFromSource()
        {
            BindingOperations.GetBindingExpressionBase(this, DescriptorPropertyDefinitionBase.ValueProperty).UpdateTarget();
        }

        internal object ComputeCategoryForItem(object item)
        {
            PropertyDescriptor propertyDescriptor = item as PropertyDescriptor;
            LocalizedCategoryAttribute attribute = PropertyGridUtilities.GetAttribute<LocalizedCategoryAttribute>(propertyDescriptor);
            if (attribute == null)
            {
                return propertyDescriptor.Category;
            }
            return attribute.LocalizedCategory;
        }

        internal object ComputeCategoryValueForItem(object item)
        {
            PropertyDescriptor propertyDescriptor = item as PropertyDescriptor;
            LocalizedCategoryAttribute attribute = PropertyGridUtilities.GetAttribute<LocalizedCategoryAttribute>(propertyDescriptor);
            if (attribute == null)
            {
                return propertyDescriptor.Category;
            }
            return attribute.CategoryValue;
        }

        internal object ComputeDescriptionForItem(object item)
        {
            PropertyDescriptor propertyDescriptor = item as PropertyDescriptor;
            DescriptionAttribute attribute = PropertyGridUtilities.GetAttribute<DescriptionAttribute>(propertyDescriptor);
            if (attribute == null)
            {
                return propertyDescriptor.Description;
            }
            return attribute.Description;
        }

        internal object ComputeNewItemTypesForItem(object item)
        {
            PropertyDescriptor property = item as PropertyDescriptor;
            NewItemTypesAttribute attribute = PropertyGridUtilities.GetAttribute<NewItemTypesAttribute>(property);
            if (attribute == null)
            {
                return null;
            }
            return attribute.Types;
        }

        internal object ComputeDefinitionKeyForItem(object item)
        {
            PropertyDescriptor property = item as PropertyDescriptor;
            DefinitionKeyAttribute attribute = PropertyGridUtilities.GetAttribute<DefinitionKeyAttribute>(property);
            if (attribute == null)
            {
                return null;
            }
            return attribute.Key;
        }

        internal object ComputeDisplayOrderForItem(object item)
        {
            PropertyDescriptor propertyDescriptor = item as PropertyDescriptor;
            List<PropertyOrderAttribute> list = propertyDescriptor.Attributes.OfType<PropertyOrderAttribute>().ToList<PropertyOrderAttribute>();
            if (list.Count > 0)
            {
                this.ValidatePropertyOrderAttributes(list);
                if (this.IsPropertyGridCategorized)
                {
                    PropertyOrderAttribute propertyOrderAttribute = list.FirstOrDefault((PropertyOrderAttribute x) => x.UsageContext == UsageContextEnum.Categorized || x.UsageContext == UsageContextEnum.Both);
                    if (propertyOrderAttribute != null)
                    {
                        return propertyOrderAttribute.Order;
                    }
                }
                else
                {
                    PropertyOrderAttribute propertyOrderAttribute2 = list.FirstOrDefault((PropertyOrderAttribute x) => x.UsageContext == UsageContextEnum.Alphabetical || x.UsageContext == UsageContextEnum.Both);
                    if (propertyOrderAttribute2 != null)
                    {
                        return propertyOrderAttribute2.Order;
                    }
                }
            }
            return 2147483647;
        }

        internal object ComputeExpandableAttributeForItem(object item)
        {
            PropertyDescriptor property = (PropertyDescriptor)item;
            ExpandableObjectAttribute attribute = PropertyGridUtilities.GetAttribute<ExpandableObjectAttribute>(property);
            return attribute != null;
        }

        internal int ComputeDisplayOrderInternal(bool isPropertyGridCategorized)
        {
            return this.ComputeDisplayOrder(isPropertyGridCategorized);
        }

        private void ExecuteResetValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ComputeCanResetValue())
            {
                this.ResetValue();
            }
        }

        private void CanExecuteResetValueCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ComputeCanResetValue();
        }

        private string ComputeDisplayName()
        {
            string text = this.PropertyDescriptor.DisplayName;
            ParenthesizePropertyNameAttribute attribute = PropertyGridUtilities.GetAttribute<ParenthesizePropertyNameAttribute>(this.PropertyDescriptor);
            if (attribute != null && attribute.NeedParenthesis)
            {
                text = "(" + text + ")";
            }
            return text;
        }

        private void ValidatePropertyOrderAttributes(List<PropertyOrderAttribute> list)
        {
        //    if (list.Count > 0)
        //    {
        //        PropertyOrderAttribute propertyOrderAttribute = list.FirstOrDefault((PropertyOrderAttribute x) => x.UsageContext == UsageContextEnum.Both);
        //        if (propertyOrderAttribute != null)
        //        {
        //            int arg_38_0 = list.Count;
        //        }
        //    }
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((DescriptorPropertyDefinitionBase)o).OnValueChanged(e.OldValue, e.NewValue);
        }

        internal virtual void OnValueChanged(object oldValue, object newValue)
        {
            this.UpdateIsExpandable();
            this.UpdateAdvanceOptions();
            CommandManager.InvalidateRequerySuggested();
        }

        public virtual void InitProperties()
        {
            this.IsReadOnly = this.ComputeIsReadOnly();
            this._category = this.ComputeCategory();
            this._categoryValue = this.ComputeCategoryValue();
            this._description = this.ComputeDescription();
            this._displayName = this.ComputeDisplayName();
            this._displayOrder = this.ComputeDisplayOrder(this.IsPropertyGridCategorized);
            this._definitionKey = this.ComputeDefinitionKey();
            this._expandableAttribute = this.ComputeExpandableAttribute();
            //this._newItemTypes = this.ComputeNewItemTypes();
            this._commandBindings = new CommandBinding[]
			{
				new CommandBinding(PropertyItemCommands.ResetValue, new ExecutedRoutedEventHandler(this.ExecuteResetValueCommand), new CanExecuteRoutedEventHandler(this.CanExecuteResetValueCommand))
			};
            this.UpdateIsExpandable();
            this.UpdateAdvanceOptions();
            BindingBase binding = this.CreateValueBinding();
            BindingOperations.SetBinding(this, DescriptorPropertyDefinitionBase.ValueProperty, binding);
        }
    }
}