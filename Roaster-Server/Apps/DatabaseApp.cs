using Roast_Server.Models;
using Roaster_Server.Models.Database;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace Roaster_Server.Apps
{
    sealed class DatabaseApp
    {
        private static readonly DatabaseApp instance = new DatabaseApp();
        public static DatabaseApp Instance
        {
            get
            {
                return instance;
            }
        }

        private string dbPath;
        private SQLiteConnection conn;

        private DatabaseApp()
        {
            dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Roaster.sqlite");

            if (!File.Exists(dbPath))
            {
                string assetsPath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets\\Roaster.sqlite");
                string targetPath = dbPath;
                File.Copy(assetsPath, targetPath);
            }
            conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), dbPath);
        }

        public void SaveData(Profile data)
        {
            var s = conn.Insert(new Profile()
            {
                CreateDate = DateTime.Now,
                Name = "Profile #1",
                RoastProfile = "test profile data"
            });
        }

        public string ReadData()
        {
            var query = conn.Table<Profile>();

            string text = "";
            foreach (var message in query)
            {
                text = text + " " + message.RoastProfile;
            }

            return text;
        }
    }
}
