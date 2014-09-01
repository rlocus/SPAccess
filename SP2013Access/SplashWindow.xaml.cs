using System.Threading;

namespace SP2013Access
{
    public interface ISplashScreen
    {
        void AddMessage(string message);

        void LoadComplete();

        void Show();
    }

    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : DynamicSplashScreen, ISplashScreen
    {
        public SplashWindow()
        {
            InitializeComponent();
        }

        public void AddMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                this.MessageTextBox.Content = message;
            });
            Thread.Sleep(500);
        }

        public void LoadComplete()
        {
            Dispatcher.InvokeShutdown();
        }
    }
}