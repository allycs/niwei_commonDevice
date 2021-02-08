namespace Allycs.Common.Devices.Services
{
    using Allycs.Common.Devices.Dtos;
    using Allycs.Common.Devices.Entities;
    using Allycs.Core;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Threading.Tasks;

    public class LoginValidatableService : PostgresService, ILoginValidatableService
    {
        private readonly ILogger<LoginValidatableService> _logger;
        private readonly IMemberService _memberService;
        private readonly IMemberLoginLogService _memberLoginLogService;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly IValidatableCodeService _validatableCodeService;

        public LoginValidatableService(IOptionsSnapshot<AppSettings> option,
            IMemberService memberService,
            IMemberLoginLogService memberLoginLogService,
            IVerificationCodeService verificationCodeService,
            IValidatableCodeService validatableCodeService,
            ILogger<LoginValidatableService> logger)
            : base(option)
        {
            _logger = logger;
            _memberService = memberService;
            _verificationCodeService = verificationCodeService;
            _validatableCodeService = validatableCodeService;
            _memberLoginLogService = memberLoginLogService;
        }

        public async Task<string> ChekcLoginCmdValidatableAsync(LoginCmd cmd, string clientIp, DateTime timeNow)
        {
            if (cmd.Account.IsNullOrWhiteSpace())
                return "账号不可为空";
            if (cmd.Password.IsNullOrWhiteSpace())
                return "密码不能为空";
            if (cmd.Password.Length > 30)
                return "密码超长";
            if (!await _memberService.ExistAccountAsync(cmd.Account).ConfigureAwait(false))
            {
                await _memberLoginLogService.NewLogAsync(new MemberLoginLog
                {
                    Account = cmd.Account,
                    Password = cmd.Password,
                    CheckOn = timeNow,
                    IsPass = false,
                    Reason = "账号不存在",
                    ClientIp = clientIp
                }).ConfigureAwait(false);
                return "账号不存在";
            }
            var memberAccount = await _memberService.GetMemberAccountAsync(cmd.Account).ConfigureAwait(false);
            if (memberAccount.IsLockout)
            {
                await _memberLoginLogService.NewLogAsync(new MemberLoginLog
                {
                    Account = cmd.Account,
                    Password = cmd.Password,
                    CheckOn = timeNow,
                    IsPass = false,
                    Reason = "账号被锁定",
                    ClientIp = clientIp
                }).ConfigureAwait(false);
                return "账号被锁定";
            }
            if (memberAccount.Password != HashGenerator.Encode(cmd.Password, memberAccount.PasswordFormat, memberAccount.PasswordSalt))
            {
                await _memberLoginLogService.NewLogAsync(new MemberLoginLog
                {
                    Account = cmd.Account,
                    Password = cmd.Password,
                    CheckOn = timeNow,
                    IsPass = false,
                    Reason = "密码不匹配",
                    ClientIp = clientIp
                }).ConfigureAwait(false);
                return "密码不匹配";
            }
            return null;
        }

        public async Task<string> CheckRegistCmdValidatableAsync(RegistCmd cmd, string clientIp, DateTime timeNow)
        {
            if (cmd.Account.IsNullOrWhiteSpace())
                return "账号不可为空";
            if (cmd.MobilePhone.IsNullOrWhiteSpace())
                return "手机号不可为空";
            if (cmd.Email.IsNullOrWhiteSpace())
                return "邮箱不可为空";
            if (cmd.RegistCode.IsNullOrWhiteSpace())
                return "推荐码不可为空";
            if (cmd.Password.IsNullOrWhiteSpace())
                return "密码不能为空";
            if (cmd.Password.Length > 30)
                return "密码超长";
            if (cmd.CheckCode.IsNullOrWhiteSpace())
                return "验证码必填";
            //if (cmd.CheckCode.Length != 6)
            //    return "验证码错误";
            var codeCheckStr = await _validatableCodeService.CheckRegistCodeAsync(cmd.CheckCode, clientIp).ConfigureAwait(false);
            if(!codeCheckStr.IsNullOrWhiteSpace())
                return codeCheckStr;
            if (!cmd.IdCard.IsNullOrWhiteSpace() && (await _memberService.ExistIdCardAsync(cmd.IdCard).ConfigureAwait(false)))
                return "用户身份证号已使用";
            if (await _memberService.ExistAccountAsync(cmd.Account).ConfigureAwait(false))
                return "账号已注册";
            if (await _memberService.ExistMobilePhoneAsync(cmd.MobilePhone).ConfigureAwait(false))
                return "手机号已注册";
            if (await _memberService.ExistEmailAsync(cmd.Email).ConfigureAwait(false))
                return "邮箱已注册";

            return null;
        }

        public async Task<string> CheckRenewPasswordCmdValidatableAsync(string currentMemberId, RenewPasswordCmd cmd, string clientIp, DateTime timeNow)
        {
            if (cmd.CheckCode.IsNullOrWhiteSpace())
                return "验证码不可为空";
            if (cmd.Password.IsNullOrWhiteSpace())
                return "密码不能为空";
            if (cmd.Password.Length > 30)
                return "密码超长";
            if (cmd.CheckCode.Length != 6)
                return "验证码错误";

            if (cmd.CodeType != CodeType.RenewPassword)
                return "验证码类型错误";

            await _verificationCodeService.UpdateVerificationCodeToDisabledByTimeAsync().ConfigureAwait(false);

            if (!await _verificationCodeService.ExistAvailableCode(currentMemberId, cmd.CheckCode, cmd.CodeType).ConfigureAwait(false))
                return "验证码错误";
            return null;
        }
    }
}