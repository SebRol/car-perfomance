using System;

namespace perf
{
   // listener of platform GPS function
   public interface IAcceleromterListener
   {
      void OnAcceleromterUpdate();
      void OnAcceleromterLaunchDetected();
   }
}
