using System;
using System.Text;

namespace perf
{
   class Adjust3
   {
      public float Gain;
      public float ShiftX;
      public float ShiftY;

      public override string ToString()
      {
         return Gain.ToString("0.00") + " // " + ShiftX.ToString("0.00") + " // " + ShiftY.ToString("0.00");
      }
   }

   class Adjust4
   {
      public float Gain;
      public float ShiftX;
      public float ShiftY;
      public float Peak;

      public override string ToString()
      {
         return Gain.ToString("0.00") + " // " + ShiftX.ToString("0.00") + " // " + ShiftY.ToString("0.00") + " // " + Peak.ToString("0.00");
      }
   }

   public class RunAdjust
   {
      float            peakAcceleration;

      readonly Adjust4 adjustDefault;
      readonly Adjust4 adjustAcceleration1;
      readonly Adjust4 adjustAcceleration2;
      readonly Adjust4 adjustAcceleration3;
      readonly Adjust3 adjustBrake;
      readonly Adjust3 adjustZeroToZero;
      readonly Adjust3 adjustDisplay;
      readonly Adjust3 adjustDistanceAcceleration;
      readonly Adjust3 adjustDistanceBrake;

      int       index;
      Adjust4[] adjustList;

      public RunAdjust()
      {
         adjustDefault              = new Adjust4();
         adjustAcceleration1        = new Adjust4();
         adjustAcceleration2        = new Adjust4();
         adjustAcceleration3        = new Adjust4();
         adjustBrake                = new Adjust3();
         adjustZeroToZero           = new Adjust3();
         adjustDisplay              = new Adjust3();
         adjustDistanceAcceleration = new Adjust3();
         adjustDistanceBrake        = new Adjust3();

         // adjusted = ( gain / ( data + shiftX ) ) + ( data + shiftY )

         adjustDefault.Gain   = 1;
         adjustDefault.ShiftX = 0;
         adjustDefault.ShiftY = 0;
         adjustDefault.Peak   = 0;

         adjustAcceleration1.Gain          = Settings.GetValueOrDefault(Settings.AccelerationAdjustTimeGain1,      Settings.DefaultAccelerationAdjustTimeGain1);
         adjustAcceleration1.ShiftX        = Settings.GetValueOrDefault(Settings.AccelerationAdjustTimeShiftX1,    Settings.DefaultAccelerationAdjustTimeShiftX1);
         adjustAcceleration1.ShiftY        = Settings.GetValueOrDefault(Settings.AccelerationAdjustTimeShiftY1,    Settings.DefaultAccelerationAdjustTimeShiftY1);
         adjustAcceleration1.Peak          = Settings.GetValueOrDefault(Settings.AccelerationAdjustTimePeak1,      Settings.DefaultAccelerationAdjustTimePeak1);

         adjustAcceleration2.Gain          = Settings.GetValueOrDefault(Settings.AccelerationAdjustTimeGain2,      Settings.DefaultAccelerationAdjustTimeGain2);
         adjustAcceleration2.ShiftX        = Settings.GetValueOrDefault(Settings.AccelerationAdjustTimeShiftX2,    Settings.DefaultAccelerationAdjustTimeShiftX2);
         adjustAcceleration2.ShiftY        = Settings.GetValueOrDefault(Settings.AccelerationAdjustTimeShiftY2,    Settings.DefaultAccelerationAdjustTimeShiftY2);
         adjustAcceleration2.Peak          = Settings.GetValueOrDefault(Settings.AccelerationAdjustTimePeak2,      Settings.DefaultAccelerationAdjustTimePeak2);

         adjustAcceleration3.Gain          = Settings.GetValueOrDefault(Settings.AccelerationAdjustTimeGain3,      Settings.DefaultAccelerationAdjustTimeGain3);
         adjustAcceleration3.ShiftX        = Settings.GetValueOrDefault(Settings.AccelerationAdjustTimeShiftX3,    Settings.DefaultAccelerationAdjustTimeShiftX3);
         adjustAcceleration3.ShiftY        = Settings.GetValueOrDefault(Settings.AccelerationAdjustTimeShiftY3,    Settings.DefaultAccelerationAdjustTimeShiftY3);
         adjustAcceleration3.Peak          = Settings.GetValueOrDefault(Settings.AccelerationAdjustTimePeak3,      Settings.DefaultAccelerationAdjustTimePeak3);
         
         adjustBrake.Gain                  = Settings.GetValueOrDefault(Settings.BrakeAdjustTimeGain,              Settings.DefaultBrakeAdjustTimeGain);
         adjustBrake.ShiftX                = Settings.GetValueOrDefault(Settings.BrakeAdjustTimeShiftX,            Settings.DefaultBrakeAdjustTimeShiftX);
         adjustBrake.ShiftY                = Settings.GetValueOrDefault(Settings.BrakeAdjustTimeShiftY,            Settings.DefaultBrakeAdjustTimeShiftY);

         adjustZeroToZero.Gain             = Settings.GetValueOrDefault(Settings.ZeroToZeroAdjustTimeGain,         Settings.DefaultZeroToZeroAdjustTimeGain);
         adjustZeroToZero.ShiftX           = Settings.GetValueOrDefault(Settings.ZeroToZeroAdjustTimeShiftX,       Settings.DefaultZeroToZeroAdjustTimeShiftX);
         adjustZeroToZero.ShiftY           = Settings.GetValueOrDefault(Settings.ZeroToZeroAdjustTimeShiftY,       Settings.DefaultZeroToZeroAdjustTimeShiftY);

         adjustDisplay.Gain                = Settings.GetValueOrDefault(Settings.SpeedAdjustDisplayGain,           Settings.DefaultSpeedAdjustDisplayGain);
         adjustDisplay.ShiftX              = Settings.GetValueOrDefault(Settings.SpeedAdjustDisplayShiftX,         Settings.DefaultSpeedAdjustDisplayShiftX);
         adjustDisplay.ShiftY              = Settings.GetValueOrDefault(Settings.SpeedAdjustDisplayShiftY,         Settings.DefaultSpeedAdjustDisplayShiftY);

         adjustDistanceAcceleration.Gain   = Settings.GetValueOrDefault(Settings.DistanceAdjustAccelGain,          Settings.DefaultDistanceAdjustAccelGain);
         adjustDistanceAcceleration.ShiftX = Settings.GetValueOrDefault(Settings.DistanceAdjustAccelShiftX,        Settings.DefaultDistanceAdjustAccelShiftX);
         adjustDistanceAcceleration.ShiftY = Settings.GetValueOrDefault(Settings.DistanceAdjustAccelShiftY,        Settings.DefaultDistanceAdjustAccelShiftY);

         adjustDistanceBrake.Gain          = Settings.GetValueOrDefault(Settings.DistanceAdjustBrakeGain,          Settings.DefaultDistanceAdjustBrakeGain);
         adjustDistanceBrake.ShiftX        = Settings.GetValueOrDefault(Settings.DistanceAdjustBrakeShiftX,        Settings.DefaultDistanceAdjustBrakeShiftX);
         adjustDistanceBrake.ShiftY        = Settings.GetValueOrDefault(Settings.DistanceAdjustBrakeShiftY,        Settings.DefaultDistanceAdjustBrakeShiftY);

         adjustList = new Adjust4[4];

         adjustList[0] = adjustDefault;
         adjustList[1] = adjustAcceleration1;
         adjustList[2] = adjustAcceleration2;
         adjustList[3] = adjustAcceleration3;
      }

