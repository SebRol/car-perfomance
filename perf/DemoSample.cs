using System;

namespace perf
{
   public class DemoSample
   {
      public long  t; // timestamp = nanoseconds since 01. januar 1970 // timestamp / 1 000 000 000 = seconds
      public float x; // x axis
      public float y; // y axis
      public float z; // z axis
      public float s; // speed
      public float d; // distance

      public DemoSample(long t, float x, float y, float z, float s, float d)
      {
         this.t = t;
         this.x = x;
         this.y = y;
         this.z = z;
         this.s = s;
         this.d = d;
      }
   }
}
