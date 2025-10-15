using System;
using System.Device.Gpio;
using System.Device.Pwm;
using System.Threading;

namespace Axtox.IoT.Devices.Emitters.Led
{
	public class LedDiode
	{
		private GpioController _gpioController;
		private PwmChannel _pwm;

		private bool _isInitialized = false;
		private void Initialize(int startBrightness)
		{
			_pwm = PwmChannel.CreateFromPin(16, 40000, startBrightness / 100); // TODO: take startBrightness from filesystem
			_pwm.Start();

			_isInitialized = true;
		}

		public void SetBrightness(int brightness)
		{
			if (brightness < 0 || brightness > 100)
				throw new ArgumentOutOfRangeException(nameof(brightness), "Brightness must be between 0 and 100.");

			if (!_isInitialized)
				Initialize(brightness);

			_pwm.DutyCycle = brightness / 100f;
			// Keep the PWM running to maintain brightness
			// In a real application, you might want to stop it later
			// pwm.Stop();
		}

		public void Unfade()
		{
			//Configuration.SetPinFunction(16, DeviceFunction.PWM1);
			var pwm = PwmChannel.CreateFromPin(16, 40000, 0); // 50% duty cycle
			pwm.Start();

			bool goingUp = true;
			float dutyCycle = .00f;
			while (true)
			{
				if (goingUp)
				{
					// slowly increase light intensity
					dutyCycle += 0.05f;

					// change direction if reaching maximum duty cycle (100%)
					if (dutyCycle > .95)
						goingUp = !goingUp;
				}
				else
				{
					// slowly decrease light intensity
					dutyCycle -= 0.05f;

					// change direction if reaching minimum duty cycle (0%)
					if (dutyCycle < 0.10)
						goingUp = !goingUp;
				}

				// update duty cycle
				pwm.DutyCycle = dutyCycle;

				Thread.Sleep(50);
			}

			pwm.Stop();
		}

		public void Blink()
		{
			_gpioController = new GpioController();

			// pick a board, uncomment one line for GpioPin; default is STM32F769I_DISCO

			// DISCOVERY4: PD15 is LED6 
			//GpioPin led = s_GpioController.OpenPin(PinNumber('D', 15), PinMode.Output);

			// ESP32 DevKit: 4 is a valid GPIO pin in, some boards like Xiuxin ESP32 may require GPIO Pin 2 instead.
			GpioPin led = _gpioController.OpenPin(16, PinMode.Output);

			// FEATHER S2: 
			//GpioPin led = s_GpioController.OpenPin(13, PinMode.Output);

			// F429I_DISCO: PG14 is LEDLD4 
			//GpioPin led = s_GpioController.OpenPin(PinNumber('G', 14), PinMode.Output);

			// NETDUINO 3 Wifi: A10 is LED onboard blue
			//GpioPin led = s_GpioController.OpenPin(PinNumber('A', 10), PinMode.Output);

			// QUAIL: PE15 is LED1  
			//GpioPin led = s_GpioController.OpenPin(PinNumber('E', 15), PinMode.Output);

			// STM32F091RC: PA5 is LED_GREEN
			//GpioPin led = s_GpioController.OpenPin(PinNumber('A', 5), PinMode.Output);

			// STM32F746_NUCLEO: PB75 is LED2
			//GpioPin led = s_GpioController.OpenPin(PinNumber('B', 7), PinMode.Output);

			//STM32F769I_DISCO: PJ5 is LD2
			//GpioPin led = s_GpioController.OpenPin(PinNumber('J', 5), PinMode.Output);

			// ST_B_L475E_IOT01A: PB14 is LD2
			//GpioPin led = s_GpioController.OpenPin(PinNumber('B', 14), PinMode.Output);

			// STM32L072Z_LRWAN1: PA5 is LD2
			//GpioPin led = s_GpioController.OpenPin(PinNumber('A', 5), PinMode.Output);

			// TI CC13x2 Launchpad: DIO_07 it's the green LED
			//GpioPin led = s_GpioController.OpenPin(7, PinMode.Output);

			// TI CC13x2 Launchpad: DIO_06 it's the red LED  
			//GpioPin led = s_GpioController.OpenPin(6, PinMode.Output);

			// ULX3S FPGA board: for the red D22 LED from the ESP32-WROOM32, GPIO5
			//GpioPin led = s_GpioController.OpenPin(5, PinMode.Output);

			// Silabs SLSTK3701A: LED1 PH14 is LLED1
			//GpioPin led = s_GpioController.OpenPin(PinNumber('H', 14), PinMode.Output);

			// Sparkfun Thing Plus - ESP32 WROOM 
			//GpioPin led = s_GpioController.OpenPin(Gpio.IO13, PinMode.Output);

			// RAK11200 on RAK5005/RAK19001/19007. The RAK19001 needs battery connected or power switch in rechargeable position
			//GpioPin led = s_GpioController.OpenPin(Gpio.IO12, PinMode.Output); // LED1 Green
			//GpioPin led = s_GpioController.OpenPin(Gpio.IO02, PinMode.Output); // LED2 Blue

			// RAK2305 
			//GpioPin led = s_GpioController.OpenPin(Gpio.IO18, PinMode.Output); // LED Green (Test LED) on device

			// Aliexpress ESP32-WROOM32, GPIO2 - onboard not-power LED
			// GpioPin led = s_GpioController.OpenPin(2, PinMode.Output);

			led.Write(PinValue.Low);

			while (true)
			{
				led.Toggle();
				Thread.Sleep(125);
				led.Toggle();
				Thread.Sleep(125);
				led.Toggle();
				Thread.Sleep(125);
				led.Toggle();
				Thread.Sleep(525);
			}
		}

		private int PinNumber(char port, byte pin)
		{
			if (port < 'A' || port > 'J')
				throw new ArgumentException();

			return ((port - 'A') * 16) + pin;
		}
	}
}
