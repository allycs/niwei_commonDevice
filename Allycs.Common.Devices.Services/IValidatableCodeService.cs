using Allycs.Common.Devices.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allycs.Common.Devices.Services
{
    public interface IValidatableCodeService
    {
        Task<string> CheckVerificationCodeCmdValidatableAsync(VerificationCodeCmd cmd, string clientIp, DateTime timeNow);
        
        Task<string> CheckRegistCodeAsync(string code, string clientIp);
        Task<string> CheckRenewPasswordCodeAsync(string memberId, string clientIp,string code);
        Task<string> CheckAuthenticationCodeAsync(CheckCodeCmd cmd, string clientIp);
        Task<string> CheckCodeAsync(CheckCodeCmd cmd, CodeType type, string clientIp);
    }
}
