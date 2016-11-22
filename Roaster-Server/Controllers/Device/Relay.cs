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
        private int RelayPin;
        private GpioPin pin;
        public bool IsOnState = false;

        public Relay(int relayPin)
        {
            RelayPin = relayPin;
            InitGPIO();
        }

        private void InitGPIO()
        {
            pin = GpioController.GetDefault().OpenPin(RelayPin);
            pin.Write(value);
            pin.SetDriveMode(GpioPinDriveMode.Output);
        }

        public void On()
        {
            value = GpioPinValue.High;
            pin.Write(value);
            IsOnState = true;
        }

        public void Off()
        {
            value = GpioPinValue.Low;
            pin.Write(value);
            IsOnState = false;
        }

        public bool IsOn()
        {
            return IsOnState;
        }
    }
}
