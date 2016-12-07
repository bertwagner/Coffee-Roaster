using Roast_Server.Models;
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

        private List<RoastProfile> currentProfile;
        private bool isProfileRunning { get; set; }
        private Stopwatch stopWatch { get; set; }

        private ProfileApp()
        {
            currentProfile = new List<RoastProfile>
            {
                new RoastProfile { TimeInSeconds = 180, HoldTemperature = 300 },
                new RoastProfile { TimeInSeconds = 240, HoldTemperature = 333 },
                new RoastProfile { TimeInSeconds = 300, HoldTemperature = 366 },
                new RoastProfile { TimeInSeconds = 360, HoldTemperature = 400 },
                new RoastProfile { TimeInSeconds = 420, HoldTemperature = 415 },
                new RoastProfile { TimeInSeconds = 480, HoldTemperature = 430 },
                new RoastProfile { TimeInSeconds = 510, HoldTemperature = 445 }
            };
            isProfileRunning = false;
            stopWatch = new Stopwatch();
        }

        internal void SetCurrentProfile(List<RoastProfile> profile)
        {
            currentProfile = profile;
        }

        public List<RoastProfile> GetCurrentProfile()
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
