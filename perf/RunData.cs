using System;
using System.Collections.Generic;
using System.Text;


namespace perf
{
   public class Event
   {
      public float time;         // second
      public float acceleration; // meter par second^2
      public float speed;        // meter per second
      public float distance;     // meter
      public float height;       // meter

      public Event(float t, float a, float s, float d, float h)
      {
         time         = t;
         acceleration = a;
         speed        = s;
         distance     = d;
         height       = h;
      }

      public override string ToString()
      {
         StringBuilder sb = new StringBuilder();

         sb.Append(time).Append(RunData.SEPERATOR_MEMBER);
         sb.Append(acceleration).Append(RunData.SEPERATOR_MEMBER);
         sb.Append(speed).Append(RunData.SEPERATOR_MEMBER);
         sb.Append(distance).Append(RunData.SEPERATOR_MEMBER);
         sb.Append(height);

         return sb.ToString();
      }
   }

   public enum SubMode
   {
      TimeSpeed,
      TimeDistance,
      Distance,
      Height
   }

   public class RunData
   {
      public const char SEPARATOR_OBJECT = ';';
      public const char SEPERATOR_MEMBER = '#';

      readonly List<Event>  events;
      string                mode;
      float                 startTime;
      RunDistance           runDistance;
      DataItemRun           itemRun;
      RunAdjust             runAdjust;
      AccelerometerRecorder accelerationRecorder;

      public RunData(string mode)
      {
         this.mode            = mode;
         events               = new List<Event>(2048); // 100 seconds recording if 1 event every 50ms (= 20Hz)
         runDistance          = new RunDistance(mode);
         runAdjust            = new RunAdjust();
         accelerationRecorder = new AccelerometerRecorder(false); // false: do not use queue mode
      }

      public void Reset()
      {
         startTime = 0;
         events.Clear();
         runDistance.Reset();
         itemRun = null;
         accelerationRecorder.Reset();
      }

      public override string ToString()
      {
         return runAdjust.ToString();
      }

      public void AddEventSlow(IDeviceGps gpsProvider, float time, float acceleration, float speed)
      {
         // record low frequence samples to get RunResults later on

         if (events.Count == 0)
         {
            // a new run started
            startTime = time;
            // init distance tracker
            runDistance.AddRunLocation(gpsProvider, time, speed, true);
            // add a null event as a start marker
            events.Add(new Event(0, 0, 0, 0, 0));
         }
         else
         {
            // update distance tracker
            runDistance.AddRunLocation(gpsProvider, time, speed, false);
         }

         Event e = new Event(time - startTime, 
                             acceleration,
                             speed, 
                             runDistance.DistanceFiltered, 
                             runDistance.HeightFiltered);
         events.Add(e);
      }

      public void AddEventFast(float axisX, float axisY, float axisZ, float speed)
      {
         // record high frequence samples to get PeakAcceleration later on
         accelerationRecorder.Add(axisX, axisY, axisZ, speed);
      }

      public void AddStartLocation(IDeviceGps gps, float time)
      {
         // refuse "location of launch" if event data is present
         if (events.Count > 0) return;

         runDistance.AddStartLocation(gps, time);
      }

      public void Load(DataItemRun r)
      {
         mode                 = r.Mode;
         string   data        = r.Data;
         string[] eventObject = data.Split(SEPARATOR_OBJECT);

         runAdjust.PeakAcceleration = r.PeakAcceleration; // use peakA to choose proper calibration curve

         foreach (string eo in eventObject)
         {
            string[] eventMemeber = eo.Split(SEPERATOR_MEMBER);
            if (eventMemeber.Length == 5)
            {
               Event e = new Event
               (
                  float.Parse(eventMemeber[0]),
                  float.Parse(eventMemeber[1]), 
                  float.Parse(eventMemeber[2]), 
                  float.Parse(eventMemeber[3]),
                  float.Parse(eventMemeber[4])
               );
               
               events.Add(e);
            }
         }
      }

