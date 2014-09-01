namespace SP2013Access
{
    public static class Globals
    {
        public static ConfigManager Configuration { get; private set; }

        public static ISplashScreen SplashScreen { get; private set; }

        static Globals()
        {
            Configuration = new ConfigManager();
            SplashScreen = new SplashWindow();
        }
    }
}