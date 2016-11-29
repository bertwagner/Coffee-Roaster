using Roast_Server.Controllers.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roaster_Server.Apps
{
    class FanApp
    {
        private Relay fan = new Relay(16);

        public FanApp()
        {
        }

        public void On()
        {
            if (fan.IsOn() == false)
            {
                fan.On();
            }
        }

        public void Off()
        {
            if (fan.IsOn() == true)
            {
                fan.Off();
            }
        }

        public bool IsOn()
        {
            return fan.IsOn();
        }
    }
}
