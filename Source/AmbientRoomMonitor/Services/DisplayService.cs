﻿using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;

namespace AmbientRoomMonitor.Services
{
    internal class DisplayService
    {
        int rowHeight = 60;
        int rowMargin = 15;

        protected DisplayScreen DisplayScreen { get; set; }

        protected Label Light { get; set; }

        protected Label Pressure { get; set; }

        protected Label Humidity { get; set; }

        protected Label Temperature { get; set; }

        Color backgroundColor = Color.FromHex("#F3F7FA");
        Color foregroundColor = Color.Black;

        Font12x20 font12X20 = new Font12x20();

        public DisplayService(IGraphicsDisplay display)
        {
            DisplayScreen = new DisplayScreen(display, RotationType._270Degrees)
            {
                BackgroundColor = backgroundColor
            };

            DisplayScreen.Controls.Add(new Box(0, 0, display.Width, rowHeight)
            {
                ForeColor = Color.FromHex("#5AC0EA")
            });
            DisplayScreen.Controls.Add(new Box(0, rowHeight, display.Width, rowHeight)
            {
                ForeColor = Color.FromHex("#84D0EF")
            });
            DisplayScreen.Controls.Add(new Box(0, rowHeight * 2, display.Width, rowHeight)
            {
                ForeColor = Color.FromHex("#A3DCF3")
            });
            DisplayScreen.Controls.Add(new Box(0, rowHeight * 3, display.Width, rowHeight)
            {
                ForeColor = Color.FromHex("#B8E4F6")
            });

            DisplayScreen.Controls.Add(new Label(rowMargin, 0, DisplayScreen.Width / 2, rowHeight)
            {
                Text = $"LIGHT",
                TextColor = foregroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            });
            DisplayScreen.Controls.Add(new Label(rowMargin, rowHeight, DisplayScreen.Width / 2, rowHeight)
            {
                Text = $"PRESSURE",
                TextColor = foregroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            });
            DisplayScreen.Controls.Add(new Label(rowMargin, rowHeight * 2, DisplayScreen.Width / 2, rowHeight)
            {
                Text = $"HUMIDITY",
                TextColor = foregroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            });
            DisplayScreen.Controls.Add(new Label(rowMargin, rowHeight * 3, DisplayScreen.Width / 2, rowHeight)
            {
                Text = $"TEMPERATURE",
                TextColor = foregroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            });

            Light = new Label(DisplayScreen.Width / 2 - rowMargin, 0, DisplayScreen.Width / 2, rowHeight)
            {
                Text = $"- Lx",
                TextColor = foregroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            DisplayScreen.Controls.Add(Light);

            Pressure = new Label(DisplayScreen.Width / 2 - rowMargin, rowHeight, DisplayScreen.Width / 2, rowHeight)
            {
                Text = $"- Mb",
                TextColor = foregroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            DisplayScreen.Controls.Add(Pressure);

            Humidity = new Label(DisplayScreen.Width / 2 - rowMargin, rowHeight * 2, DisplayScreen.Width / 2, rowHeight)
            {
                Text = $"- % ",
                TextColor = foregroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            DisplayScreen.Controls.Add(Humidity);

            Temperature = new Label(DisplayScreen.Width / 2 - rowMargin, rowHeight * 3, DisplayScreen.Width / 2, rowHeight)
            {
                Text = $"- °C",
                TextColor = foregroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            DisplayScreen.Controls.Add(Temperature);
        }

        public void UpdateAtmosphericConditions(string light, string pressure, string humidity, string temperature)
        {
            DisplayScreen.BeginUpdate();

            Light.Text = $"{light} Lx";
            Pressure.Text = $"{pressure} mb";
            Humidity.Text = $"{humidity} % ";
            Temperature.Text = $"{temperature} °C";

            DisplayScreen.EndUpdate();
        }
    }
}