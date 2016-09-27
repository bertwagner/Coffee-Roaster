using Devkoes.Restup.WebServer.Attributes;
using Devkoes.Restup.WebServer.Models.Schemas;
using Devkoes.Restup.WebServer.Rest.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebRoasterAPI.Controllers.Device;
using Windows.System;

namespace WebRoasterAPI.Controllers.API
{
    [RestController(InstanceCreationType.Singleton)]
    class SystemController
    {
        
        [UriFormat("/System/Shutdown")]
        public IGetResponse Shutdown()
        {
            ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, new TimeSpan(0));
            return new GetResponse(
                GetResponse.ResponseStatus.OK
                );
        }
    }
}
