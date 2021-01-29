namespace Allycs.Common.Devices
{
    public enum SensorType : int
    {
        Temperature = 0,
        Humidity = 1,
        CarbonDioxide = 2,
        /// <summary>
        /// 酸碱
        /// </summary>
        AcidBase = 3,
        /// <summary>
        /// 氮
        /// </summary>
        Nitrogen = 4,
        /// <summary>
        /// 磷
        /// </summary>
        Phosphorus = 5,
        /// <summary>
        /// 钾
        /// </summary>
        Potassium = 6,
        /// <summary>
        /// 光度
        /// </summary>
        Illuminance = 7
    }
}
