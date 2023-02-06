﻿using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Units;
using SimpleJpegDecoder;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MagicEightMeadow
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        IPixelBuffer questionBuffer;
        IProjectLabHardware projectLab;
        RgbPwmLed onboardLed;
        MicroGraphics graphics;
        bool isAnswering = false;

        string GetQuestionFilename => "MagicEightMeadow.m8b_question.jpg";

        public override Task Initialize()
        {
            onboardLed = new RgbPwmLed(
                Device,
                Device.Pins.OnboardLedRed,
                Device.Pins.OnboardLedGreen,
                Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            projectLab = ProjectLab.Create();

            graphics = new MicroGraphics(projectLab.Display);
            graphics.Rotation = RotationType._90Degrees;

            questionBuffer = LoadJpeg(LoadResource(GetQuestionFilename));

            var consumer = Bmi270.CreateObserver(
                handler: result => MotionSensorHandler(result),
                filter: result => MotionSensorFilter(result));
            projectLab.MotionSensor.Subscribe(consumer);

            onboardLed.SetColor(Color.Green);

            return base.Initialize();
        }

        private async void MotionSensorHandler(IChangeResult<(Acceleration3D? a3D, AngularVelocity3D? v3D, Temperature? t)> e)
        {
            if (isAnswering)
                return;
            isAnswering = true;

            onboardLed.SetColor(Color.Orange);

            DisplayAnswer();

            await Task.Delay(TimeSpan.FromSeconds(5));

            DisplayQuestion();

            onboardLed.SetColor(Color.Green);

            isAnswering = false;
        }

        private bool MotionSensorFilter(IChangeResult<(Acceleration3D? a3D, AngularVelocity3D? v3D, Temperature? t)> e)
        {
            return e.New.v3D.Value.Y.DegreesPerSecond > 0.75;
        }

        void DisplayQuestion()
        {
            graphics.DrawBuffer(0, 0, questionBuffer);
            graphics.Show();
        }

        void DisplayAnswer()
        {
            var rand = new Random();

            var buffer = LoadJpeg(LoadResource(GetAnswerFilename(rand.Next(1, 21))));

            graphics.DrawBuffer(0, 0, buffer);
            graphics.Show();
        }

        string GetAnswerFilename(int answerNumber) => $"MagicEightMeadow.m8b_{answerNumber.ToString("00")}.jpg";

        byte[] LoadResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(fileName))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        IPixelBuffer LoadJpeg(byte[] jpgData)
        {
            var decoder = new JpegDecoder();
            var jpg = decoder.DecodeJpeg(jpgData);

            return new BufferRgb888(decoder.Width, decoder.Height, jpg);
        }

        public override Task Run()
        {
            Console.WriteLine("Run...");

            DisplayQuestion();
            projectLab.MotionSensor.StartUpdating(TimeSpan.FromSeconds(1));

            return base.Run();
        }
    }
}