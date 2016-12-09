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
            List<RoastProfile> roastProfile = JsonConvert.DeserializeObject<List<RoastProfile>>(newProfile);
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

        [UriFormat("/Profile/Save")]
        public IGetResponse Save()
        {
            DatabaseApp.Instance.SaveData(new Profile());
            return new GetResponse(GetResponse.ResponseStatus.OK);
        }

        [UriFormat("/Profile/Read")]
        public IGetResponse Read()
        {
            string text = DatabaseApp.Instance.ReadData();
            return new GetResponse(GetResponse.ResponseStatus.OK, text);
        }
    }
}
