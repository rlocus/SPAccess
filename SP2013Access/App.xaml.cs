using System.Threading;
using System.Windows;

namespace SP2013Access
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ManualResetEvent _resetSplashCreated;
        private Thread _splashThread;

        protected override void OnStartup(StartupEventArgs e)
        {
            // ManualResetEvent acts as a block. It waits for a signal to be set.
            _resetSplashCreated = new ManualResetEvent(false);

            // Create a new thread for the splash screen to run on
            _splashThread = new Thread(ShowSplash);
            _splashThread.SetApartmentState(ApartmentState.STA);
            _splashThread.IsBackground = true;
            _splashThread.Name = "Splash Screen";
            _splashThread.Start();

            // Wait for the blocker to be signaled before continuing. This is essentially the same as: while(ResetSplashCreated.NotSet) {}
            _resetSplashCreated.WaitOne();
            base.OnStartup(e);

            Globals.SplashScreen.AddMessage("Configuration loading...");
            Globals.Configuration.Load();
        }

        private void ShowSplash()
        {
            // Show it
            Globals.SplashScreen.Show();

            // Now that the window is created, allow the rest of the startup to run
            _resetSplashCreated.Set();
            System.Windows.Threading.Dispatcher.Run();
        }
    }
}