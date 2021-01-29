namespace Allycs.Common.Devices.Dtos
{
    public class SensorDataDto
    {
        public string Id { get; set; }
        public int Temperature { get; set; }
        public int Humidity { get; set; }
        /// <summary>
        /// 二氧化碳
        /// </summary>
        public int CarbonDioxide { get; set; }
        /// <summary>
        /// PH酸碱度
        /// </summary>
        public int AcidBase { get; set; }
        /// <summary>
        /// 氮含量
        /// </summary>
        public int Nitrogen { get; set; }
        /// <summary>
        /// 磷含量
        /// </summary>
        public int Phosphorus { get; set; }
        /// <summary>
        /// 钾含量
        /// </summary>
        public int Potassium { get; set; }
        /// <summary>
        /// 光照度
        /// </summary>
        public int Illuminance { get; set; }
        public int DeviceType { get; set; }
        public string Remark { get; set; }
        public string Information { get; set; }
        public string UpdateOn { get; set; }
    }
}
