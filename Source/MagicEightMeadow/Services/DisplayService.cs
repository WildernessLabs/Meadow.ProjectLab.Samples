using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;

namespace MagicEightMeadow.Services
{
    internal class DisplayService
    {
        Image simonQuestion = Image.LoadFromResource("MagicEightMeadow.Resources.m8b_question.bmp");

        protected DisplayScreen DisplayScreen { get; set; }

        protected Picture SimonImage { get; set; }

        Color backgroundColor = Color.FromHex("#F3F7FA");
        Color foregroundColor = Color.Black;

        public DisplayService(IGraphicsDisplay display)
        {
            DisplayScreen = new DisplayScreen(display, RotationType._270Degrees)
            {
                BackgroundColor = backgroundColor
            };

            SimonImage = new Picture((DisplayScreen.Width - DisplayScreen.Width) / 2, 0, DisplayScreen.Width, DisplayScreen.Height, simonQuestion);
            DisplayScreen.Controls.Add(SimonImage);
        }

        //public void UpdateWeatherIcon(string icon)
        //{
        //    simonQuestion = Image.LoadFromResource(icon);
        //    SimonImage.Image = simonQuestion;
        //}

        void DisplayQuestion()
        {
            SimonImage.Image = simonQuestion;
        }

        void DisplayAnswer()
        {
            //var rand = new Random();

            //var buffer = LoadJpeg(LoadResource(GetAnswerFilename(rand.Next(1, 21))));

            //graphics.DrawBuffer((graphics.Width - questionBuffer.Width) / 2, 0, buffer);
            //graphics.Show();
        }
    }
}