using System;

namespace perf
{
    public interface IAnalyticsPlatformInfoProvider
    {
        string     AnonymousClientId    { get; set; }
        int?       ScreenColorDepthBits { get; }
        Dimensions ScreenResolution     { get; }
        string     UserLanguage         { get; }
        Dimensions ViewPortResolution   { get; }
        void       OnTracking();
        string     UserAgent            { get; }
        string     Version              { get; }
    }
}