using SP2013Access.Controls.Utilities;
using SP2013Access.Extensions;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace SP2013Access.Controls.PropertyGrid
{
    [TemplatePart(Name = "PART_PropertyItemsControl", Type = typeof(PropertyItemsControl)), TemplatePart(Name = "PART_ValueContainer", Type = typeof(ContentControl))]
    public abstract class PropertyItemBase : Control, IPropertyContainer, INotifyPropertyChanged
    {
        internal const string PART_ValueContainer = "PART_ValueContainer";
        private ContentControl _valueContainer;
        private ContainerHelperBase _containerHelper;
        public static readonly DependencyProperty AdvancedOptionsIconProperty;
        public static readonly DependencyProperty AdvancedOptionsTooltipProperty;
        public static readonly DependencyProperty DefinitionKeyProperty;
        public static readonly DependencyProperty DescriptionProperty;
        public static readonly DependencyProperty DisplayNameProperty;
        public static readonly DependencyProperty EditorProperty;
        public static readonly DependencyProperty IsExpandedProperty;
        public static readonly DependencyProperty IsExpandableProperty;
        public static readonly DependencyProperty IsSelectedProperty;
        internal static readonly RoutedEvent ItemSelectionChangedEvent;

        public event PropertyChangedEventHandler PropertyChanged;

        public ImageSource AdvancedOptionsIcon
        {
            get
            {
                return (ImageSource)base.GetValue(PropertyItemBase.AdvancedOptionsIconProperty);
            }
            set
            {
                base.SetValue(PropertyItemBase.AdvancedOptionsIconProperty, value);
            }
        }

        public object AdvancedOptionsTooltip
        {
            get
            {
                return base.GetValue(PropertyItemBase.AdvancedOptionsTooltipProperty);
            }
            set
            {
                base.SetValue(PropertyItemBase.AdvancedOptionsTooltipProperty, value);
            }
        }

        public object DefinitionKey
        {
            get
            {
                return base.GetValue(PropertyItemBase.DefinitionKeyProperty);
            }
            set
            {
                base.SetValue(PropertyItemBase.DefinitionKeyProperty, value);
            }
        }

        public string Description
        {
            get
            {
                return (string)base.GetValue(PropertyItemBase.DescriptionProperty);
            }
            set
            {
                base.SetValue(PropertyItemBase.DescriptionProperty, value);
            }
        }

        public string DisplayName
        {
            get
            {
                return (string)base.GetValue(PropertyItemBase.DisplayNameProperty);
            }
            set
            {
                base.SetValue(PropertyItemBase.DisplayNameProperty, value);
            }
        }

        public FrameworkElement Editor
        {
            get
            {
                return (FrameworkElement)base.GetValue(PropertyItemBase.EditorProperty);
            }
            set
            {
                base.SetValue(PropertyItemBase.EditorProperty, value);
            }
        }

        public bool IsExpanded
        {
            get
            {
                return (bool)base.GetValue(PropertyItemBase.IsExpandedProperty);
            }
            set
            {
                base.SetValue(PropertyItemBase.IsExpandedProperty, value);
            }
        }

        public bool IsExpandable
        {
            get
            {
                return (bool)base.GetValue(PropertyItemBase.IsExpandableProperty);
            }
            set
            {
                base.SetValue(PropertyItemBase.IsExpandableProperty, value);
            }
        }

        public bool IsSelected
        {
            get
            {
                return (bool)base.GetValue(PropertyItemBase.IsSelectedProperty);
            }
            set
            {
                base.SetValue(PropertyItemBase.IsSelectedProperty, value);
            }
        }

        public FrameworkElement ParentElement
        {
            get
            {
                return this.ParentNode as FrameworkElement;
            }
        }

        internal IPropertyContainer ParentNode { get; set; }

        internal ContentControl ValueContainer
        {
            get
            {
                return this._valueContainer;
            }
        }

        public int Level
        {
            get;
            internal set;
        }

        public IList Properties
        {
            get
            {
                return this._containerHelper.Properties;
            }
        }

        public Style PropertyContainerStyle
        {
            get
            {
                if (this.ParentNode == null)
                {
                    return null;
                }
                return this.ParentNode.PropertyContainerStyle;
            }
        }

        internal ContainerHelperBase ContainerHelper
        {
            get
            {
                return this._containerHelper;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this._containerHelper = value;
                this.RaisePropertyChanged<IList>(() => this.Properties);
            }
        }

        Binding IPropertyContainer.PropertyNameBinding
        {
            get
            {
                return null;
            }
        }

        Binding IPropertyContainer.PropertyValueBinding
        {
            get
            {
                return null;
            }
        }

        EditorDefinitionBase IPropertyContainer.DefaultEditorDefinition
        {
            get
            {
                return null;
            }
        }

        CategoryDefinitionCollection IPropertyContainer.CategoryDefinitions
        {
            get
            {
                return null;
            }
        }

        GroupDescription IPropertyContainer.CategoryGroupDescription
        {
            get
            {
                return null;
            }
        }

        Style IPropertyContainer.PropertyContainerStyle
        {
            get
            {
                return this.PropertyContainerStyle;
            }
        }

        EditorDefinitionCollection IPropertyContainer.EditorDefinitions
        {
            get
            {
                return this.ParentNode.EditorDefinitions;
            }
        }

        PropertyDefinitionCollection IPropertyContainer.PropertyDefinitions
        {
            get
            {
                return null;
            }
        }

        ContainerHelperBase IPropertyContainer.ContainerHelper
        {
            get
            {
                return this.ContainerHelper;
            }
        }

        bool IPropertyContainer.IsCategorized
        {
            get
            {
                return false;
            }
        }

        //bool IPropertyContainer.AutoGenerateProperties
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}

        FilterInfo IPropertyContainer.FilterInfo
        {
            get
            {
                return default(FilterInfo);
            }
        }

        private static void OnDefinitionKeyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PropertyItemBase propertyItemBase = o as PropertyItemBase;
            if (propertyItemBase != null)
            {
                propertyItemBase.OnDefinitionKeyChanged(e.OldValue, e.NewValue);
            }
        }

        internal virtual void OnDefinitionKeyChanged(object oldValue, object newValue)
        {
        }

        private static void OnEditorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PropertyItemBase propertyItem = o as PropertyItemBase;
            if (propertyItem != null)
            {
                propertyItem.OnEditorChanged((FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue);
            }
        }

        protected virtual void OnEditorChanged(FrameworkElement oldValue, FrameworkElement newValue)
        {
        }

        private static void OnIsExpandedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PropertyItemBase propertyItemBase = o as PropertyItemBase;
            if (propertyItemBase != null)
            {
                propertyItemBase.OnIsExpandedChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        protected virtual void OnIsExpandedChanged(bool oldValue, bool newValue)
        {
        }

        private static void OnIsSelectedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PropertyItemBase propertyItemBase = o as PropertyItemBase;
            if (propertyItemBase != null)
            {
                propertyItemBase.OnIsSelectedChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            this.RaiseItemSelectionChangedEvent();
        }

        private void RaiseItemSelectionChangedEvent()
        {
            base.RaiseEvent(new RoutedEventArgs(PropertyItemBase.ItemSelectionChangedEvent));
        }

        internal void RaisePropertyChanged<TMember>(Expression<Func<TMember>> propertyExpression)
        {
            this.Notify(this.PropertyChanged, propertyExpression);
        }

        internal void RaisePropertyChanged(string name)
        {
            this.Notify(this.PropertyChanged, name);
        }

        static PropertyItemBase()
        {
            PropertyItemBase.AdvancedOptionsIconProperty = DependencyProperty.Register("AdvancedOptionsIcon", typeof(ImageSource), typeof(PropertyItemBase), new UIPropertyMetadata(null));
            PropertyItemBase.AdvancedOptionsTooltipProperty = DependencyProperty.Register("AdvancedOptionsTooltip", typeof(object), typeof(PropertyItemBase), new UIPropertyMetadata(null));
            PropertyItemBase.DefinitionKeyProperty = DependencyProperty.Register("DefinitionKey", typeof(object), typeof(PropertyItemBase), new UIPropertyMetadata(null, new PropertyChangedCallback(PropertyItemBase.OnDefinitionKeyChanged)));
            PropertyItemBase.DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(PropertyItemBase), new UIPropertyMetadata(null));
            PropertyItemBase.DisplayNameProperty = DependencyProperty.Register("DisplayName", typeof(string), typeof(PropertyItemBase), new UIPropertyMetadata(null));
            PropertyItemBase.EditorProperty = DependencyProperty.Register("Editor", typeof(FrameworkElement), typeof(PropertyItemBase), new UIPropertyMetadata(null, new PropertyChangedCallback(PropertyItemBase.OnEditorChanged)));
            PropertyItemBase.IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(PropertyItemBase), new UIPropertyMetadata(false, new PropertyChangedCallback(PropertyItemBase.OnIsExpandedChanged)));
            PropertyItemBase.IsExpandableProperty = DependencyProperty.Register("IsExpandable", typeof(bool), typeof(PropertyItemBase), new UIPropertyMetadata(false));
            PropertyItemBase.IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(PropertyItemBase), new UIPropertyMetadata(false, new PropertyChangedCallback(PropertyItemBase.OnIsSelectedChanged)));
            PropertyItemBase.ItemSelectionChangedEvent = EventManager.RegisterRoutedEvent("ItemSelectionChangedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PropertyItemBase));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyItemBase), new FrameworkPropertyMetadata(typeof(PropertyItemBase)));
        }

        internal PropertyItemBase()
        {
            this._containerHelper = null;//new PropertiesSourceContainerHelper(this, null);
            base.GotFocus += new RoutedEventHandler(this.PropertyItemBase_GotFocus);
            base.AddHandler(PropertyItemsControl.PreparePropertyItemEvent, new PropertyItemEventHandler(this.OnPreparePropertyItemInternal));
            base.AddHandler(PropertyItemsControl.ClearPropertyItemEvent, new PropertyItemEventHandler(this.OnClearPropertyItemInternal));
        }

        private void OnPreparePropertyItemInternal(object sender, PropertyItemEventArgs args)
        {
            args.PropertyItem.Level = this.Level + 1;
            this._containerHelper.PrepareChildrenPropertyItem(args.PropertyItem, args.Item);
            args.Handled = true;
        }

        private void OnClearPropertyItemInternal(object sender, PropertyItemEventArgs args)
        {
            this._containerHelper.ClearChildrenPropertyItem(args.PropertyItem, args.Item);
            args.PropertyItem.Level = 0;
            args.Handled = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._containerHelper.ChildrenItemsControl = (base.GetTemplateChild("PART_PropertyItemsControl") as PropertyItemsControl);
            this._valueContainer = (base.GetTemplateChild("PART_ValueContainer") as ContentControl);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            this.IsSelected = true;
            if (!base.IsKeyboardFocusWithin)
            {
                base.Focus();
            }
            e.Handled = true;
        }

        private void PropertyItemBase_GotFocus(object sender, RoutedEventArgs e)
        {
            this.IsSelected = true;
            e.Handled = true;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (ReflectionHelper.IsPublicInstanceProperty(base.GetType(), e.Property.Name))
            {
                this.RaisePropertyChanged(e.Property.Name);
            }
        }
    }
}