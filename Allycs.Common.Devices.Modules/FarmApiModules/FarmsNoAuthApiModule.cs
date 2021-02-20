using Allycs.Common.Devices.Dtos;
using Allycs.Common.Devices.Services;
using Allycs.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allycs.Common.Devices.Modules.FarmApiModules
{
    public class FarmsNoAuthApiModule : BaseNancyModule
    {
        protected readonly AppSettings _settings;
        protected readonly IFarmService _farmService;
        protected readonly ILogger<FarmsNoAuthApiModule> _logger;
        public FarmsNoAuthApiModule(
            IFarmService farmService,
            IOptionsSnapshot<AppSettings> option,
        ILogger<FarmsNoAuthApiModule> logger) : base("farm", "api", "")
        {
            _farmService = farmService;
            _settings = option.Value;
            _logger = logger;

            Get("/farms", _ => GetFarmListAsync());
        }

        private async Task<Response> GetFarmListAsync()
        {
            var cmd = this.Bind<GetFarmInfoListCmd>();
            var plQuery = this.Bind<PagedListQuery>();
            var result = await _farmService.GetFarmInfosAsync(cmd, plQuery).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
