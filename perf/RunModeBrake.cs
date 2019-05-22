using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace perf
{  
   public class RunModeBrake : IRunMode
   {
      readonly      IDeviceAcceleromter accelerationProvider;
      readonly      IDeviceSound        soundProvider;
      string        status;
      bool          isTargetMaxReached;
      Stopwatch     stopWatch;
      float         speed;              // m/s
      float         speedTargetMax;     // m/s
      float         speedTargetMin;     // m/s
      List<float>   speedTargets;       // m/s
      float         acceleration;       // m/s^2
      float         stopDetectionLimit; // m/s^2
      bool          runCompleted;
      MovingAverage accelerationFilter;

      public RunModeBrake(IDeviceAcceleromter accelProvider, IDeviceSound sndProvider)
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

         isTargetMaxReached  = false;
         acceleration        = 0;
         speed               = 0;
         runCompleted        = false;

         RunResult runResult = new RunResult(ResultMode.Brake);
         runResult.Load();
         speedTargetMax      = runResult.GetSpeedTargetMax();
         speedTargets        = runResult.GetSpeedTargets();
         speedTargetMin      = speedTargets[0] * 0.5f; // index 0 is lowest target

         accelerationProvider.SetDirection(true); // apply speed-up mode at sensor

         string targets = "";

         foreach (float f in speedTargets)
         {
            if (Settings.IsSpeedUnitKph()) targets += Tools.ToKilometerPerHour(f);
            else                           targets += Tools.ToMilesPerHour    (f);
            targets += "/";
         }

         targets = Tools.ReplaceLastOccurrence(targets, "/", "");
         status  = Localization.runModeWaitGreater + " " + targets + " " + Settings.GetSpeedUnit();
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

            if ((speed > speedTargetMax) && (stopWatch.IsRunning == false) && (runCompleted == false))
            {
               isTargetMaxReached = true;
               stopWatch.Restart();
               soundProvider.Play(SoundId.brk2_brake);
               accelerationProvider.SetDirection(false); // apply slow-down mode at sensor
               Debug.LogToFileEventText("run target (brake)");
            }

            if (speedTargets.Count > 0)
            {
               if (speed > speedTargets[0]) // is a SplitTime reached?
               {
                  speedTargets.RemoveAt(0); // pop SplitTime
                  soundProvider.Play(SoundId.brk1_ready);
               }
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
         get { return "Brake"; }
      }

      string IRunMode.Mode
      {
         get { return "Brake"; }
      }

      public string ModeText
      {
         get { return Localization.runModeBrake; }
      }

      public ResultMode SetupMode 
      { 
         get { return ResultMode.Brake; }
      }

      public string Background
      {
         get { return "@drawable/speedo_mode2_bg.png"; }
      }

      public string Speedo
      {
         get { return "@drawable/speedo_mode2.png"; }
      }

      public string Needle
      {
         get { return "@drawable/speedo_mode2_needle.png"; }
      }
      
      public string ButtonRun
      {
         get { return Localization.btn_mode2_run; }
      }

      public string ButtonResults
      {
         get { return Localization.btn_mode2_results; }
      }

      public string ButtonSetup
      {
         get { return "@drawable/btn_setup_mode2.png"; }
      }

      public uint Color
      {
         get { return 4294946607u /* Convert.ToUInt32("0xFFFFAF2F", 16) */ ; }
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
               (isTargetMaxReached == true)
               && 
               (
                  ((acceleration < stopDetectionLimit) && (speed < speedTargetMin))
                  || 
                  (speed < 1)
               )
            );
         }
      }

      public bool IsReset
      {
         get { return ((isTargetMaxReached == false) && (stopWatch.IsRunning == false) && (acceleration < stopDetectionLimit)); }
      }

      public bool HasTimeSplitChanged
      {
         get { return false; }
      }

      public float SpeedTarget
      {
         get { return speedTargetMax; }
      }

      public void OnLaunchDetected()
      {
         soundProvider.Play(SoundId.brk1_ready);
         Analytics.TrackEventRun(Analytics.EVENT_RUN_BRAKE_START);
      }

      public void OnTimeSplit()
      {
      }

      public void OnFinished()
      {
         stopWatch.Stop();
         runCompleted = true;
         status = Localization.runModeFinish;
         soundProvider.Play(SoundId.brk3_stopped);
         Analytics.TrackEventRun(Analytics.EVENT_RUN_BRAKE_STOP);
      }

      public void OnReset()
      {
         Reset();
         soundProvider.Play(SoundId.failure);
         Analytics.TrackEventRun(Analytics.EVENT_RUN_BRAKE_RESET);
      }

      public override string ToString()
      {
         string result = "speed targets [m/s]: ";

         foreach (float f in speedTargets)
         {
            result += f.ToString("0.00") + " // ";
         }

         result = Tools.ReplaceLastOccurrence(result, " // ", "");
         result += Environment.NewLine;
         result += "mode: " + Mode;

         return result;
      }
   }
}
