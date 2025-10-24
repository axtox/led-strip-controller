namespace Axtox.IoT.Common.Devices.Emitters.Lighting
{
    public interface ILed
    {
        void On();
        void Off();
        void Blink(byte times = 4);
        void SetBrightness(BrightnessLevel brightness);
    }
}