      public float PeakAcceleration
      {
         get { return peakAcceleration; }

         set
         {
            peakAcceleration = value;
            index = 0;

            if (peakAcceleration > adjustAcceleration1.Peak)
            {
               index = 1;
               if (adjustAcceleration2.Peak > adjustAcceleration1.Peak)
               {
                  if (peakAcceleration > adjustAcceleration2.Peak)
                  {
                     index = 2;
                     if (adjustAcceleration3.Peak > adjustAcceleration1.Peak)
                     {
                        if (peakAcceleration > adjustAcceleration3.Peak)
                        {
                           index = 3;
                        }
                     }
                  }
               }
            }
            else if (peakAcceleration > adjustAcceleration2.Peak)
            {
               index = 2;
               if (adjustAcceleration1.Peak > adjustAcceleration2.Peak)
               {
                  if (peakAcceleration > adjustAcceleration1.Peak)
                  {
                     index = 1;
                     if (adjustAcceleration3.Peak > adjustAcceleration2.Peak)
                     {
                        if (peakAcceleration > adjustAcceleration3.Peak)
                        {
                           index = 3;
                        }
                     }
                  }
               }
            }
            else if (peakAcceleration > adjustAcceleration3.Peak)
            {
               index = 3;
               if (adjustAcceleration1.Peak > adjustAcceleration3.Peak)
               {
                  if (peakAcceleration > adjustAcceleration1.Peak)
                  {
                     index = 1;
                     if (adjustAcceleration2.Peak > adjustAcceleration3.Peak)
                     {
                        if (peakAcceleration > adjustAcceleration2.Peak)
                        {
                           index = 2;
                        }
                     }
                  }
               }
            }
         }
      }

