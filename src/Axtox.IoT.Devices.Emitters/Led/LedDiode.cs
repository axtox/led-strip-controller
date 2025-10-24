using Axtox.IoT.Common.Animations;
using Axtox.IoT.Common.Devices.Emitters.Lighting;
using Axtox.IoT.Common.Storage;
using Axtox.IoT.Devices.Emitters.Led.Storage;
using System;
using System.Device.Pwm;
using System.Diagnostics;
using System.Threading;

namespace Axtox.IoT.Devices.Emitters.Led
{
    /// <summary>
    /// Represents a PWM-controlled LED diode with state persistence and smooth brightness animations.
    /// Supports graceful fade-in/fade-out transitions, brightness adjustment with easing animations,
    /// and automatic restoration of last known state on power cycle.
    /// </summary>
    /// <remarks>
    /// The LED diode uses PWM (Pulse Width Modulation) to control brightness levels smoothly.
    /// State is persisted across power cycles using the provided <see cref="IStateStorage"/> implementation.
    /// Brightness transitions are animated using the configured <see cref="IAnimator"/> with customizable easing styles.
    /// 
    /// <example>
    /// Example usage for ESP32:
    /// <code>
    /// // Configure the pin for PWM functionality
    /// Configuration.SetPinFunction(16, DeviceFunction.PWM1);
    /// 
    /// // Create animator with custom settings
    /// IAnimator animator = new BackgroundAnimator();
    /// animator.Configure(settings =>
    /// {
    ///     settings.DurationInMilliseconds = 500;
    ///     settings.EasingStyle = EasingStyle.EaseInOutQuad;
    /// });
    /// 
    /// // Initialize LED with state storage and animator
    /// var led = new LedDiode(
    ///     new Guid("d3f9a8b2-7c4e-4e9d-9a1f-2e6c1a5b8f3e"), 
    ///     16, 
    ///     new FileSystemJsonStateStorage(), 
    ///     animator);
    /// 
    /// led.On();
    /// led.SetBrightness(50);
    /// led.Off();
    /// led.Dispose();
    /// </code>
    /// </example>
    /// </remarks>
    public class LedDiode : ILed, IAnimatable, IDisposable
    {
        private readonly LedState LedState;

        private PwmChannel _pwm;
        private IStateStorage _storage;
        private IAnimator _animator;

        /// <summary>
        /// Initializes a new instance of the <see cref="LedDiode"/> class with default animator settings.
        /// </summary>
        /// <param name="uniqueLedId">Unique identifier for the LED used for state persistence.</param>
        /// <param name="pinNumber">The GPIO pin number that supports PWM signal output. 
        /// Ensure the pin is configured for PWM functionality (e.g., Configuration.SetPinFunction(16, DeviceFunction.PWM1)).</param>
        /// <param name="ledStateStorage">Storage implementation for persisting LED state across power cycles.</param>
        /// <remarks>
        /// This constructor uses a default animator with 1250ms duration and EaseInOutQuad easing style.
        /// </remarks>
        public LedDiode(Guid uniqueLedId, byte pinNumber, IStateStorage ledStateStorage) : this(uniqueLedId, pinNumber, ledStateStorage, CreateDefaultAnimator()) { }

        private static BackgroundAnimator CreateDefaultAnimator()
        {
            var animator = new BackgroundAnimator();
            animator.Configure(settings =>
            {
                settings.DurationInMilliseconds = 1250;
                settings.EasingStyle = EasingStyle.EaseInOutQuad;
            });
            return animator;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LedDiode"/> class with custom animator.
        /// </summary>
        /// <param name="uniqueLedId">Unique identifier for the LED used for state persistence.</param>
        /// <param name="pinNumber">The GPIO pin number that supports PWM signal output. 
        /// Ensure the pin is configured for PWM functionality before creating the LED instance.
        /// Example: Configuration.SetPinFunction(16, DeviceFunction.PWM1) enables PWM on pin 16.</param>
        /// <param name="ledStateStorage">Storage implementation for persisting LED state across power cycles.</param>
        /// <param name="animator">Custom animator for controlling brightness transition effects and easing styles.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ledStateStorage"/> or <paramref name="animator"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the provided <paramref name="pinNumber"/> does not match the stored LED state.</exception>
        /// <remarks>
        /// The LED will automatically restore its previous state (on/off and brightness) from storage.
        /// If no previous state exists, the LED is initialized as off with maximum brightness setting.
        /// The PWM frequency is set to 1000 Hz by default.
        /// </remarks>
        public LedDiode(Guid uniqueLedId, byte pinNumber, IStateStorage ledStateStorage, IAnimator animator)
        {
            _storage = ledStateStorage
                ?? throw new ArgumentNullException(nameof(ledStateStorage), "Provided LED storage state is not valid.");
            _animator = animator
                ?? throw new ArgumentNullException(nameof(animator), "Provided animator is not valid.");

            LedState = ledStateStorage.Load(uniqueLedId, typeof(LedState)) as LedState;
            LedState ??= new LedState
            {
                Key = uniqueLedId,
                PinNumber = pinNumber,
                IsOn = false,
                Brightness = BrightnessLevel.Max
            };

            if (LedState.PinNumber != pinNumber)
                throw new ArgumentException("Provided pin number does not match the stored LED state.", nameof(pinNumber));

            _pwm = PwmChannel.CreateFromPin(LedState.PinNumber, 1000);

            if (LedState.IsOn)
                On();
            else
                Off();
        }

        public void On()
        {
            ThrowIfDisposed();

            _pwm.DutyCycle = 0;
            _pwm.Start();

            SetBrightness(LedState.Brightness);
            Thread.Sleep(_animator.Settings.DurationInMilliseconds);
            LedState.IsOn = true;
        }

        public void Off()
        {
            ThrowIfDisposed();

            LedState.IsOn = false;
            SetBrightness(BrightnessLevel.Min);
            Thread.Sleep(_animator.Settings.DurationInMilliseconds);

            _pwm.Stop();
        }

        public void Blink(byte times = 4)
        {
            ThrowIfDisposed();

            for (int i = 0; i < times; i++)
            {
                On();
                Off();
            }
        }

        public void SetBrightness(BrightnessLevel brightness)
        {
            ThrowIfDisposed();

            _animator.Animate(this, brightness.Level / 100f);
            if (LedState.IsOn)
                LedState.Brightness = brightness;
            Debug.WriteLine($"Animating brightness to {brightness.Level}");
        }

        public AnimatedValue GetCurrentValue() => new() { Value = (float)_pwm.DutyCycle };
        public void SetAnimatedValue(AnimatedValue value) => _pwm.DutyCycle = value.Value;

        #region IDisposable
        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _storage.Save(LedState);
                    if (_storage is IDisposable)
                        (_storage as IDisposable).Dispose();
                    _storage = null;

                    Off();
                    _animator.Dispose();
                    _animator = null;

                    _pwm.Stop();
                    _pwm.Dispose();
                    _pwm = null;
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException($"You are trying to acces already disposed object: {nameof(LedDiode)}");
        }
        #endregion
    }
}
