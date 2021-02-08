namespace Allycs.Common.Devices.Services
{
    using Allycs.Common.Devices.Dtos;
    using Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDevicesService
    {
        Task<bool> ExistDeviceInfoAsync(string id);
        Task<bool> ExistDeviceInfoByDeviceCodeAsync(string deviceCode);
        Task<bool> ExistDeviceInfoByDeviceSerialNumberAsync(string serialNumber);
        Task<bool> NewDeviceInfoAsync(DeviceInfo entity);
        Task<DeviceInfo> GetDeviceInfoAsync(string id);
        Task<bool> UpdateDeviceInfoAsync(DeviceInfo entity);

        #region DeviceData
        Task<bool> NewDeviceDataAsync(DeviceData entity);
        Task<IEnumerable<DeviceData>> GetDeviceDataSAsync(GetDeviceDataListCmd cmd, PagedListQuery plQuery);
        Task NewMultisensorDeviceDataAsync(SensorDataDto dto);
        #endregion

    }
}