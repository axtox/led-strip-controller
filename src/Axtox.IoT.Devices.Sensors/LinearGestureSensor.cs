using Axtox.IoT.Common.Devices.Sensors.Measurment;
using System.Threading;

namespace Axtox.IoT.Devices.Sensors
{
    internal class LinearGestureSensor
    {
        public int OperatingRangeCm { get; init; } = 60;
        public int DetectionThreshold { get; init; } = 15;
        private IDistanceMeasurmentDevice _distanceMeasurmentDevice;

        private Timer _sensingTimer;

        public LinearGestureSensor(IDistanceMeasurmentDevice distanceMeasurmentDevice)
        {
            _distanceMeasurmentDevice = distanceMeasurmentDevice;
            _sensingTimer = new Timer(GestureDetection, null, 0, 350);

            //var timer = new HighResTimer();
            //timer.OnHighResTimerExpired += TimerExpired;

            //timer.StartOnePeriodic(500); // Fires every 500 microseconds

        }

        public void StartSensing()
        {
            _distanceMeasurmentDevice.StartMeasurment();
        }

        public void StopSensing()
        {
            _distanceMeasurmentDevice.StopMeasurment();
        }

        private void GestureDetection(object state)
        {
            var distance = _distanceMeasurmentDevice.DistanceInMillimeter / 10; // Convert to cm
            if (distance > OperatingRangeCm)
                return;

            if (distance < DetectionThreshold)
            {
                // Too close, ignore
                return;
            }
            // Process the distance value for gesture detection
            // For example, you could log it or trigger an event
            System.Console.WriteLine($"Detected distance: {distance} cm");
        }
    }
}
