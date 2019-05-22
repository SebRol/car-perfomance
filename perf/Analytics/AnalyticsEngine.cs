
namespace perf
{
    public class AnalyticsEngine
    {
        private readonly TrackerManager  manager;
        private static   AnalyticsEngine current;

        public static AnalyticsEngine Current
        {
            get
            {
                if (current == null)
                {
                    current = new AnalyticsEngine();
                }

                return current;
            }
        }

        protected AnalyticsEngine()
        {
            manager = new TrackerManager(new DeviceAnalyticsAndroid());
        }

        private bool isAppOptOutSet;

        public bool AppOptOut
        {
            get
            {
                if (!isAppOptOutSet)
                {
                    LoadAppOptOut();
                }

                return manager.AppOptOut;
            }
            set
            {
                manager.AppOptOut = value;
                isAppOptOutSet = true;

                if (value)
                {
                    GAServiceManager.Current.Clear();
                }
            }
        }

        private void LoadAppOptOut()
        {
            manager.AppOptOut = false;
            isAppOptOutSet = true;
        }

        public bool IsDebugEnabled
        {
            get { return manager.IsDebugEnabled; }
            set { manager.IsDebugEnabled = value; }
        }

        public Tracker GetTracker(string propertyId)
        {
            return manager.GetTracker(propertyId);
        }

        public void CloseTracker(Tracker tracker)
        {
            manager.CloseTracker(tracker);
        }

        public Tracker DefaultTracker
        {
            get { return manager.DefaultTracker; }
            set { manager.DefaultTracker = value; }
        }

        public IAnalyticsPlatformInfoProvider PlatformInfoProvider
        {
            get { return manager.PlatformTrackingInfo; }
        }
    }
}
