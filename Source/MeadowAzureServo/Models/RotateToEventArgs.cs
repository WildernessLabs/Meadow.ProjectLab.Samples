using System;

namespace MeadowAzureServo.Models
{
    public class RotateToEventArgs : EventArgs
    {
        public int Angle { get; set; }

        public RotateToEventArgs(int angle)
        {
            Angle = angle;
        }
    }
}