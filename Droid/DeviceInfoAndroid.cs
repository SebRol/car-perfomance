using Android.App;
using Android.OS;

// register this class at the Xamarin.Forms DependencyService -> can be used in shared code
using perf.Droid;
[assembly: Xamarin.Forms.Dependency(typeof (DeviceInfoAndroid))]

namespace perf.Droid
{
   public class DeviceInfoAndroid : IDeviceInfo
   {
      Android.Content.Context ctx;

      public DeviceInfoAndroid()
      {
         ctx = Application.Context;
      }

      public string DeviceId
      {
         get
         {
            return Android.Provider.Settings.Secure.GetString(Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
         }
      }

      public string OS
      {
         get
         {
            return "Android";
         }
      }

      public string OSVersion
      {
         get
         {
            return Build.VERSION.Release;
         }
      }

      public string Manufacturer
      {
         get
         {
            return Build.Manufacturer;
         }
      }

      public string DeviceName
      {
         get
         {
            return Build.Manufacturer + "-" + Build.Model;
         }
      }

      public string Resolution
      {
         get
         {
            return ctx.Resources.DisplayMetrics.WidthPixels + "x" + ctx.Resources.DisplayMetrics.HeightPixels;
         }
      }

      public long RamCurrent
      {
         get
         {
            ActivityManager.MemoryInfo mi = new ActivityManager.MemoryInfo();
            ActivityManager activityManager = (ActivityManager) ctx.GetSystemService(Android.Content.Context.ActivityService);
            activityManager.GetMemoryInfo(mi);
            long megaBytes = mi.AvailMem / 1048576L; // 1024 * 1024 = 1048576 = megabyte

            return megaBytes;
         }
      }

      public long RamTotal
      {
         get
         {
            ActivityManager.MemoryInfo mi = new ActivityManager.MemoryInfo();
            ActivityManager activityManager = (ActivityManager) ctx.GetSystemService(Android.Content.Context.ActivityService);
            activityManager.GetMemoryInfo(mi);
            long megaBytes = mi.TotalMem / 1048576L; // 1024 * 1024 = 1048576 = megabyte

            return megaBytes;
         }
      }

      // seems only to work after listening a while -> do not use for crash reporting
      public int Battery
      {
         get
         {
            Android.Content.Intent batteryIntent = ctx.RegisterReceiver(null, new Android.Content.IntentFilter(Android.Content.Intent.ActionBatteryChanged));
            int rawlevel = batteryIntent.GetIntExtra("level", -1);
            double scale = batteryIntent.GetIntExtra("scale", -1);
            double level = -1;
            if (rawlevel >= 0 && scale > 0) 
            {
               level = rawlevel / scale;
            } 

            return (int)level * 100;
         }
      }

      public string Orientation
      {
         get
         {
            return ctx.Resources.Configuration.Orientation.ToString();
         }
      }

      public string AppName
      {
         get
         {
            return ctx.Resources.GetString(Resource.String.applicationName);
         }
      }

      public string AppVersion
      {
         get
         {
            return ctx.PackageManager.GetPackageInfo(ctx.PackageName, 0).VersionName + "-" + ctx.PackageManager.GetPackageInfo(ctx.PackageName, 0).VersionCode;
         }
      }

      public string AppContact
      {
         get
         {
            return ctx.Resources.GetString(Resource.String.contactMail);
         }
      }

      public override string ToString()
      {
         string s = 
            "AppName="      + AppName      + System.Environment.NewLine +
            "AppVersion="   + AppVersion   + System.Environment.NewLine +
            "DeviceId="     + DeviceId     + System.Environment.NewLine +
            "DeviceName="   + DeviceName   + System.Environment.NewLine +
            "Orientation="  + Orientation  + System.Environment.NewLine +
            "OS="           + OS           + System.Environment.NewLine +
            "OSVersion="    + OSVersion    + System.Environment.NewLine +
            "RamCurrent="   + RamCurrent   + System.Environment.NewLine +
            "RamTotal="     + RamTotal     + System.Environment.NewLine +
            "Resolution="   + Resolution;

         return s;
      }
   }
}
