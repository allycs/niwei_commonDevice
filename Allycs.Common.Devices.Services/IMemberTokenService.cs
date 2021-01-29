namespace Allycs.Common.Devices.Services
{
    using Entities;
    using Allycs.Core;
    using System.Threading.Tasks;

    public interface IMemberTokenService
    {
        Task<MemberToken> GetMemberTokenAsync(string Token);

        Task<bool> ExistAvailableTokenAsync(string token, ClientType type);

        Task<bool> ExistAvailableTokenAsync(string token);

        bool ExistWechatUserToken(string openId);

        Task<int> CountDeviceOnlineAsync();

        Task<int> CountTodayTokenAsync();

        Task UpdateMemberTokenToDisabledByTimeAsync();

        Task UpdateMemberTokenToDisabledByNewCode(string memberId, ClientType type);

        Task UpdateMemberTokenToDisabledByMemberId(string memberId);

        Task<ObjectId> NewMemberTokenAsync(string memberId, ClientType type, string clientIp);

        Task DoLogoutAsync(string token);
    }
}