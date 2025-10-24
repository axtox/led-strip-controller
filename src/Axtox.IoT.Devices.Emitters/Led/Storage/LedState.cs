using Axtox.IoT.Common.Devices.Emitters.Lighting;
using Axtox.IoT.Common.Storage;
using System;

namespace Axtox.IoT.Devices.Emitters.Led.Storage
{
    public class LedState : IState
    {
        public Guid Key { get; init; }
        public byte PinNumber { get; init; }
        public bool IsOn { get; set; }
        public BrightnessLevel Brightness { get; set; }
    }
}
