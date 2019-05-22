using System;
using MathNet.Numerics.Interpolation;

namespace perf
{
   public class DemoModeZeroToZero : IDemoMode
   {
      const double   TIME_MAX = 10.8;

      DemoSample     sample;
      IInterpolation spline;

      public DemoModeZeroToZero()
      {
         Debug.LogToFileMethod();

         sample = new DemoSample(0, 0, 0, 0, 0, 0);

         /*
         Profil Zero to Zero: (nur Zeitangabe, keine Streckenangabe)
         Geschwindigkeitsanzeige geht auf 126kmh und wieder zurück auf 0kmh
         - 0-100: 4,85s
         - 0-126 (Vmax): 6,2s
         - 100-0: 3,75s
         - 0 - 0: 10,85s
         */

         // ZERO TO ZERO
         // accelerate to 126 km/h and then back to 0 km/h
         double[] pointsX = {0,   7,   8,   9,  TIME_MAX}; // time  [s]
         double[] pointsY = {0, 110, 126, 110,         0}; // speed [km/h]

         spline = CubicSpline.InterpolateAkimaSorted(pointsX, pointsY);
      }

      public DemoSample GetSample(long timeMs)
      {
         long oldTime = sample.t;

         sample.t = timeMs * 1000000; // from millisecond to nanosecond

         double time = ((double)timeMs) / 1000.0; // from milliseconds to seconds

         if (time > TIME_MAX) // return zero speed and acceleration after 20 seconds run time
         {
            sample.s = sample.x = sample.y = sample.z = 0;
         }
         else
         {
            sample.s = (float)(spline.Interpolate(time) / 3.6); // 3.6 = from km/h to m/s

            float acceleration = (float)spline.Differentiate(time);
            sample.x = acceleration / 10;
            sample.y = acceleration / 10;
            sample.z = acceleration / 5;
         }

         // distance : integrate speed over time
         // distance += currentSpeed * (t1 - t0) // t1 = currentTime // t0 = oldTime
         float deltaTime = (float)(sample.t - oldTime) / 1000000000; // from nanoseconds to seconds
         sample.d += sample.s * deltaTime;

         return sample;
      }
   }
}
