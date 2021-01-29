
namespace Allycs.Common.Devices.Services
{
    using Allycs.Common.Devices.Dtos;
    using Allycs.Common.Devices.Entities;
    using Allycs.Core;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public interface IMemberService
    {
        bool ExistWechatMemberInfo(string openId);
        Task<bool> ExistMemberInfoAsync(string memberId);
        Task<bool> ExistSubMemberAsync(string currentMemberId, string subMemberId);
        Task<bool> ExistPhoneNumberOrTeleNumberAsync(string phoneNumber);
        Task<bool> ExistIdCardAsync(string idCard);
        Task<bool> ExistAccountAsync(string account);
        Task<bool> ExistEmailAsync(string email);
        Task<bool> ExistMobilePhoneAsync(string mobilePhone);
        Task<MemberInfo> GetMemberInfoAsync(string id);
        Task<MemberInfo> GetMemberInfoByPhoneAsync(string phoneNumber);
        Task<MemberAccount> GetMemberAccountAsync(string account);
        Task<IEnumerable<MemberAccount>> GetMemberAccountsByMemberIdAsync(string memberId);
        Task<int> CountMemberInfoAsync();
        Task<bool> UpdateMemberInfoAsync(MemberInfo entity);
        Task<bool> UpdateMemberAccountAsync(MemberAccount entity);

        Task<bool> UpdateMemberAddressLastAsync(string addressCurrent, ObjectId currentMemberId);


        Task NewMemberInfoAsync(MemberInfo entity);

        Task NewMemberAccountAsync(MemberAccount entity);

        Task<IEnumerable<MemberInfo>> GetAllMemberInfoAsync(GetMemberListCmd cmd, PagedListQuery plQuery);
        Task<int> GetAllMemberInfoCountAsync(GetMemberListCmd cmd);

        Task<CommandResult> UpdateMemberPasswordAsync(string memberId, string newPassword);
    }
}
