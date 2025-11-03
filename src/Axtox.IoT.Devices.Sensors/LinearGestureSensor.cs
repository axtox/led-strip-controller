using Axtox.IoT.Common.Devices.Sensors.Measurement;
using Axtox.IoT.Common.System.Logging;
using Axtox.IoT.Devices.Sensors.Gestures;
using System;

namespace Axtox.IoT.Devices.Sensors
{
    public delegate void GestureDetectedHandler(Gesture intensity);
    public class LinearGestureSensor : IDisposable
    {
        private ushort OperatingMaxRangeInMillimeters { get; init; } = 450;
        private ushort OperatingMinRangeInMillimeters { get; init; } = 50;
        private ushort OperatingThresholdInMillimeters { get; init; } = 10;
        private ushort OperatingTimeoutInMillisecond { get; init; } = 150;
        private ushort GestureActivationThresholdInMillimeters { get; init; } = 10;
        private ushort GestureActivationTimeInMillisecond { get; init; } = 250;

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
            {
                _isActivated = false;
                return;
            }

            if (!_isActivated)
                _isActivated = ShouldActivateGesture(readings);

            if (_isActivated)
            {
                var intensity = (float)(readings - OperatingMinRangeInMillimeters) / (OperatingMaxRangeInMillimeters - OperatingMinRangeInMillimeters) * 100.0f;
                GestureDetected?.Invoke((byte)Math.Round(intensity));
                _logger.LogDebug($"Gesture activated");
            }

            _lastDetectionTime = DateTime.UtcNow;
            _logger.LogDebug($"Detected distance: {readings} cm");
        }

        private DateTime _startedActivationCheckTime;
        private ushort _startedActivationCheckDistance;
        // you have to hold the gesture within the threshold distance for the specified time to activate
        private bool ShouldActivateGesture(ushort distance)
        {
            _logger.LogDebug($"Starting activation detection with distance: {distance} mm");
            //if it was not set then start the detection
            if (IsStartOfActivationDetection())
            {
                _startedActivationCheckDistance = distance;
                _startedActivationCheckTime = DateTime.UtcNow;
                _logger.LogDebug($"Looks like it's first time activation! Writing the distance and time.");
                return false;
            }

            var elapsedTimeSinceLastActivationCheck = DateTime.UtcNow - _startedActivationCheckTime;
            var distanceThresholdFromLastActivationCheck = Math.Abs(distance - _startedActivationCheckDistance);
            _logger.LogDebug($"Elapsed time and distance difference from last activation check: " +
                $"{elapsedTimeSinceLastActivationCheck.TotalMilliseconds} ms and {distanceThresholdFromLastActivationCheck} mm");

            if (distanceThresholdFromLastActivationCheck > GestureActivationThresholdInMillimeters)
            {
                _logger.LogInfo($"Oops! Distance threshold from last check is too big. Resetting the detection.");
                ResetActivationDetectionState();
                return false;
            }

            if (elapsedTimeSinceLastActivationCheck.TotalMilliseconds < GestureActivationTimeInMillisecond)
            {
                _logger.LogDebug($"Please hold on, we're getting there...");
                return false;
            }

            _logger.LogInfo($"We're all set. Starting detection from now on.");
            ResetActivationDetectionState();
            return true;
        }

        private bool IsStartOfActivationDetection()
            => _startedActivationCheckDistance == 0 && _startedActivationCheckTime == default;

        private void ResetActivationDetectionState()
        {
            _startedActivationCheckDistance = 0;
            _startedActivationCheckTime = default;
        }

        #region Disposal
        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                StopSensing();
                if (_distanceMeasurementDevice is IDisposable)
                    (_distanceMeasurementDevice as IDisposable)?.Dispose();
                _distanceMeasurementDevice = null;

                GestureDetected = null;
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
