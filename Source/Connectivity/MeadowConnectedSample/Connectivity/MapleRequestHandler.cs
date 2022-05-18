using CommonContracts.Models;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using MeadowConnectedSample.Controller;
using System;
using System.Threading.Tasks;

namespace MeadowConnectedSample.Connectivity
{
    public class MapleRequestHandler : RequestHandlerBase
    {
        public MapleRequestHandler() { }

        [HttpGet("/getroomambient")]
        public IActionResult GetRoomAmbient()
        {
            LedController.Instance.SetColor(Color.Magenta);

            var reading = Bme688Controller.Instance.Read().Result;
            var data = new ClimateModel() 
            { 
                Date = DateTime.Now.ToShortDateString(),
                Temperature = $"{(int)reading.Temperature.Value.Celsius}°C",
                Humidity = $"{(int)reading.Humidity.Value.Percent}%",
                Pressure = $"{(int)reading.Pressure.Value.Millibar}mbar"
            };

            LedController.Instance.SetColor(Color.Green);

            Context.Response.ContentType = ContentTypes.Application_Json;
            return new JsonResult(data);
        }
    }
}