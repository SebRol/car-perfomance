﻿namespace perf
{
    /// <summary>
    ///     Interface for Plugin.GoogleAnalytics
    /// </summary>
    public interface IGoogleAnalytics
    {
        void           InitTracker();

        ITrackerConfig Config  { get; set; }
        ITracker       Tracker { get; }
    }
}