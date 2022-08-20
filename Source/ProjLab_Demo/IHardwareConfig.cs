using System;
using Meadow;
using Meadow.Foundation.Audio;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Sensors.Light;
using Meadow.Hardware;

namespace ProjLab_Demo
{
    public interface IHardwareConfig
    {
        IGraphicsDisplay Display { get; }

        RgbPwmLed OnboardLed { get; }

        ISpiBus SpiBus { get; }
        II2cBus I2cBus { get; }

        Bh1750? Bh1750 { get; }
        Bme680? Bme688 { get; }
        Bmi270? Bmi270 { get; }

        PushButton UpButton { get; }
        PushButton DownButton { get; }
        PushButton LeftButton { get; }
        PushButton RightButton { get; }

        PiezoSpeaker Speaker { get; }

        void Initialize(IF7FeatherMeadowDevice device);
    }
}

