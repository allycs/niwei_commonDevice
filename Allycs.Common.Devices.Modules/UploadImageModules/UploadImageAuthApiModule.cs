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
    public class UploadImageAuthApiModule : NancyAuthApiModule
    {
        protected readonly ILogger<UploadImageAuthApiModule> _logger;
        private readonly IFarmService _farmService;
        private readonly IFarmValidatableService _farmValidatableService;
        public UploadImageAuthApiModule(
            IMemberService memberService,
            IMemberTokenService memberTokenService,
            IFarmService farmService,
            IFarmValidatableService farmValidatableService,
            ILogger<UploadImageAuthApiModule> logger) : base(memberTokenService, memberService,"upload-image")
        {

            _logger = logger;
            _farmService = farmService;
            _farmValidatableService = farmValidatableService;
            Post("/upload", _ => UploadImageAsync());
        }


        private async Task<Response> UploadImageAsync()
        {
            var cmd = this.Bind<UploadImagesCmd>();
            var fileCount = Request.Files.Count();
            if (fileCount < 1) return PreconditionFailed("请至少上传一张图");
            cmd.Images = new List<HttpFile>();
            foreach (var item in Request.Files)
            {
                cmd.Images.Add(item);
            }
            var err = await _farmValidatableService.CheckNewFarmInfoCmdValidatableAsync(cmd).ConfigureAwait(false);
            if (!err.IsNullOrWhiteSpace())
                return PreconditionFailed(err);
            return Ok(await _farmService.NewFarmInfoAsync(cmd, CurrentMemberId).ConfigureAwait(false));
        }
    }
}