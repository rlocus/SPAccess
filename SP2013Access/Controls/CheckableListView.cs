using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SP2013Access.Controls
{
    public class CheckableListViewItem : ListViewItem
    {
        [Category("Appearance")]
        [Bindable(true)]
        public bool IsChecked { get; set; }
    }

    public class CheckableListView : ListView
    {
        public IList CheckedItems
        {
            get
            {
                return this.Items.OfType<CheckableListViewItem>().Where(checkableListViewItem => checkableListViewItem.IsChecked).ToList();
            }
        }

        public bool IsChecked(int index)
        {
            if (index < this.Items.Count)
            {
                CheckableListViewItem checkableListViewItem = this.ItemContainerGenerator.ContainerFromIndex(index) as CheckableListViewItem;
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
