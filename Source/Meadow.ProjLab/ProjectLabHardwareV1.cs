using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Hardware;

namespace Meadow.Devices
{
    internal class ProjectLabHardwareV1 : IProjectLabHardware
    {
        private IF7FeatherMeadowDevice _device;
        private ISpiBus _spiBus;
        private St7789? _display;
        private PushButton _left;
        private PushButton _right;
        private PushButton _up;
        private PushButton _down;

        public ProjectLabHardwareV1(IF7FeatherMeadowDevice device, ISpiBus spiBus)
        {
            _device = device;
            _spiBus = spiBus;
        }

        public St7789 GetDisplay()
        {
            if (_display == null)
            {
                _display = new St7789(
                    device: _device,
                    spiBus: _spiBus,
                    chipSelectPin: _device.Pins.A03,
                    dcPin: _device.Pins.A04,
                    resetPin: _device.Pins.A05,
                    width: 240, height: 240,
                    colorMode: ColorType.Format16bppRgb565);
            }

            return _display;
        }

        public PushButton GetLeftButton()
        {
            if (_left == null)
            {
                _left = new PushButton(
                    Resolver.Device.CreateDigitalInputPort(
                        Resolver.Device.GetPin("D10"),
                        InterruptMode.EdgeBoth,
                        ResistorMode.InternalPullDown));
            }
            return _left;
        }

        public PushButton GetRightButton()
        {
            if (_right == null)
            {
                _right = new PushButton(
                    Resolver.Device.CreateDigitalInputPort(
                        Resolver.Device.GetPin("D05"),
                        InterruptMode.EdgeBoth,
                        ResistorMode.InternalPullDown));
            }
            return _right;
        }

        public PushButton GetUpButton()
        {
            if (_up == null)
            {
                _up = new PushButton(
                    Resolver.Device.CreateDigitalInputPort(
                        Resolver.Device.GetPin("D15"),
                        InterruptMode.EdgeBoth,
                        ResistorMode.InternalPullDown));
            }
            return _up;
        }

        public PushButton GetDownButton()
        {
            if (_down == null)
            {
                _down = new PushButton(
                    Resolver.Device.CreateDigitalInputPort(
                        Resolver.Device.GetPin("D02"),
                        InterruptMode.EdgeBoth,
                        ResistorMode.InternalPullDown));
            }
            return _down;
        }
    }
}

