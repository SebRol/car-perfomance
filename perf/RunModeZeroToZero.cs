using System;
using System.Diagnostics;

namespace perf
{  
   public class RunModeZeroToZero : IRunMode
   {
      readonly      IDeviceAcceleromter accelerationProvider;
      readonly      IDeviceSound        soundProvider;
      string        status;
      bool          isTargetReached;
      Stopwatch     stopWatch;
      float         speed;              // m/s
      float         speedTarget;        // m/s
      float         speedTargetMin;     // m/s
      float         acceleration;       // m/s^2
      float         stopDetectionLimit; // m/s^2
      bool          runCompleted;
      MovingAverage accelerationFilter;

      public RunModeZeroToZero(IDeviceAcceleromter accelProvider, IDeviceSound sndProvider)
      {
         var speedoFilter = Settings.GetValueOrDefault(Settings.SpeedoFilter, 1);
         var filterSize   = speedoFilter * 10;

         accelerationProvider = accelProvider;
         soundProvider        = sndProvider;
         stopWatch            = new Stopwatch();
         accelerationFilter   = new MovingAverage(filterSize);
         Reset();
      }

      public void Reset()
      {
         stopWatch.Reset();
         accelerationFilter.Reset();

         isTargetReached = false;
         acceleration    = 0;
         speed           = 0;
         runCompleted    = false;

         accelerationProvider.SetDirection(true); // apply speed-up mode at sensor

         RunResult runResult = new RunResult(ResultMode.ZeroToZero);
         runResult.Load();
         speedTarget         = runResult.GetSpeedTargetMax();
         speedTargetMin      = speedTarget * 0.5f;

         status = Localization.runModeWaitGreater;
         if (Settings.IsSpeedUnitKph()) status += " " + Tools.ToKilometerPerHour(speedTarget) + " " + Settings.GetSpeedUnit();
         else                           status += " " + Tools.ToMilesPerHour    (speedTarget) + " " + Settings.GetSpeedUnit();
      }

      public float Acceleration
      {
         set
         {
            acceleration = accelerationFilter.Get(value);
         }
      }

      public float Speed
      {
         set 
         {
            speed = value;

            if ((speed > speedTarget) && (isTargetReached == false) && (runCompleted == false))
            {
               isTargetReached = true;
               soundProvider.Play(SoundId.zero2_reached);
               accelerationProvider.SetDirection(false); // apply slow-down mode at sensor
               Debug.LogToFileEventText("run target (zerotozero)");
            }
         }
      }

      public float StopDetectionLimit
      {
         set
         {
            stopDetectionLimit = value;
         }
      }

      public static string Mode
      {
         get { return "ZeroToZero"; }
      }

      string IRunMode.Mode
      {
         get { return "ZeroToZero"; }
      }

      public ResultMode SetupMode 
      { 
         get { return ResultMode.ZeroToZero; }
      }

      public string ModeText
      {
         get { return Localization.runModeZeroToZero; }
      }

      public string Background
      {
         get { return "@drawable/speedo_mode3_bg.png"; }
      }

      public string Speedo
      {
         get { return "@drawable/speedo_mode3.png"; }
      }

      public string Needle
      {
         get { return "@drawable/speedo_mode3_needle.png"; }
      }
      
      public string ButtonRun
      {
         get { return Localization.btn_mode3_run; }
      }

      public string ButtonResults
      {
         get { return Localization.btn_mode3_results; }
      }

      public string ButtonSetup
      {
         get { return "@drawable/btn_setup_mode3.png"; }
      }

      public uint Color
      {
         get { return 4279041904u /* Convert.ToUInt32("0xFF0CFF70", 16) */ ; }
      }

      public string Status
      {
         get { return status; }
      }

      public string TimeSplit
      {
         get { return ""; }
      }

      public bool IsStarted
      {
         get { return stopWatch.IsRunning; }
      }

      public string TimeElapsed
      {
         get { return Tools.ToStopwatchTime(stopWatch.Elapsed); }
      }

      public float TimeElapsedF
      {
         get { return (float)stopWatch.Elapsed.TotalSeconds; }
      }

      public bool IsFinished
      {
         get 
         { 
            return 
            (
               (isTargetReached == true) 
               && 
               (
                  (acceleration < stopDetectionLimit) && (speed < speedTargetMin)
                  ||
                  (speed < 1)
               )
            );
         }
      }

      public bool IsReset
      {
         get { return ((isTargetReached == false) && (stopWatch.IsRunning == true) && (acceleration < stopDetectionLimit)); } 
      }

      public bool HasTimeSplitChanged
      {
         get { return false; }
      }

      public float SpeedTarget
      {
         get { return speedTarget; }
      }

      public void OnLaunchDetected()
      {
         status = Localization.runModeLaunch;
         stopWatch.Restart();
         soundProvider.Play(SoundId.zero1_launch);
         Analytics.TrackEventRun(Analytics.EVENT_RUN_ZEROTOZERO_START);
      }

      public void OnTimeSplit()
      {
      }

      public void OnFinished()
      {
         stopWatch.Stop();
         runCompleted = true;
         status = Localization.runModeFinish;
         soundProvider.Play(SoundId.zero3_stopped);
         Analytics.TrackEventRun(Analytics.EVENT_RUN_ZEROTOZERO_STOP);
      }

      public void OnReset()
      {
         Reset();
         soundProvider.Play(SoundId.failure);
         Analytics.TrackEventRun(Analytics.EVENT_RUN_ZEROTOZERO_RESET);
      }

      public override string ToString()
      {
         string result = "speed target [m/s]: " + speedTarget;

         result += Environment.NewLine;
         result += "mode: " + Mode;

         return result;
      }
   }
}