      public void Store()
      {
         // refuse to store empty run
         if (events.Count == 0) return;

         Database db = Database.GetInstance();
         DataItemVehicle itemVehicle = db.GetActiveProfile();

         if (itemRun == null)
         {
            Debug.LogToFileEventText("run insert, event count: " + events.Count);

            // this run was never stored to the database
            itemRun            = new DataItemRun();
            itemRun.VehicleId  = itemVehicle.Id;
            itemRun.Date       = DateTime.Now;
            itemRun.Mode       = mode;
            itemRun.Data       = GetEventsString();

            // attach peakA to the run
            accelerationRecorder.StorePeakAcceleration(itemVehicle, itemRun);

            // insert new run
            db.SaveRun(itemRun);
         }
         else
         {
            Debug.LogToFileEventText("run update, event count: " + events.Count);

            // update run with current events
            itemRun.Data = GetEventsString();

            // attach peakA to the run
            accelerationRecorder.StorePeakAcceleration(itemVehicle, itemRun);

            // update run
            db.UpdateRun(itemRun);
         }
      }

      public ResultItem GetResult(ResultItem item, SubMode subMode, string unit)
      {
         ResultItem result;
         string     start;
         string     end;
         float      speedStart;
         float      speedEnd;
         float      distanceStart;
         float      distanceEnd;
         float      resultValue;

         //Debug.LogToFileMethod("item: " + item + " // " + "subMode: " + subMode + " // " + "unit: " + unit);

         item.GetStartEnd(out start, out end);

         //Debug.LogToFileMethod("start: " + start + " // " + "end: " + end);

         // convert speed strings
         if (Settings.IsSpeedUnitKph())
         {
            // from kph-string to m/s-float
            speedStart = Tools.ToMeterPerSecond(start);
            speedEnd   = Tools.ToMeterPerSecond(end);
         }
         else
         {
            // from mph-string to m/s-float
            speedStart = Tools.ToMeterPerSecondM(start);
            speedEnd   = Tools.ToMeterPerSecondM(end);
         }

         //Debug.LogToFileMethod("speedStart: " + speedStart + " // " + "speedEnd: " + speedEnd);

         if (mode.Equals(RunModeZeroToZero.Mode)) // zerotozero results in [time] unit
         {
            // mode zerotozero -> stop is same as start, but with stop detection limit on top (avoid unfound speedStop)
            RunStartStop runStartStop = new RunStartStop(Database.GetInstance().GetActiveProfile());
            float speedStop = speedStart + runStartStop.GetStopLimit(); 

            resultValue = GetDifferenceZeroToZero(speedStart, speedEnd, speedStop, subMode);
            resultValue = runAdjust.GetAdjusted(resultValue, mode, subMode);
            result = new ResultItem(start + " - " + end + " - " + start, Tools.ToResultFormat(resultValue, subMode, unit));
         }
         else if (mode.Equals(RunModeBrake.Mode)) // brake results in [time] unit
         {
            // adjust speedEnd with calibrated stopDetection to avoid unfound speedEnd
            RunStartStop runStartStop = new RunStartStop(Database.GetInstance().GetActiveProfile());
            speedEnd += runStartStop.GetStopLimit();

            resultValue = GetDifferenceBrake(speedStart, speedEnd, subMode);
            resultValue = runAdjust.GetAdjusted(resultValue, mode, subMode);
            result = new ResultItem(start + " - " + end, Tools.ToResultFormat(resultValue, subMode, unit));
         }
         else // acceleretion results in [time] or [distance] unit
         {
            if (subMode == SubMode.TimeDistance)
            {
               // convert distance strings
               if (Settings.IsDistanceUnitMeter())
               {
                  // from meter-string to meter-float
                  distanceStart = float.Parse(start);
                  distanceEnd   = float.Parse(end);
               }
               else
               {
                  // from foot-string to meter-float
                  distanceStart = Tools.ToMeter(start);
                  distanceEnd   = Tools.ToMeter(end);
               }

               //Debug.LogToFileMethod("distanceStart: " + distanceStart + " // " + "distanceEnd: " + distanceEnd);

               resultValue = GetDifferenceAcceleration(distanceStart, distanceEnd, subMode);
               resultValue = runAdjust.GetAdjusted(resultValue, mode, subMode);

               float  speedAtEndDistance       = GetSpeed(distanceEnd);
               string speedAtEndDistanceString = (Settings.IsSpeedUnitKph()) ? Tools.ToKilometerPerHour(speedAtEndDistance) : Tools.ToMilesPerHour(speedAtEndDistance);
               string unitDistance             = (Settings.IsSpeedUnitKph()) ? Localization.unitKph : Localization.unitMph;

               result = new ResultItem(start + " - " + end, Tools.ToResultFormat(resultValue, subMode, unit) + " @" + speedAtEndDistanceString + unitDistance);
            }
            else
            {
               resultValue = GetDifferenceAcceleration(speedStart, speedEnd, subMode);
               resultValue = runAdjust.GetAdjusted(resultValue, mode, subMode);
               result = new ResultItem(start + " - " + end, Tools.ToResultFormat(resultValue, subMode, unit));
            }
         }

         return result;
      }
      
