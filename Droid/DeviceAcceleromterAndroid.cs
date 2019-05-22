using System;
using Android.App;
using Android.Content;
using Android.Hardware;
using System.Diagnostics;

using perf.Droid;

// register this class at the Xamarin.Forms DependencyService -> can be used in shared code
[assembly: Xamarin.Forms.Dependency(typeof (DeviceAcceleromterAndroid))]
namespace perf.Droid
{
   public class DeviceAcceleromterAndroid : Activity, IDeviceAcceleromter, ISensorEventListener
   {
      IAcceleromterListener listener;
      SensorManager         sensorManager;
      Sensor                sensor;
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
      bool                  sensorIsAccelerating;
      long                  sensorTimestamp;
      long                  sensorTimestampFirst;
      long                  sensorTimestampLast;
      long                  sensorTimestampDelta;
      long                  launchTimeLastUpdate;
      long                  launchTimeDetectedStart;
      long                  launchTimeUpdateInterval = 100;
      long                  launchTimeDetectLimit    = 300;
      float                 launchForceDetectLimit   = 3;
      float                 sensorAxisOffsetX;
      float                 sensorAxisOffsetY;
      float                 sensorAxisOffsetZ;
      bool                  isFirstRun;
      Stopwatch             stopwatch;
      string                status = "no error";
      bool                  isRawDataLoggingEnabled;

      public DeviceAcceleromterAndroid()
      {
      }

      void InitSensor()
      {
         sensorManager = Application.Context.GetSystemService(Context.SensorService) as SensorManager;
         stopwatch = new Stopwatch();
         stopwatch.Restart();
         isFirstRun = true;
         sensorIsAccelerating = true; // moving forward, not backward

         if (sensorManager != null)
         {
            sensor = sensorManager.GetDefaultSensor(SensorType.Accelerometer);
            DeviceDebugAndroid.LogToFileStatic("accelerometer sensor requested");

            if (sensor != null)
            {
               // SensorDelay.Ui   = approx 15 Hz = every 66 milliseconds
               // SensorDelay.Game = approx 50 Hz = every 20 milliseconds
               sensorManager.RegisterListener(this, sensor, SensorDelay.Game);
               status = "sensor init";
               DeviceDebugAndroid.LogToFileStatic("accelerometer sensor init ok");
            }
            else
            {
               status = "sensor not enabled";
               DeviceDebugAndroid.LogToFileStatic("ERROR: accelerometer sensor is null");
            }
         }
         else
         {
            status = "sensor manager not enabled";
            DeviceDebugAndroid.LogToFileStatic("ERROR: accelerometer sensorManager is null");
         }
      }

      public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy) // ISensorEventListener
      {
         status = "sensor accuracy changed to " + accuracy;
         DeviceDebugAndroid.LogToFileMethodStatic(status);
      }

