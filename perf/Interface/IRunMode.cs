
namespace perf
{
   public interface IRunMode
   {
      void Reset();

      // mode type
      string     Mode            { get; } // for run mode type comparison
      ResultMode SetupMode       { get; } // for result setup and display

      // gui elements
      string ModeText            { get; } // for gui labels
      string Background          { get; }
      string Speedo              { get; }
      string Needle              { get; }
      string ButtonRun           { get; }
      string ButtonResults       { get; }
      string ButtonSetup         { get; }
      uint   Color               { get; } // ARGB

      // input data
      float  Acceleration        { set; } // meter per square-second
      float  Speed               { set; } // meter per second
      float  StopDetectionLimit  { set; } // meter per square-second

      // output data
      string Status              { get; }
      string TimeSplit           { get; }
      string TimeElapsed         { get; }
      float  TimeElapsedF        { get; }
      bool   IsStarted           { get; }
      bool   IsFinished          { get; }
      bool   IsReset             { get; }
      bool   HasTimeSplitChanged { get; }

      // input events
      void OnLaunchDetected();
      void OnTimeSplit();
      void OnFinished();
      void OnReset();
   }
}
