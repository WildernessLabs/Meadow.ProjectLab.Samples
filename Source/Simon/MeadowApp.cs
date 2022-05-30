using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Audio;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Simon
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2, MeadowApp>
    {
        int ANIMATION_DELAY = 50;
        const int DOT_UP = 0;
        const int DOT_DOWN = 1;
        const int DOT_LEFT = 2;
        const int DOT_RIGHT = 3;

        bool isAnimating;
        float[] notes;

        SimonGame game;

        PushButton[] buttons;
        MicroGraphics graphics;
        PiezoSpeaker speaker;

        public MeadowApp()
        {
            Initialize();

            DrawAllDots(true);
            game.OnGameStateChanged += OnGameStateChanged;
            game.Reset();
        }

        void Initialize()
        {
            speaker = new PiezoSpeaker(Device, Device.Pins.D11);

            var onboardLed = new RgbPwmLed(
                device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            notes = new float[] { 261.63f, 329.63f, 392, 523.25f };

            game = new SimonGame();

            var config = new SpiClockConfiguration(
                speed: new Frequency(48000, Frequency.UnitType.Kilohertz),
                mode: SpiClockConfiguration.Mode.Mode3);
            var spiBus = Device.CreateSpiBus(
                clock: Device.Pins.SCK,
                copi: Device.Pins.MOSI,
                cipo: Device.Pins.MISO,
                config: config);
            var display = new St7789
            (
                device: Device,
                spiBus: spiBus,
                chipSelectPin: Device.Pins.A03,
                dcPin: Device.Pins.A04,
                resetPin: Device.Pins.A05,
                width: 240,
                height: 240,
                displayColorMode: ColorType.Format16bppRgb565
            );
            graphics = new MicroGraphics(display)
            {
                Rotation = RotationType._90Degrees,
                Stroke = 5
            };
            graphics.Clear();

            buttons = new PushButton[4];
            buttons[0] = new PushButton(Device, Device.Pins.D15, ResistorMode.InternalPullDown);
            buttons[0].Clicked += ButtonUpClicked;
            buttons[2] = new PushButton(Device, Device.Pins.D02, ResistorMode.InternalPullDown);
            buttons[2].Clicked += ButtonDownClicked;
            buttons[3] = new PushButton(Device, Device.Pins.D10, ResistorMode.InternalPullDown);
            buttons[3].Clicked += ButtonLeftClicked;
            buttons[1] = new PushButton(Device, Device.Pins.D05, ResistorMode.InternalPullDown);
            buttons[1].Clicked += ButtonRightClicked;

            onboardLed.SetColor(Color.Green);
        }

        async void ButtonUpClicked(object sender, EventArgs e)
        {
            await OnButton(DOT_UP);
        }

        async void ButtonDownClicked(object sender, EventArgs e)
        {
            await OnButton(DOT_DOWN);
        }

        async void ButtonLeftClicked(object sender, EventArgs e)
        {
            await OnButton(DOT_LEFT);
        }

        async void ButtonRightClicked(object sender, EventArgs e)
        {
            await OnButton(DOT_RIGHT);
        }

        async Task OnButton(int buttonIndex)
        {
            if (isAnimating == false)
            {
                await DrawDotFilled(buttonIndex);
                game.EnterStep(buttonIndex);
            }
        }

        void OnGameStateChanged(object sender, SimonEventArgs e)
        {
            Task.Run(async () =>
            {
                switch (e.GameState)
                {
                    case GameState.Start:
                        break;
                    case GameState.NextLevel:
                        await ShowStartAnimation();
                        await ShowNextLevelAnimation(game.Level);
                        await ShowSequenceAnimation(game.Level);
                        break;
                    case GameState.GameOver:
                        await ShowGameOverAnimation();
                        game.Reset();
                        break;
                    case GameState.Win:
                        await ShowGameWonAnimation();
                        break;
                }
            });
        }

        async Task DrawDotFilled(int index, int duration = 400)
        {
            DrawDot(index, true);
            await speaker.PlayTone(notes[index], duration);
            DrawDot(index, false);
        }

        void DrawAllDots(bool isOn)
        {
            DrawDot(DOT_UP, isOn, false);
            DrawDot(DOT_DOWN, isOn, false);
            DrawDot(DOT_LEFT, isOn, false);
            DrawDot(DOT_RIGHT, isOn, false);
            graphics.Show();
        }

        async Task ShowStartAnimation()
        {
            if (isAnimating)
                return;
            isAnimating = true;

            DrawAllDots(false);
            await Task.Delay(ANIMATION_DELAY);
            DrawAllDots(true);
            await Task.Delay(ANIMATION_DELAY);
            DrawAllDots(false);
            await Task.Delay(ANIMATION_DELAY);
            DrawAllDots(true);
            await Task.Delay(ANIMATION_DELAY);
            DrawAllDots(false);

            isAnimating = false;
        }

        async Task ShowNextLevelAnimation(int level)
        {
            if (isAnimating)
                return;
            isAnimating = true;

            DrawAllDots(false);
            for (int i = 0; i < level; i++)
            {
                await Task.Delay(ANIMATION_DELAY);
                DrawAllDots(true);
                await Task.Delay(ANIMATION_DELAY * 3);
                DrawAllDots(false);
            }

            isAnimating = false;
        }

        async Task ShowSequenceAnimation(int level)
        {
            if (isAnimating)
                return;
            isAnimating = true;

            var steps = game.GetStepsForLevel();
            DrawAllDots(false);
            for (int i = 0; i < level; i++)
            {
                await Task.Delay(100);
                await DrawDotFilled(steps[i], 400);
            }

            isAnimating = false;
        }

        async Task ShowGameOverAnimation()
        {
            if (isAnimating)
                return;
            isAnimating = true;

            //await Task.Delay(750);
            await speaker.PlayTone(123.47f, 750);

            for (int i = 0; i < 20; i++)
            {
                DrawAllDots(false);
                await Task.Delay(50);
                DrawAllDots(true);
                await Task.Delay(50);
            }

            isAnimating = false;
        }

        async Task ShowGameWonAnimation()
        {
            await ShowStartAnimation();
            await ShowStartAnimation();
            await ShowStartAnimation();
            await ShowStartAnimation();
        }

        void DrawDot(int index, bool isFilled, bool update = true) 
        {
            switch (index) 
            {
                case DOT_UP:
                    graphics.DrawCircle(120, 55, 40, Color.Black, true);
                    graphics.DrawCircle(120, 55, 40, Color.Red, isFilled);
                    break;
                case DOT_DOWN:
                    graphics.DrawCircle(120, 185, 40, Color.Black, true);
                    graphics.DrawCircle(120, 185, 40, Color.Yellow, isFilled);
                    break;
                case DOT_LEFT:
                    graphics.DrawCircle(55, 120, 40, Color.Black, true);
                    graphics.DrawCircle(55, 120, 40, Color.Green, isFilled);
                    break;
                case DOT_RIGHT:
                    graphics.DrawCircle(185, 120, 40, Color.Black, true);
                    graphics.DrawCircle(185, 120, 40, Color.Blue, isFilled);
                    break;
            }

            if (update)
            {
                graphics.Show();
            }
        }
    }
}