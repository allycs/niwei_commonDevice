namespace Allycs.Common.Devices.Services
{
    using Allycs.Common.Devices.Entities;
    using Allycs.Core;
    using System;
    using System.Threading.Tasks;

    public interface IVerificationCodeService
    {
        Task<bool> ExistAvailableCodeWithoutMemberAsync(string clientIp, string code, CodeType type);

        Task<bool> ExistAvailableRegistCodeByClientIpAsync(string clientIp);

        Task<int> NewVerificationCodeAsync(VerificationCode entity);

        Task<VerificationCode> NewVerificationCodeAsync(string code, CodeType type, DateTime timeNow, ObjectId? memberId = null, string nationCode = "86");

        Task UpdateVerificationCodeToDisabledByTimeAsync();

        Task UpdateVerificationCodeToDisabledByNewCode(string memberId, CodeType type);

        Task<bool> ExistAvailableCode(string memberId, string code, CodeType type);

        Task UpdateVerificationCodeToDisabledByUsed(int id);

        Task<VerificationCode> GetAvailableCode(string memberId, string code, CodeType type);

        Task<VerificationCode> GetAvailableRegistCodeByClientIpAsync(string ClientIp);
    }
}