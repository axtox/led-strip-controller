using System;

namespace Axtox.IoT.Common.Devices.Sensors.Measurment
{
	public interface IDistanceMeasurmentDevice
	{
		event Action<double> DistanceReady;
		double DistanceInMillimeter { get; }
		void StartMeasurment();
		void StopMeasurment();
	}
}
