namespace Axtox.IoT.Common.Devices.Sensors.Measurement
{
    public delegate void DistanceReadyHandler(ushort distanceInMillimeter);

    public interface IDistanceMeasurementDevice
    {
        event DistanceReadyHandler DistanceReady;
        ushort DistanceInMillimeter { get; }
        void StartMeasurement();
        void StopMeasurement();
    }
}
