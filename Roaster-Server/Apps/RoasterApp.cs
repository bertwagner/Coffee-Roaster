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

        /*TODO: 
         * Load Defulat profile, save profiles, load profiles
         * Create run data controller: first crack, second crack, cold start temperature (winter vs summer), bean weight
         * 
        */

        private StateApp state;
        private FanApp fan;
        private HeaterApp heater;
        private TemperatureProbeApp temperatureProbe;
        private ProfileApp profile;
        private HoldApp hold;

        private bool runLoop { get; set; }

        public RoasterApp()
        {
            state = new StateApp();
            fan = new FanApp();
            heater = new HeaterApp();
            temperatureProbe = new TemperatureProbeApp(TemperatureScale.Fahrenheit);
            profile = new ProfileApp();
            hold = new HoldApp();

            runLoop = true;

            // Start the roaster loop on a new thread
            Task t = Task.Factory.StartNew(() => { StartRoasterLoop(); });
        }

        public RoasterStatus GetRoasterStatus()
        {
            RoasterStatus status = new RoasterStatus();
            status.CurrentHoldTemperature = hold.GetTemperature();
            status.CurrentTemperature = temperatureProbe.CurrentTemperature();
            status.IsHoldOn = hold.IsOn();
            status.IsFanOn = fan.IsOn();
            status.IsHeaterOn = heater.IsOn();
            status.IsProfileRunning = profile.IsRunning();
            status.ProfileElapsedTime = profile.ElapsedRunTime().TotalSeconds.ToString();
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
                if (!hold.IsOn() && temperatureProbe.CurrentTemperature() > 100)
                {
                    heater.Off();
                    fan.On();
                }

                // If hold is off turn the heater and fan off.  
                // We choose 90 because once the fan turns off, some residual heat will increase the temperture
                // and we don't want the fan getting toggled on/off rapidly if the temperature sits around 100*F
                if (!hold.IsOn() && temperatureProbe.CurrentTemperature() < 90)
                {
                    heater.Off();
                    fan.Off();
                }

                // If the Hold button is on, alternate between turning the heater on/off until the Hold temperature is reached
                if (hold.IsOn())
                {
                    MaintainTemperature(hold.GetTemperature());
                }

                // If the roast profile is on, follow the temperature and time settings indicated by the profile
                if (profile.IsRunning())
                {
                    Debug.WriteLine("Profile starting to run");
                    var currentRoastProfile = profile.GetCurrentProfile();

                    for (int i = 0; i < currentRoastProfile.Count; i++)
                    {
                        while (profile.ElapsedRunTime().TotalSeconds < currentRoastProfile[i].TimeInSeconds)
                        {
                            MaintainTemperature(Convert.ToDecimal(currentRoastProfile[i].HoldTemperature));

                            //Exit loop if profile has been turned off
                            if (!profile.IsRunning())
                                break;
                        }
                        //Exit loop if profile has been turned off
                        if (!profile.IsRunning())
                            break;
                    }

                    profile.Stop();
                    heater.Off();
                    Debug.WriteLine("Profile completed running");
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

            Debug.WriteLine("Maintaining: {0}, Actual: {2}, Heater: {1}", temperature, heater.IsOn(), temperatureProbe.CurrentTemperature());

        }

        public void Shutdown()
        {
            runLoop = false;
            ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, new TimeSpan(0));
        }
    }
}
