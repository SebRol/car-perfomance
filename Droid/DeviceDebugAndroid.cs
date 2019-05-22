using System;
using System.IO;
using System.Text;
using Android.Media;
using perf.Droid;
using Android.App;

// register this class at the Xamarin.Forms DependencyService -> can be used in shared code
[assembly: Xamarin.Forms.Dependency(typeof (DeviceDebugAndroid))]
namespace perf.Droid
{
   public class SingleMediaScanner : Java.Lang.Object, MediaScannerConnection.IMediaScannerConnectionClient
   {
      // let Android scan content to make folder visisble in other apps and easy file transfer to PC
      // Java.Lang.Object needed to auto-implement IObject and IDisposable

      MediaScannerConnection mediaScannerConnection;
      string fileName;

      public SingleMediaScanner(string absoluteFilePathAndName)
      {
         fileName = absoluteFilePathAndName;
         mediaScannerConnection = new MediaScannerConnection(Application.Context, this);
      }

      public void Connect()
      {
         mediaScannerConnection.Connect();
      }

      public void OnMediaScannerConnected() // IMediaScannerConnectionClient
      {
         mediaScannerConnection.ScanFile(fileName, null);
      }

      public void OnScanCompleted(string path, Android.Net.Uri uri) // IMediaScannerConnectionClient
      {
         mediaScannerConnection.Disconnect();
      }
   }

   public class DeviceDebugAndroid : IDeviceDebug
   {
      const string TAG = " *** ";
      const string DELIMITER = " // ";
      static char[] FILE_SEPARATORS = {'\\', '/'};

      static string GetFileNameWithoutExtension(string s)
      {
         string result = s;
         int i;

         i = result.LastIndexOf(".", StringComparison.Ordinal); // remove .cs
         if (i >= 0)
         {
            result = result.Remove(i);
         }

         i = result.LastIndexOf(".", StringComparison.Ordinal); // remove .xaml
         if (i >= 0)
         {
            result = result.Remove(i);
         }

         i = result.LastIndexOfAny(FILE_SEPARATORS); // remove c:/projects/bla/
         if (i >= 0)
         {
            result = result.Remove(0, i + 1);
         }

         return result;
      }

      static string GetMethodClassTimeString(
         string s,
         string methodName,
         string sourceFilePath)
      {
         string result = "";

         var file = GetFileNameWithoutExtension(sourceFilePath);
         var now = string.Format("{0:HH-mm-ss-fff}", DateTime.Now);

         if (s != null)
         {
            if (s.Length != 0)
            {
               result = s + DELIMITER;
            }
         }

         result += methodName + "()" + DELIMITER + file + DELIMITER + now;

         return result;
      }

      public void Print(string s, string methodName = "", string sourceFilePath = "")
      {
         Android.Util.Log.Debug(TAG, GetMethodClassTimeString(s, methodName, sourceFilePath));
      }

      public void LogToFileMethod(string s = "", string methodName = "", string sourceFilePath = "")
      {
         LogToFileStatic(GetMethodClassTimeString(s, methodName, sourceFilePath));
      }

      public void LogToFile(string s)
      {
         LogToFileStatic(s);
      }

      public void LogToFile(float time, float acceleration, float speedAccel, float speedGps, float distanceFiltered, float distanceGps, float heightFiltered, float heightGps)
      {
         LogToFileStatic(time, acceleration, speedAccel, speedGps, distanceFiltered, distanceGps, heightFiltered, heightGps);
      }

      public void LogToFileFlush()
      {
         LogToFileFlushStatic();
      }

      public void LogToFileEventText(string s)
      {
         LogToFileEventTextStatic(s);
      }

      public string GetLogFilePath()
      {
         return GetLogFilePathStatic();
      }

      /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      /// Below is App Logging
      /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      const  string FOLDER_NAME = "car-performance";
      static int    count;
      static string fileName;
      static string eventText;
      static int    storagePermissionState;
      static int    storageWriteFileState;

