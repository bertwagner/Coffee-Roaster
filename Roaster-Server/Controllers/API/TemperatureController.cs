using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roast_Server.Controllers.Device;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Schemas;
using Restup.Webserver.Models.Contracts;

namespace Roast_Server.Controllers.API
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
