using CommonContracts.Models;
using Connectivity.Common.Models;
using Meadow.Foundation.Web.Maple;
using Meadow.Foundation.Web.Maple.Routing;
using MeadowConnectedSample.Controller;
using MeadowConnectedSample.Models.Logical;

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

        [HttpGet("/getbh1750data")]
        public IActionResult GetBh1750Data()
        {
            var reading = MainController.Instance.IlluminanceReading;
            var data = new IlluminanceModel()
            {
                Illuminance = $"{(int)reading?.Lux}lx"
            };

            Context.Response.ContentType = ContentTypes.Application_Json;
            return new JsonResult(data);
        }

        [HttpGet("/getbmi270data")]
        public IActionResult GetBmi270Data()
        {
            var reading = MainController.Instance.MotionReading;
            var data = new MotionModel()
            {
                Acceleration3D = $"({reading.acceleration3D.Value.X.CentimetersPerSecondSquared:N2}, " +
                                 $"{reading.acceleration3D.Value.Y.CentimetersPerSecondSquared:N2}, " +
                                 $"{reading.acceleration3D.Value.Z.CentimetersPerSecondSquared:N2})",
                AngularVelocity3D = $"({reading.angularVelocity3D.Value.X.DegreesPerSecond:N2}, " +
                                    $"{reading.angularVelocity3D.Value.Y.DegreesPerSecond:N2}, " +
                                    $"{reading.angularVelocity3D.Value.Z.DegreesPerSecond:N2})",
                Temperature = $"{reading.temperature.Value.Celsius:N2}°C"
            };

            Context.Response.ContentType = ContentTypes.Application_Json;
            return new JsonResult(data);
        }

        [HttpGet("/getbme688data")]
        public IActionResult GetBme688Data()
        {
            var reading = MainController.Instance.AmbientReading;
            var data = new ClimateModel()
            {
                Temperature = $"{(int)reading.Temperature.Value.Celsius}°C",
                Humidity = $"{(int)reading.Humidity.Value.Percent}%",
                Pressure = $"{(int)reading.Pressure.Value.Millibar}mbar"
            };

            Context.Response.ContentType = ContentTypes.Application_Json;
            return new JsonResult(data);
        }
    }
}