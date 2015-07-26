using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace SP2013Access.Controls
{
    /// <summary>
    /// Interaction logic for SPListViewControl.xaml
    /// </summary>
    public partial class SPListViewControl : UserControl
    {
        public SPListViewControl()
        {
            InitializeComponent();
            this.PaggingControl.DataContext = this;
        }

        public IPageControlContract Contract
        {
            get { return /* new PageControlContract();*/ null; }
        }

        private void PaggingControl_PreviewPageChange(object sender, PageChangedEventArgs args)
        {
            List<Object> items = PaggingControl.ItemsSource.ToList();
            int count = items.Count;
        }

        private void PaggingControl_PageChanged(object sender, PageChangedEventArgs args)
        {
            List<Object> items = PaggingControl.ItemsSource.ToList();
            int count = items.Count;
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
            return null;
        }
    }
}
