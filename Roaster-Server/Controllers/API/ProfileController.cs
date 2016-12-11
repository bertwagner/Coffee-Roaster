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
using Roaster_Server.Models.Database;

namespace Roast_Server.Controllers.API
{
    [RestController(InstanceCreationType.Singleton)]
    class ProfileController
    {
        [UriFormat("/Profile/Run/{newProfile}")]
        public IGetResponse RunProfile(string newProfile)
        {
            RoastProfile roastProfile = JsonConvert.DeserializeObject<RoastProfile>(newProfile);
            ProfileApp.Instance.SetCurrentProfile(roastProfile);
            ProfileApp.Instance.Run();
            LogApp.Instance.StartLog();

            return new GetResponse(GetResponse.ResponseStatus.OK);
        }

        [UriFormat("/Profile/Stop")]
        public IGetResponse StopProfile()
        {
            LogApp.Instance.StopLog();
            ProfileApp.Instance.Stop();
            return new GetResponse(GetResponse.ResponseStatus.OK);
        }

        [UriFormat("/Profile/Save/{newProfile}")]
        public IGetResponse Save(string newProfile)
        {
            RoastProfile roastProfile = JsonConvert.DeserializeObject<RoastProfile>(newProfile);
            ProfileApp.Instance.SetCurrentProfile(roastProfile);
            RoastProfile profile = ProfileApp.Instance.GetCurrentProfile();
            DatabaseApp.Instance.SaveProfile(profile);
            return new GetResponse(GetResponse.ResponseStatus.OK);
        }

        [UriFormat("/Profile/Load/{id}")]
        public IGetResponse Load(int id)
        {
            RoastProfile profile = DatabaseApp.Instance.LoadProfile(id);
            return new GetResponse(GetResponse.ResponseStatus.OK, profile);
        }

        [UriFormat("Profiles")]
        public IGetResponse Profiles()
        {
            Dictionary<int, string> profiles = DatabaseApp.Instance.GetProfiles();
            return new GetResponse(GetResponse.ResponseStatus.OK, profiles);
        }
    }
}
