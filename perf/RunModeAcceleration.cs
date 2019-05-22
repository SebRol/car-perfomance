using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace perf
{  
   public class RunModeAcceleration : IRunMode
   {
      readonly      IDeviceAcceleromter accelerationProvider;
      readonly      IDeviceSound        soundProvider;
      string        status;
      string        timeSplit;
      Stopwatch     stopWatch;
      float         speed;              // m/s
      float         speedTargetMax;     // m/s
      float         speedTargetLast;    // m/s
      List<float>   speedTargets;       // m/s
      float         acceleration;       // m/s^2
      float         stopDetectionLimit; // m/s^2
      MovingAverage accelerationFilter;

      public RunModeAcceleration(IDeviceAcceleromter accelProvider, IDeviceSound sndProvider)
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
         status = Localization.runModeWait;

         stopWatch.Reset();
         accelerationFilter.Reset();

         timeSplit       = "0.00s";
         acceleration    = 0;
         speed           = 0;
         speedTargetLast = 0;

         RunResult runResult = new RunResult(ResultMode.Speed);
         runResult.Load();
         speedTargetMax      = runResult.GetSpeedTargetMax();
         speedTargets        = runResult.GetSpeedTargets();

         accelerationProvider.SetDirection(true);
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

            if (speedTargets.Count > 0)
            {
               if (speed > speedTargets[0]) // is a SplitTime reached?
               {
                  speedTargetLast = speedTargets[0];
                  speedTargets.RemoveAt(0); // pop SplitTime
                  timeSplit = Tools.ToStopwatchTime(stopWatch.Elapsed);
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
         get { return "Acceleration"; }
      }

      string IRunMode.Mode
      {
         get { return "Acceleration"; }
      }

      public ResultMode SetupMode 
      { 
         get { return ResultMode.Speed; }
      }

      public string ModeText
      {
         get { return Localization.runModeAcceleration; }
      }

      public string Background
      {
         get { return "@drawable/speedo_mode1_bg.png"; }
      }

      public string Speedo
      {
         get { return "@drawable/speedo_mode1.png"; }
      }
      
      public string Needle
      {
         get { return "@drawable/speedo_mode1_needle.png"; }
      }

      public string ButtonRun
      {
         get { return Localization.btn_mode1_run; }
      }

      public string ButtonResults
      {
         get { return Localization.btn_mode1_results; }
      }

      public string ButtonSetup
      {
         get { return "@drawable/btn_setup_mode1.png"; }
      }

      public uint Color
      {
         get { return 4278255615u /* Convert.ToUInt32("0xFF00FFFF", 16) */ ; }
      }
      
      public string Status
      {
         get { return status; }
      }

      public string TimeSplit
      {
         get { return timeSplit; }
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
         get { return speed > speedTargetMax; }
      }

      public bool IsReset
      {
         get { return ((stopWatch.IsRunning == true) && (acceleration < stopDetectionLimit)); } 
      }

      public bool HasTimeSplitChanged
      {
         get { return speedTargetLast > 0; }
      }

      public void OnLaunchDetected()
      {
         status = Localization.runModeLaunch;
         stopWatch.Restart();
         soundProvider.Play(SoundId.acc1_launch);
         Analytics.TrackEventRun(Analytics.EVENT_RUN_ACCELERATION_START);
      }
      
      public void OnTimeSplit()
      {
         soundProvider.Play(SoundId.acc2_reached);
         Analytics.TrackEventTimeSplit(Analytics.EVENT_RUN_ACCELERATION_SPLIT, timeSplit, (int)speedTargetLast);
         speedTargetLast = 0;
      }

      public void OnFinished()
      {
         stopWatch.Stop();
         status = Localization.runModeFinish;
         Analytics.TrackEventRun(Analytics.EVENT_RUN_ACCELERATION_STOP);
      }

      public void OnReset()
      {
         Reset();
         soundProvider.Play(SoundId.failure);
         Analytics.TrackEventRun(Analytics.EVENT_RUN_ACCELERATION_RESET);
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
