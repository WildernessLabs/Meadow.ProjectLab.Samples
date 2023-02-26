using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace RotatingCube
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        IProjectLabHardware projLab;

        MicroGraphics graphics;

        Cube3d cube;

        Color cubeColor;

        readonly Angle ButtonStep = new Angle(1);
        readonly TimeSpan motionUpdateInterval = TimeSpan.FromMilliseconds(250);
        readonly int cubeSize = 60;
        readonly Color initalColor = Color.Cyan;

        public override Task Run()
        {
            projLab.MotionSensor.StartUpdating(motionUpdateInterval);

            cube = new Cube3d(graphics.Width / 2, graphics.Height / 2, cubeSize);
            cubeColor = initalColor;

            Show3dCube();

            return base.Run();
        }

        public override Task Initialize()
        {
            Console.WriteLine("Initialize...");

            projLab = ProjectLab.Create();

            graphics = new MicroGraphics(projLab.Display);

            projLab.RightButton.Clicked += RightButton_Clicked;
            projLab.LeftButton.Clicked += LeftButton_Clicked;
            projLab.UpButton.Clicked += UpButton_Clicked;
            projLab.DownButton.Clicked += DownButton_Clicked;

            projLab.UpButton.LongClickedThreshold = TimeSpan.FromMilliseconds(500);
            projLab.UpButton.LongClicked += UpButton_LongClicked;

            projLab.DownButton.LongClickedThreshold = TimeSpan.FromMilliseconds(500);
            projLab.DownButton.LongClicked += DownButton_LongClicked;

            projLab.MotionSensor.Updated += MotionSensor_Updated;

            Console.WriteLine("Init complete");
            return base.Initialize();
        }

        public void Show3dCube()
        {
            while (true)
            {
                graphics.Clear();

                cube.Update();

                DrawWireframe(cubeColor);

                graphics.Show();

                cubeColor = cubeColor.WithHue(cubeColor.Hue + 0.001);
            }
        }

        void DrawWireframe(Color color)
        {
            graphics.DrawLine(cube.Wireframe[0, 0], cube.Wireframe[0, 1], cube.Wireframe[1, 0], cube.Wireframe[1, 1], color);
            graphics.DrawLine(cube.Wireframe[1, 0], cube.Wireframe[1, 1], cube.Wireframe[2, 0], cube.Wireframe[2, 1], color);
            graphics.DrawLine(cube.Wireframe[2, 0], cube.Wireframe[2, 1], cube.Wireframe[3, 0], cube.Wireframe[3, 1], color);
            graphics.DrawLine(cube.Wireframe[3, 0], cube.Wireframe[3, 1], cube.Wireframe[0, 0], cube.Wireframe[0, 1], color);

            //cross face above
            graphics.DrawLine(cube.Wireframe[1, 0], cube.Wireframe[1, 1], cube.Wireframe[3, 0], cube.Wireframe[3, 1], color);
            graphics.DrawLine(cube.Wireframe[0, 0], cube.Wireframe[0, 1], cube.Wireframe[2, 0], cube.Wireframe[2, 1], color);

            graphics.DrawLine(cube.Wireframe[4, 0], cube.Wireframe[4, 1], cube.Wireframe[5, 0], cube.Wireframe[5, 1], color);
            graphics.DrawLine(cube.Wireframe[5, 0], cube.Wireframe[5, 1], cube.Wireframe[6, 0], cube.Wireframe[6, 1], color);
            graphics.DrawLine(cube.Wireframe[6, 0], cube.Wireframe[6, 1], cube.Wireframe[7, 0], cube.Wireframe[7, 1], color);
            graphics.DrawLine(cube.Wireframe[7, 0], cube.Wireframe[7, 1], cube.Wireframe[4, 0], cube.Wireframe[4, 1], color);

            graphics.DrawLine(cube.Wireframe[0, 0], cube.Wireframe[0, 1], cube.Wireframe[4, 0], cube.Wireframe[4, 1], color);
            graphics.DrawLine(cube.Wireframe[1, 0], cube.Wireframe[1, 1], cube.Wireframe[5, 0], cube.Wireframe[5, 1], color);
            graphics.DrawLine(cube.Wireframe[2, 0], cube.Wireframe[2, 1], cube.Wireframe[6, 0], cube.Wireframe[6, 1], color);
            graphics.DrawLine(cube.Wireframe[3, 0], cube.Wireframe[3, 1], cube.Wireframe[7, 0], cube.Wireframe[7, 1], color);
        }

        private void MotionSensor_Updated(object sender, IChangeResult<(Acceleration3D? Acceleration3D, AngularVelocity3D? AngularVelocity3D, Temperature? Temperature)> e)
        {
            cube.XVelocity += new Angle(e.New.Acceleration3D.Value.X.Gravity);
            cube.YVelocity -= new Angle(e.New.Acceleration3D.Value.Y.Gravity);
        }

        private void UpButton_LongClicked(object sender, EventArgs e)
        {
            if (cube != null)
            {
                cube.XVelocity = cube.YVelocity = cube.ZVelocity = new Angle(0);
            }
        }

        private void DownButton_LongClicked(object sender, EventArgs e)
        {
            if (cube != null)
            {
                cube.XRotation = cube.YRotation = cube.ZRotation = new Angle(0);
            }
        }

        private void DownButton_Clicked(object sender, EventArgs e)
        {
            if (cube != null)
            {
                cube.XVelocity -= ButtonStep;
            }
        }

        private void UpButton_Clicked(object sender, EventArgs e)
        {
            if (cube != null)
            {
                cube.XVelocity += ButtonStep;
            }
        }

        private void LeftButton_Clicked(object sender, EventArgs e)
        {
            if (cube != null)
            {
                cube.YVelocity -= ButtonStep;
            }
        }

        private void RightButton_Clicked(object sender, EventArgs e)
        {
            if (cube != null)
            {
                cube.YVelocity += ButtonStep;
            }
        }
    }
}