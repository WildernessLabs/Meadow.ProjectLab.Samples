using System;
using System.Text;

namespace ProjLab_Demo
{
    //TODO: this has to be hardware specific, since the MCPs aren't on v1, etc.
    public class HardwareBringupStatus
    {
        public bool I2c { get; set; } = false;
        public bool Spi { get; set; } = false;
        public bool Bh1750 { get; set; } = false;
        public bool Bme688 { get; set; } = false;
        public bool Bmi270 { get; set; } = false;
        public bool Mcp_1 { get; set; } = false;
        public bool Mcp_2 { get; set; } = false;
        public bool BtnUp { get; set; } = false;
        public bool BtnRight { get; set; } = false;
        public bool BtnDown { get; set; } = false;
        public bool BtnLeft { get; set; } = false;

        public bool AllGood
        {
            get => I2c && Spi && Bh1750 && Bme688 && Bmi270 && Mcp_1 && Mcp_2 && BtnUp && BtnRight && BtnDown && BtnLeft;
        }

        public override string ToString()
        {
            StringBuilder text = new StringBuilder();

            text.Append($"Hardware Bringup Status: {(AllGood ? "Success" : "Failure")}\r\n");
            text.Append($"\tI2C: {(I2c ? "Success" : "Fail")}\r\n");
            text.Append($"\tSPI: {(Spi ? "Success" : "Fail")}\r\n");
            text.Append($"\tBH1750: {(Bh1750 ? "Success" : "Fail")}\r\n");
            text.Append($"\tBME688: {(Bme688 ? "Success" : "Fail")}\r\n");
            text.Append($"\tBmi270: {(Bmi270 ? "Success" : "Fail")}\r\n");
            text.Append($"\tMCP_1: {(Mcp_1 ? "Success" : "Fail")}\r\n");
            text.Append($"\tMCP_2: {(Mcp_2 ? "Success" : "Fail")}\r\n");
            text.Append($"\tUp Button: {(BtnUp ? "Success" : "Fail")}\r\n");
            text.Append($"\tRight Button: {(BtnRight ? "Success" : "Fail")}\r\n");
            text.Append($"\tDown Button: {(BtnDown ? "Success" : "Fail")}\r\n");
            text.Append($"\tLeft Button: {(BtnLeft ? "Success" : "Fail")}\r\n");

            return text.ToString();
        }
    }
}
