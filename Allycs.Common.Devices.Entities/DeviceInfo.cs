namespace Allycs.Common.Devices.Entities
{
    using Allycs.Core;
    using Dapper;
    using System;

    public class DeviceInfo
    {
        [Key]
        public string Id { get; set; } = ObjectId.NewId();

        public string DeviceCode { get; set; }
        public string DeviceName { get; set; }
        public string SerialNumber { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string ConfigJson { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public string Address { get; set; }
        public DeviceStatus Status { get; set; }
        public DeviceType Type { get; set; }
        public string RegistBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime RegistOn { get; set; } = DateTime.Now;
    }
}