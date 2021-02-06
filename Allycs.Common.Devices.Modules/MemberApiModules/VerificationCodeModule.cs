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
            _verificationCodeService = verificationCodeService;
            _validatableCodeService = validatableCodeService;
            _memberTokenService = memberTokenService;
            _loginValidatableService = loginValidatableService;
            _memberLoginLogService = memberLoginLogService;
            _logger = logger;
            //注册验证码
            Get("/code/regist", p => GetRegistCodeAsync());
            Post("/code/regist/check/{code}", p => CheckRegistCodeAsync((string)p.code));
            ///短信验证码（验证码随机生成）
            Post("/code/verification", _ => GetCodeAsync());
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
                return Ok(new { code = await _verificationCodeService.GetAvailableRegistCodeByClientIpAsync(ClientIP).ConfigureAwait(false) });
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
            var err = await _validatableCodeService.CheckRegistCodeAsync(code, ClientIP).ConfigureAwait(false);
            if (!err.IsNullOrWhiteSpace())
                return PreconditionFailed(err);
            return Ok(new { message = "验证合法" });
        }
    }
}