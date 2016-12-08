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
    class LogController
    {
        [UriFormat("/Log/GetCurrentLog")]
        public IGetResponse GetCurrentLog()
        {
            return new GetResponse(GetResponse.ResponseStatus.OK, LogApp.Instance.GetCurrentLog());
        }

        // Get log data since last request

        [UriFormat("/Log/SetFirstCrack")]
        public IGetResponse SetFirstCrack()
        {
            LogApp.Instance.SetFirstCrack();
            return new GetResponse(GetResponse.ResponseStatus.OK);
        }

        [UriFormat("/Log/SetSecondCrack")]
        public IGetResponse SetSecondCrack()
        {
            LogApp.Instance.SetSecondCrack();
            return new GetResponse(GetResponse.ResponseStatus.OK);
        }

        // Save log with already saved profile to database
    }
}
