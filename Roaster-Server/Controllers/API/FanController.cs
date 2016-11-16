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
