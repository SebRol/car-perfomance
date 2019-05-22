
namespace perf
{
   // abstracts platform Debug function
   public interface IDeviceDebug
   {
      void Print(string s, string methodName = "", string sourceFilePath = "");
      void LogToFileMethod(string s = "", string methodName = "", string sourceFilePath = "");
      void LogToFile(string s);
      void LogToFile(float time, float acceleration, float speedAccel, float speedGps, float distanceFiltered, float distanceGps, float heightFiltered, float heightGps);
      void LogToFileFlush();
      void LogToFileEventText(string s);
      string GetLogFilePath();
   }
}
