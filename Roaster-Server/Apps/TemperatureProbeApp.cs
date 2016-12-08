using Roast_Server.Controllers.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roaster_Server.Apps
{
    public enum TemperatureScale { Fahrenheit, Celcius };

    sealed class TemperatureProbeApp
    {
        private static readonly TemperatureProbeApp instance = new TemperatureProbeApp();
        public static TemperatureProbeApp Instance
        {
            get
            {
                return instance;
            }
        }

        private TemperatureScale scale;
        private decimal previousFahrenheitTemperature;
        private TemperatureProbe probe;

        private TemperatureProbeApp()
        {
            scale = TemperatureScale.Fahrenheit;
            previousFahrenheitTemperature = -999;
            probe = new TemperatureProbe();
        }

        public decimal CurrentTemperature()
        {
            decimal currentTemperature = (scale == TemperatureScale.Fahrenheit) ? probe.GetProbeTemperatureDataFahrenheit() : probe.GetProbeTemperatureDataCelsius();


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
    }
}
