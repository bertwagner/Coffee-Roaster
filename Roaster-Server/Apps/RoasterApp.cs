using Roast_Server.Controllers.Device;
using Roast_Server.Models;
using Roaster_Server.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace Roaster_Server.Apps
{
    sealed class RoasterApp
    {
        private static readonly RoasterApp instance = new RoasterApp();

        public static RoasterApp Instance
        {
            get
            {
                return instance;
            }
        }

        private FanApp fan;
        private HeaterApp heater;

        private RoasterApp()
        {
            fan = new FanApp();
            heater = new HeaterApp();

            runLoop = true;

            // Start the roaster loop on a new thread
            Task t = Task.Factory.StartNew(() => { StartRoasterLoop(); });
        }

        private bool runLoop { get; set; }

        public RoasterStatus GetRoasterStatus()
        {
            RoasterStatus status = new RoasterStatus();
            status.CurrentHoldTemperature = HoldApp.Instance.GetTemperature();
            status.CurrentTemperature = TemperatureProbeApp.Instance.CurrentTemperature();
            status.IsHoldOn = HoldApp.Instance.IsOn();
            status.IsFanOn = fan.IsOn();
            status.IsHeaterOn = heater.IsOn();
            status.IsProfileRunning = ProfileApp.Instance.IsRunning();
            status.ProfileElapsedTime = ProfileApp.Instance.ElapsedRunTime().TotalSeconds.ToString();
            status.RoastSchedule = ProfileApp.Instance.GetCurrentProfile().RoastSchedule;

            return status;
        }

        public void StartRoasterLoop()
        {
            while (runLoop)
            {
                // The fan must always be on if the heater is on so components don't melt
                if (!fan.IsOn() && heater.IsOn())
                {
                    fan.On();
                }

                // If the hold is off, make sure the fan stays on until the temperature goes below 100*F
                if (!HoldApp.Instance.IsOn() && TemperatureProbeApp.Instance.CurrentTemperature() > 100)
                {
                    heater.Off();
                    fan.On();
                }

                // If hold is off turn the heater and fan off.  
                // We choose 90 because once the fan turns off, some residual heat will increase the temperture
                // and we don't want the fan getting toggled on/off rapidly if the temperature sits around 100*F
                if (!HoldApp.Instance.IsOn() && TemperatureProbeApp.Instance.CurrentTemperature() < 90)
                {
                    heater.Off();
                    fan.Off();
                }

                // If the Hold button is on, alternate between turning the heater on/off until the Hold temperature is reached
                if (HoldApp.Instance.IsOn())
                {
                    MaintainTemperature(HoldApp.Instance.GetTemperature());
                }

                // If the roast profile is on, follow the temperature and time settings indicated by the profile
                if (ProfileApp.Instance.IsRunning())
                {
                    Debug.WriteLine("Profile starting to run");
                    var currentRoastProfile = ProfileApp.Instance.GetCurrentProfile();

                    for (int i = 0; i < currentRoastProfile.RoastSchedule.Count; i++)
                    {
                        while (ProfileApp.Instance.ElapsedRunTime().TotalSeconds < currentRoastProfile.RoastSchedule[i].TimeInSeconds)
                        {
                            MaintainTemperature(Convert.ToDecimal(currentRoastProfile.RoastSchedule[i].HoldTemperature));

                            //Exit loop if profile has been turned off
                            if (!ProfileApp.Instance.IsRunning())
                                break;
                        }
                        //Exit loop if profile has been turned off
                        if (!ProfileApp.Instance.IsRunning())
                            break;
                    }

                    ProfileApp.Instance.Stop();
                    heater.Off();
                    Debug.WriteLine("Profile completed running");
                }


                // Log the time/temperature data
                Debug.WriteLine("Temperature: {0}, Hold: {1}, Hold Temperature: {2}", TemperatureProbeApp.Instance.CurrentTemperature(), (HoldApp.Instance.IsOn()) ? "On" : "Off", HoldApp.Instance.GetTemperature());

                // Slow down our loop so it only runs every 100ms
                Task.Delay(100).Wait();
            }
        }

        private void MaintainTemperature(decimal temperature)
        {
            fan.On();
            if (TemperatureProbeApp.Instance.CurrentTemperature() <= temperature)
            {
                heater.On();
            }
            else
            {
                heater.Off();
            }

            Debug.WriteLine("Maintaining: {0}, Actual: {2}, Heater: {1}", temperature, heater.IsOn(), TemperatureProbeApp.Instance.CurrentTemperature());

        }

        public void Shutdown()
        {
            runLoop = false;
            ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, new TimeSpan(0));
        }
    }
}
