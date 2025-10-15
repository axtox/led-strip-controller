using Axtox.IoT.Devices.Emitters.Led;
using nanoFramework.Hardware.Esp32;

namespace Axtox.IoT.LedStripController
{
    public class Program
    {
        public static void Main()
        {
            Configuration.SetPinFunction(16, DeviceFunction.PWM1);
            new LedDiode().SetBrightness(1);
        }
    }
}
