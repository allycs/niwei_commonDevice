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

namespace Allycs.Common.Devices.Modules.FarmApiModules
{
    public class FarmsAuthApiModule : NancyFarmAuthApiModule
    {
        protected readonly ILogger<FarmsAuthApiModule> _logger;
        private readonly IFarmService _farmService;
        private readonly IFarmValidatableService _farmValidatableService;
        public FarmsAuthApiModule(
            IMemberService memberService,
            IMemberTokenService memberTokenService,
            IFarmService farmService,
            IFarmValidatableService farmValidatableService,
            ILogger<FarmsAuthApiModule> logger) : base(memberTokenService, memberService)
        {

            _logger = logger;
            _farmService = farmService;
            _farmValidatableService = farmValidatableService;
            Post("/farm", _ => NewDeviceAsync());

        }
        private async Task<Response> NewDeviceAsync()
        {
            var cmd = this.Bind<NewFarmInfoCmd>();
            var fileCount = Request.Files.Count();
            if (fileCount != 1) return PreconditionFailed("请上传主图");
            cmd.MainImg = Request.Files.FirstOrDefault();
            var err = await _farmValidatableService.CheckNewFarmInfoCmdValidatableAsync(cmd).ConfigureAwait(false);
            if (!err.IsNullOrWhiteSpace())
                return PreconditionFailed(err);
            return Ok(await _farmService.NewFarmInfoAsync(cmd, CurrentMemberId).ConfigureAwait(false));
        }
    }
}