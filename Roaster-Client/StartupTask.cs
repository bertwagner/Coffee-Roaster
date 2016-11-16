using Windows.ApplicationModel.Background;
using Restup.Webserver.Http;
using Restup.Webserver.File;

namespace Roaster_Client
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral deferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();

            // Start the HTTP server
            var configuration = new HttpServerConfiguration()
                .ListenOnPort(8800)
                .RegisterRoute(new StaticFileRouteHandler(@"Web"))
                .EnableCors();

            var httpServer = new HttpServer(configuration);
            await httpServer.StartServerAsync();
        }
    }
}
