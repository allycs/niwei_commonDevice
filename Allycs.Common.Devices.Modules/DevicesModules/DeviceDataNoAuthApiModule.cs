namespace Allycs.Common.Devices.Modules.DevicesModules
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nancy;
    using Nancy.ModelBinding;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Allycs.Core;
    using System.IO;
    using Allycs.Common.Devices.Dtos;
    using Allycs.Common.Devices.Services;

    public class DeviceDataNoAuthApiModule : BaseNancyModule
    {
        protected readonly AppSettings _settings;
        protected readonly IDevicesService _devicesService;
        protected readonly ILogger<DeviceDataNoAuthApiModule> _logger;
        public DeviceDataNoAuthApiModule(
            IDevicesService devicesService,
            IOptionsSnapshot<AppSettings> option,
        ILogger<DeviceDataNoAuthApiModule> logger) : base("device", "api", "")
        {
            _devicesService = devicesService;
            _settings = option.Value;
            _logger = logger;

            Post("/data", _ => UploadDeviceDataAsync());
            //TODO 改为需要身份验证
            Get("/data", _ => GetDeviceDataAsync());
        }

        private async Task<Response> UploadDeviceDataAsync()
        {
            var cmd = this.Bind<SensorDataDto>();
            if(cmd.Id.IsNullOrWhiteSpace()||cmd.Id.Length!=24)
                return Forbidden("设备不存在！");
            var existDevice = await _devicesService.ExistDeviceInfoAsync(cmd.Id).ConfigureAwait(false);
            if (!existDevice)
                return Forbidden("设备不存在！");
            await _devicesService.NewMultisensorDeviceDataAsync(cmd).ConfigureAwait(false);
            return Ok(cmd);
        }

        private async Task<Response> GetDeviceDataAsync()
        {
            var cmd = this.BindAndValidate<GetDeviceDataListCmd>();
            var plQuery = this.BindAndValidate<PagedListQuery>();
            var result = await _devicesService.GetDeviceDataSAsync(cmd, plQuery).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
