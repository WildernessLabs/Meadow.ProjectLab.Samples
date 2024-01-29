using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Camera;
using System.Threading.Tasks;

namespace TrackingEyeBall;

public class MeadowApp : App<F7CoreComputeV2>
{
    PersonSensor? personSensor;
    EyeballController? eyeballController;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        var projLab = ProjectLab.Create();
        eyeballController = new EyeballController(projLab.Display!);

        personSensor = new PersonSensor(projLab.I2cBus);

        return Task.CompletedTask;
    }

    private void DisplaySensorData(PersonSensorResults sensorData)
    {
        if (sensorData.NumberOfFaces == 0)
        {
            eyeballController.CloseEye();
            Resolver.Log.Info("No faces found");
            return;
        }

        eyeballController.DrawEyeball();

        for (int i = 0; i < sensorData.NumberOfFaces; ++i)
        {
            var face = sensorData.FaceData[i];
            Resolver.Log.Info($"Face #{i}: {face.BoxConfidence} confidence, ({face.BoxLeft}, {face.BoxTop}), ({face.BoxRight}, {face.BoxBottom}), facing: {face.IsFacing}");
        }
    }

    public override async Task Run()
    {
        Resolver.Log.Info("Run...");

        eyeballController.DrawEyeball();

        while (true)
        {
            var sensorData = personSensor.GetSensorData();
            DisplaySensorData(sensorData);

            //eyeballController.Delay();
            //eyeballController.RandomEyeMovement();
            //eyeballController.Delay();
            //eyeballController.CenterEye();

            await Task.Delay(3000);
        }
    }
}