namespace Allycs.Common.Devices.Services
{
    using Dtos;
    using System;
    using System.Threading.Tasks;

    public interface ILoginValidatableService
    {
        Task<string> CheckRegistCmdValidatableAsync(RegistCmd cmd, string clientIp, DateTime timeNow);

        Task<string> ChekcLoginCmdValidatableAsync(LoginCmd cmd, string clientIp, DateTime timeNow);

        Task<string> CheckRenewPasswordCmdValidatableAsync(string currentMemberId, RenewPasswordCmd cmd, string clientIP, DateTime timeNow);
    }
}