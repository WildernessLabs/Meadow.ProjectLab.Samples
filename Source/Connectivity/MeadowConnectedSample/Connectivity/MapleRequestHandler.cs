using CommonContracts.Models;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using MeadowConnectedSample.Controller;

namespace MeadowConnectedSample.Connectivity
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
            var reading = Bme688Controller.Instance.Read().Result;
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
            var reading = Bh1750Controller.Instance.Read().Result;
            var data = new IlluminanceModel()
            {
                Illuminance = $"{(int)reading.Lux}lx"
            };

            Context.Response.ContentType = ContentTypes.Application_Json;
            return new JsonResult(data);
        }
    }
}