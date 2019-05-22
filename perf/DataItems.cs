using System;
using SQLite;

namespace perf
{
   public static class DataItem
   {
      public static DataItemMeta CreateDefaultMeta()
      {
         DataItemMeta d = new DataItemMeta
         {
            Version       = 0, 
            ActiveProfile = 0
         };

         return d;
      }

      public static DataItemVehicle CreateDefaultVehicle()
      {
         DataItemVehicle d = new DataItemVehicle
         {
            Model       = Localization.pageProfileEditDefault,
            Color       = "#2AB3FF",
            Calibration = 0,
            AxisOffsetX = 0,
            AxisOffsetY = 0,
            AxisOffsetZ = 0
         };

         return d;
      }

      public static DataItemRun CreateDefaultRun()
      {
         DataItemRun d = new DataItemRun 
         {
            VehicleId        = -1,
            Mode             = String.Empty,
            Date             = DateTime.Now,
            PeakAcceleration = 0,
            Data             = String.Empty
         };

         return d;
      }
   }

   public class DataItemMeta
   {
      [PrimaryKey, AutoIncrement]
      public int Id {get; set;}

      public int Version {get; set;}

      public int ActiveProfile {get; set;}
   }

   public class DataItemVehicle
   {
      [PrimaryKey, AutoIncrement]
      public int Id {get; set;}

      public string Model {get; set;}

      public string Color {get; set;}

      public float Calibration {get; set;}

      public float AxisOffsetX {get; set;}

      public float AxisOffsetY {get; set;}

      public float AxisOffsetZ {get; set;}
   }

   public class DataItemRun
   {
      [PrimaryKey, AutoIncrement]
      public int Id {get; set;}

      // ForeignKey: this 'run' is linked to one vehicle
      public int VehicleId {get; set;}

      // Acceleration, Brake or ZeroToZero
      public string Mode {get; set;}

      public DateTime Date {get; set;}

      public float PeakAcceleration {get; set;}

      // all samples packed in one string
      public string Data {get; set;}
   }
}
