using Roast_Server.Models;
using Roaster_Server.Models;
using Roaster_Server.Models.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roaster_Server.Apps
{
    sealed class ProfileApp
    {
        private static readonly ProfileApp instance = new ProfileApp();

        public static ProfileApp Instance
        {
            get
            {
                return instance;
            }
        }

        private RoastProfile currentProfile;
        private bool isProfileRunning { get; set; }
        private Stopwatch stopWatch { get; set; }

        private ProfileApp()
        {
            currentProfile = new RoastProfile
            {
                Name = "",
                BeanGrams = 25,
                RoastSchedule = new List<RoastSchedule>
                {
                    new RoastSchedule { TimeInSeconds = 180, HoldTemperature = 300 },
                    new RoastSchedule { TimeInSeconds = 240, HoldTemperature = 333 },
                    new RoastSchedule { TimeInSeconds = 300, HoldTemperature = 366 },
                    new RoastSchedule { TimeInSeconds = 360, HoldTemperature = 400 },
                    new RoastSchedule { TimeInSeconds = 420, HoldTemperature = 415 },
                    new RoastSchedule { TimeInSeconds = 480, HoldTemperature = 430 },
                    new RoastSchedule { TimeInSeconds = 510, HoldTemperature = 445 }
                }
            };
            isProfileRunning = false;
            stopWatch = new Stopwatch();
        }

        internal void SetCurrentProfile(RoastProfile profile)
        {
            currentProfile = profile;
        }

        public RoastProfile GetCurrentProfile()
        {
            return currentProfile;
        }

        public void Run()
        {
            isProfileRunning = true;
            stopWatch.Restart();
        }

        public void Stop()
        {
            isProfileRunning = false;
            stopWatch.Stop();
        }

        public bool IsRunning()
        {
            return isProfileRunning;
        }

        public TimeSpan ElapsedRunTime()
        {
            return stopWatch.Elapsed;
        }
    }
}
