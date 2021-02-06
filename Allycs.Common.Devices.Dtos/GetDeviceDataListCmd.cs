namespace Allycs.Common.Devices.Dtos
{
    using System;

    public class GetDeviceDataListCmd
    {
        public SensorType? Type { get; set; }
        public DateTime? StartOn { get; set; }
        public DateTime? EndOn { get; set; }
        public string DeviceId { get; set; }
    }
}