      public int Index 
      { 
         get
         {
            return index; 
         } 
      }

      public float GetAdjusted(float valueRaw, string mode, SubMode subMode)
      {
         float result = 0;

         switch (subMode)
         {
            case SubMode.TimeDistance:
            case SubMode.TimeSpeed:
               result = GetAdjustedTime(valueRaw, mode);
               break;
            case SubMode.Distance:
            case SubMode.Height:
               result = GetAdjustedDistance(valueRaw, mode);
               break;
         }

         return result;
      }

      public float GetAdjustedDisplaySpeed(float speedRaw)
      {
         float result = 0;

         // adjusted = ( gain / ( raw + shiftX ) ) + ( raw + shiftY )
         result = (DisplayGain / (speedRaw + DisplayShiftX)) + (speedRaw + DisplayShiftY);

         return result;
      }

      public override string ToString()
      {
         StringBuilder result = new StringBuilder();

         result.Append("peak acceleration [m/s^2]: "                 + peakAcceleration           + Environment.NewLine);
         result.Append("calibration curve: "                         + index                      + Environment.NewLine);
         result.Append("time acceleration adjust 0 [g,x,y,p]: "      + adjustDefault              + Environment.NewLine);
         result.Append("time acceleration adjust 1 [g,x,y,p]: "      + adjustAcceleration1        + Environment.NewLine);
         result.Append("time acceleration adjust 2 [g,x,y,p]: "      + adjustAcceleration2        + Environment.NewLine);
         result.Append("time acceleration adjust 3 [g,x,y,p]: "      + adjustAcceleration3        + Environment.NewLine);
         result.Append("time brake adjust [g,x,y]: "                 + adjustBrake                + Environment.NewLine);
         result.Append("time zerotozero adjust [g,x,y]: "            + adjustZeroToZero           + Environment.NewLine);
         result.Append("speed display adjust [g,x,y]: "              + adjustDisplay              + Environment.NewLine);
         result.Append("disctance adjust acceleration [g,x,y]: "     + adjustDistanceAcceleration + Environment.NewLine);
         result.Append("disctance adjust brake/zerotozero [g,x,y]: " + adjustDistanceBrake);

         return result.ToString();
      }

      float TimaAccelerationAdjust   { get { return adjustList[index].Gain;          } }
      float TimeBrakeGain            { get { return adjustBrake.Gain;                } }
      float TimeZeroToZeroGain       { get { return adjustZeroToZero.Gain;           } }
      float DisplayGain              { get { return adjustDisplay.Gain;              } }
      float DisplayShiftX            { get { return adjustDisplay.ShiftX;            } }
      float DisplayShiftY            { get { return adjustDisplay.ShiftY;            } }
      float DistanceAccelerationGain { get { return adjustDistanceAcceleration.Gain; } }
      float DistanceBrakeGain        { get { return adjustDistanceBrake.Gain;        } }

      float GetAdjustedTime(float timeRaw, string mode)
      {
         float result = 0;

         // adjusted = raw * gain

         switch (mode)
         {
            case "Acceleration":
            result = timeRaw * TimaAccelerationAdjust;
            break;
            case "Brake":
            result = timeRaw * TimeBrakeGain;
            break;
            case "ZeroToZero":
            result = timeRaw * TimeZeroToZeroGain;
            break;
         }

         return result;
      }

      float GetAdjustedDistance(float distanceRaw, string mode)
      {
         float result = 0;

         // adjusted = raw * gain

         switch (mode)
         {
            case "Acceleration":
            result = distanceRaw * DistanceAccelerationGain;
            break;
            case "Brake":
            case "ZeroToZero":
            result = distanceRaw * DistanceBrakeGain;
            break;
         }

         return result;
      }
   }
}
