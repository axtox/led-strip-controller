using Axtox.IoT.Common.Devices.Sensors.Measurement;
using Iot.Device.Vl53L0X;
using System;
using System.Device.Gpio;
using System.Device.I2c;

namespace Axtox.IoT.Devices.Sensors.Measurement
{
    public class InfraredDistanceMeasurementDevice : IDisposable, IDistanceMeasurementDevice
    {
        private Vl53L0X _sensor;
        private GpioController _controller;
        private int _interruptPinId;

        /// <summary>
        /// Creates a new instance of the InfraredDistanceMeasurementDevice with the default I2C VL53L0X device address 
        /// which is responsible to read distance values from the VL53L0X sensor.
        /// </summary>
        /// <param name="controller">The GPIO controller that is used to host the system</param>
        /// <param name="interruptPinId">The GPIO pin that is used to listen for measurement ready signal from VL53L0X GPIO1 pin</param>
        /// <param name="busId">I2C bus ID that VL53L0X is connected to</param>
        public InfraredDistanceMeasurementDevice(int interruptPinId, int busId) : this(new GpioController(), interruptPinId, busId, Vl53L0X.DefaultI2cAddress) { }

        /// <summary>
        /// Creates a new instance of the InfraredDistanceMeasurementDevice which is responsible to read distance values from the VL53L0X sensor.
        /// </summary>
        /// <param name="controller">The GPIO controller that is used to host the system</param>
        /// <param name="interruptPinId">The GPIO pin that is used to listen for measurement ready signal from VL53L0X GPIO1 pin</param>
        /// <param name="busId">I2C bus ID that VL53L0X is connected to</param>
        /// <param name="deviceAddress">I2C VL53L0X device address</param>
        public InfraredDistanceMeasurementDevice(GpioController controller, int interruptPinId, int busId, int deviceAddress)
        {
            var i2cDevice = I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress));
            _sensor = new Vl53L0X(i2cDevice);
            _sensor.SetSignalRateLimit(0.1); // Optional tuning
                                             //_sensor.
                                             //_sensor.SetMeasurementTimingBudgetMicroseconds(33000); // ~30 Hz
                                             //_sensor.StartContinuous(0); // Continuous mode


            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _interruptPinId = interruptPinId;
        }

        public event DistanceReadyHandler DistanceReady;

        public ushort DistanceInMillimeter => _sensor.Distance;

        public void StartMeasurement()
        {
            _sensor.StartContinuousMeasurement();

            _controller.OpenPin(_interruptPinId, PinMode.InputPullUp);
            _controller.RegisterCallbackForPinValueChangedEvent(_interruptPinId, PinEventTypes.Falling, MeasurmentReady);
        }

        public void StopMeasurement()
        {
            _sensor.Reset();

            _controller.UnregisterCallbackForPinValueChangedEvent(_interruptPinId, MeasurmentReady);
            if (_controller.IsPinOpen(_interruptPinId))
                _controller.ClosePin(_interruptPinId);
        }

        private void MeasurmentReady(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            DistanceReady?.Invoke(DistanceInMillimeter);
        }

        #region IDisposable Support
        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                DistanceReady = null;
                StopMeasurement();

                _sensor?.Dispose();
                _sensor = null;

                _controller?.Dispose();
                _controller = null;

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
