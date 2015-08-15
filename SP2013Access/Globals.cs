namespace SP2013Access
{
    public static class Globals
    {
        static Globals()
        {
            Configuration = new ConfigManager();
            SplashScreen = new SplashWindow();
        }

        public static ConfigManager Configuration { get; private set; }
        public static ISplashScreen SplashScreen { get; private set; }
    }
}