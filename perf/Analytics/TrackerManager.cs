using System.Collections.Generic;

namespace perf
{
    public sealed class TrackerManager : IServiceManager
    {
        private readonly Dictionary<string, Tracker> trackers;

        public TrackerManager(IAnalyticsPlatformInfoProvider platformTrackingInfo)
        {
            trackers = new Dictionary<string, Tracker>();
            PlatformTrackingInfo = platformTrackingInfo;
            GAServiceManager.Current.UserAgent = PlatformTrackingInfo.UserAgent;
        }

        public Tracker DefaultTracker { get; set; }
        public bool IsDebugEnabled { get; set; }
        public bool AppOptOut { get; set; }
        public IAnalyticsPlatformInfoProvider PlatformTrackingInfo { get; set; }

        public void SendPayload(Payload payload)
        {
            if(!AppOptOut)
            {
                GAServiceManager.Current.SendPayload(payload);
            }
        }

        public string UserAgent
        {
            get { return GAServiceManager.Current.UserAgent; }
            set { GAServiceManager.Current.UserAgent = value; }
        }

        public Tracker GetTracker(string propertyId)
        {
            propertyId = propertyId ?? string.Empty;
            if(!trackers.ContainsKey(propertyId))
            {
                var tracker = new Tracker(propertyId, PlatformTrackingInfo, this);
                trackers.Add(propertyId, tracker);
                if(DefaultTracker == null)
                {
                    DefaultTracker = tracker;
                }
                return tracker;
            }
            return trackers[propertyId];
        }

        public void CloseTracker(Tracker tracker)
        {
            trackers.Remove(tracker.TrackingId);
            if(DefaultTracker == tracker)
            {
                DefaultTracker = null;
            }
        }
    }
}