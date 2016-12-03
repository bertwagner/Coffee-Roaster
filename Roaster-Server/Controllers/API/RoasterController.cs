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
using Roast_Server.Models;
using Newtonsoft;
using Newtonsoft.Json;

namespace Roast_Server.Controllers.API
{
    [RestController(InstanceCreationType.Singleton)]
    class RoasterController
    {
        RoasterApp roaster = new RoasterApp();

        [UriFormat("/Roaster/Shutdown")]
        public IGetResponse Shutdown()
        {
            roaster.Shutdown();

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

        [UriFormat("/Roaster/Profile/Run/{profile}")]
        public IGetResponse RunProfile(string profile)
        {
            List<RoastProfile> roastProfile = JsonConvert.DeserializeObject<List<RoastProfile>>(profile);
            roaster.SetRoastProfile(roastProfile);
            roaster.RunProfile();

            return new GetResponse(GetResponse.ResponseStatus.OK);
        }

        [UriFormat("/Roaster/Profile/Stop")]
        public IGetResponse StopProfile()
        {
            roaster.StopProfile();
            return new GetResponse(GetResponse.ResponseStatus.OK);
        }
    }
}