      public float GetDistanceFiltered()
      {
         return runDistance.DistanceFiltered;
      }

      public float GetDistanceGps()
      {
         return runDistance.DistanceGps;
      }

      public float GetHeightFiltered()
      {
         return runDistance.HeightFiltered;
      }

      public float GetHeightGps()
      {
         return runDistance.HeightGps;
      }

      public List<Event> GetEvents()
      {
         return events;
      }

      public List<Event> GetEventsAdjusted(SubMode subMode)
      {
         List<Event> result = new List<Event>();

         // copy events to new list, adjust time and distance according to calibration
         foreach (Event e in events) 
         {
            result.Add(new Event(runAdjust.GetAdjusted(e.time, mode, subMode), // time adjusted
                                 e.acceleration,
                                 e.speed,
                                 runAdjust.GetAdjusted(e.distance, mode, subMode), // distance adjusted
                                 e.height));
         }

         return result;
      }

      public float GetPeakAcceleration()
      {
         return runAdjust.PeakAcceleration;
      }

      public int GetCalibrationIndex()
      {
         return runAdjust.Index;
      }

      public string GetEventsCommaSeparated()
      {
         StringBuilder result = new StringBuilder();

         result.Append("Calibraion: "      ).Append(Environment.NewLine);
         result.Append(runAdjust.ToString()).Append(Environment.NewLine);
         result.Append(""                  ).Append(Environment.NewLine);
         result.Append("Data:"             ).Append(Environment.NewLine);

         result.Append("Time[s];Acceleration[m/s^2];Speed[m/s];Distance[m];Height[m]");
         result.Append(Environment.NewLine);

         foreach (Event e in events)
         {
            result.Append(e.time.ToString("0.00")         + SEPARATOR_OBJECT + 
                          e.acceleration.ToString("0.00") + SEPARATOR_OBJECT + 
                          e.speed.ToString("0.00")        + SEPARATOR_OBJECT + 
                          e.distance.ToString("0.00")     + SEPARATOR_OBJECT + 
                          e.height.ToString("0.00"));
            result.Append(Environment.NewLine);
         }

         return result.ToString();
      }

