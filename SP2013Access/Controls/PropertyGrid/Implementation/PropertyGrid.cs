﻿using SP2013Access.Controls.PropertyGrid.Commands;
using SP2013Access.Controls.Utilities;
using SP2013Access.Extensions;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace SP2013Access.Controls.PropertyGrid
{
    [TemplatePart(Name = PART_DragThumb, Type = typeof(Thumb))]
    [TemplatePart(Name = PART_PropertyItemsControl, Type = typeof(PropertyItemsControl))]
    [StyleTypedProperty(Property = "PropertyContainerStyle", StyleTargetType = typeof(PropertyItemBase)), TemplatePart(Name = "PART_PropertyItemsControl", Type = typeof(PropertyItemsControl)), TemplatePart(Name = "PART_DragThumb", Type = typeof(Thumb))]
    public class PropertyGrid : Control, ISupportInitialize, IPropertyContainer, INotifyPropertyChanged
    {
        private const string PART_DragThumb = "PART_DragThumb";
        internal const string PART_PropertyItemsControl = "PART_PropertyItemsControl";
        private static readonly ComponentResourceKey SelectedObjectAdvancedOptionsMenuKey;
        private Thumb _dragThumb;
        private bool _hasPendingSelectedObjectChanged;
        private int _initializationCount;
        private ContainerHelperBase _containerHelper;
        private PropertyDefinitionCollection _propertyDefinitions;
        private EditorDefinitionCollection _editorDefinitions;
        private readonly WeakEventListener<NotifyCollectionChangedEventArgs> _propertyDefinitionsListener;
        private readonly WeakEventListener<NotifyCollectionChangedEventArgs> _editorDefinitionsListener;
        private readonly WeakEventListener<NotifyCollectionChangedEventArgs> _categoryDefinitionsListener;
        private CategoryDefinitionCollection _categoryDefinitions;
        private readonly WeakEventListener<NotifyCollectionChangedEventArgs> _selectedObjectsListener;
        private Binding _propertyValueBinding;
        private Binding _propertyNameBinding;
        private IList _selectedObjects;
        public static readonly DependencyProperty AdvancedOptionsMenuProperty;
        public static readonly DependencyProperty AutoGeneratePropertiesProperty;
        public static readonly DependencyProperty ShowSummaryProperty;
        public static readonly DependencyProperty FilterProperty;
        public static readonly DependencyProperty FilterWatermarkProperty;
        public static readonly DependencyProperty IsCategorizedProperty;
        public static readonly DependencyProperty CategoryGroupDescriptionProperty;
        public static readonly DependencyProperty DefaultEditorDefinitionProperty;
        public static readonly DependencyProperty NameColumnWidthProperty;
        public static readonly DependencyProperty PropertiesSourceProperty;
        public static readonly DependencyProperty PropertyContainerStyleProperty;
        public static readonly DependencyProperty IsReadOnlyProperty;
        public static readonly DependencyProperty SelectedObjectProperty;
        public static readonly DependencyProperty SelectedObjectTypeProperty;
        public static readonly DependencyProperty SelectedObjectTypeNameProperty;
        public static readonly DependencyProperty SelectedObjectNameProperty;
        public static readonly DependencyProperty SelectedObjectsOverrideProperty;
        private static readonly DependencyPropertyKey SelectedPropertyItemPropertyKey;
        public static readonly DependencyProperty SelectedPropertyItemProperty;
        public static readonly DependencyProperty SelectedPropertyProperty;
        public static readonly DependencyProperty ShowAdvancedOptionsProperty;
        public static readonly DependencyProperty ShowPreviewProperty;
        public static readonly DependencyProperty ShowSearchBoxProperty;
        public static readonly DependencyProperty ShowSortOptionsProperty;
        public static readonly DependencyProperty ShowTitleProperty;
        public static readonly DependencyProperty UpdateTextBoxSourceOnEnterKeyProperty;
        public static readonly RoutedEvent PropertyValueChangedEvent;
        public static readonly RoutedEvent SelectedPropertyItemChangedEvent;
        public static readonly RoutedEvent SelectedObjectChangedEvent;
        public static readonly RoutedEvent PreparePropertyItemEvent;
        public static readonly RoutedEvent ClearPropertyItemEvent;

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyValueChangedEventHandler PropertyValueChanged
        {
            add
            {
                base.AddHandler(PropertyGrid.PropertyValueChangedEvent, value);
            }
            remove
            {
                base.RemoveHandler(PropertyGrid.PropertyValueChangedEvent, value);
            }
        }

        public event RoutedPropertyChangedEventHandler<PropertyItemBase> SelectedPropertyItemChanged
        {
            add
            {
                base.AddHandler(PropertyGrid.SelectedPropertyItemChangedEvent, value);
            }
            remove
            {
                base.RemoveHandler(PropertyGrid.SelectedPropertyItemChangedEvent, value);
            }
        }

        public event RoutedPropertyChangedEventHandler<object> SelectedObjectChanged
        {
            add
            {
                base.AddHandler(PropertyGrid.SelectedObjectChangedEvent, value);
            }
            remove
            {
                base.RemoveHandler(PropertyGrid.SelectedObjectChangedEvent, value);
            }
        }

        public event PropertyItemEventHandler PreparePropertyItem
        {
            add
            {
                base.AddHandler(PropertyGrid.PreparePropertyItemEvent, value);
            }
            remove
            {
                base.RemoveHandler(PropertyGrid.PreparePropertyItemEvent, value);
            }
        }

        public event PropertyItemEventHandler ClearPropertyItem
        {
            add
            {
                base.AddHandler(PropertyGrid.ClearPropertyItemEvent, value);
            }
            remove
            {
                base.RemoveHandler(PropertyGrid.ClearPropertyItemEvent, value);
            }
        }

        public ContextMenu AdvancedOptionsMenu
        {
            get
            {
                return (ContextMenu)base.GetValue(PropertyGrid.AdvancedOptionsMenuProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.AdvancedOptionsMenuProperty, value);
            }
        }

        public bool AutoGenerateProperties
        {
            get
            {
                return (bool)base.GetValue(PropertyGrid.AutoGeneratePropertiesProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.AutoGeneratePropertiesProperty, value);
            }
        }

        public bool ShowSummary
        {
            get
            {
                return (bool)base.GetValue(PropertyGrid.ShowSummaryProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.ShowSummaryProperty, value);
            }
        }

        public EditorDefinitionCollection EditorDefinitions
        {
            get
            {
                return this._editorDefinitions;
            }
            set
            {
                if (this._editorDefinitions != value)
                {
                    EditorDefinitionCollection editorDefinitions = this._editorDefinitions;
                    this._editorDefinitions = value;
                    this.OnEditorDefinitionsChanged(editorDefinitions, value);
                }
            }
        }

        public string Filter
        {
            get
            {
                return (string)base.GetValue(PropertyGrid.FilterProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.FilterProperty, value);
            }
        }

        public string FilterWatermark
        {
            get
            {
                return (string)base.GetValue(PropertyGrid.FilterWatermarkProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.FilterWatermarkProperty, value);
            }
        }

        public bool IsCategorized
        {
            get
            {
                return (bool)base.GetValue(PropertyGrid.IsCategorizedProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.IsCategorizedProperty, value);
            }
        }

        public CategoryDefinitionCollection CategoryDefinitions
        {
            get
            {
                return this._categoryDefinitions;
            }
            set
            {
                if (this._categoryDefinitions != value)
                {
                    CategoryDefinitionCollection categoryDefinitions = this._categoryDefinitions;
                    this._categoryDefinitions = value;
                    this.OnCategoryDefinitionsChanged(categoryDefinitions, value);
                }
            }
        }

        public GroupDescription CategoryGroupDescription
        {
            get
            {
                return (GroupDescription)base.GetValue(PropertyGrid.CategoryGroupDescriptionProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.CategoryGroupDescriptionProperty, value);
            }
        }

        public Binding PropertyNameBinding
        {
            get
            {
                return this._propertyNameBinding;
            }
            set
            {
                if (this._propertyNameBinding != value)
                {
                    this.ValidatePropertyBinding<Binding>(value, () => this.PropertyNameBinding);
                    this._propertyNameBinding = value;
                }
            }
        }

        public Binding PropertyValueBinding
        {
            get
            {
                return this._propertyValueBinding;
            }
            set
            {
                if (this._propertyValueBinding != value)
                {
                    this.ValidatePropertyBinding<Binding>(value, () => this.PropertyValueBinding);
                    this._propertyValueBinding = value;
                }
            }
        }

        public EditorDefinitionBase DefaultEditorDefinition
        {
            get
            {
                return (EditorDefinitionBase)base.GetValue(PropertyGrid.DefaultEditorDefinitionProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.DefaultEditorDefinitionProperty, value);
            }
        }

        public double NameColumnWidth
        {
            get
            {
                return (double)base.GetValue(PropertyGrid.NameColumnWidthProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.NameColumnWidthProperty, value);
            }
        }

        public IList Properties
        {
            get
            {
                return this._containerHelper.Properties;
            }
        }

        public IEnumerable PropertiesSource
        {
            get
            {
                return (IEnumerable)base.GetValue(PropertyGrid.PropertiesSourceProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.PropertiesSourceProperty, value);
            }
        }

        public Style PropertyContainerStyle
        {
            get
            {
                return (Style)base.GetValue(PropertyGrid.PropertyContainerStyleProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.PropertyContainerStyleProperty, value);
            }
        }

        public PropertyDefinitionCollection PropertyDefinitions
        {
            get
            {
                return this._propertyDefinitions;
            }
            set
            {
                if (this._propertyDefinitions != value)
                {
                    PropertyDefinitionCollection propertyDefinitions = this._propertyDefinitions;
                    this._propertyDefinitions = value;
                    this.OnPropertyDefinitionsChanged(propertyDefinitions, value);
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return (bool)base.GetValue(PropertyGrid.IsReadOnlyProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.IsReadOnlyProperty, value);
            }
        }

        public object SelectedObject
        {
            get
            {
                return base.GetValue(PropertyGrid.SelectedObjectProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.SelectedObjectProperty, value);
            }
        }

        public Type SelectedObjectType
        {
            get
            {
                return (Type)base.GetValue(PropertyGrid.SelectedObjectTypeProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.SelectedObjectTypeProperty, value);
            }
        }

        public string SelectedObjectTypeName
        {
            get
            {
                return (string)base.GetValue(PropertyGrid.SelectedObjectTypeNameProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.SelectedObjectTypeNameProperty, value);
            }
        }

        public string SelectedObjectName
        {
            get
            {
                return (string)base.GetValue(PropertyGrid.SelectedObjectNameProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.SelectedObjectNameProperty, value);
            }
        }

        public IList SelectedObjects
        {
            get
            {
                return this._selectedObjects;
            }
        }

        public IList SelectedObjectsOverride
        {
            get
            {
                return (IList)base.GetValue(PropertyGrid.SelectedObjectsOverrideProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.SelectedObjectsOverrideProperty, value);
            }
        }

        public PropertyItemBase SelectedPropertyItem
        {
            get
            {
                return (PropertyItemBase)base.GetValue(PropertyGrid.SelectedPropertyItemProperty);
            }
            internal set
            {
                base.SetValue(PropertyGrid.SelectedPropertyItemPropertyKey, value);
            }
        }

        public object SelectedProperty
        {
            get
            {
                return base.GetValue(PropertyGrid.SelectedPropertyProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.SelectedPropertyProperty, value);
            }
        }

        public bool ShowAdvancedOptions
        {
            get
            {
                return (bool)base.GetValue(PropertyGrid.ShowAdvancedOptionsProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.ShowAdvancedOptionsProperty, value);
            }
        }

        public bool ShowPreview
        {
            get
            {
                return (bool)base.GetValue(PropertyGrid.ShowPreviewProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.ShowPreviewProperty, value);
            }
        }

        public bool ShowSearchBox
        {
            get
            {
                return (bool)base.GetValue(PropertyGrid.ShowSearchBoxProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.ShowSearchBoxProperty, value);
            }
        }

        public bool ShowSortOptions
        {
            get
            {
                return (bool)base.GetValue(PropertyGrid.ShowSortOptionsProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.ShowSortOptionsProperty, value);
            }
        }

        public bool ShowTitle
        {
            get
            {
                return (bool)base.GetValue(PropertyGrid.ShowTitleProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.ShowTitleProperty, value);
            }
        }

        public bool UpdateTextBoxSourceOnEnterKey
        {
            get
            {
                return (bool)base.GetValue(PropertyGrid.UpdateTextBoxSourceOnEnterKeyProperty);
            }
            set
            {
                base.SetValue(PropertyGrid.UpdateTextBoxSourceOnEnterKeyProperty, value);
            }
        }

        FilterInfo IPropertyContainer.FilterInfo
        {
            get
            {
                return new FilterInfo
                {
                    Predicate = this.CreateFilter(this.Filter),
                    InputString = this.Filter
                };
            }
        }

        ContainerHelperBase IPropertyContainer.ContainerHelper
        {
            get
            {
                return this._containerHelper;
            }
        }

        protected virtual void OnEditorDefinitionsChanged(EditorDefinitionCollection oldValue, EditorDefinitionCollection newValue)
        {
            if (oldValue != null)
            {
                CollectionChangedEventManager.RemoveListener(oldValue, this._editorDefinitionsListener);
            }
            if (newValue != null)
            {
                CollectionChangedEventManager.AddListener(newValue, this._editorDefinitionsListener);
            }
            this.Notify(this.PropertyChanged, () => this.EditorDefinitions);
        }

        private void OnEditorDefinitionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this._containerHelper.NotifyEditorDefinitionsCollectionChanged();
        }

        private static void OnFilterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid propertyGrid = o as PropertyGrid;
            if (propertyGrid != null)
            {
                propertyGrid.OnFilterChanged((string)e.OldValue, (string)e.NewValue);
            }
        }

        protected virtual void OnFilterChanged(string oldValue, string newValue)
        {
            this.Notify(this.PropertyChanged, () => ((IPropertyContainer)this).FilterInfo);
        }

        private static void OnIsCategorizedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid propertyGrid = o as PropertyGrid;
            if (propertyGrid != null)
            {
                propertyGrid.OnIsCategorizedChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        protected virtual void OnIsCategorizedChanged(bool oldValue, bool newValue)
        {
            this.UpdateThumb();
        }

        protected virtual void OnCategoryDefinitionsChanged(CategoryDefinitionCollection oldValue, CategoryDefinitionCollection newValue)
        {
            if (oldValue != null)
            {
                CollectionChangedEventManager.RemoveListener(oldValue, this._categoryDefinitionsListener);
            }
            if (newValue != null)
            {
                CollectionChangedEventManager.AddListener(newValue, this._categoryDefinitionsListener);
            }
            this.Notify(this.PropertyChanged, () => this.CategoryDefinitions);
        }

        private void OnCategoryDefinitionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this._containerHelper.NotifyCategoryDefinitionsCollectionChanged();
        }

        private static object OnCoerceCategoryGroupDescription(DependencyObject o, object value)
        {
            return value;
        }

        private static void OnCategoryGroupDescriptionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid propertyGrid = o as PropertyGrid;
            if (propertyGrid != null)
            {
                propertyGrid.OnCategoryGroupDescriptionChanged((GroupDescription)e.OldValue, (GroupDescription)e.NewValue);
            }
        }

        private void OnCategoryGroupDescriptionChanged(GroupDescription oldValue, GroupDescription newValue)
        {
            this.UpdateThumb();
        }

        private static object OnCoerceDefaultEditorDefinition(DependencyObject o, object value)
        {
            return value;
        }

        private static void OnNameColumnWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid propertyGrid = o as PropertyGrid;
            if (propertyGrid != null)
            {
                propertyGrid.OnNameColumnWidthChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        protected virtual void OnNameColumnWidthChanged(double oldValue, double newValue)
        {
            if (this._dragThumb != null)
            {
                ((TranslateTransform)this._dragThumb.RenderTransform).X = newValue;
            }
        }

        private static object OnCoercePropertiesSourceChanged(DependencyObject o, object value)
        {
            return value;
        }

        private static void OnPropertiesSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid propertyGrid = o as PropertyGrid;
            if (propertyGrid != null)
            {
                propertyGrid.OnPropertiesSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
            }
        }

        private void OnPropertiesSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            this.UpdateContainerHelper();
        }

        private static void OnPropertyContainerStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid propertyGrid = o as PropertyGrid;
            if (propertyGrid != null)
            {
                propertyGrid.OnPropertyContainerStyleChanged((Style)e.OldValue, (Style)e.NewValue);
            }
        }

        protected virtual void OnPropertyContainerStyleChanged(Style oldValue, Style newValue)
        {
        }

        protected virtual void OnPropertyDefinitionsChanged(PropertyDefinitionCollection oldValue, PropertyDefinitionCollection newValue)
        {
            if (oldValue != null)
            {
                CollectionChangedEventManager.RemoveListener(oldValue, this._propertyDefinitionsListener);
            }
            if (newValue != null)
            {
                CollectionChangedEventManager.AddListener(newValue, this._propertyDefinitionsListener);
            }
            this.Notify(this.PropertyChanged, () => this.PropertyDefinitions);
        }

        private void OnPropertyDefinitionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this._containerHelper.NotifyPropertyDefinitionsCollectionChanged();
            if (base.IsLoaded)
            {
                this.UpdateContainerHelper();
            }
        }

        private static void OnSelectedObjectChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid propertyGrid = o as PropertyGrid;
            if (propertyGrid != null)
            {
                propertyGrid.OnSelectedObjectChanged(e.OldValue, e.NewValue);
            }
        }

        protected virtual void OnSelectedObjectChanged(object oldValue, object newValue)
        {
            if (this._initializationCount != 0)
            {
                this._hasPendingSelectedObjectChanged = true;
                return;
            }
            this.UpdateContainerHelper();
            base.RaiseEvent(new RoutedPropertyChangedEventArgs<object>(oldValue, newValue, PropertyGrid.SelectedObjectChangedEvent));
        }

        private static void OnSelectedObjectTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid propertyGrid = o as PropertyGrid;
            if (propertyGrid != null)
            {
                propertyGrid.OnSelectedObjectTypeChanged((Type)e.OldValue, (Type)e.NewValue);
            }
        }

        protected virtual void OnSelectedObjectTypeChanged(Type oldValue, Type newValue)
        {
        }

        private static object OnCoerceSelectedObjectName(DependencyObject o, object baseValue)
        {
            PropertyGrid propertyGrid = o as PropertyGrid;
            if (propertyGrid != null && propertyGrid.SelectedObject is FrameworkElement && string.IsNullOrEmpty((string)baseValue))
            {
                return "<no name>";
            }
            return baseValue;
        }

        private static void OnSelectedObjectNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid propertyGrid = o as PropertyGrid;
            if (propertyGrid != null)
            {
                propertyGrid.SelectedObjectNameChanged((string)e.OldValue, (string)e.NewValue);
            }
        }

        protected virtual void SelectedObjectNameChanged(string oldValue, string newValue)
        {
        }

        private static object OnCoerceSelectedObjectsOverride(DependencyObject sender, object value)
        {
            PropertyGrid.ValidateSelectedObjectsCollection((IList)value);
            return value;
        }

        private static void SelectedObjectsOverrideChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((PropertyGrid)sender).OnSelectedObjectsOverrideChanged((IList)args.OldValue, (IList)args.NewValue);
        }

        private void OnSelectedObjectsOverrideChanged(IList oldValue, IList newValue)
        {
            this.SetSelectedObjects(newValue ?? new ObservableCollection<object>());
        }

        private static void OnSelectedPropertyItemChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid propertyGrid = o as PropertyGrid;
            if (propertyGrid != null)
            {
                propertyGrid.OnSelectedPropertyItemChanged((PropertyItemBase)e.OldValue, (PropertyItemBase)e.NewValue);
            }
        }

        protected virtual void OnSelectedPropertyItemChanged(PropertyItemBase oldValue, PropertyItemBase newValue)
        {
            if (oldValue != null)
            {
                oldValue.IsSelected = false;
            }
            if (newValue != null)
            {
                newValue.IsSelected = true;
            }
            this.SelectedProperty = ((newValue != null) ? this._containerHelper.ItemFromContainer(newValue) : null);
            base.RaiseEvent(new RoutedPropertyChangedEventArgs<PropertyItemBase>(oldValue, newValue, PropertyGrid.SelectedPropertyItemChangedEvent));
        }

        private static void OnSelectedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            PropertyGrid propertyGrid = sender as PropertyGrid;
            if (propertyGrid != null)
            {
                propertyGrid.OnSelectedPropertyChanged(args.OldValue, args.NewValue);
            }
        }

        private void OnSelectedPropertyChanged(object oldValue, object newValue)
        {
            object objA = this._containerHelper.ItemFromContainer(this.SelectedPropertyItem);
            if (!object.Equals(objA, newValue))
            {
                this.SelectedPropertyItem = this._containerHelper.ContainerFromItem(newValue);
            }
        }

        static PropertyGrid()
        {
            PropertyGrid.SelectedObjectAdvancedOptionsMenuKey = new ComponentResourceKey(typeof(PropertyGrid), "SelectedObjectAdvancedOptionsMenu");
            PropertyGrid.AdvancedOptionsMenuProperty = DependencyProperty.Register("AdvancedOptionsMenu", typeof(ContextMenu), typeof(PropertyGrid), new UIPropertyMetadata(null));
            PropertyGrid.AutoGeneratePropertiesProperty = DependencyProperty.Register("AutoGenerateProperties", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(true));
            PropertyGrid.ShowSummaryProperty = DependencyProperty.Register("ShowSummary", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(true));
            PropertyGrid.FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(PropertyGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(PropertyGrid.OnFilterChanged)));
            PropertyGrid.FilterWatermarkProperty = DependencyProperty.Register("FilterWatermark", typeof(string), typeof(PropertyGrid), new UIPropertyMetadata("Search"));
            PropertyGrid.IsCategorizedProperty = DependencyProperty.Register("IsCategorized", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(true, new PropertyChangedCallback(PropertyGrid.OnIsCategorizedChanged)));
            PropertyGrid.CategoryGroupDescriptionProperty = DependencyProperty.Register("CategoryGroupDescription", typeof(GroupDescription), typeof(PropertyGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(PropertyGrid.OnCategoryGroupDescriptionChanged), new CoerceValueCallback(PropertyGrid.OnCoerceCategoryGroupDescription)));
            PropertyGrid.DefaultEditorDefinitionProperty = DependencyProperty.Register("DefaultEditorDefinition", typeof(EditorDefinitionBase), typeof(PropertyGrid), new UIPropertyMetadata(null, null, new CoerceValueCallback(PropertyGrid.OnCoerceDefaultEditorDefinition)));
            PropertyGrid.NameColumnWidthProperty = DependencyProperty.Register("NameColumnWidth", typeof(double), typeof(PropertyGrid), new UIPropertyMetadata(150.0, new PropertyChangedCallback(PropertyGrid.OnNameColumnWidthChanged)));
            PropertyGrid.PropertiesSourceProperty = DependencyProperty.Register("PropertiesSource", typeof(IEnumerable), typeof(PropertyGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(PropertyGrid.OnPropertiesSourceChanged), new CoerceValueCallback(PropertyGrid.OnCoercePropertiesSourceChanged)));
            PropertyGrid.PropertyContainerStyleProperty = DependencyProperty.Register("PropertyContainerStyle", typeof(Style), typeof(PropertyGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(PropertyGrid.OnPropertyContainerStyleChanged)));
            PropertyGrid.IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(false));
            PropertyGrid.SelectedObjectProperty = DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertyGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(PropertyGrid.OnSelectedObjectChanged)));
            PropertyGrid.SelectedObjectTypeProperty = DependencyProperty.Register("SelectedObjectType", typeof(Type), typeof(PropertyGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(PropertyGrid.OnSelectedObjectTypeChanged)));
            PropertyGrid.SelectedObjectTypeNameProperty = DependencyProperty.Register("SelectedObjectTypeName", typeof(string), typeof(PropertyGrid), new UIPropertyMetadata(string.Empty));
            PropertyGrid.SelectedObjectNameProperty = DependencyProperty.Register("SelectedObjectName", typeof(string), typeof(PropertyGrid), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(PropertyGrid.OnSelectedObjectNameChanged), new CoerceValueCallback(PropertyGrid.OnCoerceSelectedObjectName)));
            PropertyGrid.SelectedObjectsOverrideProperty = DependencyProperty.Register("SelectedObjectsOverride", typeof(IList), typeof(PropertyGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(PropertyGrid.SelectedObjectsOverrideChanged), new CoerceValueCallback(PropertyGrid.OnCoerceSelectedObjectsOverride)));
            PropertyGrid.SelectedPropertyItemPropertyKey = DependencyProperty.RegisterReadOnly("SelectedPropertyItem", typeof(PropertyItemBase), typeof(PropertyGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(PropertyGrid.OnSelectedPropertyItemChanged)));
            PropertyGrid.SelectedPropertyItemProperty = PropertyGrid.SelectedPropertyItemPropertyKey.DependencyProperty;
            PropertyGrid.SelectedPropertyProperty = DependencyProperty.Register("SelectedProperty", typeof(object), typeof(PropertyGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(PropertyGrid.OnSelectedPropertyChanged)));
            PropertyGrid.ShowAdvancedOptionsProperty = DependencyProperty.Register("ShowAdvancedOptions", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(false));
            PropertyGrid.ShowPreviewProperty = DependencyProperty.Register("ShowPreview", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(false));
            PropertyGrid.ShowSearchBoxProperty = DependencyProperty.Register("ShowSearchBox", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(true));
            PropertyGrid.ShowSortOptionsProperty = DependencyProperty.Register("ShowSortOptions", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(true));
            PropertyGrid.ShowTitleProperty = DependencyProperty.Register("ShowTitle", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(true));
            PropertyGrid.UpdateTextBoxSourceOnEnterKeyProperty = DependencyProperty.Register("UpdateTextBoxSourceOnEnterKey", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(true));
            PropertyGrid.PropertyValueChangedEvent = EventManager.RegisterRoutedEvent("PropertyValueChanged", RoutingStrategy.Bubble, typeof(PropertyValueChangedEventHandler), typeof(PropertyGrid));
            PropertyGrid.SelectedPropertyItemChangedEvent = EventManager.RegisterRoutedEvent("SelectedPropertyItemChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<PropertyItemBase>), typeof(PropertyGrid));
            PropertyGrid.SelectedObjectChangedEvent = EventManager.RegisterRoutedEvent("SelectedObjectChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(PropertyGrid));
            PropertyGrid.PreparePropertyItemEvent = EventManager.RegisterRoutedEvent("PreparePropertyItem", RoutingStrategy.Bubble, typeof(PropertyItemEventHandler), typeof(PropertyGrid));
            PropertyGrid.ClearPropertyItemEvent = EventManager.RegisterRoutedEvent("ClearPropertyItem", RoutingStrategy.Bubble, typeof(PropertyItemEventHandler), typeof(PropertyGrid));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid), new FrameworkPropertyMetadata(typeof(PropertyGrid)));
        }

        public PropertyGrid()
        {
            this._propertyDefinitionsListener = new WeakEventListener<NotifyCollectionChangedEventArgs>(new Action<object, NotifyCollectionChangedEventArgs>(this.OnPropertyDefinitionsCollectionChanged));
            this._editorDefinitionsListener = new WeakEventListener<NotifyCollectionChangedEventArgs>(new Action<object, NotifyCollectionChangedEventArgs>(this.OnEditorDefinitionsCollectionChanged));
            this._selectedObjectsListener = new WeakEventListener<NotifyCollectionChangedEventArgs>(new Action<object, NotifyCollectionChangedEventArgs>(this.OnSelectedObjectsCollectionChanged));
            this._categoryDefinitionsListener = new WeakEventListener<NotifyCollectionChangedEventArgs>(new Action<object, NotifyCollectionChangedEventArgs>(this.OnCategoryDefinitionsCollectionChanged));
            this.UpdateContainerHelper();
            this.EditorDefinitions = new EditorDefinitionCollection();
            this.PropertyDefinitions = new PropertyDefinitionCollection();
            this.CategoryDefinitions = new CategoryDefinitionCollection();
            this.SetSelectedObjects(new ObservableCollection<object>());
            base.AddHandler(PropertyItemBase.ItemSelectionChangedEvent, new RoutedEventHandler(this.OnItemSelectionChanged));
            base.AddHandler(PropertyItemsControl.PreparePropertyItemEvent, new PropertyItemEventHandler(this.OnPreparePropertyItemInternal));
            base.AddHandler(PropertyItemsControl.ClearPropertyItemEvent, new PropertyItemEventHandler(this.OnClearPropertyItemInternal));
            base.CommandBindings.Add(new CommandBinding(PropertyGridCommands.ClearFilter, new ExecutedRoutedEventHandler(this.ClearFilter), new CanExecuteRoutedEventHandler(this.CanClearFilter)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this._dragThumb != null)
            {
                this._dragThumb.DragDelta -= new DragDeltaEventHandler(this.DragThumb_DragDelta);
            }
            this._dragThumb = (base.GetTemplateChild("PART_DragThumb") as Thumb);
            if (this._dragThumb != null)
            {
                this._dragThumb.DragDelta += new DragDeltaEventHandler(this.DragThumb_DragDelta);
            }
            this._containerHelper.ChildrenItemsControl = (base.GetTemplateChild("PART_PropertyItemsControl") as PropertyItemsControl);
            TranslateTransform translateTransform = new TranslateTransform();
            translateTransform.X = this.NameColumnWidth;

            if (this._dragThumb != null)
            {
                this._dragThumb.RenderTransform = translateTransform;
            }

            this.UpdateThumb();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            TextBox textBox = e.OriginalSource as TextBox;
            if (this.SelectedPropertyItem != null && e.Key == Key.Return && this.UpdateTextBoxSourceOnEnterKey && textBox != null && !textBox.AcceptsReturn)
            {
                BindingExpression bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
                if (bindingExpression != null)
                {
                    bindingExpression.UpdateSource();
                }
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (ReflectionHelper.IsPublicInstanceProperty(base.GetType(), e.Property.Name))
            {
                this.Notify(this.PropertyChanged, e.Property.Name);
            }
        }

        private void OnItemSelectionChanged(object sender, RoutedEventArgs args)
        {
            PropertyItemBase propertyItemBase = (PropertyItemBase)args.OriginalSource;
            if (propertyItemBase.IsSelected)
            {
                this.SelectedPropertyItem = propertyItemBase;
                return;
            }
            if (object.ReferenceEquals(propertyItemBase, this.SelectedPropertyItem))
            {
                this.SelectedPropertyItem = null;
            }
        }

        private void OnPreparePropertyItemInternal(object sender, PropertyItemEventArgs args)
        {
            this._containerHelper.PrepareChildrenPropertyItem(args.PropertyItem, args.Item);
            args.Handled = true;
        }

        private void OnClearPropertyItemInternal(object sender, PropertyItemEventArgs args)
        {
            this._containerHelper.ClearChildrenPropertyItem(args.PropertyItem, args.Item);
            args.Handled = true;
        }

        private void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.NameColumnWidth = Math.Max(0.0, this.NameColumnWidth + e.HorizontalChange);
        }

        private void ClearFilter(object sender, ExecutedRoutedEventArgs e)
        {
            this.Filter = string.Empty;
        }

        private void CanClearFilter(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(this.Filter);
        }

        private void UpdateContainerHelper()
        {
            ItemsControl childrenItemsControl = null;
            if (this._containerHelper != null)
            {
                childrenItemsControl = this._containerHelper.ChildrenItemsControl;
                this._containerHelper.ClearHelper();
                if (this._containerHelper is ObjectContainerHelperBase)
                {
                    ContextMenu contextMenu = (ContextMenu)base.FindResource(PropertyGrid.SelectedObjectAdvancedOptionsMenuKey);
                    if (this.AdvancedOptionsMenu == contextMenu)
                    {
                        this.AdvancedOptionsMenu = null;
                    }
                }
            }
            ObjectContainerHelperBase objectContainerHelperBase = null;
            if (this.PropertiesSource != null)
            {
                this._containerHelper = new PropertiesSourceContainerHelper(this, this.PropertiesSource);
            }
            else
            {
                if (this.SelectedObjects != null && this.SelectedObjects.Count > 0)
                {
                    objectContainerHelperBase = new ObjectsContainerHelper(this, this.SelectedObjects);
                }
                else
                {
                    if (this.SelectedObject != null)
                    {
                        objectContainerHelperBase = new ObjectContainerHelper(this, this.SelectedObject);
                    }
                    else
                    {
                        this._containerHelper = new PropertiesContainerHelper(this);
                    }
                }
            }
            if (objectContainerHelperBase != null)
            {
                objectContainerHelperBase.ChildrenItemsControl = childrenItemsControl;
                this._containerHelper = objectContainerHelperBase;
                objectContainerHelperBase.GenerateProperties();
                if (this.AdvancedOptionsMenu == null)
                {
                    this.AdvancedOptionsMenu = (ContextMenu)base.FindResource(PropertyGrid.SelectedObjectAdvancedOptionsMenuKey);
                }
            }
            //if (!(this._containerHelper is ObjectContainerHelper) && this._containerHelper != null)
            //{
            //}
            this._containerHelper.ChildrenItemsControl = childrenItemsControl;
            this.Notify(this.PropertyChanged, () => this.Properties);
        }

        private void SetSelectedObjects(IList newSelectedObjectsList)
        {
            if (newSelectedObjectsList == null)
            {
                throw new ArgumentNullException("newSelectedObjectsList");
            }
            //if (this._selectedObjects != null)
            //{
            //}
            INotifyCollectionChanged notifyCollectionChanged = this._selectedObjects as INotifyCollectionChanged;
            INotifyCollectionChanged notifyCollectionChanged2 = newSelectedObjectsList as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                CollectionChangedEventManager.RemoveListener(notifyCollectionChanged, this._selectedObjectsListener);
            }
            if (notifyCollectionChanged2 != null)
            {
                CollectionChangedEventManager.AddListener(notifyCollectionChanged2, this._selectedObjectsListener);
            }
            this._selectedObjects = newSelectedObjectsList;
            this.UpdateContainerHelper();
        }

        private void OnSelectedObjectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PropertyGrid.ValidateSelectedObjectsCollection((IList)sender);
            this.UpdateContainerHelper();
        }

        private static void ValidateSelectedObjectsCollection(IList objectList)
        {
            if (objectList != null)
            {
                if (objectList.Cast<object>().Any((object o) => o == null))
                {
                    throw new InvalidOperationException("The SelectedObjects collection cannot contain any null entries");
                }
            }
        }

        private void UpdateThumb()
        {
            if (this._dragThumb != null)
            {
                if (this.IsCategorized)
                {
                    this._dragThumb.Margin = new Thickness(6.0, 0.0, 0.0, 0.0);
                    return;
                }
                this._dragThumb.Margin = new Thickness(-1.0, 0.0, 0.0, 0.0);
            }
        }

        protected virtual Predicate<object> CreateFilter(string filter)
        {
            return null;
        }

        public void Update()
        {
            this._containerHelper.UpdateValuesFromSource();
        }

        private void ValidatePropertyBinding<TMember>(Binding value, Expression<Func<TMember>> property)
        {
            if (value == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(value.BindingGroupName))
            {
                throw new InvalidOperationException("BindingGroupName must be null on " + ReflectionHelper.GetPropertyOrFieldName<TMember>(property));
            }
            if (value.IsAsync)
            {
                throw new InvalidOperationException("IsAsync must be false on " + ReflectionHelper.GetPropertyOrFieldName<TMember>(property));
            }
            if (value.RelativeSource != null)
            {
                throw new InvalidOperationException("RelativeSource must be null on " + ReflectionHelper.GetPropertyOrFieldName<TMember>(property));
            }
            if (value.Source != null)
            {
                throw new InvalidOperationException("Source must be null on " + ReflectionHelper.GetPropertyOrFieldName<TMember>(property));
            }
        }

        public static void AddPreparePropertyItemHandler(UIElement element, PropertyItemEventHandler handler)
        {
            element.AddHandler(PropertyGrid.PreparePropertyItemEvent, handler);
        }

        public static void RemovePreparePropertyItemHandler(UIElement element, PropertyItemEventHandler handler)
        {
            element.RemoveHandler(PropertyGrid.PreparePropertyItemEvent, handler);
        }

        internal static void RaisePreparePropertyItemEvent(UIElement source, PropertyItemBase propertyItem, object item)
        {
            source.RaiseEvent(new PropertyItemEventArgs(PropertyGrid.PreparePropertyItemEvent, source, propertyItem, item));
        }

        public static void AddClearPropertyItemHandler(UIElement element, PropertyItemEventHandler handler)
        {
            element.AddHandler(PropertyGrid.ClearPropertyItemEvent, handler);
        }

        public static void RemoveClearPropertyItemHandler(UIElement element, PropertyItemEventHandler handler)
        {
            element.RemoveHandler(PropertyGrid.ClearPropertyItemEvent, handler);
        }

        internal static void RaiseClearPropertyItemEvent(UIElement source, PropertyItemBase propertyItem, object item)
        {
            source.RaiseEvent(new PropertyItemEventArgs(PropertyGrid.ClearPropertyItemEvent, source, propertyItem, item));
        }

        public override void BeginInit()
        {
            base.BeginInit();
            this._initializationCount++;
        }

        public override void EndInit()
        {
            base.EndInit();
            if (--this._initializationCount == 0)
            {
                if (this._hasPendingSelectedObjectChanged)
                {
                    this.UpdateContainerHelper();
                    this._hasPendingSelectedObjectChanged = false;
                }
                this._containerHelper.OnEndInit();
            }
        }
    }

    #region PropertyValueChangedEvent Handler/Args

    public delegate void PropertyValueChangedEventHandler(object sender, PropertyValueChangedEventArgs e);

    public class PropertyValueChangedEventArgs : RoutedEventArgs
    {
        public object NewValue
        {
            get;
            set;
        }

        public object OldValue
        {
            get;
            set;
        }

        public PropertyValueChangedEventArgs(RoutedEvent routedEvent, object source, object oldValue, object newValue)
            : base(routedEvent, source)
        {
            NewValue = newValue;
            OldValue = oldValue;
        }
    }

    #endregion PropertyValueChangedEvent Handler/Args

    #region PropertyItemCreatedEvent Handler/Args

    public delegate void PropertyItemEventHandler(object sender, PropertyItemEventArgs e);

    public class PropertyItemEventArgs : RoutedEventArgs
    {
        public PropertyItemBase PropertyItem
        {
            get;
            private set;
        }

        public object Item
        {
            get;
            private set;
        }

        public PropertyItemEventArgs(RoutedEvent routedEvent, object source, PropertyItemBase propertyItem, object item)
            : base(routedEvent, source)
        {
            this.PropertyItem = propertyItem;
            this.Item = item;
        }
    }

    #endregion PropertyItemCreatedEvent Handler/Args
}