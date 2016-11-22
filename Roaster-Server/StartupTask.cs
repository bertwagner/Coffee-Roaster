using Windows.ApplicationModel.Background;
using Restup.Webserver.Rest;
using Roast_Server.Controllers.API;
using Restup.Webserver.Http;

namespace Roaster_Server
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral deferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();

            // Start the API server
            var restRouteHandler = new RestRouteHandler();
            restRouteHandler.RegisterController<RoasterController>();

            var configuration = new HttpServerConfiguration()
              .ListenOnPort(9900)
              .RegisterRoute("api", restRouteHandler)
              .EnableCors();

            var httpServer = new HttpServer(configuration);
            await httpServer.StartServerAsync();
        }


    }
}