      float GetDifferenceAcceleration(float start, float end, SubMode subMode)
      {
         float result     = 0;
         Event eventStart = null;
         Event eventEnd   = null;

         // search for start in events
         foreach (Event e in events)
         {
            if (subMode == SubMode.TimeDistance)
            {
               if (e.distance >= start)
               {
                  eventStart = e;
                  break;
               }
            }
            else
            {
               if (e.speed >= start)
               {
                  eventStart = e;
                  break;
               }
            }
         }

         // search for end in events
         foreach (Event e in events)
         {
            if (subMode == SubMode.TimeDistance)
            {
               if (e.distance >= end)
               {
                  eventEnd = e;
                  break;
               }
            }
            else
            {
               if (e.speed >= end)
               {
                  eventEnd = e;
                  break;
               }
            }
         }

         result = Subtract(eventEnd, eventStart, subMode);

         return result;
      }

      float GetDifferenceBrake(float start, float end, SubMode subMode)
      {
         float result     = 0;
         Event eventStart = null;
         Event eventEnd   = null;
         int   indexStart = 0;

         // check index out of bounds
         if (events.Count > 0)
         {
            // search for start in events in reverse order -> omit matchig events at the beginning of the events
            for (indexStart = events.Count - 1; indexStart >= 0; indexStart--)
            {
               Event e = events[indexStart];

               if (e.speed >= start)
               {
                  eventStart = e;
                  break;
               }
            }
         }

         // do not allow negative array index
         indexStart = (indexStart < 0) ? 0 : indexStart;

         // search for end in events, start search where eventStart was found
         for (int i = indexStart; i < events.Count; i++)
         {
            Event e = events[i];

            if (e.speed <= end)
            {
               eventEnd = e;
               break;
            }
         }

         result = Subtract(eventEnd, eventStart, subMode);

         return result;
      }

      float GetDifferenceZeroToZero(float start, float target, float end, SubMode subMode)
      {
         float result          = 0;
         Event eventStart      = null;
         Event eventTarget     = null;
         Event eventEnd        = null;
         float eventTargetTime = float.MaxValue;

         // search for start in events
         foreach (Event e in events)
         {
            if (e.speed >= start)
            {
               eventStart = e;
               break;
            }
         }

         // search for taget in events
         foreach (Event e in events)
         {
            if (e.speed >= target)
            {
               eventTarget = e;
               eventTargetTime = e.time;
               break;
            }
         }

         // search for end in events
         foreach (Event e in events)
         {
            if (e.speed <= end)
            {
               // ensure end is later than target
               if (e.time > eventTargetTime)
               {
                  eventEnd = e;
                  break;
               }
            }
         }

         result = Subtract(eventEnd, eventStart, subMode);

         return result;
      }

      float Subtract(Event eventEnd, Event eventStart, SubMode subMode)
      {
         float result = 0;

         if ((eventEnd != null) && (eventStart != null))
         {
            switch (subMode)
            {
            case SubMode.TimeSpeed:
            case SubMode.TimeDistance:
               result = eventEnd.time - eventStart.time;
               break;
            case SubMode.Distance:
               result = eventEnd.distance - eventStart.distance;
               break;
            case SubMode.Height:
               result = eventEnd.height - eventStart.height;
               break;
            }
         }

         //Debug.LogToFileMethod("eventEnd: " + eventEnd);
         //Debug.LogToFileMethod("eventStart: " + eventStart);
         //Debug.LogToFileMethod("result: " + result);

         return result;
      }

      float GetSpeed(float distanceEnd)
      {
         float result   = 0;
         Event eventEnd = null;

         // search for end in events
         foreach (Event e in events)
         {
            if (e.distance >= distanceEnd)
            {
               eventEnd = e;
               break;
            }
         }

         if (eventEnd != null)
         {
            result = eventEnd.speed;
         }

         return result;
      }

      string GetEventsString() // does operate on raw events 
      {
         StringBuilder builder = new StringBuilder();

         foreach (Event e in events)
         {
            builder.Append(e.ToString()).Append(SEPARATOR_OBJECT);
         }

         // remove last seperator
         if (builder.Length > 0) builder.Remove(builder.Length - 1, 1);

         return builder.ToString();
      }
   }
}
