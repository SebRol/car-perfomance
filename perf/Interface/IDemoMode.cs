using System;

namespace perf
{
   // abstracts DEMO_MODE function
   public interface IDemoMode
   {
      // returns next sample for given time
      // time is elapsed milliseconds since the first sample
      DemoSample GetSample(long timeMs);
   }
}