      static bool IsStoragePermissionGranted()
      {
         bool result = false;

         if (storagePermissionState == 1)
         {
            // permission was evaluated and granted OK
            result = true;
         }
         else if (storagePermissionState == 2)
         {
            // permission was evaluated and granted NOT OK
            result = false;
         }
         else
         {
            // permission was never evaluated
            if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(Application.Context, Android.Manifest.Permission.WriteExternalStorage) == Android.Content.PM.Permission.Granted)
            {
               // targetSdkVersion >= Android M, we can use Context.CheckSelfPermission()
               // targetSdkVersion <  Android M, we have to use PermissionChecker
               if (Android.Support.V4.Content.PermissionChecker.CheckSelfPermission(Application.Context, Android.Manifest.Permission.WriteExternalStorage) == Android.Support.V4.Content.PermissionChecker.PermissionGranted)
               {
                  // permission granted OK
                  storagePermissionState = 1;
                  result = true;
               }
               else
               {
                  // permission granted NOT OK
                  storagePermissionState = 2;
                  result = false;
               }
            }
            else
            {
               // permission granted NOT OK
               storagePermissionState = 2;
               result = false;
            }
         }

         return result;
      }

      static bool IsStorageWriteFileGranted()
      {
         bool result = false;

         if (storageWriteFileState == 1)
         {
            // write file was evaluated and granted OK
            result = true;
         }
         else if (storageWriteFileState == 2)
         {
            // write file was evaluated and granted NOT OK
            result = false;
         }
         else
         {
            // write file was never evaluated
            try
            {
               if (! Directory.Exists(fileName))
               {
                  // build filename path
                  fileName = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                  fileName = Path.Combine(fileName, FOLDER_NAME);
                  // create directory
                  Directory.CreateDirectory(fileName);
               }
               // create file
               string tempFileName = Path.Combine(fileName, "app-log.test");
               using (var streamWriter = new StreamWriter(tempFileName, true))
               {
                  // write file
                  streamWriter.WriteLine("write file test");
                  streamWriter.Flush();
               }
               // delete file
               File.Delete(tempFileName);
               // no exception until yet -> write file granted OK
               storageWriteFileState = 1;
               result = true;
            }
            catch (Exception)
            {
               // write file granted NOT OK
               storageWriteFileState = 2;
               result = false;
            }
         }

         return result;
      }

      static bool IsLoggingEnabled()
      {
         if (Config.Debug.LoggingEnabled == true)
         {
            if (IsStoragePermissionGranted() == true)
            {
               if (IsStorageWriteFileGranted() == true)
               {
                  return true;
               }
            }
         }

         return false;
      }

      static void CreateFileIfNeeded(bool writeHeader)
      {
         if (IsLoggingEnabled() == false) return;

         // new file on first start OR every 1000 events
         if (count == 0 || count >= 1000)
         {
            count = 0;

            // build filename path
            fileName = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            fileName = Path.Combine(fileName, FOLDER_NAME);

            // test if folder exists
            if (! Directory.Exists(fileName))
            {
               Directory.CreateDirectory(fileName);
               new SingleMediaScanner(fileName).Connect(); // make folder visible for other apps and pc file transfer
            }

            fileName = Path.Combine(fileName, string.Format("app-log-{0:yyyy-MM-dd-HH-mm-ss}.csv", DateTime.Now));

            if (writeHeader)
            {
               // write header to file
               WriteHeader();
            }

            new SingleMediaScanner(fileName).Connect(); // make file visible for other apps and pc file transfer
         }

         // events
         count++;
      }

      public static void WriteHeader()
      {
         if (IsLoggingEnabled() == false) return;

         using (var streamWriter = new StreamWriter(fileName, true))
         {
            streamWriter.WriteLine("Time[s];Acceleration[m/s^2];SpeedAccel[m/s];SpeedGps[m/s];DistanceFiltered[m];DistanceGps[m];HeightFiltered[m];HeightGps[m];Event[text]");
         }
      }

      public static void LogToFileMethodStatic(
         string s = "",
         [System.Runtime.CompilerServices.CallerMemberName] string methodName     = "",
         [System.Runtime.CompilerServices.CallerFilePath]   string sourceFilePath = "")
      {
         if (IsLoggingEnabled() == false) return;

         CreateFileIfNeeded(false);
         LogToFileStatic(GetMethodClassTimeString(s, methodName, sourceFilePath));
      }

      public static void LogToFileStatic(string s)
      {
         if (IsLoggingEnabled() == false) return;

         CreateFileIfNeeded(false);

         // write line to file
         using (var streamWriter = new StreamWriter(fileName, true))
         {
            streamWriter.WriteLine(s);
         }
      }

