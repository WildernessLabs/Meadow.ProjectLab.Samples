﻿
using Meadow;
using Meadow.Foundation.Grove.Servos;
using Meadow.Units;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MeadowAzureServo.Views
{
    public class ServoController
    {
        private static readonly Lazy<ServoController> instance =
            new Lazy<ServoController>(() => new ServoController());
        public static ServoController Instance => instance.Value;

        private Servo servo;
        private Task animationTask = null;
        private CancellationTokenSource cancellationTokenSource = null;

        protected int _rotationAngle;

        private ServoController()
        {
            Initialize();
        }

        private void Initialize()
        {
            servo = new Servo(MeadowApp.Device.CreatePwmPort(MeadowApp.Device.Pins.D12, new Frequency(100)));
        }

        public void RotateTo(Angle angle)
        {
            servo.RotateTo(angle);
        }

        public void StopSweep()
        {
            cancellationTokenSource?.Cancel();
        }

        public void StartSweep()
        {
            animationTask = new Task(async () =>
            {
                cancellationTokenSource = new CancellationTokenSource();
                await StartSweep(cancellationTokenSource.Token);
            });
            animationTask.Start();
        }
        protected async Task StartSweep(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested) { break; }

                while (_rotationAngle < 180)
                {
                    if (cancellationToken.IsCancellationRequested) { break; }

                    _rotationAngle++;
                    servo.RotateTo(new Angle(_rotationAngle, Angle.UnitType.Degrees));
                    await Task.Delay(50);
                }

                while (_rotationAngle > 0)
                {
                    if (cancellationToken.IsCancellationRequested) { break; }

                    _rotationAngle--;
                    servo.RotateTo(new Angle(_rotationAngle, Angle.UnitType.Degrees));
                    await Task.Delay(50);
                }
            }
        }
    }
}
