using System.Collections.ObjectModel;
using System.Windows.Controls;
using NLog;

namespace SP2013Access.Controls
{
    /// <summary>
    ///     Interaction logic for LogViewer.xaml
    /// </summary>
    public partial class LogViewer : UserControl
    {
        public LogViewer()
        {
            InitializeComponent();
            LogEntries = new ObservableCollection<LogEventInfo>();
            DataContext = LogEntries;
        }

        public ObservableCollection<LogEventInfo> LogEntries { get; set; }
    }
}