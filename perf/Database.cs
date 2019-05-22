using System;
using System.IO;
using System.Collections.Generic;
using SQLite;

namespace perf
{
   public class Database : IDisposable
   {
      readonly SQLiteConnection database;
      static readonly object    locker = new object();
      static Database           instance;

      string DatabasePath 
      {
         get 
         {            
            const string sqliteFilename = "sqlite.db3";

            #if __IOS__
            string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
            var path = Path.Combine(libraryPath, sqliteFilename);
            #else
            #if __ANDROID__
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, sqliteFilename);
            #else
            // WinPhone
            var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sqliteFilename);;
            #endif
            #endif
            return path;
         }
      }

      private Database()
      {
         // if the database doesn't exist, it will create the database and all the tables
         database = new SQLiteConnection(DatabasePath);
         // create the tables
         database.CreateTable<DataItemMeta>();
         database.CreateTable<DataItemVehicle>();
         database.CreateTable<DataItemRun>();

         Debug.LogToFileMethod("database path: " + DatabasePath);
      }

      public static Database GetInstance()
      {
         if (instance == null)
         {
            instance = new Database();
         }

         return instance;
      }

      public void Dispose() // IDisposable
      {
         database.Close();
      }

      public bool IsVehicleEmpty()
      {
         lock (locker) 
         {
            return database.Table<DataItemVehicle>().Count() == 0;
         }
      }

      public bool IsVehicleExisting(int id)
      {
         bool result = false;

         lock (locker) 
         {
            List<DataItemRun> runs = database.Query<DataItemRun>("SELECT EXISTS(SELECT 1 FROM [DataItemVehicle] WHERE id = ? LIMIT 1);", id);
            if (runs != null)
            {
               if (runs.Count >= 1)
               {
                  result = true;
               }
            }
         }

         return result;
      }

      public IEnumerable<DataItemVehicle> GetVehicles()
      {
         lock (locker) 
         {
            return database.Table<DataItemVehicle>();
         }
      }

      public DataItemVehicle GetVehicle(int id)
      {
         if (IsVehicleEmpty()      == true)  return DataItem.CreateDefaultVehicle();
         if (IsVehicleExisting(id) == false) return DataItem.CreateDefaultVehicle();

         lock (locker) 
         {
            return database.Get<DataItemVehicle>(id);
         }
      }

      public DataItemVehicle GetFirstVehicle()
      {
         lock (locker) 
         {
            return database.Table<DataItemVehicle>().First();
         }
      }

      public int SaveVehicle(DataItemVehicle item)
      {
         int result = 0;

         lock (locker) 
         {
            if (item.Id != 0) 
            {
               Debug.LogToFileMethod("item update: " + item.Model);
               database.Update(item);
               result = item.Id;
            }
            else 
            {
               Debug.LogToFileMethod("item insert: " + item.Model);
               result = database.Insert(item);
            }
         }

         return result;
      }

      public int DeleteVehicle(int id)
      {
         lock (locker) 
         {
            Debug.LogToFileMethod("item: " + id);

#if false
            // also delete all runs associated with this vehicle
            database.Query<DataItemRun>("DELETE FROM [DataItemRun] WHERE [VehicleId] = ?;", id);
#endif

            // delete vehicle
            return database.Delete<DataItemVehicle>(id);
         }
      }
      
      public int SaveRun(DataItemRun item)
      {
         int result = 0;

         lock (locker) 
         {
            result = database.Insert(item);
            Debug.LogToFileEventText("run inserted to database // id: " + item.Id + " // date: " + item.Date);
         }

         return result;
      }
      
      public int UpdateRun(DataItemRun item)
      {
         int result = 0;

         lock (locker) 
         {
            result = database.Update(item);
            Debug.LogToFileEventText("run updated to database // id: " + item.Id + " // date: " + item.Date);
         }

         return result;
      }

      public int DeleteRun(DataItemRun item)
      {
         int result = 0;

         lock (locker)
         {
            result = database.Delete(item);
            Debug.LogToFileEventText("run deleted from database // id: " + item.Id + " // date: " + item.Date);
         }

         return result;
      }

      public DataItemRun GetRun(int id)
      {
         lock (locker) 
         {
            return database.Get<DataItemRun>(id);
         }
      }

      public DataItemRun GetRunLast()
      {
         DataItemRun result = DataItem.CreateDefaultRun();

         lock (locker) 
         {
            if (IsRunEmpty() == false)
            {
               List<DataItemRun> runs = database.Query<DataItemRun>("SELECT * FROM [DataItemRun] ORDER BY [Id] DESC LIMIT 1;");
               if (runs != null)
               {
                  result = runs[0];
               }
            }
         }

         return result;
      }
      
      public DataItemRun GetRunLast(string mode)
      {
         DataItemRun result = DataItem.CreateDefaultRun();

         lock (locker) 
         {
            if (IsRunEmpty() == false)
            {
               List<DataItemRun> runs = database.Query<DataItemRun>("SELECT * FROM [DataItemRun] WHERE [Mode] = ? ORDER BY [Id] DESC LIMIT 1;", mode);
               if (runs != null)
               {
                  if (runs.Count > 0)
                  {
                     result = runs[0];
                  }
               }
            }
         }

         return result;
      }
      
      public DataItemRun GetRunPrevious(string mode, int current)
      {
         DataItemRun result = DataItem.CreateDefaultRun();

         lock (locker) 
         {
            if (IsRunEmpty() == false)
            {
               List<DataItemRun> runs = database.Query<DataItemRun>("SELECT * FROM [DataItemRun] WHERE [Mode] = ? AND [Id] < ? ORDER BY [Id] DESC LIMIT 1;", mode, current);
               if (runs != null)
               {
                  if (runs.Count > 0)
                  {
                     result = runs[0];
                  }
               }
            }
         }

         return result;
      }

      public DataItemRun GetRunNext(string mode, int current)
      {
         DataItemRun result = DataItem.CreateDefaultRun();

         lock (locker) 
         {
            if (IsRunEmpty() == false)
            {
               List<DataItemRun> runs = database.Query<DataItemRun>("SELECT * FROM [DataItemRun] WHERE [Mode] = ? AND [Id] > ? ORDER BY [Id] ASC LIMIT 1;", mode, current);
               if (runs != null)
               {
                  if (runs.Count > 0)
                  {
                     result = runs[0];
                  }
               }
            }
         }

         return result;
      }

      public bool IsRunEmpty()
      {
         lock (locker) 
         {
            return database.Table<DataItemRun>().Count() == 0;
         }
      }

      public void SaveActiveProfile(DataItemVehicle item)
      {
         lock (locker) 
         {
            Debug.LogToFileMethod("item: " + item.Model);

            if (IsMetaEmpty())
            {
               // empty database -> create and store default meta item
               SaveMeta(DataItem.CreateDefaultMeta());
            }

            // the meta table always has one entry only -> get first
            DataItemMeta meta = GetMeta();

            // update active profile
            meta.ActiveProfile = item.Id;
            UpdateMeta(meta);
         }
      }

      public DataItemVehicle GetActiveProfile()
      {
         DataItemVehicle result;

         if (IsMetaEmpty())
         {
            if (!IsVehicleEmpty())
            {
               // upgrade from old database
               result = GetFirstVehicle();
            }
            else
            {
               // empty database -> create and store default item
               int id = SaveVehicle(DataItem.CreateDefaultVehicle());
               result = GetVehicle(id);
            }

            // create meta table
            SaveActiveProfile(result);
         }
         else
         {
            // the meta table has only one entry at any time -> get first
            DataItemMeta meta = GetMeta();
            result = GetVehicle(meta.ActiveProfile);
         }

         return result;
      }

      public DataItemMeta GetMeta()
      {
         lock (locker) 
         {
            return database.Table<DataItemMeta>().First();
         }
      }

      public int SaveMeta(DataItemMeta item)
      {
         int result = 0;

         if (IsMetaEmpty()) // the meta table shall have only one entry at all time
         {
            lock (locker)
            {
               Debug.LogToFileMethod("item: " + item.Id);
               result = database.Insert(item);
            }
         }

         return result;
      }

      public int UpdateMeta(DataItemMeta item)
      {
         int result = 0;

         lock (locker) 
         {
            Debug.LogToFileMethod("item: " + item.Id);
            result = database.Update(item);
         }

         return result;
      }

      public bool IsMetaEmpty()
      {
         lock (locker) 
         {
            return database.Table<DataItemMeta>().Count() == 0;
         }
      }
   }
}