      public static void LogToFileStatic(float time, float acceleration, float speedAccel, float speedGps, float distanceFiltered, float distanceGps, float heightFiltered, float heightGps)
      {
         if (IsLoggingEnabled() == false) return;

         CreateFileIfNeeded(true);

         // build one line
         var data = new StringBuilder();
         data.Append(time.ToString("0.00"));
         data.Append(';');
         data.Append(acceleration.ToString("0.00"));
         data.Append(';');
         data.Append(speedAccel.ToString("0.00"));
         data.Append(';');
         data.Append(speedGps.ToString("0.00"));
         data.Append(';');
         data.Append(distanceFiltered.ToString("0.00"));
         data.Append(';');
         data.Append(distanceGps.ToString("0.00"));
         data.Append(';');
         data.Append(heightFiltered.ToString("0.00"));
         data.Append(';');
         data.Append(heightGps.ToString("0.00"));

         if (eventText != null)
         {
            if (eventText.Equals("run start"))
            {
               WriteHeader();
            }

            data.Append(';');
            data.Append(eventText);
            eventText = null;
         }

         // write line to file
         using (var streamWriter = new StreamWriter(fileName, true))
         {
            streamWriter.WriteLine(data);
         }
      }

      public static void LogToFileFlushStatic()
      {
         if (IsLoggingEnabled() == false) return;

         if (eventText != null)
         {
            using (var streamWriter = new StreamWriter(fileName, true))
            {
               streamWriter.WriteLine(eventText);
            }

            eventText = null;
         }
      }

      // set here, used (written to file) above
      public static void LogToFileEventTextStatic(string s)
      {
         if (IsLoggingEnabled() == false) return;

         if (eventText == null)
         {
            eventText = s; // a fresh text comes in
         }
         else
         {
            eventText += " // " + s; // apend another text to existing one
         }
      }

      public static string GetLogFilePathStatic()
      {
         if (Config.Debug.LoggingEnabled  == false) return "logging disabled by config";
         if (IsStoragePermissionGranted() == false) return "storage permission not granted";
         if (IsStorageWriteFileGranted()  == false) return "storage write file not granted";

         return fileName;
      }

      /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      /// Below is Sensor Raw Logging
      /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      static int    sensorCount;
      static string sensorFileName;
      static float  sensorGpsSpeed;

      public static void WriteHeaderSensor()
      {
         if (IsLoggingEnabled() == false) return;

         using (var streamWriter = new StreamWriter(sensorFileName, true))
         {
            streamWriter.WriteLine("Time[s];AccelX[m/s^2];AccelY[m/s^2];AccelZ[m/s^2];SpeedGps[m/s];PeakA[m/s^2]");
         }
      }

      static void CreateFileSensorIfNeeded(bool writeHeader)
      {
         if (IsLoggingEnabled() == false) return;

         // new file on first start OR every 10000 events
         if (sensorCount == 0 || sensorCount >= 10000)
         {
            sensorCount = 0;

            // build filename path
            sensorFileName = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            sensorFileName = Path.Combine(sensorFileName, FOLDER_NAME);

            // test if folder exists
            if (!Directory.Exists(sensorFileName))
            {
               Directory.CreateDirectory(sensorFileName);
               new SingleMediaScanner(fileName).Connect(); // make folder visible for other apps and pc file transfer
            }

            sensorFileName = Path.Combine(sensorFileName, string.Format("sensor-log-{0:yyyy-MM-dd-HH-mm-ss}.csv", DateTime.Now));

            if (writeHeader)
            {
               // write header to file
               WriteHeaderSensor();
            }

            new SingleMediaScanner(fileName).Connect(); // make file visible for other apps and pc file transfer
         }

         // events
         sensorCount++;
      }

      public static void LogToFileSensorStatic(float time, float xAxis, float yAxis, float zAxis, float peakA = 0)
      {
         if (IsLoggingEnabled() == false) return;

         CreateFileSensorIfNeeded(true);

         // build one line
         var data = new StringBuilder();
         data.Append(time.ToString("0.000"));
         data.Append(';');
         data.Append(xAxis);
         data.Append(';');
         data.Append(yAxis);
         data.Append(';');
         data.Append(zAxis);
         data.Append(';');
         data.Append(sensorGpsSpeed);
         data.Append(';');
         data.Append(peakA);
         data.Append(';');

         // write line to file
         using (var streamWriter = new StreamWriter(sensorFileName, true))
         {
            streamWriter.WriteLine(data);
         }
      }

      // set here, used (written to file) above
      public static void LogToFileSensorGpsSpeedStatic(float gpsSpeed)
      {
         if (IsLoggingEnabled() == false) return;

         sensorGpsSpeed = gpsSpeed;
      }
   }
}
