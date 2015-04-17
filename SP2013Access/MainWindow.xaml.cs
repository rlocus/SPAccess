using System.Collections.Generic;
using SharePoint.Remote.Access.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SP2013Access.Controls.PropertyGrid;

namespace SP2013Access
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<RecentSite> RecentSites
        {
            get;
            private set;
        }

        public ObservableCollection<CustomPropertyItem> PropertyItems
        {
            get;
            private set;
        }

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            RecentSites = new ObservableCollection<RecentSite>();
            LoadMenu();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ClientTreeView.SiteLoading += ClientTreeView_SiteOpening;
            Globals.SplashScreen.LoadComplete();
            LoadSite();
        }

        private void LoadMenu()
        {
            RecentMenuItem.DataContext = this;
            RecentSites.Clear();

            foreach (RecentSite recentSite in Globals.Configuration.RecentSites.OrderBy(recentSite => recentSite.Url))
            {
                RecentSites.Add(recentSite);
            }
        }

        private void LoadSiteMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            LoadSite();
        }

        private void ClientTreeView_SiteOpening(object sender, EventArgs e)
        {
            LoadSite();
        }

        private void LoadSite(RecentSite recentSite = null)
        {
            if (recentSite == null)
            {
                recentSite = Globals.Configuration.RecentSites.LastOrDefault();
            }

            var openSiteWindow = new OpenSiteWindow(recentSite)
            {
                Owner = this,
            };

            if (openSiteWindow.ShowDialog() == true)
            {
                SPClientContext clientContext = openSiteWindow.ClientContext;
                ClientTreeView.Fill(clientContext);
                LoadMenu();

                PropertyItems = new ObservableCollection<CustomPropertyItem>();
                PropertyItems.Add(new CustomPropertyItem() {DisplayName = "Url", Value = clientContext.Url});
                PropertyItems.Add(new CustomPropertyItem() { DisplayName = "UserName", Value = clientContext.UserName });
                PropertyGrid.PropertiesSource = PropertyItems;
            }
        
        }

        private void RecentMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var menuItem = e.OriginalSource as MenuItem;
            var recentSite = menuItem.DataContext as RecentSite;
            LoadSite(recentSite);
        }
    }
}