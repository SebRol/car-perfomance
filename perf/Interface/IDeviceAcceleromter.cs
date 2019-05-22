using System;

namespace perf
{
   // abstracts platform Accelerometer function
   public interface IDeviceAcceleromter
   {
      // listener is called on new sensor data available
      void SetListener(IAcceleromterListener listener);

      // launch detetciton settings
      void SetForceDetectLimit(float limit);
      void SetTimeDetectLimit(long limit);
      void SetTimeUpdateInterval(long interval);

      // overwrite the sensor speed (drift correction)
      void SetSpeed(float speed);

      // switch acceleration/deceleration
      void SetDirection(bool isAccelerating);

      // compensate earth gravitation
      void SetAxisOffset(float x, float y, float z);

      // sensor measure time in s
      float GetTimeStamp();

      // axis component in m/s^2
      float GetX();
      float GetY();
      float GetZ();

      // axis component since last update in m/s^2
      float GetXDelta();
      float GetYDelta();
      float GetZDelta();

      // acceleration of all axes in m/s^2
      float GetAcceleration();

      // velocity in m/s
      float GetSpeed();

      // debug info
      string GetInfo();
      string GetStatus();

      // lifecyle management
      void Init();
      void DeInit();
   }
}
