using System;

namespace Axtox.IoT.Devices.Sensors.Gestures
{
    public readonly struct Gesture
    {
        private const byte MaximumIntensity = 100;
        private const byte MinimumIntensity = 0;

        public byte Intensity { get; init; }

        public Gesture(byte intensity)
        {
            if (intensity < MinimumIntensity || intensity > MaximumIntensity)
                throw new ArgumentOutOfRangeException(nameof(intensity), "Gesture intensity level must be between 0 and 100.");

            Intensity = intensity;
        }

        public static readonly Gesture Full = new() { Intensity = MaximumIntensity };
        public static readonly Gesture Low = new() { Intensity = MinimumIntensity };

        public static implicit operator Gesture(byte intensity) => new(intensity);
    }
}
