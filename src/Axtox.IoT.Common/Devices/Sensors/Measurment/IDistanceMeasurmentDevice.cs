namespace Axtox.IoT.Common.Devices.Sensors.Measurment
{
    public delegate void DistanceReadyHandler(ushort distanceInMillimeter);

    public interface IDistanceMeasurmentDevice
    {
        event DistanceReadyHandler DistanceReady;
        ushort DistanceInMillimeter { get; }
        void StartMeasurment();
        void StopMeasurment();
    }
}
