using System;
using MathNet.Numerics.Interpolation;

namespace perf
{
   public class DemoModeBrake : IDemoMode
   {
      const double   TIME_MAX = 16.6;

      DemoSample     sample;
      IInterpolation spline;

      public DemoModeBrake()
      {
         Debug.LogToFileMethod();

         sample = new DemoSample(0, 0, 0, 0, 0, 0);

         /*
         Profil Brake: (nur Zeitangabe, keine Streckenangabe)
         Anzeige geht auf 175kmh und wieder zurück auf 0kmh
         - 100-0: 3,6s
         */

         // BRAKE
         // accelerate to 175 km/h and then back to 0 km/h
         double[] pointsX = {0,   2,   4,   6,   8,  10,  12,  13,  14,  15, TIME_MAX}; // time  [s]
         double[] pointsY = {0,  33,  66, 100, 140, 175, 100,  75,  50,  25,        0}; // speed [km/h]

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
