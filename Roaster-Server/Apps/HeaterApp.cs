using Roast_Server.Controllers.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roaster_Server.Apps
{
    class HeaterApp
    {
        private Relay heater = new Relay(5);

        public HeaterApp()
        {
        }

        public void On()
        {
            if (heater.IsOn() == false)
            {
                heater.On();
            }
        }

        public void Off()
        {
            if (heater.IsOn() == true)
            {
                heater.Off();
            }
        }

        public bool IsOn()
        {
            return heater.IsOn();
        }
    }
}
