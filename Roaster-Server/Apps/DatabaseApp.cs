using Newtonsoft.Json;
using Roast_Server.Models;
using Roaster_Server.Models;
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

        public void SaveProfile(RoastProfile data)
        {
            Profile profile = new Models.Database.Profile
            {
                CreateDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                Name = data.Name,
                BeanGrams = data.BeanGrams,
                RoastSchedule = JsonConvert.SerializeObject(data.RoastSchedule)
            };
            var s = conn.Insert(profile);
        }

        public RoastProfile LoadProfile(int id)
        {
            Profile profile = conn.Table<Profile>().Where(x => x.Id == id).First();
            RoastProfile roastProfile = new RoastProfile
            {
                Name = profile.Name,
                BeanGrams = profile.BeanGrams,
                RoastSchedule = JsonConvert.DeserializeObject<List<RoastSchedule>>(profile.RoastSchedule)
            };
            
            return roastProfile;
        }

        public void DeleteProfile(int id)
        {
            var profile = conn.Table<Profile>().Where(x => x.Id == id).First();

            conn.Delete(profile);
        }

        public Dictionary<int,string> GetProfiles()
        {
            var profiles = conn.Table<Profile>().ToDictionary(x => x.Id, x => x.Name);
            return profiles;
        }
    }
}
