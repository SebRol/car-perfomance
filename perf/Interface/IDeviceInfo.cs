

namespace perf
{
   // abstracts platform INFO function
   public interface IDeviceInfo
   {
      string DeviceId         { get; }
      string OS               { get; }
      string OSVersion        { get; }
      string Manufacturer     { get; }
      string DeviceName       { get; }
      string Resolution       { get; }
      long   RamCurrent       { get; }
      long   RamTotal         { get; }
      int    Battery          { get; }
      string Orientation      { get; }

      string AppName          { get; }
      string AppVersion       { get; }
      string AppContact       { get; }

      string ToString();
   }
}
