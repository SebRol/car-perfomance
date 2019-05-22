using System;
using System.Diagnostics;
using Android.Hardware;
using Android.Runtime;
using Xamarin.Forms;

namespace perf
{
   public class TestDataAccelerometer
   {
      static TestData testData = new TestDataAccelerometer01();
      static int index;
      static int counter;
      static bool enabled;

      public static bool IsEnabled() 
      { 
         counter++;

         if (counter >= testData.Data.Length)
         {
            enabled = !enabled;
            counter = 0;
         }

         return enabled;
      }

      public static void Advance() 
      { 
         index++;

         if (index >= testData.Data.Length)
         {
            index = 0;
         }
      }

      public static long  Timestamp { get { return (long)(testData.Data[index].Time * 1000000000); } }
      public static float X         { get { return testData.Data[index].X; } }
      public static float Y         { get { return testData.Data[index].Y; } }
      public static float Z         { get { return testData.Data[index].Z; } }
      public static float Speed     { get { return testData.Data[index].Speed; } }
      public static float PeakA     { get { return testData.Data[index].PeakA; } }
   }
}
