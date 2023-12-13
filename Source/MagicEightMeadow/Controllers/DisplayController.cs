using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using System;

namespace MagicEightMeadow.Controllers
{
    internal class DisplayController
    {
        private Random random;

        private Image simonQuestion;

        private DisplayScreen displayScreen;

        private Picture simonImage;

        Color backgroundColor = Color.FromHex("#00000C");

        public DisplayController(IGraphicsDisplay display)
        {
            random = new Random();

            displayScreen = new DisplayScreen(display, RotationType._270Degrees)
            {
                BackgroundColor = backgroundColor
            };

            simonQuestion = Image.LoadFromResource("MagicEightMeadow.Resources.m8b_question.bmp");
            simonImage = new Picture((displayScreen.Width - displayScreen.Width) / 2, 0, displayScreen.Width, displayScreen.Height, simonQuestion);
            displayScreen.Controls.Add(simonImage);
        }

        public void ShowQuestion()
        {
            simonImage.Image = simonQuestion;
        }

        public void ShowAnswer()
        {
            var simonAnswer = Image.LoadFromResource($"MagicEightMeadow.Resources.m8b_{random.Next(1, 21).ToString("00")}.bmp");
            simonImage.Image = simonAnswer;
        }
    }
}