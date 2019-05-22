
// register this class at the Xamarin.Forms DependencyService -> can be used in shared code
using perf.Droid;
[assembly: Xamarin.Forms.Dependency(typeof (DeviceExitAndroid))]

namespace perf.Droid
{
   public class DeviceExitAndroid : IDeviceExit
   {
      public void Exit() // IDeviceExit
      {
         Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
      }
   }
}
