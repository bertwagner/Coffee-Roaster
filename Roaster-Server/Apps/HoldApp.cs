using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roaster_Server.Apps
{
    class HoldApp
    {
        private decimal holdTemperature { get; set; }
        private bool isHoldOn { get; set; }

        public HoldApp()
        {
            isHoldOn = false;
            holdTemperature = 100;
        }

        public bool IsOn()
        {
            return isHoldOn;
        }

        public void On(decimal temperature)
        {
            holdTemperature = temperature;
            isHoldOn = true;
        }

        public void Off()
        {
            isHoldOn = false;
        }

        public decimal GetTemperature()
        {
            return holdTemperature;
        }
    }
}
