using Roast_Server.Controllers.Device;
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
        private Relay fan = new Relay(16);
        private Relay heater = new Relay(5);
        private TemperatureProbe probe = new TemperatureProbe();

        private decimal previousFahrenheitTemperature { get; set; }
        private decimal holdFahrenheitTemperature { get; set; }
        private bool isHoldOn { get; set; }
        private bool runLoop { get; set; }

        public RoasterApp()
        {
            // Initialize values
            previousFahrenheitTemperature = -999;
            isHoldOn = false;
            holdFahrenheitTemperature = 0;
            runLoop = true;

            // Start the roaster loop on a new thread
            Task t = Task.Factory.StartNew(() => { StartRoasterLoop(); });
        }

        #region hardware methods
        public void Shutdown()
        {
            runLoop = false;
            ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, new TimeSpan(0)); 
        }

        public void FanOn()
        {
            if (fan.IsOn() == false)
            {
                fan.On();
            }
        }

        public void FanOff()
        {
            if (fan.IsOn() == true)
            {
                fan.Off();
            }
        }

        public void HeaterOn()
        {
            if (heater.IsOn() == false)
            {
                heater.On();
            }
        }

        public void HeaterOff()
        {
            if (fan.IsOn() == true)
            {
                heater.Off();
            }
        }

        public decimal GetFahrenheitTemperature()
        {
            decimal currentTemperature = probe.GetProbeTemperatureDataFahrenheit();

            for (var i = 0; i < 10; i++)
            {
                decimal temperatureDifference = Math.Abs(currentTemperature - previousFahrenheitTemperature);
                if (temperatureDifference < 100 || previousFahrenheitTemperature == -999) // -999 is the initialized temperature
                {
                    previousFahrenheitTemperature = currentTemperature;
                    break;
                }
            }

            return currentTemperature;
        }
        #endregion

        public void StartRoasterLoop()
        {
            while (runLoop)
            {
                // The fan must always be on if the heater is on so components don't melt
                if (!fan.IsOn() && heater.IsOn())
                {
                    FanOn();
                }

                // If the hold is off, make sure the fan stays on until the temperature goes below 100*F
                if (!IsHoldOn() && previousFahrenheitTemperature > 100)
                {
                    HeaterOff();
                    FanOn();
                }

                // If hold is off turn the heater and fan off.  
                // We choose 90 because once the fan turns off, some residual heat will increase the temperture
                // and we don't want the fan getting toggled on/off rapidly if the temperature sits around 100*F
                if (!IsHoldOn() && previousFahrenheitTemperature < 90)
                {
                    HeaterOff();
                    FanOff();
                }

                // If the Hold button is on, alternate between turning the heater on/off until the Hold temperature is reached
                if (IsHoldOn())
                {
                    decimal currentTemperature = GetFahrenheitTemperature();
                    if (currentTemperature <= holdFahrenheitTemperature)
                    {
                        HeaterOn();
                    }
                    else
                    {
                        HeaterOff();
                    }

                    Debug.WriteLine("Temperature: {0}, Hold Temperature: {1}", currentTemperature, holdFahrenheitTemperature);
                }

                // Log the time/temperature data
                Debug.WriteLine("Temperature: {0}, Hold: {1}, Hold Temperature: {2}", GetFahrenheitTemperature(), (IsHoldOn()) ? "On" : "Off", holdFahrenheitTemperature);

                // Slow down our loop so it only runs every 100ms
                Task.Delay(100).Wait();
            }
        }

        public void HoldOn(decimal temperature)
        {
            holdFahrenheitTemperature = temperature;
            isHoldOn = true;
        }

        public void HoldOff()
        {
            isHoldOn = false;
            HeaterOff();
        }

        public bool IsHoldOn()
        {
            return isHoldOn;
        }

        
    }
}
