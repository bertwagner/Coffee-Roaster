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
