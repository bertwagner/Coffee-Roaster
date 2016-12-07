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
        [UriFormat("/Roaster/Shutdown")]
        public IGetResponse Shutdown()
        {
            RoasterApp.Instance.Shutdown();

            return new GetResponse(
                GetResponse.ResponseStatus.OK
                );
        }

        [UriFormat("/Roaster/HoldTemperature/On/{temperature}")]
        public IGetResponse HoldTemperatureOn(decimal temperature)
        {
            HoldApp.Instance.On(temperature);

            return new GetResponse(GetResponse.ResponseStatus.OK);
        }

        [UriFormat("/Roaster/HoldTemperature/Off")]
        public IGetResponse HoldTemperatureOff()
        {
            HoldApp.Instance.Off();

            return new GetResponse(GetResponse.ResponseStatus.OK);
        }

        [UriFormat("/Roaster/Status/Get")]
        public IGetResponse GetRoasterStatus()
        {
            RoasterStatus status = RoasterApp.Instance.GetRoasterStatus();

            return new GetResponse(GetResponse.ResponseStatus.OK, status);
        }

        [UriFormat("/Roaster/Profile/Run/{newProfile}")]
        public IGetResponse RunProfile(string newProfile)
        {
            List<RoastProfile> roastProfile = JsonConvert.DeserializeObject<List<RoastProfile>>(newProfile);
            ProfileApp.Instance.SetCurrentProfile(roastProfile);
            ProfileApp.Instance.Run();

            return new GetResponse(GetResponse.ResponseStatus.OK);
        }

        [UriFormat("/Roaster/Profile/Stop")]
        public IGetResponse StopProfile()
        {
            ProfileApp.Instance.Stop();
            return new GetResponse(GetResponse.ResponseStatus.OK);
        }
    }
}
