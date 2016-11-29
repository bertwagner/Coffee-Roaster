using Roast_Server.Controllers.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roaster_Server.Apps
{
    public enum TemperatureScale { Fahrenheit, Celcius };

    class TemperatureProbeApp
    {
        private TemperatureProbe probe = new TemperatureProbe();
        private TemperatureScale scale;
        private decimal previousFahrenheitTemperature;

        public TemperatureProbeApp(TemperatureScale temperatureScale)
        {
            scale = temperatureScale;
            previousFahrenheitTemperature = -999;
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
