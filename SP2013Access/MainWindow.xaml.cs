using System.Collections.Generic;
using System.ComponentModel;
using SharePoint.Remote.Access.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SP2013Access
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<SPClientContext> _clientContexts;
        public SPClientContext[] ClientContexts
        {
            get { return _clientContexts.ToArray(); }
        }

        public ObservableCollection<RecentSite> RecentSites
        {
            get;
            private set;
        }
        
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            RecentSites = new ObservableCollection<RecentSite>();
            _clientContexts = new List<SPClientContext>();
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
                _clientContexts.Add(clientContext);
                ClientTreeView.Fill(clientContext);
                LoadMenu();
            }

        }

        private void RecentMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var menuItem = e.OriginalSource as MenuItem;
            if (menuItem != null)
            {
                var recentSite = menuItem.DataContext as RecentSite;
                LoadSite(recentSite);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            foreach (SPClientContext clientContext in ClientContexts)
            {
                clientContext.Dispose();
            }
        }
    }
}