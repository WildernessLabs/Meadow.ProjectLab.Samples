using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Peripherals.Sensors.Buttons;

namespace GalleryViewer.Hardware
{
    internal interface IGalleryViewerHardware
    {
        public IGraphicsDisplay Display { get; }

        public IButton RightButton { get; }

        public IButton LeftButton { get; }

        public RgbPwmLed RgbPwmLed { get; }

        public void Initialize();
    }
}