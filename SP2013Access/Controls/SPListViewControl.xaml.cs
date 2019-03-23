using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SP2013Access.Controls
{
    /// <summary>
    ///     Interaction logic for SPListViewControl.xaml
    /// </summary>
    public partial class SPListViewControl : UserControl
    {
        public SPListViewControl()
        {
            InitializeComponent();
            PagingControl.DataContext = this;
        }

        public IPageControlContract Contract => new PageControlContract();

        private void PagingControl_PreviewPageChange(object sender, PageChangedEventArgs args)
        {
            var items = PagingControl.ItemsSource.ToList();
            var count = items.Count;
        }

        private void PagingControl_PageChanged(object sender, PageChangedEventArgs args)
        {
            var items = PagingControl.ItemsSource.ToList();
            var count = items.Count;
        }

        private void SortableListViewColumnHeaderClicked(object sender, RoutedEventArgs e)
        {
            ((ExListView)sender).GridViewColumnHeaderClicked(e.OriginalSource as GridViewColumnHeader);
        }
    }

    public class PageControlContract : IPageControlContract
    {
        public uint GetTotalCount()
        {
            return 0;
        }

        public ICollection<object> FetchRange(uint startingIndex, uint numberOfRecords, object filterTag)
        {
            return new List<object>()
            {
                new {Property = "1", Value = "1"},
                 new {Property = "2", Value = "2"},
                  new {Property = "3", Value = "1"}
            };
        }
    }
}