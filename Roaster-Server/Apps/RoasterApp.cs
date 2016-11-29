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

        // NEXT TIME REFACTOR THESE ROASTER FUNCTIONS
        private decimal holdFahrenheitTemperature { get; set; }
        private bool isHoldOn { get; set; }
        private bool runLoop { get; set; }
        private List<RoastProfile> roastProfile { get; set; }

        public RoasterApp()
        {
            fan = new FanApp();
            heater = new HeaterApp();
            temperatureProbe = new TemperatureProbeApp(TemperatureScale.Fahrenheit);

            isHoldOn = false;
            holdFahrenheitTemperature = 100;
            runLoop = true;

            roastProfile = new List<RoastProfile>
            {
                new RoastProfile { TimeInSeconds = 180, HoldTemperature = 300 },
                new RoastProfile { TimeInSeconds = 240, HoldTemperature = 333 },
                new RoastProfile { TimeInSeconds = 300, HoldTemperature = 366 },
                new RoastProfile { TimeInSeconds = 360, HoldTemperature = 400 },
                new RoastProfile { TimeInSeconds = 420, HoldTemperature = 415 },
                new RoastProfile { TimeInSeconds = 480, HoldTemperature = 430 },
                new RoastProfile { TimeInSeconds = 510, HoldTemperature = 445 }
            };

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
            status.RoastProfile = GetRoastProfile();

            return status;
        }

        public void StartRoasterLoop()
        {
            while (runLoop)
            {
                // The fan must always be on if the heater is on so components don't melt
                if (!fan.IsOn() && heater.IsOn())
                {
                    
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
                if (!IsHoldOn() && temperatureProbe.CurrentTemperature() < 90)
                {
                    heater.Off();
                    fan.Off();
                }

                // If the Hold button is on, alternate between turning the heater on/off until the Hold temperature is reached
                if (IsHoldOn())
                {
                    decimal currentTemperature = temperatureProbe.CurrentTemperature();
                    if (currentTemperature <= holdFahrenheitTemperature)
                    {
                        heater.On();
                    }
                    else
                    {
                        heater.Off();
                    }

                    Debug.WriteLine("Temperature: {0}, Hold Temperature: {1}", currentTemperature, holdFahrenheitTemperature);
                }

                // Log the time/temperature data
                Debug.WriteLine("Temperature: {0}, Hold: {1}, Hold Temperature: {2}", temperatureProbe.CurrentTemperature(), (IsHoldOn()) ? "On" : "Off", holdFahrenheitTemperature);

                // Slow down our loop so it only runs every 100ms
                Task.Delay(100).Wait();
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

        public List<RoastProfile> GetRoastProfile()
        {
            return roastProfile;
        }
    }
}
