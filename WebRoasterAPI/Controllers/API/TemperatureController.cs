using Devkoes.Restup.WebServer.Attributes;
using Devkoes.Restup.WebServer.Models.Schemas;
using Devkoes.Restup.WebServer.Rest.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebRoasterAPI.Controllers.Device;

namespace WebRoasterAPI.Controllers.API
{
    [RestController(InstanceCreationType.Singleton)]
    class TemperatureController
    {
        TemperatureProbe probe = new TemperatureProbe();

        [UriFormat("/Temperature/GetTemperature")]
        public IGetResponse GetTemperature()
        {
            decimal temp = probe.GetProbeTemperatureDataFahrenheit();

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                temp
                );
        }
    }
}
