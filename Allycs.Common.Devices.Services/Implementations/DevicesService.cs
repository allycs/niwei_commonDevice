namespace Allycs.Common.Devices.Services
{
    using Allycs.Common.Devices.Entities;
    using Dapper;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System.Threading.Tasks;

    public class DevicesService : PostgresService, IDevicesService
    {
        private readonly AppSettings _settings;
        private readonly ILogger<DevicesService> _logger;

        public DevicesService(IOptionsSnapshot<AppSettings> option,
            ILogger<DevicesService> logger)
            : base(option)
        {
            _settings = option.Value;
            _logger = logger;
        }

        public async Task<bool> ExistDeviceInfoAsync(string id)
        {
            using var conn = CreateConnection();
            return (await conn.RecordCountAsync<DeviceInfo>($" WHERE id='{id}'").ConfigureAwait(false)) > 0;
        }

        public async Task<bool> NewDeviceInfoAsync(DeviceInfo entity)
        {
            using var conn = CreateConnection();
            return await conn.InsertAsync<DeviceInfo>(entity).ConfigureAwait(false);
        }

        public async Task<DeviceInfo> GetDeviceInfoAsync(string id)
        {
            using var conn = CreateConnection();
            return await conn.GetAsync<DeviceInfo>(id).ConfigureAwait(false);
        }
        public async Task<bool> UpdateDeviceInfoAsync(DeviceInfo entity)
        {
            using var conn = CreateConnection();
            return await conn.UpdateAsync(entity).ConfigureAwait(false)>0;
        }
    }
}