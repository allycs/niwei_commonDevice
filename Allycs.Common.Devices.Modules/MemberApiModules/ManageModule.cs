using Allycs.Common.Devices.Dtos;
using Allycs.Common.Devices.Entities;
using Allycs.Common.Devices.Services;
using Allycs.Core;
using Microsoft.Extensions.Logging;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allycs.Common.Devices.Modules.MemberApiModules
{
    public class ManageModule : NancyAuthApiModule
    {
        private readonly IMemberService _memberService;
        private readonly IMemberTokenService _memberTokenService;
        private readonly IVerificationCodeService _verificationCodeService;
        private IValidatableCodeService _validatableCodeService;
        private readonly ILoginValidatableService _loginValidatableService;

        private readonly IMemberLoginLogService _memberLoginLogService;
        protected readonly ILogger<ManageModule> _logger;

        public ManageModule(
            IMemberService memberService,
            IMemberTokenService memberTokenService,
            ILoginValidatableService loginValidatableService,
            IVerificationCodeService verificationCodeService,
            IValidatableCodeService validatableCodeService,
        IMemberLoginLogService memberLoginLogService,
            ILogger<ManageModule> logger) : base(memberTokenService, memberService)
        {
            _memberService = memberService;
            _memberTokenService = memberTokenService;
            _loginValidatableService = loginValidatableService;
            _verificationCodeService = verificationCodeService;
            _validatableCodeService = validatableCodeService;
            _memberLoginLogService = memberLoginLogService;
            _logger = logger;

            //修改密码验证码
            Get("/code/renew-password", p => GetRenewPasswordCodeAsync());
            Post("/code/renew-password/check/{code}", p => CheckRenewPasswordCodeAsync((string)p.code));

            Get("/check-auth", _ => CheckAuth());
            Post("/renew-password", _ => DoRenewPasswordAsync());

        }

        private Response CheckAuth()
        {
            return Ok(CurrentToken);
        }
        private async Task<Response> DoRenewPasswordAsync()
        {
            var cmd = this.BindAndValidate<RenewPasswordCmd>();
            if (!ModelValidationResult.IsValid)
                return BadRequest(ModelValidationResult.Errors.First().Value.ToString());

            var timeNow = DateTime.Now;
            var err = await _loginValidatableService.CheckRenewPasswordCmdValidatableAsync(CurrentMemberId, cmd, ClientIP, timeNow).ConfigureAwait(false);
            if (!err.IsNullOrWhiteSpace())
                return PreconditionFailed(err);
            var verificationCode = await _verificationCodeService.GetAvailableCode(CurrentMemberId, cmd.CheckCode, CodeType.RenewPassword).ConfigureAwait(false);

            await _verificationCodeService.UpdateVerificationCodeToDisabledByUsed(verificationCode.Id).ConfigureAwait(false);

            var passwordFormat = EnumHelper.Random(PasswordFormatType.None);
            var passwordSalt = HashGenerator.Salt();
            var password = HashGenerator.Encode(cmd.Password, passwordFormat, passwordSalt);
            var accounts = await _memberService.GetMemberAccountsByMemberIdAsync(CurrentMemberId).ConfigureAwait(false);
            foreach (var item in accounts)
            {
                item.Password = password;
                item.PasswordSalt = passwordSalt;
                item.PasswordFormat = passwordFormat;
                await _memberService.UpdateMemberAccountAsync(item).ConfigureAwait(false);
            }
            return Ok(new { message = $"密码已更新,请重新登录！" });
        }


        private async Task<Response> GetRenewPasswordCodeAsync()
        {
            var memberInfo = await _memberService.GetMemberInfoAsync(CurrentMemberId).ConfigureAwait(false);
            var exist = await _verificationCodeService.ExistAvailableRenewPasswordCodeAsync(CurrentMemberId, ClientIP).ConfigureAwait(false);
            if (exist)
            {
                var code = await _verificationCodeService.GetAvailableRenewPasswordCodeByClientIpAsync(CurrentMemberId, ClientIP).ConfigureAwait(false);
                return Ok(new { code = code });
            }
            else
            {
                var help = VerifyCodeHelper.GetSingleObj();
                var code = help.CreateVerifyCode(VerifyCodeHelper.VerifyCodeType.MixVerifyCode);
                var entity = new VerificationCode
                {
                    Code = code,
                    MemberId = CurrentMemberId,
                    Type = CodeType.RenewPassword,
                    CreatedOn = DateTime.Now,
                    IsDisabled = false,
                    ClientIp = ClientIP
                };
                await _verificationCodeService.NewVerificationCodeAsync(entity).ConfigureAwait(false);
                return Ok(new { code = code });
            }
        }

        private async Task<Response> CheckRenewPasswordCodeAsync(string code)
        {
            var err = await _validatableCodeService.CheckRenewPasswordCodeAsync(CurrentMemberId, ClientIP,code).ConfigureAwait(false);
            if (!err.IsNullOrWhiteSpace())
                return PreconditionFailed(err);
            return Ok(new { message = "验证合法" });
        }
    }
}