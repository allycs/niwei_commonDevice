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
            Post("/", _ => UploadDeviceDataAsync());
        }

        private async Task<Response> UploadDeviceDataAsync()
        {
            var cmd = this.Bind<SensorDataDto>();
            return Ok(cmd);
        }
    }
}
