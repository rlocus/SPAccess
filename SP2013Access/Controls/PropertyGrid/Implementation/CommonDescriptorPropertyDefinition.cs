using SP2013Access.Controls.PropertyGrid.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup.Primitives;

namespace SP2013Access.Controls.PropertyGrid
{
    internal class CommonDescriptorPropertyDefinition : DescriptorPropertyDefinitionBase
    {
        private readonly List<PropertyDescriptor> _propertyDescriptors;
        private readonly List<DependencyPropertyDescriptor> _dpDescriptors = new List<DependencyPropertyDescriptor>();
        private readonly List<MarkupObject> _markupObjects = new List<MarkupObject>();
        private readonly IEnumerable _selectedObjects;
        private static readonly DependencyProperty MultipleValuesProperty = DependencyProperty.Register("MultipleValues", typeof(IEnumerable), typeof(CommonDescriptorPropertyDefinition), new UIPropertyMetadata(null, new PropertyChangedCallback(CommonDescriptorPropertyDefinition.OnMultipleValuesPropertyChanged)));

        public List<PropertyDescriptor> PropertyDescriptors
        {
            get
            {
                return this._propertyDescriptors;
            }
        }

        internal override PropertyDescriptor PropertyDescriptor
        {
            get
            {
                return this.PropertyDescriptors.First<PropertyDescriptor>();
            }
        }

        private IEnumerable<object> ValueInstances
        {
            get
            {
                return this._selectedObjects.Cast<object>();
            }
        }

        private int SelectedObjectsCount
        {
            get
            {
                return this.PropertyDescriptors.Count<PropertyDescriptor>();
            }
        }

        private IEnumerable MultipleValues
        {
            get
            {
                return (IEnumerable)base.GetValue(CommonDescriptorPropertyDefinition.MultipleValuesProperty);
            }
            set
            {
                base.SetValue(CommonDescriptorPropertyDefinition.MultipleValuesProperty, value);
            }
        }

        internal CommonDescriptorPropertyDefinition(List<PropertyDescriptor> propertyDescriptorList, IEnumerable<object> selectedObjects, bool isPropertyGridCategorized)
            : base(isPropertyGridCategorized)
        {
            if (propertyDescriptorList == null)
            {
                throw new ArgumentNullException("propertyDescriptor");
            }
            if (propertyDescriptorList.Count == 0)
            {
                throw new InvalidOperationException("propertyDescriptorList is empty ! There are no common properties.");
            }
            if (selectedObjects == null)
            {
                throw new ArgumentNullException("selectedObjects");
            }
            this._selectedObjects = selectedObjects;
            this._propertyDescriptors = propertyDescriptorList;
            foreach (PropertyDescriptor current in propertyDescriptorList)
            {
                this._dpDescriptors.Add(DependencyPropertyDescriptor.FromProperty(current));
            }
            foreach (object current2 in this.ValueInstances)
            {
                this._markupObjects.Add(MarkupWriter.GetMarkupObjectFor(current2));
            }
        }

        private static void OnMultipleValuesPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            CommonDescriptorPropertyDefinition commonDescriptorPropertyDefinition = (CommonDescriptorPropertyDefinition)sender;
            commonDescriptorPropertyDefinition.UpdateIsExpandable();
            commonDescriptorPropertyDefinition.RaiseContainerHelperInvalidated();
        }

        internal override ObjectContainerHelperBase CreateContainerHelper(IPropertyContainer parent)
        {
            if (!base.IsExpandable)
            {
                return new ObjectContainerHelper(parent, null);
            }
            return new ObjectsContainerHelper(parent, this.MultipleValues);
        }

