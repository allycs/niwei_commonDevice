using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Allycs.Common.Devices.Entities;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Allycs.Common.Devices.Services
{
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
        public async Task<bool> ExistDeviceAsync(string id)
        {
            using (var conn = CreateConnection())
            {
                return (await conn.RecordCountAsync<DeviceInfo>($" WHERE id='{id}'").ConfigureAwait(false)) > 0;
            }
        }
    }
}
