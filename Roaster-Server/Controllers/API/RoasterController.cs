using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roast_Server.Controllers.Device;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Schemas;
using Restup.Webserver.Models.Contracts;
using Windows.System;
using Roaster_Server.Apps;
using System.Diagnostics;
using Roaster_Server.Models;

namespace Roast_Server.Controllers.API
{
    [RestController(InstanceCreationType.Singleton)]
    class RoasterController
    {
        RoasterApp roaster = new RoasterApp();

        [UriFormat("/Roaster/Shutdown")]
        public IGetResponse Shutdown()
        {
            //roaster.Shutdown();

            return new GetResponse(
                GetResponse.ResponseStatus.OK
                );
        }

        [UriFormat("/Roaster/HoldTemperature/On/{temperature}")]
        public IGetResponse HoldTemperatureOn(decimal temperature)
        {
            roaster.HoldOn(temperature);

            return new GetResponse(GetResponse.ResponseStatus.OK);
        }

        [UriFormat("/Roaster/HoldTemperature/Off")]
        public IGetResponse HoldTemperatureOff()
        {
            roaster.HoldOff();

            return new GetResponse(GetResponse.ResponseStatus.OK);
        }

        [UriFormat("/Roaster/Status/Get")]
        public IGetResponse GetRoasterStatus()
        {
            RoasterStatus status = roaster.GetRoasterStatus();

            return new GetResponse(GetResponse.ResponseStatus.OK, status);
        }
    }
}
