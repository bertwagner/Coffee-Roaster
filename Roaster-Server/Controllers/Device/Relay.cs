using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Roast_Server.Controllers.Device
{
    class Relay
    {
        private GpioPinValue value = GpioPinValue.Low;
        private int relayPin;
        private GpioPin pin;
        private bool isOn = false;

        public Relay(int relayPin)
        {
            this.relayPin = relayPin;
            InitGPIO();
        }

        private void InitGPIO()
        {
            pin = GpioController.GetDefault().OpenPin(relayPin);
            pin.Write(value);
            pin.SetDriveMode(GpioPinDriveMode.Output);
        }

        public void On()
        {
            value = GpioPinValue.High;
            pin.Write(value);
            isOn = true;
        }

        public void Off()
        {
            value = GpioPinValue.Low;
            pin.Write(value);
            isOn = false;
        }

        public bool IsOn()
        {
            return isOn;
        }
    }
}