        protected override BindingBase CreateValueBinding()
        {
            MultiBinding multiBinding = new MultiBinding();
            multiBinding.Converter = new CommonPropertyConverter(base.PropertyType);
            multiBinding.ValidationRules.Add(new CommonPropertyExceptionValidationRule(base.PropertyType));
            multiBinding.Mode = (base.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay);
            foreach (object current in this.ValueInstances)
            {
                Binding item = new Binding(base.PropertyName)
                {
                    Source = current,
                    Mode = base.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay,
                    ValidatesOnDataErrors = true,
                    ValidatesOnExceptions = true
                };
                multiBinding.Bindings.Add(item);
            }
            return multiBinding;
        }

        protected override bool ComputeIsReadOnly()
        {
            Type typeFromHandle = typeof(IList);
            if (base.PropertyType.GetInterface(typeFromHandle.Name) != null)
            {
                return true;
            }
            return this.PropertyDescriptors.FirstOrDefault((PropertyDescriptor x) => x.IsReadOnly) != null;
        }

        //internal override ITypeEditor CreateDefaultEditor()
        //{
        //    object obj = null;
        //    if (PropertyGridUtilities.IsSameForAllObject(this.PropertyDescriptors, (object o) => ((PropertyDescriptor)o).Converter.GetType(), out obj))
        //    {
        //        return PropertyGridUtilities.CreateDefaultEditor(base.PropertyType, this.PropertyDescriptors.First<PropertyDescriptor>().Converter);
        //    }
        //    return new TextBlockEditor();
        //}
        protected override string ComputeCategory()
        {
            object obj = null;
            PropertyGridUtilities.IsSameForAllObject(this.PropertyDescriptors, new Func<object, object>(base.ComputeCategoryForItem), out obj);
            if (obj == null)
            {
                return CategoryAttribute.Default.Category;
            }
            return obj as string;
        }

        protected override string ComputeCategoryValue()
        {
            object obj = null;
            PropertyGridUtilities.IsSameForAllObject(this.PropertyDescriptors, new Func<object, object>(base.ComputeCategoryValueForItem), out obj);
            if (obj == null)
            {
                return CategoryAttribute.Default.Category;
            }
            return obj as string;
        }

        protected override object ComputeDefinitionKey()
        {
            object obj = null;
            PropertyGridUtilities.IsSameForAllObject(this.PropertyDescriptors, new Func<object, object>(base.ComputeDefinitionKeyForItem), out obj);
            if (obj == null)
            {
                return null;
            }
            return obj;
        }

        protected override string ComputeDescription()
        {
            object obj = null;
            PropertyGridUtilities.IsSameForAllObject(this.PropertyDescriptors, new Func<object, object>(base.ComputeDescriptionForItem), out obj);
            if (obj == null)
            {
                return null;
            }
            return obj as string;
        }

        protected override int ComputeDisplayOrder(bool isPropertyGridCategorized)
        {
            object obj = null;
            base.IsPropertyGridCategorized = isPropertyGridCategorized;
            PropertyGridUtilities.IsSameForAllObject(this.PropertyDescriptors, new Func<object, object>(base.ComputeDisplayOrderForItem), out obj);
            if (obj == null)
            {
                return 2147483647;
            }
            return (int)obj;
        }

        protected override bool ComputeExpandableAttribute()
        {
            object obj = null;
            PropertyGridUtilities.IsSameForAllObject(this.PropertyDescriptors, new Func<object, object>(base.ComputeExpandableAttributeForItem), out obj);
            return obj != null && (bool)obj;
        }

        protected override bool ComputeIsExpandable()
        {
            if (this.MultipleValues != null && this.MultipleValues.Cast<object>().Any<object>())
            {
                return this.MultipleValues.Cast<object>().All((object o) => o != null);
            }
            return false;
        }

        public override void InitProperties()
        {
            base.InitProperties();
            MultiBinding multiBinding = new MultiBinding();
            multiBinding.Mode = BindingMode.OneWay;
            multiBinding.Converter = new MultipleValuesConverter();
            foreach (object current in this.ValueInstances)
            {
                Binding item = new Binding(base.PropertyName)
                {
                    Source = current,
                    Mode = BindingMode.OneWay
                };
                multiBinding.Bindings.Add(item);
            }
            BindingOperations.SetBinding(this, CommonDescriptorPropertyDefinition.MultipleValuesProperty, multiBinding);
        }

