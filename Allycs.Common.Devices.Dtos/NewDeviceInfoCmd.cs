namespace Allycs.Common.Devices.Dtos
{
    using System;

    public class NewDeviceInfoCmd
    {
        public string DeviceCode { get; set; }
        public string DeviceName { get; set; }
        public string SerialNumber { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string ConfigJson { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public string Address { get; set; }
        public DeviceStatus Status { get; set; } = DeviceStatus.Enable;
        public DeviceType Type { get; set; } = DeviceType.Multisensor;
    }
}