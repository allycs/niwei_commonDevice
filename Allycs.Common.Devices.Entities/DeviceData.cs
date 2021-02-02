using Allycs.Core;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices.Entities
{
    public class DeviceData
    {
        [Key]
        public string Id { get; set; } = ObjectId.NewId();
        public string DeviceId { get; set; }
        public SensorType SensorType { get; set; }
        public int Data { get; set; }
        public string DataStr { get; set; }
        public string UnitName { get; set; }
        public string Remark { get; set; }
        public string Information { get; set; }
        public DateTime UpdateOn { get; set; }
    }
}