        protected override IList<Type> ComputeNewItemTypes()
        {
            object obj = null;
            PropertyGridUtilities.IsSameForAllObject(this.PropertyDescriptors, new Func<object, object>(base.ComputeNewItemTypesForItem), out obj);
            if (obj == null)
            {
                return null;
            }
            return (IList<Type>)obj;
        }

        protected override object ComputeAdvancedOptionsTooltip()
        {
            object[] tooltips = new object[this.SelectedObjectsCount];
            DependencyObject[] array = (
                from o in this.ValueInstances
                select o as DependencyObject).ToArray<DependencyObject>();
            for (int i = 0; i < this.SelectedObjectsCount; i++)
            {
                object obj;
                base.UpdateAdvanceOptionsForItem(this._markupObjects[i], array[i], this._dpDescriptors[i], out obj);
                tooltips[i] = tooltips;
            }
            object result = "Advanced Properties";
            object obj2 = null;
            if (PropertyGridUtilities.IsSameForAllObject(tooltips, (object o) => object.Equals(o, tooltips[0]), out obj2))
            {
                result = tooltips[0];
            }
            return result;
        }

        protected override void ResetValue()
        {
            List<PropertyDescriptor> list = this.PropertyDescriptors.ToList<PropertyDescriptor>();
            List<object> list2 = this.ValueInstances.ToList<object>();
            for (int i = 0; i < this.SelectedObjectsCount; i++)
            {
                list[i].ResetValue(list2[i]);
            }
            base.UpdateAdvanceOptions();
        }

        protected override bool ComputeCanResetValue()
        {
            IList<PropertyDescriptor> list = this.PropertyDescriptors.ToList<PropertyDescriptor>();
            IList<object> list2 = this.ValueInstances.ToList<object>();
            for (int i = 0; i < this.SelectedObjectsCount; i++)
            {
                if (!list[i].CanResetValue(list2[i]))
                {
                    return false;
                }
            }
            return !base.IsReadOnly;
        }

        //internal override ITypeEditor CreateAttributeEditor()
        //{
        //    if (this.IsAttributePresentForAllSelectedObjects<EditorAttribute>())
        //    {
        //        object obj = null;
        //        PropertyGridUtilities.IsSameForAllObject(this.PropertyDescriptors, (object o) => Type.GetType(this.GetAttribute<EditorAttribute>((PropertyDescriptor)o).EditorTypeName), out obj);
        //        Type type = obj as Type;
        //        if (type != null)
        //        {
        //            object obj2 = Activator.CreateInstance(type);
        //            if (obj2 is ITypeEditor)
        //            {
        //                return (ITypeEditor)obj2;
        //            }
        //        }
        //    }
        //    if (this.IsAttributePresentForAllSelectedObjects<ItemsSourceAttribute>())
        //    {
        //        object obj3 = null;
        //        PropertyGridUtilities.IsSameForAllObject(this.PropertyDescriptors, (object o) => this.GetAttribute<ItemsSourceAttribute>((PropertyDescriptor)o), out obj3);
        //        ItemsSourceAttribute itemsSourceAttribute = obj3 as ItemsSourceAttribute;
        //        if (itemsSourceAttribute != null)
        //        {
        //            return new ItemsSourceAttributeEditor(itemsSourceAttribute);
        //        }
        //    }
        //    return null;
        //}
        private T GetAttribute<T>(PropertyDescriptor pd) where T : Attribute
        {
            return PropertyGridUtilities.GetAttribute<T>(pd);
        }

        //private bool IsAttributePresentForAllSelectedObjects<T>() where T : Attribute
        //{
        //    object obj = null;
        //    PropertyGridUtilities.IsSameForAllObject(this.PropertyDescriptors, (object o) => this.GetAttribute<T>((PropertyDescriptor)o) != null, out obj);
        //    return obj != null && (bool)obj;
        //}
    }
}