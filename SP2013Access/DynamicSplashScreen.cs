using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access
{
    public class DynamicSplashScreen : Window
    {
        public DynamicSplashScreen()
        {
            ShowInTaskbar = false;
            WindowStartupLocation = WindowStartupLocation.Manual;
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;
            Topmost = true;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //calculate it manually since CenterScreen substracts taskbar height from available area
            Left = (SystemParameters.PrimaryScreenWidth - Width)/2;
            Top = (SystemParameters.PrimaryScreenHeight - Height)/2;
        }

        //.... see implementaion above ..
        public void Capture(string filePath)
        {
            Capture(filePath, new PngBitmapEncoder());
        }

        public void Capture(string filePath, BitmapEncoder encoder)
        {
            var bmp = new RenderTargetBitmap((int) Width, (int) Height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(this);
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            using (Stream stm = File.Create(filePath))
            {
                encoder.Save(stm);
            }
        }
    }
}