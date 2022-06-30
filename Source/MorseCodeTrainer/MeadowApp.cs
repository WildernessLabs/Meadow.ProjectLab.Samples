using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Audio;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Hardware;
using MorseCodeTrainer.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace MorseCodeTrainer
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>, IApp
    {
        Dictionary<string, string> morseCode;

        PushButton button;
        PiezoSpeaker piezo;

        Timer timer;
        Stopwatch stopWatch;
        string answer;
        string question;

        async Task IApp.Initialize()
        {
            piezo = new PiezoSpeaker(Device, Device.Pins.D11);

            var onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            DisplayController.Instance.Initialize();

            button = new PushButton(Device, Device.Pins.D10, ResistorMode.InternalPullDown);
            button.PressStarted += ButtonPressStarted;
            button.PressEnded += ButtonPressEnded;

            stopWatch = new Stopwatch();

            timer = new Timer(2000);
            timer.Elapsed += TimerElapsed;

            LoadMorseCode();

            ShowLetterQuestion();

            onboardLed.SetColor(Color.Green);
        }

        void LoadMorseCode()
        {
            morseCode = new Dictionary<string, string>();
            morseCode.Add("O-", "A");
            morseCode.Add("-OOO", "B");
            morseCode.Add("-O-O", "C");
            morseCode.Add("-OO", "D");
            morseCode.Add("O", "E");
            morseCode.Add("OO-O", "F");
            morseCode.Add("--O", "G");
            morseCode.Add("OOOO", "H");
            morseCode.Add("OO", "I");
            morseCode.Add("O---", "J");
            morseCode.Add("-O-", "K");
            morseCode.Add("O-OO", "L");
            morseCode.Add("--", "M");
            morseCode.Add("-O", "N");
            morseCode.Add("---", "O");
            morseCode.Add("O--O", "P");
            morseCode.Add("--O-", "Q");
            morseCode.Add("O-O", "R");
            morseCode.Add("OOO", "S");
            morseCode.Add("-", "T");
            morseCode.Add("OO-", "U");
            morseCode.Add("OOO-", "V");
            morseCode.Add("O--", "W");
            morseCode.Add("-OO-", "X");
            morseCode.Add("-O--", "Y");
            morseCode.Add("--OO", "Z");
            morseCode.Add("-----", "0");
            morseCode.Add("O----", "1");
            morseCode.Add("OO---", "2");
            morseCode.Add("OOO--", "3");
            morseCode.Add("OOOO-", "4");
            morseCode.Add("OOOOO", "5");
            morseCode.Add("-OOOO", "6");
            morseCode.Add("--OOO", "7");
            morseCode.Add("---OO", "8");
            morseCode.Add("----O", "9");
        }

        async void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!morseCode.ContainsKey(answer)) { return; }

            timer.Stop();

            bool isCorrect = morseCode[answer] == question;

            DisplayController.Instance.DrawCorrectIncorrectMessage(question, answer, isCorrect);

            await Task.Delay(2000);

            if (isCorrect)
            {
                ShowLetterQuestion();
            }
            else
            {
                answer = string.Empty;
                DisplayController.Instance.ShowLetterQuestion(question);
            }

            timer.Start();
        }

        async void ButtonPressStarted(object sender, EventArgs e)
        {
            await piezo.PlayTone(new Meadow.Units.Frequency(440));
            stopWatch.Reset();
            stopWatch.Start();
            timer.Stop();
        }

        void ButtonPressEnded(object sender, EventArgs e)
        {
            piezo.StopTone();
            stopWatch.Stop();

            if (stopWatch.ElapsedMilliseconds < 200)
            {
                answer += "O";
            }
            else
            {
                answer += "-";
            }

            DisplayController.Instance.UpdateAnswer(answer, Color.White);
            timer.Start();
        }

        void ShowLetterQuestion()
        {
            answer = string.Empty;
            question = morseCode.ElementAt(new Random().Next(0, morseCode.Count)).Value;
            DisplayController.Instance.ShowLetterQuestion(question);
        }

        public override async Task Run()
        {
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
    }
}