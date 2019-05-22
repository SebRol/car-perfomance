using System;

namespace perf
{
   // listener of platform GPS function
   public interface IGpsListener
   {
      void OnGpsStatusUpdate();
      void OnGpsLocationUpdate();
   }
}
