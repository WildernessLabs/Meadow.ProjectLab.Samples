using Meadow.Foundation;
using Meadow.Foundation.Leds;
using System;

namespace MeadowConnectedSample.Controller
{
    public class LedController
    {
        private static readonly Lazy<LedController> instance =
            new Lazy<LedController>(() => new LedController());
        public static LedController Instance => instance.Value;

        RgbPwmLed rgbPwmLed;
        bool isAnimating;

        private LedController()
        {
            Initialize();
        }

        private void Initialize()
        {
            rgbPwmLed = new RgbPwmLed(
                MeadowApp.Device.Pins.OnboardLedRed,
                MeadowApp.Device.Pins.OnboardLedGreen,
                MeadowApp.Device.Pins.OnboardLedBlue
            );
        }

        public void SetColor(Color color)
        {
            rgbPwmLed.SetColor(color);
        }

        public void Toggle() 
        {
            if (rgbPwmLed.IsOn || isAnimating)
            {
                rgbPwmLed.Stop();
                rgbPwmLed.IsOn = false;
                isAnimating = false;
            }
            else
            {
                rgbPwmLed.Stop();
                rgbPwmLed.SetColor(GetRandomColor());
                rgbPwmLed.IsOn = true;
            }
        }

        public void StartBlink()
        {
            rgbPwmLed.Stop();
            rgbPwmLed.StartBlink(GetRandomColor());
            isAnimating = true;
        }

        public void StartPulse()
        {
            rgbPwmLed.Stop();
            rgbPwmLed.StartPulse(GetRandomColor());
            isAnimating = true;
        }

        protected Color GetRandomColor()
        {
            var random = new Random();
            return Color.FromHsba(random.NextDouble(), 1, 1);
        }
    }
}