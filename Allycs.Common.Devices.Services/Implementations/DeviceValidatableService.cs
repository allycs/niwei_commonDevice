using Allycs.Common.Devices.Dtos;
using Allycs.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allycs.Common.Devices.Services
{
    public class DeviceValidatableService : PostgresService, IDeviceValidatableService
    {
        private readonly AppSettings _settings;
        private readonly IDevicesService _devicesService;
        private readonly ILogger<DeviceValidatableService> _logger;

        public DeviceValidatableService(
            IOptionsSnapshot<AppSettings> option,
            IDevicesService devicesService,
            ILogger<DeviceValidatableService> logger)
            : base(option)
        {
            _settings = option.Value;
            _devicesService = devicesService;
            _logger = logger;
        }
        public async Task<string> CheckNewDeviceInfoCmdValidatableAsync(NewDeviceInfoCmd cmd)
        {
            if (cmd.DeviceCode.IsNullOrWhiteSpace())
                return "设备编号必填";
            if (await _devicesService.ExistDeviceInfoByDeviceCodeAsync(cmd.DeviceCode).ConfigureAwait(false))
                return "设备编号已存在";
            if (cmd.DeviceName.IsNullOrWhiteSpace())
                return "设备名称必填";
            if (cmd.SerialNumber.IsNullOrWhiteSpace())
                return "设备序列号必填";
            if (await _devicesService.ExistDeviceInfoByDeviceSerialNumberAsync(cmd.SerialNumber).ConfigureAwait(false))
                return "设备序列号已存在";
            return null;

        }
    }
}
