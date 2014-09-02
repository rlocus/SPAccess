using SP2013Access.Controls.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;

namespace SP2013Access.Controls.PropertyGrid
{
    public class PropertyItemCollection : ReadOnlyObservableCollection<PropertyItem>
    {
        internal static readonly string CategoryPropertyName;
        internal static readonly string CategoryOrderPropertyName;
        internal static readonly string PropertyOrderPropertyName;
        internal static readonly string DisplayNamePropertyName;
        private bool _preventNotification;

        internal Predicate<object> FilterPredicate
        {
            get
            {
                return this.GetDefaultView().Filter;
            }
            set
            {
                this.GetDefaultView().Filter = value;
            }
        }

        public ObservableCollection<PropertyItem> EditableCollection
        {
            get;
            private set;
        }

        static PropertyItemCollection()
        {
            PropertyItem p = null;
            PropertyItemCollection.CategoryPropertyName = ReflectionHelper.GetPropertyOrFieldName<string>(() => p.Category);
            PropertyItemCollection.CategoryOrderPropertyName = ReflectionHelper.GetPropertyOrFieldName<int>(() => p.CategoryOrder);
            PropertyItemCollection.PropertyOrderPropertyName = ReflectionHelper.GetPropertyOrFieldName<int>(() => p.PropertyOrder);
            PropertyItemCollection.DisplayNamePropertyName = ReflectionHelper.GetPropertyOrFieldName<string>(() => p.DisplayName);
        }

        public PropertyItemCollection(ObservableCollection<PropertyItem> editableCollection)
            : base(editableCollection)
        {
            this.EditableCollection = editableCollection;
        }

        private ICollectionView GetDefaultView()
        {
            return CollectionViewSource.GetDefaultView(this);
        }

        public void GroupBy(string name)
        {
            this.GetDefaultView().GroupDescriptions.Add(new PropertyGroupDescription(name));
        }

        public void SortBy(string name, ListSortDirection sortDirection)
        {
            this.GetDefaultView().SortDescriptions.Add(new SortDescription(name, sortDirection));
        }

        public void Filter(string text)
        {
            Predicate<object> filter = PropertyItemCollection.CreateFilter(text);
            this.GetDefaultView().Filter = filter;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this._preventNotification)
            {
                return;
            }
            base.OnCollectionChanged(args);
        }

        internal void UpdateItems(IEnumerable<PropertyItem> newItems)
        {
            if (newItems == null)
            {
                throw new ArgumentNullException("newItems");
            }
            this._preventNotification = true;
            using (this.GetDefaultView().DeferRefresh())
            {
                this.EditableCollection.Clear();
                foreach (PropertyItem current in newItems)
                {
                    this.EditableCollection.Add(current);
                }
            }
            this._preventNotification = false;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        internal void UpdateCategorization(GroupDescription groupDescription, bool isPropertyGridCategorized)
        {
            foreach (PropertyItem current in base.Items)
            {
                current.DescriptorDefinition.DisplayOrder = current.DescriptorDefinition.ComputeDisplayOrderInternal(isPropertyGridCategorized);
                current.PropertyOrder = current.DescriptorDefinition.DisplayOrder;
            }
            ICollectionView defaultView = this.GetDefaultView();
            using (defaultView.DeferRefresh())
            {
                defaultView.GroupDescriptions.Clear();
                defaultView.SortDescriptions.Clear();
                if (groupDescription != null)
                {
                    defaultView.GroupDescriptions.Add(groupDescription);
                    this.SortBy(PropertyItemCollection.CategoryOrderPropertyName, ListSortDirection.Ascending);
                    this.SortBy(PropertyItemCollection.CategoryPropertyName, ListSortDirection.Ascending);
                }
                this.SortBy(PropertyItemCollection.PropertyOrderPropertyName, ListSortDirection.Ascending);
                this.SortBy(PropertyItemCollection.DisplayNamePropertyName, ListSortDirection.Ascending);
            }
        }

        internal void RefreshView()
        {
            this.GetDefaultView().Refresh();
        }

        internal static Predicate<object> CreateFilter(string text)
        {
            Predicate<object> result = null;
            if (!string.IsNullOrEmpty(text))
            {
                result = delegate(object item)
                {
                    PropertyItem propertyItem = item as PropertyItem;
                    return propertyItem.DisplayName != null && propertyItem.DisplayName.ToLower().StartsWith(text.ToLower());
                };
            }
            return result;
        }
    }
}