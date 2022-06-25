using CommonContracts.Models;
using Meadow.Foundation.Web.Maple;
using Meadow.Foundation.Web.Maple.Routing;
using MeadowApp.Controller;

namespace MeadowApp.Connectivity
{
    public class MapleRequestHandler : RequestHandlerBase
    {
        public MapleRequestHandler() { }

        [HttpPost("/toggle")]
        public IActionResult Toggle()
        {
            LedController.Instance.Toggle();
            return new OkResult();
        }

        [HttpPost("/blink")]
        public IActionResult Blink()
        {
            LedController.Instance.StartBlink();
            return new OkResult();
        }

        [HttpPost("/pulse")]
        public IActionResult Pulse()
        {
            LedController.Instance.StartPulse();
            return new OkResult();
        }

        [HttpGet("/getbme688data")]
        public IActionResult GetBme688Data()
        {
            var reading = Bme688Controller.Instance.AmbientReading;
            var data = new ClimateModel() 
            { 
                Temperature = $"{(int)reading.Temperature.Value.Celsius}°C",
                Humidity = $"{(int)reading.Humidity.Value.Percent}%",
                Pressure = $"{(int)reading.Pressure.Value.Millibar}mbar"
            };

            Context.Response.ContentType = ContentTypes.Application_Json;
            return new JsonResult(data);
        }

        [HttpGet("/getbh1750data")]
        public IActionResult getBh1750Data()
        {
            var reading = Bh1750Controller.Instance.IlluminanceReading;
            var data = new IlluminanceModel()
            {
                Illuminance = $"{(int)reading?.Lux}lx"
            };

            Context.Response.ContentType = ContentTypes.Application_Json;
            return new JsonResult(data);
        }
    }
}