namespace Allycs.Common.Devices.Modules.FarmApiModules
{
    using Dtos;
    using Services;
    using Microsoft.Extensions.Logging;
    using Nancy;
    using Nancy.ModelBinding;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.IO;

    public class UploadImageAuthApiModule : NancyAuthApiModule
    {
        protected readonly ILogger<UploadImageAuthApiModule> _logger;
        private readonly IUploadImagesService _uploadImagesService;
        private readonly IFarmValidatableService _farmValidatableService;

        public UploadImageAuthApiModule(
            IMemberService memberService,
            IMemberTokenService memberTokenService,
            IUploadImagesService uploadImagesService,
            IFarmValidatableService farmValidatableService,
            ILogger<UploadImageAuthApiModule> logger) : base(memberTokenService, memberService, "upload-image")
        {
            _logger = logger;
            _uploadImagesService = uploadImagesService;
            _farmValidatableService = farmValidatableService;
            Post("/upload", _ => UploadImageAsync());
            Get("/list", _ => GetUploadImagesAsync());
            Get("/image/{id}/{extension}", p => GetUploadImage((string)p.id,(string)p.extension));
        }

        private Response GetUploadImage(string id,string extension)
        {
            var failPath = Path.Combine(_uploadImagesService.GetUploadDirectory(), id + "." + extension);
            return Response.AsFile(failPath);
        }

        private async Task<Response> GetUploadImagesAsync()
        {
            var cmd = this.Bind<GetUploadImageListCmd>();
            var plQuery = this.Bind<PagedListQuery>();
            var result = await _uploadImagesService.GetFarmInfosAsync(cmd, plQuery).ConfigureAwait(false);
            return Ok(result);
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
            //var err = await _.CheckNewFarmInfoCmdValidatableAsync(cmd).ConfigureAwait(false);
            //if (!err.IsNullOrWhiteSpace())
            //    return PreconditionFailed(err);
            return Ok(await _uploadImagesService.NewUploadImageAsync(cmd, CurrentMemberId).ConfigureAwait(false));
        }
    }
}