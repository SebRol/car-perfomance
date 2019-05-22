using System;
using Xamarin.Forms;

using perf.Droid;
// register this class at the Xamarin.Forms DependencyService -> can be used in shared code
[assembly: Xamarin.Forms.Dependency(typeof (DeviceLocaleAndroid))]
namespace perf.Droid
{
   public class DeviceLocaleAndroid : IDeviceLocale
   {
      public DeviceLocaleAndroid()
      {
      }

      public LocaleId GetLocale()
      {
         LocaleId result = LocaleId.English;

         if (Java.Util.Locale.Default.ToString().ToLower().Contains("de"))
         {
            result = LocaleId.German;
         }

         return result;
      }
   }
}
