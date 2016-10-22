using Devkoes.Restup.WebServer.Attributes;
using Devkoes.Restup.WebServer.Models.Schemas;
using Devkoes.Restup.WebServer.Rest.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebRoasterAPI.Controllers.Device;
using WebRoasterAPI.Models;

namespace WebRoasterAPI.Controllers.API
{
    [RestController(InstanceCreationType.Singleton)]
    class ProfileController
    {
        bool running = false;
        CancellationTokenSource cts;
        TemperatureController temperature = new TemperatureController();
        FanController fan = new FanController();
        HeaterController heater = new HeaterController();

        [UriFormat("/Profile/Start/{profileId}")]
        public IGetResponse Start(int profileId)
        {
            running = true;
            cts = new CancellationTokenSource();

            RunProfile(profileId, cts.Token);

            
            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                "Running profile #" + profileId
                );
        }

        [UriFormat("/Profile/Stop")]
        public IGetResponse Stop()
        {

            // Cancel running our profile
            cts.Cancel();

            Debug.WriteLine("roast profile cancelled");

            //Make sure fan stays on until heater is cooled down
            CoolDown();

            // finally turn the fan off
            fan.ChangeState("Off");


            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                "Stopping profile"
                );
        }

        async Task RunProfile(int profileId, CancellationToken ct)
        {
            

            //Lookup the profile based on Id. Hardcoding for now.
            List<RoastProfile> profile = new List<RoastProfile>();
            if (profileId == 1)
            {
                /*profile.Add(new RoastProfile { TimeInSeconds = 180, HoldTemperature = 300 });
                profile.Add(new RoastProfile { TimeInSeconds = 240, HoldTemperature = 333 });
                profile.Add(new RoastProfile { TimeInSeconds = 300, HoldTemperature = 366 });
                profile.Add(new RoastProfile { TimeInSeconds = 360, HoldTemperature = 400 });
                profile.Add(new RoastProfile { TimeInSeconds = 420, HoldTemperature = 415 });
                profile.Add(new RoastProfile { TimeInSeconds = 480, HoldTemperature = 430 });
                profile.Add(new RoastProfile { TimeInSeconds = 510, HoldTemperature = 445 });*/
                profile.Add(new RoastProfile { TimeInSeconds = 15, HoldTemperature = 125 });
                profile.Add(new RoastProfile { TimeInSeconds = 15, HoldTemperature = 150 });
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            int currentProfileStep = 0;
            try
            {
                while (running)
                {
                    ct.ThrowIfCancellationRequested();
                    // Make sure fan is always running 
                    if ((bool)fan.GetState().ContentData == false)
                    {
                        fan.ChangeState("On");
                    }


                    float currentTemperature = Convert.ToSingle(temperature.GetTemperature().ContentData);

                    if (sw.ElapsedMilliseconds / 1000.0 < profile[currentProfileStep].TimeInSeconds)
                    {
                        if (currentTemperature < profile[currentProfileStep].HoldTemperature)
                        {
                            heater.ChangeState("On");
                        }
                        else
                        {
                            heater.ChangeState("Off");
                        }
                    }
                    else
                    {
                        if (currentProfileStep == profile.Count - 1)
                        {
                            running = false;
                        }
                        else
                        {
                            currentProfileStep++;
                            sw.Restart();
                        }

                    }
                    Debug.WriteLine("Current temp: " + currentTemperature + ", current step: " + currentProfileStep + ", time: " + sw.ElapsedMilliseconds / 1000.0);

                    Task.Delay(100).Wait();
                }
            }
            finally
            {
                sw.Stop();



                // finally turn the fan off
                fan.ChangeState("Off");

                Debug.WriteLine("profile run complete. cancelling...");

                Stop();
            }
        }

        public void CoolDown()
        {
            //Enter cool down
            bool coolingDown = true;
            int below100Count = 0;
            heater.ChangeState("Off");
            while (coolingDown)
            {
                // Make sure fan is always running 
                if ((bool)fan.GetState().ContentData == false)
                {
                    fan.ChangeState("On");
                }
                

                float currentTemperature = Convert.ToSingle(temperature.GetTemperature().ContentData);

                // wait to get 20 consectuvie under 100 temp readings
                if (currentTemperature < 100)
                {
                    below100Count++;

                    if (below100Count == 20)
                    {
                        coolingDown = false;
                    }
                }
                else
                {
                    // reset if temperature rises above 100
                    below100Count = 0;
                }

                Debug.WriteLine("Current temp: " + currentTemperature + ", current step: cool down");
                Task.Delay(100).Wait();
            }
        }
    }
}
