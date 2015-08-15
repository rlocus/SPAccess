using System;
using System.Windows;
using System.Windows.Controls;
using NLog;
using SharePoint.Remote.Access.Helpers;
using SP2013Access.Extensions;
using SP2013Access.ViewModels;

namespace SP2013Access.Controls
{
    /// <summary>
    ///     Interaction logic for SPClientTreeView.xaml
    /// </summary>
    public partial class SPClientTreeView : UserControl
    {
        private readonly SPContextCollectionViewModel _viewModel;

        public SPClientTreeView()
        {
            InitializeComponent();
            _viewModel = new SPContextCollectionViewModel();
        }

        public bool IsMulti { get; set; }

        public void Fill(SPClientContext clientContext, Logger logger)
        {
            if (logger != null)
            {
                _viewModel.FailEvent += (senser, e) => Dispatcher.Invoke(() => logger.Error(e.Exception.Message));
            }
            //if (IsMulti)
            //{
            _viewModel.Add(clientContext);
            DataContext = _viewModel;
            //}
        }

        private void MenuItem_Refresh(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                var treeViewItemViewModel = (menuItem.DataContext) as TreeViewItemViewModel;
                if (treeViewItemViewModel != null) treeViewItemViewModel.Refresh();
            }
        }

        private void MenuItem_Expand(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                var treeViewItemViewModel = (menuItem.DataContext) as TreeViewItemViewModel;
                var cp = menuItem.TemplatedParent as ContentPresenter;
                var tvi = cp?.TemplatedParent as TreeViewItem;
                tvi?.ExpandCollapseAll<TreeViewItemViewModel>(!tvi.IsExpanded,
                    item => item == treeViewItemViewModel || item.HasDummyChild == false);
            }
        }

        public event EventHandler SiteLoadingEvent;

        protected virtual void OnSiteLoading(EventArgs e)
        {
            var handler = SiteLoadingEvent;
            handler?.Invoke(this, e);
        }

        private void LoadSiteButton_Click(object sender, RoutedEventArgs e)
        {
            OnSiteLoading(e);
        }

        public void Unload()
        {
            _viewModel.Dispose();
        }
    }
}