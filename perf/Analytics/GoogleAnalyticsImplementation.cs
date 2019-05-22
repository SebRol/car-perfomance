
namespace perf
{
    public class GoogleAnalyticsImplementation : IGoogleAnalytics
    {
        static GoogleAnalyticsImplementation()
        {
            StaticConfig = new TrackerConfig();
        }

        public GoogleAnalyticsImplementation()
        {
            var platform = new DeviceAnalyticsAndroid();

            Config.AppVersion = platform.Version;
            TrackerFactory.Config = Config;
        }

        static ITrackerConfig StaticConfig { get; set; }

        public ITrackerConfig Config
        {
            get { return StaticConfig;  }
            set { StaticConfig = value; }
        }

        public ITracker Tracker
        {
            get { return TrackerFactory.Current.GetTracker(); }
        }

        public void InitTracker()
        {
            TrackerFactory.Current.InitTracker(Config);
        }
    }
}
