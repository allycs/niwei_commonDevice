using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices.Services
{
    using Dtos;
    using Dapper;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Allycs.Core;

    public class ValidatableCodeService : PostgresService, IValidatableCodeService
    {
        private readonly ILogger<ValidatableCodeService> _logger;
        private readonly IMemberService _memberService;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly IMemberLoginLogService _memberLoginLogService;

        public ValidatableCodeService(IOptionsSnapshot<AppSettings> option,
            IMemberService memberService,
            IVerificationCodeService verificationCodeService,
            IMemberLoginLogService memberLoginLogService,
            ILogger<ValidatableCodeService> logger)
            : base(option)
        {
            _logger = logger;
            _memberService = memberService;
            _verificationCodeService = verificationCodeService;
            _memberLoginLogService = memberLoginLogService;
        }

        public async Task<string> CheckVerificationCodeCmdValidatableAsync(VerificationCodeCmd cmd, string clientIp, DateTime timeNow)
        {
            if (cmd.MemberId.IsNullOrWhiteSpace())
                return "请输入手机号";
            if (cmd.CodeType == CodeType.All )
                return "请指定验证码类型";
            return null;
        }

        public async Task<string> CheckSendRegistCodeAsync(string phoneNumber)
        {
            if (await _memberService.ExistAccountAsync(phoneNumber))
                return "手机号已注册";
            if (await _memberService.ExistMobilePhoneAsync(phoneNumber))
                return "手机号已被使用";
            //if (!await _smsService.CanSendCodeAsync(phoneNumber).ConfigureAwait(false))
            //    return "验证码发送过于频繁";
            return null;
        }

        public async Task<string> CheckRegistCodeAsync(string code, string clientIp)
        {
            if (code.IsNullOrWhiteSpace() || clientIp.IsNullOrWhiteSpace() || !clientIp.isIP())
                return "数据验证不足！";
            return await CheckCodeAsync(new CheckCodeCmd { CheckCode=code,}, CodeType.Regist, clientIp).ConfigureAwait(false);
        }

        public async Task<string> CheckRenewPasswordCodeAsync(string memberId, string clientIp, string code)
        {
            if (code.IsNullOrWhiteSpace())
                return "验证码不可为空";
            if (!await _verificationCodeService.ExistAvailableRenewPasswordCodeAsync(memberId, clientIp, code).ConfigureAwait(false))
            {
                return "验证码错误";
            }
            return null;
        }

        public async Task<string> CheckAuthenticationCodeAsync(CheckCodeCmd cmd, string clientIp)
        {
            return await CheckCodeAsync(cmd, CodeType.All, clientIp).ConfigureAwait(false);
        }

        public async Task<string> CheckCodeAsync(CheckCodeCmd cmd, CodeType type, string clientIp)
        {
            if (cmd.CheckCode.IsNullOrWhiteSpace())
                return "验证码不可为空";
            if (!await _verificationCodeService.ExistAvailableCode(cmd.MemberId, cmd.CheckCode, type).ConfigureAwait(false))
            {
                return "验证码错误";
            }
            var verificationCode = await _verificationCodeService.GetAvailableCode(cmd.MemberId, cmd.CheckCode, type).ConfigureAwait(false);

            return null;
        }
    }
}
