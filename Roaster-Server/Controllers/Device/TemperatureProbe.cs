using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;

namespace Roast_Server.Controllers.Device
{
    class TemperatureProbe
    {

        public SpiDevice thermocouple;
        public byte[] thermocoupleData = new byte[4];

        public TemperatureProbe()
        {
            InitSPI();
        }

        private void InitSPI()
        {
            try
            {
                var settings = new SpiConnectionSettings(0);
                settings.ClockFrequency = 5000000;
                settings.Mode = SpiMode.Mode0;

                string spiAqs = SpiDevice.GetDeviceSelector("SPI0");
                var deviceInfo = DeviceInformation.FindAllAsync(spiAqs).AsTask();
                Task.WaitAll(deviceInfo);
                var device = SpiDevice.FromIdAsync(deviceInfo.Result.First().Id, settings).AsTask();
                Task.WaitAll(device);
                thermocouple = device.Result;
            }

            catch (Exception ex)
            {
                throw new Exception("SPI Initialization Failed", ex);
            }
        }

        private void GetData()
        {
            try
            {
                byte[] readBuffer = new byte[4];
                thermocouple.Read(readBuffer);

                //Data from the sensor is big endian.  We need to convert to little endian.
                Array.Reverse(readBuffer);

                thermocoupleData = readBuffer;
            }
            catch (Exception ex)
            {
                throw new Exception("Read error", ex);
            }
        }

        public decimal GetProbeTemperatureDataCelsius()
        {
            GetData();
            uint data = BitConverter.ToUInt32(thermocoupleData, 0);

            // Any of the last 3 bits are 1s - error with circuit
            if ((data & 0x7) != 0)
            {
                throw new Exception("Error with MAX31855 device");
            }

            // Negative value, drop the lower 18 bits and explicitly extend sign bits.
            if ((data & 0x80000000) != 0)
            {
                data = ((data >> 18) & 0x00003FFFF) | 0xFFFFC000;
            }
            // Positive value, just drop the lower 18 bits.
            else
            {
                data >>= 18;
            }

            decimal celsius = data;

            // LSB = 0.25 degrees C 
            celsius *= 0.25m;
            return celsius;
        }

        public decimal GetInternalTemperatureDataCelcius()
        {
            GetData();
            uint data = BitConverter.ToUInt32(thermocoupleData, 0);

            // Ignore bottom 4 digits
            data >>= 4;

            // pull the bottom 11 bits off 
            decimal celsius = data & 0x7FF;

            // check sign bit! 
            if ((data & 0x800) == 1)
            {
                // Convert to negative value by extending sign and casting to signed type. 
                celsius = 0xF800 | (data & 0x7FF);
            }

            //LSB = .0625 degrees C
            celsius *= .0625m;
            return celsius;
        }

        public decimal GetProbeTemperatureDataFahrenheit()
        {
            try
            {
                return ConvertCelsiusToFahrenheit(GetProbeTemperatureDataCelsius());
            }
            catch (Exception ex)
            {
                return -1.0m;
            }
        }

        public decimal GetInternalTemperatureDataFahrenheit()
        {
            return ConvertCelsiusToFahrenheit(GetInternalTemperatureDataCelcius());
        }

        private decimal ConvertCelsiusToFahrenheit(decimal c)
        {
            c *= 9.0m;
            c /= 5.0m;
            c += 32.0m;
            return c;
        }
    }
}
