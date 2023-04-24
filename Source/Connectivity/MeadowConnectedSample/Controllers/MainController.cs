using Meadow.Devices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MeadowConnectedSample.Controller
{
    public class MainController
    {
        private TimeSpan UPDATE_INTERVAL = TimeSpan.FromMinutes(1);
        private CancellationTokenSource? cancellationTokenSource;

        bool IsSampling = false;
        IProjectLabHardware hardware;

        public MainController(IProjectLabHardware hardware)
        {
            this.hardware = hardware;

            //LastLocationInfo = new LocationModel();
            //LastAtmosphericConditions = new AtmosphericModel();

            //GnssController.GnssPositionInfoUpdated += GnssPositionInfoUpdated;
        }

        async Task StartUpdating(TimeSpan updateInterval, CancellationToken cancellationToken)
        {
            Console.WriteLine("ClimateMonitorAgent.StartUpdating()");

            if (IsSampling)
                return;
            IsSampling = true;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }


            }
        }
    }
}