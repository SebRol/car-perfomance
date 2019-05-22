using System;
using System.Collections.Generic;
using MathNet.Filtering;

namespace perf
{
   class Sample
   {
      public float x;
      public float y;
      public float z;
      public float s;

      public Sample(float x, float y, float z, float s)
      {
         this.x = x;
         this.y = y;
         this.z = z;
         this.s = s;
      }
   }

   public class AccelerometerRecorder
   {
      const int    SIZE_MAX                 = 128;  // 2 seconds of data at 50Hz = 100 samples

      const double FILTER_SAMPLE_RATE       = 50;   // X-distance between two samples // see DeviceAcceleromterAndroid -> SensorDelay.Game
      const double FILTER_CUT_OFF_RATE      = 2;    // Y-distance between two samples when the filter shall kick in
      const int    FILTER_ORDER             = 8;    // filer smoothness

      const double POSITION_DEVIATION_LIMIT = 1;    // limit [m/s^2] until two axis are considered equal

      bool                        isQueueMode;
      bool                        isPeakAStored;
      IReadOnlyCollection<Sample> samples;
      double[]                    samplesX;
      double[]                    samplesY;
      double[]                    samplesZ;
      double[]                    samplesS;

      public AccelerometerRecorder(bool isQueue)
      {
         Debug.LogToFileMethod();
         isQueueMode = isQueue;
         Reset();
      }

      public void Reset()
      {
         if (isQueueMode) samples = new Queue<Sample>(SIZE_MAX);
         else             samples = new List <Sample>(SIZE_MAX);

         samplesX = null;
         samplesY = null;
         samplesZ = null;
         samplesS = null;

         isPeakAStored = false;
      }

		public void Add(float x, float y, float z, float s)
		{
         if (isQueueMode)
         {
            Queue<Sample> q = (Queue<Sample>)samples;

            if (q.Count >= SIZE_MAX) q.Dequeue(); // limit size of queue

            q.Enqueue(new Sample(x, y, z, s));
         }
         else
         {
            List<Sample> l = (List<Sample>)samples;

            if (l.Count >= SIZE_MAX) return; // limit size of list

            l.Add(new Sample(x, y, z, s));
         }
		}
      
      public bool IsPositionEqual(DataItemVehicle itemVehicle)
      {
         bool   result  = true;
         Sample average = GetAverage(); // get average of each axis for all samples

         if (Math.Abs((itemVehicle.AxisOffsetX - average.x)) > POSITION_DEVIATION_LIMIT) result = false;
         if (Math.Abs((itemVehicle.AxisOffsetY - average.y)) > POSITION_DEVIATION_LIMIT) result = false;
         if (Math.Abs((itemVehicle.AxisOffsetZ - average.z)) > POSITION_DEVIATION_LIMIT) result = false;

         return result;
      }

      public void StoreAxisOffset(DataItemVehicle itemVehicle)
      {
         Sample average = GetAverage(); // get average of each axis for all samples

         // store to given profile
         itemVehicle.AxisOffsetX = (float)average.x;
         itemVehicle.AxisOffsetY = (float)average.y;
         itemVehicle.AxisOffsetZ = (float)average.z;

         Debug.LogToFile("axis offset stored to profile [x,y,z]: " + 
                         average.x.ToString("0.00") + ", " + 
                         average.y.ToString("0.00") + ", " + 
                         average.z.ToString("0.00"));
      }

      public void StorePeakAcceleration(DataItemVehicle itemVehicle, DataItemRun itemRun)
      {
         if (samples.Count == 0) 
         {
            Debug.LogToFileMethod("peak-a: no samlpes");
            return;
         }

         if (isPeakAStored == true)
         {
            // refuse to process peakA again
            Debug.LogToFileEventText("peak-a: already found");
            return;
         }

         double peakA     = 0;
         double speed     = 0;
         double magnitude = 0;
         int    count     = samples.Count;

         // user calibrated profile before run -> get accelerometer offsets
         // to eleminate earth gravitation and slope of the device in the car mount
         double offsetX = itemVehicle.AxisOffsetX;
         double offsetY = itemVehicle.AxisOffsetY;
         double offsetZ = itemVehicle.AxisOffsetZ;

         // from collection to array with offset compensation ("normalize" to axis origin)
         ToArray(offsetX, offsetY, offsetZ);

         // apply low pass filter to each "normalized" axis
         OnlineFilter filter = OnlineFilter.CreateLowpass(ImpulseResponse.Finite, 
                                                          FILTER_SAMPLE_RATE, 
                                                          FILTER_CUT_OFF_RATE, 
                                                          FILTER_ORDER);

         double[] filteredX = filter.ProcessSamples(samplesX);
         filter.Reset();

         double[] filteredY = filter.ProcessSamples(samplesY);
         filter.Reset();

         double[] filteredZ = filter.ProcessSamples(samplesZ);
         filter.Reset();
       
         // search max value of magnitude of all filtered axes
         for (int i = 0; i < count; i++)
         {
            // magnitude of all components
            magnitude = Math.Sqrt(filteredX[i] * filteredX[i] + 
                                  filteredY[i] * filteredY[i] + 
                                  filteredZ[i] * filteredZ[i]);

            // store if new max
            if (magnitude > peakA)
            {
               peakA = magnitude;
               speed = samplesS[i];
            }
         }

         // store to given run
         itemRun.PeakAcceleration = (float)peakA;

         Debug.LogToFileEventText("found peak-a [m/s^2]: " + peakA.ToString("0.00") + 
                                  " at speed [m/s]: "      + speed.ToString("0.00"));

         if ((count >= SIZE_MAX) && (peakA > 0)) 
         {
            isPeakAStored = true;  
         }
      }

      Sample GetAverage()
      {
         Sample result = new Sample(0, 0, 0, 0);
         double offsetX = 0;
         double offsetY = 0;
         double offsetZ = 0;

         // avoid division by zero
         if (samples.Count == 0) 
         {
            Debug.LogToFileMethod("axis offset: no samlpes");
            return result;
         }

         // sum up all samples for each axis
         foreach (Sample s in samples)
         {
            offsetX += s.x;
            offsetY += s.y;
            offsetZ += s.z;
         }

         // get average
         result.x = (float) (offsetX / samples.Count);
         result.y = (float) (offsetY / samples.Count);
         result.z = (float) (offsetZ / samples.Count);

         return result;
      }

      void ToArray(double offsetX, double offsetY, double offsetZ)
      {
         int      count = samples.Count;
         Sample[] samplesArray;
         Sample   s;

         // below for() needs an array, because for() does access samples by uniform index operator
         if (isQueueMode) samplesArray = ((Queue<Sample>)samples).ToArray();
         else             samplesArray = ((List<Sample>) samples).ToArray();

         // create class members
         samplesX = new double[count];
         samplesY = new double[count];
         samplesZ = new double[count];
         samplesS = new double[count];

         // convert from sample-objects to distinct array for each axis (with offset compensation)
         for (int i = 0; i < count; i++)
         {
            s = samplesArray[i];

            samplesX[i] = s.x - offsetX;
            samplesY[i] = s.y - offsetY;
            samplesZ[i] = s.z - offsetZ;
            samplesS[i] = s.s;
         }
      }
   }
}
