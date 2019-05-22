using perf.Droid;

// register this class at the Xamarin.Forms DependencyService -> can be used in shared code
[assembly: Xamarin.Forms.Dependency(typeof (DeviceResourceAndroid))]

namespace perf.Droid
{
   public static class DeviceResourceAndroid : IDeviceResource
   {
      static bool isEnglish;

      static void IDeviceResource.SetEnglish(bool isEng)
      {
         isEnglish = isEng;
      }

      static string IDeviceResource.GetLoacleString(IDeviceResourceString key)
      {
         string s = "@string/" + key.ToString();
         string result;

         if (isEnglish == false)
         {
            s = s + "_de";
         }

         result = 
         return 
      }
   }
}
