﻿using System;
using System.Threading.Tasks;

namespace perf
{
    public class TrackerFactory
    {
        private static TrackerFactory current;
        private static Tracker tracker;

        public TrackerFactory()
        {
            InitTracker(Config);
        }

        public TrackerFactory(ITrackerConfig config)
        {
            InitTracker(config);
        }

        public Uri ConfigPath { get; set; }
        public static ITrackerConfig Config { get; set; }

        public static TrackerFactory Current
        {
            get
            {
                if(current == null && Config != null)
                {
                    current = new TrackerFactory(Config);
                }
                return current;
            }
        }

        public ITracker GetTracker()
        {
            if(tracker == null)
            {
                InitTracker(Config);
            }
            return tracker;
        }

        public ITracker GetTracker(ITrackerConfig config)
        {
            InitTracker(config);
            return tracker;
        }

        public void InitTracker(ITrackerConfig config)
        {
            Config = config;
            Config.Validate();

            var analyticsEngine = AnalyticsEngine.Current;
            analyticsEngine.IsDebugEnabled = Config.Debug;

            GAServiceManager.Current.DispatchPeriod = Config.DispatchPeriod;

            tracker = analyticsEngine.GetTracker(Config.TrackingId);
            tracker.SetStartSession(Config.SessionTimeout.HasValue);
            tracker.IsUseSecure = Config.UseSecure;
            tracker.AppName = Config.AppName;
            tracker.AppVersion = Config.AppVersion;
            tracker.AppId = Config.AppId;
            tracker.AppInstallerId = Config.AppInstallerId;
            tracker.IsAnonymizeIpEnabled = Config.AnonymizeIp;
            tracker.SampleRate = Config.SampleFrequency;
            tracker.IsDebug = Config.Debug;
        }

        public Task Dispatch()
        {
            return GAServiceManager.Current.Dispatch();
        }
    }
}