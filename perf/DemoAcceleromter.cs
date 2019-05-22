using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace perf
{
   public class DemoAcceleromter : IDeviceAcceleromter, IAcceleromterListener
   {
      const uint UPDATE_RATE_MS = 20; // milliseconds between two calls to update
      // SensorDelay.Ui   = approx 15 Hz = every 66 milliseconds
      // SensorDelay.Game = approx 50 Hz = every 20 milliseconds

      IDemoMode             mode;
      IDeviceAcceleromter   accelerometerProvider;
      IAcceleromterListener listener;
      float                 sensorX;
      float                 sensorY;
      float                 sensorZ;
      float                 sensorXLast;
      float                 sensorYLast;
      float                 sensorZLast;
      float                 sensorXDelta;
      float                 sensorYDelta;
      float                 sensorZDelta;
      float                 sensorAcceleration;
      float                 sensorSpeed;
      long                  sensorTimestamp;
      long                  sensorTimestampFirst;
      long                  sensorTimestampLast;
      bool                  isFirstRun;
      Stopwatch             stopwatch;
      Stopwatch             countdown;
      string                status;

      public DemoAcceleromter(string mode, IDeviceAcceleromter accelerometer)
      {
         Debug.LogToFileMethod();

         if (mode.Equals("Acceleration"))
         {
            this.mode = new DemoModeAcceleration();
            Analytics.TrackEventRun(Analytics.EVENT_RUN_ACCELERATION_DEMO);
         }
         else if (mode.Equals("Brake"))
         {
            this.mode = new DemoModeBrake();
            Analytics.TrackEventRun(Analytics.EVENT_RUN_BRAKE_DEMO);
         }
         else // ZeroToZero
         {
            this.mode = new DemoModeZeroToZero();
            Analytics.TrackEventRun(Analytics.EVENT_RUN_ZEROTOZERO_DEMO);
         }

         accelerometerProvider = accelerometer;
         stopwatch             = new Stopwatch();
         isFirstRun            = true;
         status                = "init";
         countdown             = new Stopwatch();
         countdown.Start();
      }

      bool OnTimer()
      {
         bool result = false;

         if (stopwatch.IsRunning)
         {
            DemoSample s = mode.GetSample(stopwatch.ElapsedMilliseconds);
            Update(s);
            result = true; // restart timer
         }

         return result;
      }

      void Update(DemoSample s)
      {
         sensorX = s.x;
         sensorY = s.y;
         sensorZ = s.z;

         sensorTimestamp = s.t;

         // e.Timestamp = nanoseconds since 01. januar 1970
         // e.Timestamp / 1 000 000 000 = seconds

         if (isFirstRun)
         {
            isFirstRun = false;
            sensorTimestampFirst = sensorTimestamp;
            sensorAcceleration = 0;
            sensorSpeed = 0;
         }
         else
         {
            sensorXDelta = sensorX - sensorXLast;
            sensorYDelta = sensorY - sensorYLast;
            sensorZDelta = sensorZ - sensorZLast;

            sensorSpeed = s.s;

            // acceleration magnitude of all components
            sensorAcceleration = (float)Math.Sqrt(sensorX * sensorX +
                                                  sensorY * sensorY +
                                                  sensorZ * sensorZ);
         }

         sensorXLast = sensorX;
         sensorYLast = sensorY;
         sensorZLast = sensorZ;
         sensorTimestampLast = sensorTimestamp;

         if (listener != null)
         {
            listener.OnAcceleromterUpdate();
         }
      }

      public float GetDistance()
      {
         return mode.GetSample(stopwatch.ElapsedMilliseconds).d;
      }

      public void OnAcceleromterUpdate() // IAcceleromterListener
      {
         // ignore
      }

      public void OnAcceleromterLaunchDetected() // IAcceleromterListener
      {
         if (countdown.ElapsedMilliseconds > 2000) // respect settle-down-timer of PageRun
         {
            if (listener != null)
            {
               status = "launchDetected";
               stopwatch.Start();
               Device.StartTimer(TimeSpan.FromMilliseconds(UPDATE_RATE_MS), OnTimer);
               listener.OnAcceleromterLaunchDetected();
            }
         }
      }

      public void SetListener(IAcceleromterListener listener) // IDeviceAcceleromter
      {
         this.listener = listener;

         if (this.listener == null)
         {
            accelerometerProvider.SetListener(null);

            stopwatch.Stop();
            countdown.Stop();
            Debug.LogToFileMethod("accelerometer sensor off (demo mode)");
         }
         else
         {
            accelerometerProvider.SetListener(this);

            if (status.Equals("launchDetected"))
            {
               stopwatch.Start();
               Device.StartTimer(TimeSpan.FromMilliseconds(UPDATE_RATE_MS), OnTimer);
               Debug.LogToFileMethod("accelerometer sensor on (demo mode)");
            }
         }
      }

      public void SetForceDetectLimit(float limit) // IDeviceAcceleromter
      {
         accelerometerProvider.SetForceDetectLimit(limit);
      }

      public void SetTimeDetectLimit(long limit) // IDeviceAcceleromter
      {
         accelerometerProvider.SetTimeDetectLimit(limit);
      }

      public void SetTimeUpdateInterval(long interval) // IDeviceAcceleromter
      {
         accelerometerProvider.SetTimeUpdateInterval(interval);
      }

      public void SetSpeed(float speed) // IDeviceAcceleromter
      {
         // ignore
      }

      public void SetDirection(bool isAccelerating) // IDeviceAcceleromter
      {
         accelerometerProvider.SetDirection(isAccelerating);
      }

      public void SetAxisOffset(float x, float y, float z) // IDeviceAcceleromter
      {
         accelerometerProvider.SetAxisOffset(x, y, z);
      }

      public float GetTimeStamp() // IDeviceAcceleromter
      {
         return (sensorTimestamp - sensorTimestampFirst) / 1000000000.0f; // seconds
      }

      public float GetX() // IDeviceAcceleromter
      {
         return sensorX; // left right
      }

      public float GetY() // IDeviceAcceleromter
      {
         return sensorY; // up down
      }

      public float GetZ() // IDeviceAcceleromter
      {
         return sensorZ; // forward backward
      }

      public float GetXDelta() // IDeviceAcceleromter
      {
         return sensorXDelta; // left right
      }

      public float GetYDelta() // IDeviceAcceleromter
      {
         return sensorYDelta; // up down
      }

      public float GetZDelta() // IDeviceAcceleromter
      {
         return sensorZDelta; // forward backward
      }

      public float GetAcceleration() // IDeviceAcceleromter
      {
         return sensorAcceleration;
      }

      public float GetSpeed() // IDeviceAcceleromter
      {
         return sensorSpeed;
      }

      public string GetInfo() // IDeviceAcceleromter
      {
         string result;

         result = "Version:"     + 0 + " / Vendor:"     + "Dummy" +
                  " / Power:"    + 0 + " / Resolution:" + 0 +
                  " / MinDelay:" + 0;

         return result;
      }

      public string GetStatus() // IDeviceAcceleromter
      {
         return status;
      }

      public void Init() // IDeviceAcceleromter
      {
         // ignore
      }

      public void DeInit() // IDeviceAcceleromter
      {
         // ignore
      }
   }
}
