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
    class FanController
    {
        Relay fan = new Relay(16);

        [UriFormat("/Fan/ChangeState/{state}")]
        public IGetResponse ChangeState(string state)
        {
            if (state == "On")
            {
                fan.On();
            } 
            else
            {
                fan.Off();
            }

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                "Fan " + state
                );
        }

        [UriFormat("/Fan/GetState")]
        public IGetResponse GetState()
        {
            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                fan.GetState()
                );
        }
    }
}
