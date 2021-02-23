namespace Allycs.Common.Devices.Services
{
    using Allycs.Common.Devices.Dtos;
    using Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFarmService
    {
        string GetUploadDirectory();
        Task<bool> ExistFarmInfoAsync(string id);
        Task<bool> ExistFarmInfoByNameAsync(string farmName);
        Task<bool> ExistFarmInfoByTelephoneAsync(string telephone);
        Task<bool> NewFarmInfoAsync(FarmInfo entity);
        Task<string> NewFarmInfoAsync(NewFarmInfoCmd cmd,string currentMemberId);
        Task<FarmInfo> GetFarmInfoAsync(string id);
        Task<bool> UpdateFarmInfoAsync(FarmInfo entity);
        Task<string> UpdateFarmInfoAsync(UpdateFarmInfoCmd cmd, string currentMemberId);
        Task<IEnumerable<FarmInfo>> GetFarmInfosAsync(GetFarmInfoListCmd cmd, PagedListQuery plQuery);
    }
}