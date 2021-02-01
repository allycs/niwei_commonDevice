namespace Allycs.Common.Devices.Services
{
    using Entities;
    using System.Threading.Tasks;

    public interface IDevicesService
    {
        Task<bool> ExistDeviceInfoAsync(string id);

        Task<bool> NewDeviceInfoAsync(DeviceInfo entity);
        Task<DeviceInfo> GetDeviceInfoAsync(string id);
        Task<bool> UpdateDeviceInfoAsync(DeviceInfo entity);
    }
}