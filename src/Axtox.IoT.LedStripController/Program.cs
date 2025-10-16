using Axtox.IoT.Devices.Emitters.Led;
using Axtox.IoT.Devices.Sensors;
using Axtox.IoT.Devices.Sensors.Measurment;
using nanoFramework.Hardware.Esp32;
using System.Threading;

namespace Axtox.IoT.LedStripController
{
    public class Program
    {
        public static void Main()
        {
            Configuration.SetPinFunction(16, DeviceFunction.PWM1);
            var distanceSensor = new InfraredDistanceMeasurmentDevice(17, 1);
            var gestureMeasurmentDevice = new LinearGestureSensor(distanceSensor);
            gestureMeasurmentDevice.GestureDetected += (distance) =>
            {
                // Handle gesture detection
                new LedDiode().SetBrightness(distance);
            };
            gestureMeasurmentDevice.StartSensing();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
