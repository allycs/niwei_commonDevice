using Allycs.Common.Devices.Dtos;
using Allycs.Common.Devices.Entities;
using Allycs.Common.Devices.Services;
using Allycs.Core;
using Microsoft.Extensions.Logging;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allycs.Common.Devices.Modules.DevicesModules
{
    public class DeviceAuthApiModule : NancyDeviceAuthApiModule
    {
        protected readonly ILogger<DeviceAuthApiModule> _logger;
        private readonly IDevicesService _devicesService;
        private readonly IDeviceValidatableService _deviceValidatableService;
        public DeviceAuthApiModule(
            IMemberService memberService,
            IMemberTokenService memberTokenService,
            IDevicesService devicesService,
            IDeviceValidatableService deviceValidatableService,
            ILogger<DeviceAuthApiModule> logger) : base(memberTokenService, memberService)
        {

            _logger = logger;
            _devicesService = devicesService;
            _deviceValidatableService = deviceValidatableService;
            Post("/device", _ => NewDeviceAsync());

        }
        private async Task<Response> NewDeviceAsync()
        {
            var cmd = this.Bind<NewDeviceInfoCmd>();

            var timeNow = DateTime.Now;
            var err = await _deviceValidatableService.CheckNewDeviceInfoCmdValidatableAsync(cmd).ConfigureAwait(false);
            if (!err.IsNullOrWhiteSpace())
                return PreconditionFailed(err);
            var deviceInfo = new DeviceInfo
            {
                DeviceCode = cmd.DeviceCode,
                DeviceName = cmd.DeviceName,
                SerialNumber = cmd.SerialNumber,
                Longitude = cmd.Longitude,
                Latitude = cmd.Latitude,
                ConfigJson = cmd.ConfigJson,
                Remark = cmd.Remark,
                Address = cmd.Address,
                Status = cmd.Status,
                Type = cmd.Type,
                RegistBy = CurrentMemberId,
                RegistOn = timeNow,
                ModifiedOn = timeNow
            };
            if (await _devicesService.NewDeviceInfoAsync(deviceInfo).ConfigureAwait(false))
                return Ok(deviceInfo);
            return Conflict("服务端请求冲突,请联系管理员！" );
        }
    }
}