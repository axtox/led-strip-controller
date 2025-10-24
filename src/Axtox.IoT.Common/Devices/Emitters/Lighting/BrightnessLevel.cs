using System;

namespace Axtox.IoT.Common.Devices.Emitters.Lighting
{
    public class BrightnessLevel
    {
        private const byte MaximumBrightess = 100;
        private const byte MinimumBrightess = 0;

        private byte level;
        public byte Level
        {
            get => level;
            init => SetLevel(value);
        }

        private BrightnessLevel() { }

        public BrightnessLevel(byte level)
        {
            SetLevel(level);
        }

        private void SetLevel(byte level)
        {
            if (level < MinimumBrightess || level > MaximumBrightess)
                throw new ArgumentOutOfRangeException(nameof(level), "Brightness level must be between 0 and 100.");

            this.level = level;
        }

        public static readonly BrightnessLevel Max = new() { level = MaximumBrightess };
        public static readonly BrightnessLevel Min = new() { level = MinimumBrightess };

        public static implicit operator BrightnessLevel(byte level) => new(level);
    }
}
