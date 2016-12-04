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
    class RoasterApp
    {
        private FanApp fan;
        private HeaterApp heater;
        private TemperatureProbeApp temperatureProbe;
        private ProfileApp profile;

        private decimal holdFahrenheitTemperature { get; set; }
        private bool isHoldOn { get; set; }
        private bool runLoop { get; set; }

        public RoasterApp()
        {
            fan = new FanApp();
            heater = new HeaterApp();
            temperatureProbe = new TemperatureProbeApp(TemperatureScale.Fahrenheit);
            profile = new ProfileApp();

            isHoldOn = false;
            holdFahrenheitTemperature = 100;
            runLoop = true;

            // Start the roaster loop on a new thread
            Task t = Task.Factory.StartNew(() => { StartRoasterLoop(); });
        }

        public RoasterStatus GetRoasterStatus()
        {
            RoasterStatus status = new RoasterStatus();
            status.CurrentHoldTemperature = GetHoldTemperture();
            status.CurrentTemperature = temperatureProbe.CurrentTemperature();
            status.IsHoldOn = IsHoldOn();
            status.IsFanOn = fan.IsOn();
            status.IsHeaterOn = heater.IsOn();
            status.IsProfileRunning = profile.IsRunning();
            status.ProfileElapsedTime = profile.ElapsedRunTime().ToString();
            status.RoastProfile = profile.GetCurrentProfile();

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
                if (!IsHoldOn() && temperatureProbe.CurrentTemperature() > 100)
                {
                    heater.Off();
                    fan.On();
                }

                // If hold is off turn the heater and fan off.  
                // We choose 90 because once the fan turns off, some residual heat will increase the temperture
                // and we don't want the fan getting toggled on/off rapidly if the temperature sits around 100*F
                if (!IsHoldOn() && temperatureProbe.CurrentTemperature() < 85)
                {
                    heater.Off();
                    fan.Off();
                }

                // If the Hold button is on, alternate between turning the heater on/off until the Hold temperature is reached
                if (IsHoldOn())
                {
                    MaintainTemperature(GetHoldTemperture());
                }

                // If the roast profile is on, follow the temperature and time settings indicated by the profile
                if (profile.IsRunning())
                {
                    var currentRoastProfile = profile.GetCurrentProfile();

                    for (int i = 0; i < currentRoastProfile.Count; i++)
                    {
                        while (profile.ElapsedRunTime().Seconds < currentRoastProfile[i].TimeInSeconds)
                        {
                            MaintainTemperature(Convert.ToDecimal(currentRoastProfile[i].HoldTemperature));

                            //Exit loop if profile has been turned off
                            if (!profile.IsRunning())
                                return;
                        }
                        //Exit loop if profile has been turned off
                        if (!profile.IsRunning())
                            return;
                    }

                    profile.Stop();
                    heater.Off();
                }


                // Log the time/temperature data
                Debug.WriteLine("Temperature: {0}, Hold: {1}, Hold Temperature: {2}", temperatureProbe.CurrentTemperature(), (IsHoldOn()) ? "On" : "Off", holdFahrenheitTemperature);

                // Slow down our loop so it only runs every 100ms
                Task.Delay(100).Wait();
            }
        }

        private void MaintainTemperature(decimal temperature)
        {
            fan.On();
            if (temperatureProbe.CurrentTemperature() <= temperature)
            {
                heater.On();
            }
            else
            {
                heater.Off();
            }

        }

        public void Shutdown()
        {
            runLoop = false;
            ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, new TimeSpan(0));
        }

        public void HoldOn(decimal temperature)
        {
            holdFahrenheitTemperature = temperature;
            isHoldOn = true;
        }

        public void HoldOff()
        {
            isHoldOn = false;
            heater.Off();
        }

        public bool IsHoldOn()
        {
            return isHoldOn;
        }

        public decimal GetHoldTemperture()
        {
            return holdFahrenheitTemperature;
        }

        public void SetRoastProfile(List<RoastProfile> newProfile)
        {
            profile.SetCurrentProfile(newProfile);
        }

        public void RunProfile()
        {
            profile.Run();
        }

        public void StopProfile()
        {
            profile.Stop();
        }
    }
}
