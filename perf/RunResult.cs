using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace perf
{
   public enum ResultMode
   {
      Speed,
      Distance,
      Brake,
      ZeroToZero
   }

   // observable, sorted collection of ResultItem
   public class RunResult
   {
      public const string TAG_START = "Start @";
      public const string TAG_END   = "End @";

      public event NotifyCollectionChangedEventHandler CollectionChanged;
      public int                                       Count { get { return items.Count; } }
      public ObservableCollection<ResultItem>          Items { get { return items; } }

      readonly ObservableCollection<ResultItem>        items;
      ResultMode                                       mode;

      public RunResult(ResultMode m)
      {
         items = new ObservableCollection<ResultItem>();
         items.CollectionChanged += OnCollectionChanged;
         mode = m;
      }

      public override string ToString()
      {
         // example for Acceleration: "Start @0,End @100;Start @0,End @200;Start @0,End @240;Start @0,End @250;Start @100,End @200;"
         // example for Distance:     "Start @0,End @660 (1/8mile);Start @0,End @1320 (1/4mile);Start @0,End @2640 (1/2mile);"
         // example for Brake:        "Start @70,End @0;"
         // example for ZeroToZero:   "Start @0,Target @60;"

         StringBuilder sb = new StringBuilder();

         foreach (ResultItem ri in items)
         {
            sb.Append(ri.ToString()).Append(";");
         }

         return sb.ToString();
      }

      public ResultItem Add(string s1, string s2)
      {
         ResultItem item = new ResultItem(s1, s2);
         items.Add(item);
         Sort();

         return item;
      }

      public bool Remove(ResultItem item)
      {
         return items.Remove(item);
      }

      public void Clear()
      {
         items.Clear();
      }

      public void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         if (CollectionChanged != null) CollectionChanged.Invoke(sender, e);
      }

      public void AddDefaultItems()
      {
         /*
         Speed:
         [0-100; 0-200; 0-300]kph
         [0-60; 0-100; 0-150]mph

         Distance:
         [0-201 (1/8mile); 0-402 (1/4mile); 0-804 (1/2mile)]m
         [0-660 (1/8mile); 0-1320 (1/4mile); 0-2640 (1/2mile)]ft

         Brake:
         100-0kph
         70-0mph

         Zero-Zero:
         [0-100-0; 0-200-0]kph
         [0-60-0; 0-100-0]mph
         */

         const string startZero = TAG_START + "0";

         switch (mode)
         {
         case ResultMode.Speed:
            if (Settings.IsSpeedUnitKph())
            {
               items.Add(new ResultItem(startZero, "End @100"));

               if (AppStore.Instance.IsPurchased())
               {
                  items.Add(new ResultItem(startZero, "End @200"));
                  items.Add(new ResultItem(startZero, "End @300"));
               }
            }
            else
            {
               items.Add(new ResultItem(startZero, "End @60"));

               if (AppStore.Instance.IsPurchased())
               {
                  items.Add(new ResultItem(startZero, "End @100"));
                  items.Add(new ResultItem(startZero, "End @150"));
               }
            }
            break;

         case ResultMode.Distance:
            if (Settings.IsDistanceUnitMeter())
            {
               items.Add(new ResultItem(startZero, "End @201 (1/8mile)"));
               items.Add(new ResultItem(startZero, "End @402 (1/4mile)"));
               items.Add(new ResultItem(startZero, "End @804 (1/2mile)"));
            }
            else
            {
               items.Add(new ResultItem(startZero, "End @660 (1/8mile)"));
               items.Add(new ResultItem(startZero, "End @1320 (1/4mile)"));
               items.Add(new ResultItem(startZero, "End @2640 (1/2mile)"));
            }
            break;

         case ResultMode.Brake:
            if (Settings.IsSpeedUnitKph())
            {
               items.Add(new ResultItem("Start @100", "End @0"));
            }
            else
            {
               items.Add(new ResultItem("Start @70", "End @0"));
            }
            break;

         case ResultMode.ZeroToZero:
            if (Settings.IsSpeedUnitKph())
            {
               items.Add(new ResultItem(startZero, "Target @100"));
            }
            else
            {
               items.Add(new ResultItem(startZero, "Target @60"));
            }
            break;
         }
      }

      public void Load()
      {
         string s = null;

         switch (mode)
         {
         case ResultMode.Speed:
            s = Settings.GetValueOrDefault(Settings.ResultSetupSpeed,      "");
            break;
         case ResultMode.Distance:
            s = Settings.GetValueOrDefault(Settings.ResultSetupDistance,   "");
            break;
         case ResultMode.Brake:
            s = Settings.GetValueOrDefault(Settings.ResultSetupBrake,      "");
            break;
         case ResultMode.ZeroToZero:
            s = Settings.GetValueOrDefault(Settings.ResultSetupZeroToZero, "");
            break;
         }

         if (s != null)
         {
            if (s.Length != 0)
            {
               List<ResultItem> result = Parse(s);
               if (result.Count > 0)
               {
                  items.Clear();

                  foreach (ResultItem ri in result)
                  {
                     items.Add(ri);
                  }
               }
               else
               {
                  AddDefaultItems();
               }
            }
            else
            {
               AddDefaultItems();
            }
         }
         else
         {
            AddDefaultItems();
         }

         switch (mode)
         {
         case ResultMode.Speed:
         case ResultMode.Brake:
         case ResultMode.ZeroToZero:
            if (Settings.IsSpeedUnitKph() == false) ToMph();
            break;
         case ResultMode.Distance:
            if (Settings.IsDistanceUnitMeter() == false) ToFeet();
            break;
         }
      }

      public void Store()
      {
         switch (mode)
         {
         case ResultMode.Speed:
            if (Settings.IsSpeedUnitKph() == false) ToKph();
            Settings.AddOrUpdateValue(Settings.ResultSetupSpeed, ToString());
            break;
         case ResultMode.Distance:
            if (Settings.IsDistanceUnitMeter() == false) ToMeter();
            Settings.AddOrUpdateValue(Settings.ResultSetupDistance, ToString());
            break;
         case ResultMode.Brake:
            if (Settings.IsSpeedUnitKph() == false) ToKph();
            Settings.AddOrUpdateValue(Settings.ResultSetupBrake, ToString());
            break;
         case ResultMode.ZeroToZero:
            if (Settings.IsSpeedUnitKph() == false) ToKph();
            Settings.AddOrUpdateValue(Settings.ResultSetupZeroToZero, ToString());
            break;
         }
      }

      public float GetSpeedTargetMax()
      {
         float  result = 0;
         float  speed;
         string start;
         string end;
         string speedString;

         foreach (ResultItem ri in items)
         {
            ri.GetStartEnd(out start, out end);

            switch (mode)
            {
            case ResultMode.Brake:
               speedString = start;
               break;
            default:
               speedString = end;
               break;
            }

            speed = float.Parse(speedString);

            if (speed > result)
            {
               // found a higher value -> store it
               result = speed;
            }
         }

         if (Settings.IsSpeedUnitKph())
         {
            result = Tools.ToMeterPerSecondF(result);
         }
         else
         {
            result = Tools.ToMeterPerSecondFM(result);
         }

         return result;
      }

      public List<float> GetSpeedTargets()
      {
         List<float> result = new List<float>(10);
         float       speed;
         string      start;
         string      end;
         string      speedString;
         bool        isSpeedUnitKph = Settings.IsSpeedUnitKph();

         foreach (ResultItem ri in items)
         {
            ri.GetStartEnd(out start, out end);

            switch (mode)
            {
            case ResultMode.Brake:
               speedString = start;
               break;
            default:
               speedString = end;
               break;
            }

            speed = float.Parse(speedString);

            if (isSpeedUnitKph) speed = Tools.ToMeterPerSecondF(speed);
            else                speed = Tools.ToMeterPerSecondFM(speed);

            result.Add(speed);
         }

         return result.Distinct().OrderBy(x => x).ToList(); // remove doppelgaenger, keep unique only, then sort
      }

      void Sort()
      {
         // convert collection to list, use LINQ to sort. items must be IComparable.
         List<ResultItem> sorted = items.OrderBy(x => x).ToList();

         // rearange collection (keep original object)
         for (int i = 0; i < sorted.Count(); i++)
         {
            items.Move(items.IndexOf(sorted[i]), i);
         }
      }

      List<ResultItem> Parse(string s)
      {
         List<ResultItem> result = new List<ResultItem>();

         const char separatorLine      = ';';
         const char separatorLeftRight = ',';

         string[] lines = s.Split(separatorLine);
         foreach (string l in lines)
         {
            string[] leftRight = l.Split(separatorLeftRight);
            if (leftRight.Length == 2)
            {
               ResultItem ri = new ResultItem(leftRight[0], leftRight[1]);
               result.Add(ri);
            }
         }

         return result;
      }
      
      void ToKph()
      {
         foreach (ResultItem ri in items)
         {
            ri.ToKph();
         }
      }

      void ToMph()
      {
         foreach (ResultItem ri in items)
         {
            ri.ToMph();
         }
      }
      
      void ToMeter()
      {
         foreach (ResultItem ri in items)
         {
            ri.ToMeter();
         }
      }

      void ToFeet()
      {
         foreach (ResultItem ri in items)
         {
            ri.ToFeet();
         }
      }
   }
}
