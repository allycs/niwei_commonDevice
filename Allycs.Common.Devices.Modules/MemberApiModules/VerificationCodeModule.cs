namespace Allycs.Common.Devices.Modules.MemberApiModules
{
    using Allycs.Core;
    using Dtos;
    using Entities;
    using Microsoft.Extensions.Logging;
    using Nancy;
    using Nancy.ModelBinding;
    using Services;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class VerificationCodeModule : BaseNancyModule
    {
        private readonly IMemberService _memberService;
        private readonly IVerificationCodeService _verificationCodeService;
        private IValidatableCodeService _validatableCodeService;
        private readonly IMemberTokenService _memberTokenService;

        //private readonly IAdministrativeDivisionsValidatableService _administrativeDivisionsValidatableService;
        private readonly ILoginValidatableService _loginValidatableService;

        private readonly IMemberLoginLogService _memberLoginLogService;
        protected readonly ILogger<VerificationCodeModule> _logger;

        public VerificationCodeModule(
            IMemberService memberService,
            IValidatableCodeService validatableCodeService,
            IMemberTokenService memberTokenService,
            ILoginValidatableService loginValidatableService,
            IVerificationCodeService verificationCodeService,
            IMemberLoginLogService memberLoginLogService,
            ILogger<VerificationCodeModule> logger)
        {
            _memberService = memberService;
            _validatableCodeService = validatableCodeService;
            _memberTokenService = memberTokenService;
            _loginValidatableService = loginValidatableService;
            _verificationCodeService = verificationCodeService;
            _memberLoginLogService = memberLoginLogService;
            _logger = logger;
            //注册验证码
            Get("/code/regist", p => GetRegistCodeAsync());
            Post("/code/regist/check/{code}", p => CheckRegistCodeAsync((string)p.code));
            ///短信验证码（验证码随机生成）
            Post("/code/verification", _ => GetCodeAsync());
            //修改密码验证码
            Get("/code/renew-password", p => GetRenewPasswordCodeAsync());
            Post("/code/renew-password", p => CheckRenewPasswordCodeAsync());
            //身份验证码
            Get("/code/authentication/{phone}", p => SendAuthenticationCodeAsync((string)p.phone));
            Post("/code/authentication/check", p => CheckAuthenticationCodeAsync());
        }

        private async Task<Response> GetCodeAsync()
        {
            var cmd = this.BindAndValidate<VerificationCodeCmd>();
            if (!ModelValidationResult.IsValid)
                return BadRequest(ModelValidationResult.Errors.First().Value.ToString());
            var timeNow = DateTime.Now;
            var err = await _validatableCodeService.CheckVerificationCodeCmdValidatableAsync(cmd, ClientIP, timeNow).ConfigureAwait(false);
            if (!err.IsNullOrWhiteSpace())
                return PreconditionFailed(err);

            return Ok("待实现");
        }

        private async Task<Response> GetRegistCodeAsync()
        {
            var exist = await _verificationCodeService.ExistAvailableRegistCodeByClientIpAsync(ClientIP).ConfigureAwait(false);
            if (exist)
                return Ok(new { code = await _verificationCodeService.GetAvailableRegistCodeByClientIpAsync(ClientIP).ConfigureAwait(false));
            else
            {
                var help = VerifyCodeHelper.GetSingleObj();
                var code = help.CreateVerifyCode(VerifyCodeHelper.VerifyCodeType.MixVerifyCode);
                var entity = new VerificationCode
                {
                    Code = code,
                    Type = CodeType.Regist,
                    CreatedOn = DateTime.Now,
                    IsDisabled = false,
                    ClientIp = ClientIP
                };
                await _verificationCodeService.NewVerificationCodeAsync(entity).ConfigureAwait(false);
                return Ok(new { code = code });
            }
        }

        private async Task<Response> CheckRegistCodeAsync(string code)
        {
            var cmd = this.BindAndValidate<CheckCodeCmd>();
            var err = await _validatableCodeService.CheckRegistCodeAsync(code, ClientIP).ConfigureAwait(false);
            if (!err.IsNullOrWhiteSpace())
                return PreconditionFailed(err);
            return Ok(new { message = "验证合法" });
        }

        private async Task<Response> GetRenewPasswordCodeAsync()
        {
          
            if (!await _memberService.ExistMobilePhoneAsync(phone).ConfigureAwait(false))
                return PreconditionFailed("手机号未注册");
            var memberInfo = await _memberService.GetMemberInfoByPhoneAsync(phone).ConfigureAwait(false);
            if (memberInfo.Type == MemberType.Employee && !memberInfo.MainMemberId.IsNullOrWhiteSpace())
                return PreconditionFailed("子账号无权修改密码，请主账号联系管理员");
            var result = await _smsService.SendRenewPasswordCodeAsync(phone, SmsCode.GetSmsCode(6), ClientIP).ConfigureAwait(false);
            return Ok(new { message = result });
        }

        private async Task<Response> CheckRenewPasswordCodeAsync()
        {
            var cmd = this.BindAndValidate<CheckCodeCmd>();
            var err = await _sendSmsValidatableService.CheckRenewPasswordCodeAsync(cmd, ClientIP).ConfigureAwait(false);
            if (!err.IsNullOrWhiteSpace())
                return PreconditionFailed(err);
            return Ok(new { message = "验证合法" });
        }

        private async Task<Response> SendAuthenticationCodeAsync(string phone)
        {
            var can = await _smsService.CanSendCodeAsync(phone).ConfigureAwait(false);
            if (!can)
                return PreconditionFailed("验证码发送过于频繁");
            var result = await _smsService.SendAuthenticationCodeAsync(phone, SmsCode.GetSmsCode(6), ClientIP).ConfigureAwait(false);
            return Ok(new { message = result });
        }

        private async Task<Response> CheckAuthenticationCodeAsync()
        {
            var cmd = this.BindAndValidate<CheckCodeCmd>();
            var err = await _sendSmsValidatableService.CheckAuthenticationCodeAsync(cmd, ClientIP).ConfigureAwait(false);
            if (!err.IsNullOrWhiteSpace())
                return PreconditionFailed(err);
            return Ok(new { message = "验证合法" });
        }
    }
}