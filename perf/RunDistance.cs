using System;
using System.Collections.Generic;
using System.Linq;

namespace perf
{
   public class RunDistance
   {
      float                distanceFiltered;
      float                distanceGps;
      float                heightFiltered;
      float                heightGps;
      float                lastTime;
      string               mode;
      double               startLatitude;
      double               startLongitude;
      double               startAltitude;
      double               lastLatitude;
      double               lastLongitude;
      List<double>         startLatitudeList;
      List<double>         startLongitudeList;
      List<double>         startAltitudeList;
      MovingAverage        distanceFilter;
      MovingAverage        heightFilter;

      public RunDistance(string runMode)
      {
         mode = runMode;
         startLatitudeList  = new List<double>(16);
         startLongitudeList = new List<double>(16);
         startAltitudeList  = new List<double>(16);

         int filterSize = (int)Settings.GetValueOrDefault(Settings.DistanceFilter, Settings.DefaultDistanceFilter);

         distanceFilter = new MovingAverage(filterSize);
         heightFilter   = new MovingAverage(filterSize);
      }

      public void Reset()
      {
         distanceFiltered = 0;
         distanceGps      = 0;
         heightFiltered   = 0;
         heightGps        = 0;
         lastTime         = 0;
         startLatitude    = 0;
         startLongitude   = 0;
         startAltitude    = 0;
         lastLatitude     = 0;
         lastLongitude    = 0;
         startLatitudeList.Clear();
         startLongitudeList.Clear();
         startAltitudeList.Clear();
         distanceFilter.Reset();
         heightFilter.Reset();
      }

      public float DistanceFiltered { get { return distanceFiltered; } }

      public float DistanceGps      { get { return distanceGps;      } }

      public float HeightFiltered   { get { return heightFiltered;   } }

      public float HeightGps        { get { return heightGps;        } }

      public void AddStartLocation(IDeviceGps gps, float time)
      {
         startLatitudeList.Add(gps.GetLatitude());
         startLongitudeList.Add(gps.GetLongitude());
         startAltitudeList.Add(gps.GetAltitude());

         lastTime = time;
      }

      public void AddRunLocation(IDeviceGps gps, float time, float speed, bool isRunStart)
      {
         float deltaTime = time - lastTime;
         float distance  = 0;
         float height    = 0;

         if (isRunStart)
         {
            if ((mode.Equals(RunModeAcceleration.Mode)) || (mode.Equals(RunModeZeroToZero.Mode)))
            {
               // vehicle launched from zero speed

               // in mode acceleration OR zerotozero -> we can make the start location more precise
               if ((startLatitudeList.Count > 0))
               {
                  // some locations were reported until vehicle launched -> use them as starting point
                  startLatitude  = startLatitudeList.Average();
                  startLongitude = startLongitudeList.Average();
                  startAltitude  = startAltitudeList.Average();
               }
               else
               {
                  // cannot make location more precise -> use given location
                  startLatitude  = gps.GetLatitude();
                  startLongitude = gps.GetLongitude();
                  startAltitude  = gps.GetAltitude();
               }
            }
            else
            {
               // vehicle reached target speed in mode "brake"

               // in mode brake -> use last known location as starting point
               if (startLatitudeList.Count > 0)
               {
                  // some locations were reported since vehicle launched in brake mode
                  startLatitude  = startLatitudeList.Last();
                  startLongitude = startLongitudeList.Last();
                  startAltitude  = startAltitudeList.Last();

                  // check if given location is same as last known location
                  if (IsLocationEqual(startLatitude, startLongitude, gps.GetLatitude(), gps.GetLongitude()))
                  {
                     // given location is same as last known location -> add the distance (=speed*time) traveled since last gps update
                     // this is the case in 95% of all cases, because this method is called 20 times a seconds, but gps update is only 1Hz
                     distance = speed * deltaTime;
                  }
                  else
                  {
                     // this is the case in 5% of all cases (more ore less)
                     // given location is the freshest location available
                     startLatitude  = gps.GetLatitude();
                     startLongitude = gps.GetLongitude();
                     startAltitude  = gps.GetAltitude();
                  }
               }
               else
               {
                  // no locations were reported since launch -> use given location as starting point
                  startLatitude  = gps.GetLatitude();
                  startLongitude = gps.GetLongitude();
                  startAltitude  = gps.GetAltitude();
               }
            }
         }
         else
         {
            // given location is not a start location -> measurement is running

            distanceGps = gps.GetDistance(startLatitude, startLongitude, gps.GetLatitude(), gps.GetLongitude());

            if ((mode.Equals(RunModeAcceleration.Mode)) || (mode.Equals(RunModeZeroToZero.Mode)))
            {
               if (IsLocationEqual(lastLatitude, lastLongitude, gps.GetLatitude(), gps.GetLongitude()))
               {
                  // no gps update since last call -> add distance traveled since last gps update to the filter
                  // this is the case in 95% of all cases, because this method is called 20 times a seconds, but gps update is only 1Hz
                  distance = distanceFiltered + speed * deltaTime;
               }
               else
               {
                  // given location is a new one
                  // insert a reference from the gps-driver to the filter (correct our speed/time drift)
                  distance = distanceGps;
               }
            }
            else
            {
               // we are in mode "brake" -> just integrate speed by time -> add distance traveled to the filter
               // inserting a reference from the gps-driver would be more incorrect
               distance = distanceFiltered + speed * deltaTime;
            }

            // "height since start"
            height = gps.GetHeight(startAltitude, gps.GetAltitude());
            // add to the height-filter
            heightFiltered = heightFilter.Get(height);
            // overwrite to the raw buffer
            heightGps = height;
         }

         if (distance > 0.01) // prevent update of filter if no distance was calculated
         {
            // apply moving average filter
            distanceFiltered = distanceFilter.Get(distance);
         }

         lastTime      = time;
         lastLatitude  = gps.GetLatitude();
         lastLongitude = gps.GetLongitude();
      }

      bool IsLocationEqual(double lat1, double lon1, double lat2, double lon2)
      {
         const double DELTA_MAX = 0.00001;

         if (Math.Abs(lat1 - lat2) < DELTA_MAX)
         {
            if (Math.Abs(lon1 - lon2) < DELTA_MAX)
            {
               return true;
            }
         }

         return false;
      }
   }
}
