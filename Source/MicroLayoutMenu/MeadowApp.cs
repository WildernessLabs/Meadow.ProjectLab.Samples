using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Displays.UI;
using System.Threading.Tasks;

namespace MicroLayoutMenu
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7CoreComputeV2>
    {
        private DisplayScreen _screen;
        private Menu _menu;

        public override Task Initialize()
        {
            var menuItems = new string[]
                {
                    "Item A",
                    "Item B",
                    "Item C",
                    "Item D",
                    "Item E",
                    "Item F",
                };

            var projLab = ProjectLab.Create();
            _screen = new DisplayScreen(projLab.Display, Meadow.Foundation.Graphics.RotationType._270Degrees);
            _menu = new Menu(menuItems, _screen);

            projLab.UpButton.Clicked += (s, e) => _menu.Up();
            projLab.DownButton.Clicked += (s, e) => _menu.Down();

            return base.Initialize();
        }

        public override Task Run()
        {
            return base.Run();
        }
    }
}