      public void OnSensorChanged(SensorEvent e) // ISensorEventListener
      {
         if (e != null)
         {
            if ((e.Sensor.Type == SensorType.LinearAcceleration) || (e.Sensor.Type == SensorType.Accelerometer))
            {
               long launchTimeNow = stopwatch.ElapsedMilliseconds;

               sensorX = e.Values[0];
               sensorY = e.Values[1];
               sensorZ = e.Values[2];

               sensorTimestamp = e.Timestamp;

               // e.Timestamp = nanoseconds since 01. januar 1970
               // e.Timestamp / 1 000 000 000 = seconds

               if (isFirstRun)
               {
                  isFirstRun = false;
                  launchTimeLastUpdate = launchTimeNow;
                  launchTimeDetectedStart = 0;
                  sensorTimestampFirst = sensorTimestamp;
                  sensorAcceleration = 0;
                  sensorSpeed = 0;
                  DeviceDebugAndroid.LogToFileMethodStatic("first values received");
                  DeviceDebugAndroid.LogToFileMethodStatic("accelerometer info: " + GetInfo());
               }
               else
               {
                  sensorXDelta = sensorX - sensorXLast;
                  sensorYDelta = sensorY - sensorYLast;
                  sensorZDelta = sensorZ - sensorZLast;

                  sensorTimestampDelta = sensorTimestampLast - sensorTimestamp;

                  var dt = sensorTimestampDelta / 1000000000.0f; // from nanoseconds to seconds
                  var sensorSpeedX = sensorXDelta * dt; // from acceleration to velocity (x component)
                  var sensorSpeedY = sensorYDelta * dt; // from acceleration to velocity (y component)
                  var sensorSpeedZ = sensorZDelta * dt; // from acceleration to velocity (z component)

                  // speed magnitude of all components
                  var magnitude = (float)Math.Sqrt(sensorSpeedX * sensorSpeedX + sensorSpeedY * sensorSpeedY + sensorSpeedZ * sensorSpeedZ);

                  // integrate the magnitude onto previous speed
                  // take the direction into account
                  if (sensorIsAccelerating) sensorSpeed += magnitude;
                  else                      sensorSpeed -= magnitude;

                  // acceleration calculation
                  var sensorXCompensated = sensorX - sensorAxisOffsetX;
                  var sensorYCompensated = sensorY - sensorAxisOffsetY;
                  var sensorZCompensated = sensorZ - sensorAxisOffsetZ;

                  // acceleration magnitude of all components
                  sensorAcceleration = (float)Math.Sqrt(sensorXCompensated * sensorXCompensated + 
                                                        sensorYCompensated * sensorYCompensated + 
                                                        sensorZCompensated * sensorZCompensated);

                  if (isRawDataLoggingEnabled)
                  {
                     DeviceDebugAndroid.LogToFileSensorStatic(GetTimeStamp(), sensorX, sensorY, sensorZ, sensorAcceleration);
                  }

                  long launchTimeDiff = launchTimeNow - launchTimeLastUpdate;

                  if (launchTimeDiff > launchTimeUpdateInterval)
                  {
                     launchTimeLastUpdate = launchTimeNow;
                     var launchForce = Math.Abs(sensorXDelta + sensorYDelta + sensorZDelta);

                     if (launchForce * 10 > launchForceDetectLimit)
                     {
                        if (launchTimeDetectedStart == 0)
                        {
                           launchTimeDetectedStart = launchTimeNow;
                        }

                        if (launchTimeNow - launchTimeDetectedStart > launchTimeDetectLimit)
                        {
                           if (listener != null)
                           {
                              listener.OnAcceleromterLaunchDetected();
                              launchTimeDetectedStart = 0;
                           }
                        }
                     }
                  }
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
         }
      }

      public void SetListener(IAcceleromterListener listener) // IDeviceAcceleromter
      {
         this.listener = listener;
      }

      public void SetForceDetectLimit(float limit) // IDeviceAcceleromter
      {
         launchForceDetectLimit = limit;
      }

      public void SetTimeDetectLimit(long limit) // IDeviceAcceleromter
      {
         launchTimeDetectLimit = limit;
      }

      public void SetTimeUpdateInterval(long interval) // IDeviceAcceleromter
      {
         launchTimeUpdateInterval = interval;
      }

      public void SetSpeed(float speed) // IDeviceAcceleromter
      {
         sensorSpeed = speed;
      }

      public void SetDirection(bool isAccelerating) // IDeviceAcceleromter
      {
         sensorIsAccelerating = isAccelerating;
      }

      public void SetAxisOffset(float x, float y, float z) // IDeviceAcceleromter
      {
         sensorAxisOffsetX = x;
         sensorAxisOffsetY = y;
         sensorAxisOffsetZ = z;
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

         if (sensor != null)
         {
            result = "Version:"     + sensor.Version + " / Vendor:"     + sensor.Vendor + 
                     " / Power:"    + sensor.Power   + " / Resolution:" + sensor.Resolution +
                     " / MinDelay:" + sensor.MinDelay;
         }
         else
         {
            result = "No info available";
         }

         return result;
      }

      public string GetStatus() // IDeviceAcceleromter
      {
         return status;
      }

      public void Init() // IDeviceAcceleromter
      {
         InitSensor();

         float setting = Settings.GetValueOrDefault(Settings.RawSensorLogging, Settings.DefaultRawSensorLogging);
         isRawDataLoggingEnabled = (setting != 0);
      }

      public void DeInit() // IDeviceAcceleromter
      {
         if (sensorManager != null)
         {
            sensorManager.UnregisterListener(this);
            sensorManager.Dispose();
            sensorManager = null;
         }

         if (sensor != null)
         {
            sensor.Dispose();
            sensor = null;
         }

         if (stopwatch != null)
         {
            stopwatch.Stop();
            stopwatch = null;
         }

         listener = null;
         status = "accelerometer sensor shut off";
      }
   }
}
