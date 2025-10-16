using Axtox.IoT.Common.Devices.Sensors.Measurment;
using System;
using System.Diagnostics;

namespace Axtox.IoT.Devices.Sensors
{
    public class LinearGestureSensor
    {
        private int OperatingMaxRangeInMillimeters { get; init; } = 600;
        private int OperatingMinRangeInMillimeters { get; init; } = 100;
        private int OperatingThresholdInMillimeters { get; init; } = 10;
        private int OperatingTimeoutInMilliecond { get; init; } = 150;
        private int GestureActivationThresholdInMillimeters { get; init; } = 10;
        private int GestureActivationTimeInMillisecond { get; init; } = 500;

        private IDistanceMeasurmentDevice _distanceMeasurmentDevice;
        public event Action<int> GestureDetected;

        public LinearGestureSensor(IDistanceMeasurmentDevice distanceMeasurmentDevice)
        {
            _distanceMeasurmentDevice = distanceMeasurmentDevice;
            _distanceMeasurmentDevice.DistanceReady += GestureDetection;
        }

        public void StartSensing()
        {
            _distanceMeasurmentDevice.StartMeasurment();
        }

        public void StopSensing()
        {
            _distanceMeasurmentDevice.StopMeasurment();
        }

        private bool _isActivated = false;
        private DateTime _lastDetectionTime;
        private double _lastDetectionDistance;
        private void GestureDetection(double readings)
        {
            if (readings > OperatingMaxRangeInMillimeters
                || readings < OperatingMinRangeInMillimeters)
                return;

            var timeFromLastChange = DateTime.UtcNow - _lastDetectionTime;
            if (timeFromLastChange.TotalMilliseconds > OperatingTimeoutInMilliecond)
            {
                _isActivated = false;
                Debug.WriteLine($"Operation deactivated");
            }

            if (!_isActivated)
            {
                _isActivated = ShouldActivateGesture(readings);
                if (_isActivated)
                {
                    GestureDetected?.Invoke((int)(((readings - OperatingMinRangeInMillimeters) / (OperatingMaxRangeInMillimeters - OperatingMinRangeInMillimeters)) * 100));
                    Debug.WriteLine($"Operation activated");
                }
            }

            Debug.WriteLine($"Detected distance: {readings} cm");
        }

        private DateTime _lastInteractionTime;
        private double _lastInteractionDistance;
        private bool ShouldActivateGesture(double distance)
        {
            Debug.WriteLine($"Starting activation detection with distance: {distance} mm");
            //if it was not set then start the detection
            if (IsActivationDetectionReset())
            {
                _lastInteractionDistance = distance;
                _lastInteractionTime = DateTime.UtcNow;
                Debug.WriteLine($"Looks like it's first time activation! Writing the distance and time.");
                return false;
            }

            var elapsedTimeSinceLastInteraction = DateTime.UtcNow - _lastInteractionTime;
            var distanceThresholdFromLastInteraction = Math.Abs(distance - _lastInteractionDistance);
            Debug.WriteLine($"Elapsed time and distance difference from last activation: " +
                $"{elapsedTimeSinceLastInteraction.TotalMilliseconds} ms and {distanceThresholdFromLastInteraction} mm");

            if (distanceThresholdFromLastInteraction > GestureActivationThresholdInMillimeters)
            {
                Debug.WriteLine($"Oops! Distance threshold from last interaction is too big. Resetting the detection.");
                ResetActivationDetectionState();
                return false;
            }

            if (distanceThresholdFromLastInteraction <= GestureActivationThresholdInMillimeters
                && elapsedTimeSinceLastInteraction.TotalMilliseconds > GestureActivationTimeInMillisecond)
            {
                Debug.WriteLine($"We're all set. Starting detection from now on.");
                ResetActivationDetectionState();
                return true;
            }
            return false;
        }

        private bool IsActivationDetectionReset()
            => _lastInteractionDistance == 0 && _lastInteractionTime == default;

        private void ResetActivationDetectionState()
        {
            _lastInteractionDistance = 0;
            _lastInteractionTime = default;
        }
    }
}
