using System;
using Xamarin.Forms;

namespace perf
{
   public static class Debug
   {
      static IDeviceDebug debugProvider;

      public static void Init()
      {
         if (debugProvider == null)
         {
            // DependencyService decides if to use iOS or Android
            debugProvider = DependencyService.Get<IDeviceDebug>();
         }
      }
         
      public static void Print(
         string s,
         [System.Runtime.CompilerServices.CallerMemberName] string methodName     = "",
         [System.Runtime.CompilerServices.CallerFilePath]   string sourceFilePath = "")
      {
         if (debugProvider != null)
         {
            debugProvider.Print(s, methodName, sourceFilePath);
         }
      }

      public static void LogToFileMethod(
         string s = "",
         [System.Runtime.CompilerServices.CallerMemberName] string methodName     = "",
         [System.Runtime.CompilerServices.CallerFilePath]   string sourceFilePath = "")
      {
         if (debugProvider != null)
         {
            debugProvider.LogToFileMethod(s, methodName, sourceFilePath);
         }
      }
      
      public static void LogToFile(string s)
      {
         if (debugProvider != null)
         {
            debugProvider.LogToFile(s);
         }
      }

      public static void LogToFile(float time, float acceleration, float speedAccel, float speedGps, float distanceFiltered, float distanceGps, float heightFiltered, float heightGps)
      {
         if (debugProvider != null)
         {
            debugProvider.LogToFile(time, acceleration, speedAccel, speedGps, distanceFiltered, distanceGps, heightFiltered, heightGps);
         }
      }

      public static void LogToFileFlush()
      {
         if (debugProvider != null)
         {
            debugProvider.LogToFileFlush();
         }
      }

      public static void LogToFileEventText(string s)
      {
         if (debugProvider != null)
         {
            debugProvider.LogToFileEventText(s);
         }
      }

      public static string GetLogFilePath()
      {
         if (debugProvider != null)
         {
            return debugProvider.GetLogFilePath();
         }

         return "";
      }
   }
}
