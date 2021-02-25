namespace Allycs.Common.Devices.Services
{
    using Allycs.Common.Devices.Dtos;
    using Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUploadImagesService
    {
        Task<bool> NewUploadImageAsync(UploadImages entity);

        Task<UploadImages> GetUploadImageAsync(string id);
        Task<IEnumerable<UploadImages>> GetFarmInfosAsync(GetUploadImageListCmd cmd, PagedListQuery plQuery);
    }
}