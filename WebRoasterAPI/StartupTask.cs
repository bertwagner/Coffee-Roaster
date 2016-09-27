using Devkoes.Restup.WebServer.File;
using Devkoes.Restup.WebServer.Http;
using Devkoes.Restup.WebServer.Rest;
using System;
using System.Diagnostics;
using WebRoasterAPI.Controllers;
using WebRoasterAPI.Controllers.API;
using WebRoasterAPI.Controllers.Device;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;

namespace WebRoasterAPI
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral deferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();

            

            var restRouteHandler = new RestRouteHandler();
            restRouteHandler.RegisterController<HeaterController>();
            restRouteHandler.RegisterController<FanController>();
            restRouteHandler.RegisterController<TemperatureController>();
            restRouteHandler.RegisterController<SystemController>();

            var httpServer = new HttpServer(8800);
            httpServer.RegisterRoute(new StaticFileRouteHandler(@"Web"));
            httpServer.RegisterRoute("api", restRouteHandler);
            await httpServer.StartServerAsync();

            
        }

        
    }
}