using nanoFramework.Hardware.Esp32;

namespace Axtox.IoT.LedStripController
{
	public class Program
	{
		public static void Main()
		{
			Configuration.SetPinFunction(16, DeviceFunction.PWM1);
			new LedDiode().SetBrightness(10);
			//Console.WriteLine("Hello from nanoFramework!");

			//Thread.Sleep(Timeout.Infinite);

			// Browse our samples repository: https://github.com/nanoframework/samples
			// Check our documentation online: https://docs.nanoframework.net/
			// Join our lively Discord community: https://discord.gg/gCyBu8T
		}
	}
}
