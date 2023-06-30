﻿using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Camera;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThermalCamera
{
    // Change F7FeatherV2 to F7CoreComputeV2 for ProjectLab v3
    public class MeadowApp : App<F7FeatherV2>
    {
        RgbPwmLed onboardLed;
        IProjectLabHardware projectLab;
        Mlx90640 thermalCamera;
        MicroGraphics graphics;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            projectLab = ProjectLab.Create();
            Resolver.Log.Info($"Running on ProjectLab Hardware {projectLab.RevisionString}");

            onboardLed = projectLab.RgbLed;
            onboardLed.SetColor(Color.Red);

            graphics = new MicroGraphics(projectLab.Display);

            thermalCamera = new Mlx90640(projectLab.I2cBus);

            onboardLed.SetColor(Color.Green);

            return base.Initialize();
        }

        public override Task Run()
        {
            Console.WriteLine("Run...");

            float[] frame;

            //define a scaled pixel size that fills the 240x240 display
            int pixelW = 7;
            int pixelH = 10;

            Color pixelColor;

            while (true)
            {
                //get the raw thermal data from the camera
                frame = thermalCamera.ReadRawData();

                byte pixelValue;

                graphics.Clear();

                //loop over every data point from the thermal camera (resolution is 32x24)
                for (byte height = 0; height < 24; height++)
                {
                    for (byte width = 0; width < 32; width++)
                    {
                        pixelValue = (byte)((byte)frame[height * 32 + width] << 0);

                        //calculate a color based on the pixel value, using read and green to get shades of yellow
                        pixelColor = new Color(pixelValue, pixelValue, 0);
                        graphics.DrawRectangle(8 + width * pixelW, height * pixelH, pixelW, pixelH, pixelColor, true);
                    }
                }

                graphics.Show();

                Thread.Sleep(100);
            }
        }
    }
}