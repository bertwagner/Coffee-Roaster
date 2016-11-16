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
    class HeaterController
    {
        Relay heater = new Relay(5);

        [UriFormat("/Heater/ChangeState/{state}")]
        public IGetResponse ChangeState(string state)
        {
            if (state == "On")
            {
                heater.On();
            } 
            else
            {
                heater.Off();
            }

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                "Heater " + state
                );
        }
    }
}
