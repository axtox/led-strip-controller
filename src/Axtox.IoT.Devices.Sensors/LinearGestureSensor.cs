using Axtox.IoT.Common.Devices.Sensors.Measurement;
using Axtox.IoT.Common.System.Logging;
using Axtox.IoT.Devices.Sensors.Gestures;
using System;

namespace Axtox.IoT.Devices.Sensors
{
    public delegate void GestureDetectedHandler(Gesture intensity);
    {
        private ushort OperatingMaxRangeInMillimeters { get; init; } = 600;
        private ushort OperatingMinRangeInMillimeters { get; init; } = 100;
        private ushort OperatingThresholdInMillimeters { get; init; } = 10;
        private ushort OperatingTimeoutInMilliecond { get; init; } = 150;
        private ushort GestureActivationThresholdInMillimeters { get; init; } = 10;
        private ushort GestureActivationTimeInMillisecond { get; init; } = 500;

        private IDistanceMeasurementDevice _distanceMeasurementDevice;
        private ILogger _logger;

        public LinearGestureSensor(IDistanceMeasurementDevice distanceMeasurementDevice, ILogger logger)
        {
            _logger = logger;
            _distanceMeasurementDevice = distanceMeasurementDevice;
            _distanceMeasurementDevice.DistanceReady += GestureDetection;
        }

        public event GestureDetectedHandler GestureDetected;

        public void StartSensing()
        {
            _distanceMeasurementDevice.StartMeasurement();
        }

        public void StopSensing()
        {
            _distanceMeasurementDevice.StopMeasurement();
        }

        private bool _isActivated = false;
        private DateTime _lastDetectionTime;
        private ushort _lastDetectionDistance;
        private void GestureDetection(ushort readings)
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
                GestureDetected?.Invoke((byte)(((readings - OperatingMinRangeInMillimeters) / (OperatingMaxRangeInMillimeters - OperatingMinRangeInMillimeters)) * 100));
                    Debug.WriteLine($"Operation activated");
                }
            }

            Debug.WriteLine($"Detected distance: {readings} cm");
        }

        private DateTime _lastInteractionTime;
        private ushort _lastInteractionDistance;
        private bool ShouldActivateGesture(ushort distance)
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
