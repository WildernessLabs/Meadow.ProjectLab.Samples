using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Peripherals.Leds;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace Simon
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        int ANIMATION_DELAY = 50;
        const int DOT_UP = 0;
        const int DOT_DOWN = 1;
        const int DOT_LEFT = 2;
        const int DOT_RIGHT = 3;

        bool isAnimating;
        Frequency[] notes;

        SimonGame game;

        RgbPwmLed onboardLed;
        MicroGraphics graphics;
        ProjectLab projLab;
        PushButton[] buttons;

        public override Task Initialize()
        {
            projLab = new ProjectLab();

            Resolver.Log.Info($"Running on ProjectLab Hardware {projLab.RevisionString}");

            onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue,
                CommonType.CommonAnode);
            onboardLed.SetColor(Color.Red);

            notes = new Frequency[]
            {
                new Frequency(261.63f),
                new Frequency(329.63f),
                new Frequency(392),
                new Frequency(523.25f)
            };

            game = new SimonGame();

            graphics = new MicroGraphics(projLab.Display)
            {
                Rotation = RotationType._90Degrees,
                Stroke = 5
            };
            graphics.Clear();

            buttons = new PushButton[4];
            buttons[0] = projLab.UpButton;
            buttons[0].Clicked += ButtonUpClicked;
            buttons[2] = projLab.DownButton;
            buttons[2].Clicked += ButtonDownClicked;
            buttons[3] = projLab.LeftButton;
            buttons[3].Clicked += ButtonLeftClicked;
            buttons[1] = projLab.RightButton;
            buttons[1].Clicked += ButtonRightClicked;

            onboardLed.SetColor(Color.Green);

            return base.Initialize();
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
            await projLab.Speaker.PlayTone(notes[index], TimeSpan.FromMilliseconds(duration));
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
            await projLab.Speaker.PlayTone(new Frequency(123.47f), TimeSpan.FromMilliseconds(750));

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

        public override Task Run()
        {
            DrawAllDots(true);
            game.OnGameStateChanged += OnGameStateChanged;
            game.Reset();

            return base.Run();
        }
    }
}