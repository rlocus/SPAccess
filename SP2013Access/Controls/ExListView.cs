using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SP2013Access.Controls
{
    public class CheckableListViewItem : ListViewItem
    {
        [Category("Appearance")]
        [Bindable(true)]
        public bool IsChecked { get; set; }
    }

    public class ExListView : ListView
    {
        private GridViewColumnHeader _lastHeaderClicked;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        public void GridViewColumnHeaderClicked(GridViewColumnHeader clickedHeader)
        {
            if (clickedHeader != null)
            {
                if (clickedHeader.Role != GridViewColumnHeaderRole.Padding)
                {
                    ListSortDirection direction;
                    if (!Equals(clickedHeader, _lastHeaderClicked))
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        direction = _lastDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    }
                    var col = clickedHeader.Column.DisplayMemberBinding;
                    if (col != null)
                    {
                        string sortString = ((Binding)col).Path.Path;
                        Sort(sortString, direction);
                    }
                    if (direction == ListSortDirection.Ascending)
                    {
                        clickedHeader.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        clickedHeader.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && !Equals(_lastHeaderClicked, clickedHeader))
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = clickedHeader;
                    _lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(ItemsSource ?? Items);
            dataView.SortDescriptions.Clear();
            SortDescription sort = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sort);
            dataView.Refresh();
        }

        public IList CheckedItems
        {
            get
            {
                return
                    Items.OfType<CheckableListViewItem>()
                        .Where(checkableListViewItem => checkableListViewItem.IsChecked)
                        .ToList();
            }
        }

        public bool IsChecked(int index)
        {
            if (index < Items.Count)
            {
                var checkableListViewItem = ItemContainerGenerator.ContainerFromIndex(index) as CheckableListViewItem;
                return checkableListViewItem != null && checkableListViewItem.IsChecked;
            }
            throw new IndexOutOfRangeException();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is CheckableListViewItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CheckableListViewItem();
        }
    }
}