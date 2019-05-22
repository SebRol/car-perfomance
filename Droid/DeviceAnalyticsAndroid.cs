using System;
using Android.OS;
using Android.App;
using System.IO;
using perf;

namespace perf
{
   public sealed partial class DeviceAnalyticsAndroid : IAnalyticsPlatformInfoProvider
   {
      public DeviceAnalyticsAndroid()
      {
         var device = new AnalytivsDeviceInfo();

         ScreenResolution   = device.Display;
         UserLanguage       = device.LanguageCode;
         UserAgent          = device.UserAgent;
         ViewPortResolution = device.ViewPortResolution;
         Version            = device.VersionNumber;

         GetAnonymousClientId(device);
      }
   }

   public class AnalytivsDeviceInfo : IAnalyticsDeviceInfo
   {
      private readonly string GoogleAnalyticsFolder = "ga-store";

      public AnalytivsDeviceInfo()
      {
         UserAgent             = Java.Lang.JavaSystem.GetProperty("http.agent");
         Display               = new Dimensions(Android.App.Application.Context.Resources.DisplayMetrics.HeightPixels,
                                                Android.App.Application.Context.Resources.DisplayMetrics.WidthPixels);
         GoogleAnalyticsFolder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), GoogleAnalyticsFolder);
      }

      public string Id
      {
         get { return Build.Serial; }
      }

      public string Version
      {
         get
         { 
            Android.Content.Context ctx = Application.Context;
            return ctx.PackageManager.GetPackageInfo(ctx.PackageName, 0).VersionName + "-" + ctx.PackageManager.GetPackageInfo(ctx.PackageName, 0).VersionCode;
         }
      }

      public string UserAgent { get; set; }

      public string VersionNumber
      {
         get
         {
            return Version;
         }
      }

      public string LanguageCode
      {
         get { return Java.Util.Locale.Default.Language; }
      }

      public Dimensions Display { get; set; }

      public Dimensions ViewPortResolution { get; set; }

      public string GenerateAppId(bool usingPhoneId = false, string prefix = null, string suffix = null)
      {
         var appId = "";

         if (!string.IsNullOrEmpty(prefix))
         {
            appId += prefix;
         }

         appId += Guid.NewGuid().ToString();

         if (usingPhoneId)
         {
            appId += Id;
         }

         if (!string.IsNullOrEmpty(suffix))
         {
            appId += suffix;
         }

         return appId;
      }

      public string ReadFile(string path)
      {
         if (!File.Exists(Path.Combine(GoogleAnalyticsFolder, path)))
         {
            return string.Empty;
         }

         return File.ReadAllText(Path.Combine(GoogleAnalyticsFolder, path));
      }

      public void WriteFile(string path, string content)
      {
         if (!Directory.Exists(GoogleAnalyticsFolder))
         {
            Directory.CreateDirectory(GoogleAnalyticsFolder);
         }

         File.WriteAllText(Path.Combine(GoogleAnalyticsFolder, path), content);
      }
   }
}
