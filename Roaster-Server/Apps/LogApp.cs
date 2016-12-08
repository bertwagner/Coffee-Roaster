using Roast_Server.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Roaster_Server.Apps
{
    sealed class LogApp
    {
        private static readonly LogApp instance = new LogApp();
        public static LogApp Instance
        {
            get
            {
                return instance;
            }
        }
        private RoastLog currentLog;
        Stopwatch stopWatch;
        private bool isLogging;
        private object thisLock = new object();

        private LogApp()
        {
            currentLog = new RoastLog();
            stopWatch = new Stopwatch();
            isLogging = false;
        }

        public void StartLog()
        {

            currentLog = new RoastLog();
            stopWatch.Restart();
            lock(thisLock)
            {
                isLogging = true;
            }

            Task t = Task.Factory.StartNew(() => { LogData(); });
        }

        private void LogData()
        {
            while(isLogging)
            {
                lock (thisLock) 
                { 
                    currentLog.TimeTemperatureEntries.Add(new TimeTemperature
                    {
                        Seconds = stopWatch.Elapsed.TotalSeconds,
                        Temperature = TemperatureProbeApp.Instance.CurrentTemperature()
                    });
                }
                Task.Delay(1000).Wait();
            }

        }

        public void StopLog()
        {
            lock(thisLock)
            {
                isLogging = false;
            }
            stopWatch.Stop();
        }

        public RoastLog GetCurrentLog()
        {
            lock(thisLock)
            {
                return currentLog;
            }
        }

        public void SetFirstCrack()
        {
            lock (thisLock)
            {
                currentLog.FirstCrackSeconds = stopWatch.Elapsed.TotalSeconds;
            }
        }

        public void SetSecondCrack()
        {
            lock (thisLock)
            {
                currentLog.SecondCrackSeconds = stopWatch.Elapsed.TotalSeconds;
            }
        }
    }
}
