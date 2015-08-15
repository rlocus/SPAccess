using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NLog;
using NLog.Config;
using SP2013Access.Logging;

namespace SP2013Access
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Logger _logger;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            RecentSites = new ObservableCollection<RecentSite>();
            LoadMenu();
        }

        public ObservableCollection<RecentSite> RecentSites { get; }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ClientTreeView.SiteLoadingEvent += ClientTreeView_SiteOpening;
            Globals.SplashScreen.LoadComplete();

            var logViewer = LogViewer;
            _logger = LogManager.GetLogger("Logger");

            Dispatcher.Invoke(() =>
            {
                var target = new LogViewerTarget
                {
                    Name = "LogViewer",
                    Layout =
                        "[${longdate:useUTC=false}] :: [${level:uppercase=true}] :: ${logger}:${callsite} :: ${message} ${exception:innerFormat=tostring:maxInnerExceptionLevel=10:separator=,:format=tostring}",
                    TargetLogViewer = logViewer
                };
                SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Trace);
                _logger.Info("Ready");
                LoadSite();
            });
        }

        private void LoadMenu()
        {
            RecentMenuItem.DataContext = this;
            RecentSites.Clear();

            foreach (var recentSite in Globals.Configuration.RecentSites.OrderBy(recentSite => recentSite.Url))
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
                Owner = this
            };

            if (openSiteWindow.ShowDialog() == true)
            {
                var clientContext = openSiteWindow.ClientContext;
                if (clientContext != null)
                {
                    _logger.Info("Connected to {0}", clientContext.Url);
                    ClientTreeView.Fill(clientContext, _logger);
                }
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
            ClientTreeView.Unload();
        }
    }
}