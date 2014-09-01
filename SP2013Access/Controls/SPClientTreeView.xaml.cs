using SharePoint.Remote.Access.Helpers;
using SP2013Access.Extensions;
using SP2013Access.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SP2013Access.Controls
{
    /// <summary>
    /// Interaction logic for SPClientTreeView.xaml
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

        public void Fill(SPClientContext clientContext)
        {
            if (IsMulti)
            {
                _viewModel.Add(clientContext);
                base.DataContext = _viewModel;
            }
            else
            {
                var viewModel = new SPContextViewModel(clientContext);
                viewModel.LoadChildren();
                base.DataContext = viewModel;
            }
        }

        private void MenuItem_Refresh(object sender, RoutedEventArgs e)
        {
            TreeViewItemViewModel treeViewItemViewModel = ((sender as MenuItem).DataContext) as TreeViewItemViewModel;
            treeViewItemViewModel.Refresh();
        }

        private void MenuItem_Expand(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            TreeViewItemViewModel treeViewItemViewModel = ((sender as MenuItem).DataContext) as TreeViewItemViewModel;

            if (menuItem != null)
            {
                ContentPresenter cp = menuItem.TemplatedParent as ContentPresenter;

                if (cp != null)
                {
                    TreeViewItem tvi = cp.TemplatedParent as TreeViewItem;
                    if (tvi != null)
                    {
                        tvi.ExpandCollapseAll<TreeViewItemViewModel>(!tvi.IsExpanded,
                            item => item == treeViewItemViewModel || item.HasDummyChild == false);
                    }
                }
            }
        }

        public event EventHandler SiteLoading;

        protected virtual void OnSiteLoading(EventArgs e)
        {
            EventHandler handler = SiteLoading;
            if (handler != null) handler(this, e);
        }

        private void LoadSiteButton_Click(object sender, RoutedEventArgs e)
        {
            OnSiteLoading(e);
        }
    }
}