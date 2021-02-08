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
using Mapster;

namespace Allycs.Common.Devices.Modules.MemberApiModules
{
    public class LoginModule : BaseNancyModule
    {
        private readonly IMemberService _memberService;
        private readonly IMemberTokenService _memberTokenService;
        IVerificationCodeService _verificationCodeService;
        private readonly ILoginValidatableService _loginValidatableService;
      
        private readonly IMemberLoginLogService _memberLoginLogService;
        protected readonly ILogger<LoginModule> _logger;

        public LoginModule(
            IMemberService memberService,
            IMemberTokenService memberTokenService,
            ILoginValidatableService loginValidatableService,
            IVerificationCodeService verificationCodeService,
            IMemberLoginLogService memberLoginLogService,
            ILogger<LoginModule> logger) : base("member", "api", "")
        {
            _memberService = memberService;
            _memberTokenService = memberTokenService;
            _loginValidatableService = loginValidatableService;
            _verificationCodeService = verificationCodeService;
            _memberLoginLogService = memberLoginLogService;
            _logger = logger;
            Post("/test/debug/init", _ => InitDebugData());
            Post("/login", _ => DoLoginAsync());
            Post("/regist", _ => DoRegistAsync());
            Post("/logout", _ => DoLogoutAsync());
        }

        private async Task<Response> DoLoginAsync()
        {
            var cmd = this.Bind<LoginCmd>();
            if (!ModelValidationResult.IsValid)
                return BadRequest();
            var timeNow = DateTime.Now;
            var err = await _loginValidatableService.ChekcLoginCmdValidatableAsync(cmd, ClientIP, timeNow).ConfigureAwait(false);
            if (!err.IsNullOrWhiteSpace())
                return PreconditionFailed(err);
            return await RealLogin(cmd, timeNow).ConfigureAwait(false);
        }

        private async Task<Response> DoLogoutAsync()
        {
            var cmd = this.Bind<LogoutCmd>();
            await _memberTokenService.DoLogoutAsync(cmd.Token).ConfigureAwait(false);
            return Ok("安全登出");
        }

        private async Task<Response> RealLogin(LoginCmd cmd, DateTime timeNow)
        {
            await _memberLoginLogService.NewLogAsync(new MemberLoginLog
            {
                Account = cmd.Account,
                Password = cmd.Password,
                CheckOn = timeNow,
                IsPass = true,
                Reason = "登录成功",
                PassOn = timeNow,
                ClientIp = ClientIP
            }).ConfigureAwait(false);
            var memberAccount = await _memberService.GetMemberAccountAsync(cmd.Account).ConfigureAwait(false);
            var token = await _memberTokenService.NewMemberTokenAsync(memberAccount.MemberId, cmd.ClientType, ClientIP);
            return Ok(new
            {
                TokenString = token.ToString(),
                TokenObjectId = token,
                MemberInfo = await _memberService.GetMemberInfoAsync(memberAccount.MemberId).ConfigureAwait(false)
            });
        }

        private async Task<Response> DoRegistAsync()
        {
            var cmd = this.Bind<RegistCmd>();

            var timeNow = DateTime.Now;
            var err = await _loginValidatableService.CheckRegistCmdValidatableAsync(cmd, ClientIP, timeNow).ConfigureAwait(false);
            if (!err.IsNullOrWhiteSpace())
                return PreconditionFailed(err);
            var verificationCode = await _verificationCodeService.GetAvailableCode(cmd.MobilePhone, cmd.CheckCode, cmd.CodeType).ConfigureAwait(false);
           
            await _verificationCodeService.UpdateVerificationCodeToDisabledByUsed(verificationCode.Id).ConfigureAwait(false);
          
            var entity = cmd.Adapt<MemberInfo>();
            if (entity.Realname.IsNullOrWhiteSpace())
                entity.Realname = "待设定";
            entity.MobilePhone = cmd.MobilePhone;
            entity.Id = ObjectId.NewId();
            entity.CreatedOn = timeNow;
            entity.ModifiedOn = timeNow;
            try
            {
                await _memberService.NewMemberInfoAsync(entity).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError("LoginModule=regist路由出现后端错误：" + e.Message);
            }
            var passwordFormat = EnumHelper.Random(PasswordFormatType.None);
            var passwordSalt = HashGenerator.Salt();
            var password = HashGenerator.Encode(cmd.Password, passwordFormat, passwordSalt);
            await _memberService.NewMemberAccountAsync(new MemberAccount
            {
                Account = cmd.MobilePhone,
                MemberId = entity.Id,
                Password = password,
                PasswordFormat = passwordFormat,
                PasswordSalt = passwordSalt
            });
            return Ok(entity);
        }

        private async Task<Response> InitDebugData()
        {
            var timeNow = DateTime.Now;
            var entity =new MemberInfo { 
                Id=ObjectId.NewId(),
                Realname="崔朝辉",
                Sex= SexType.Unknown,
                Telephone="16653555299",
                MobilePhone="16653555299",
                Type= MemberType.SystemAdministrator,
                Email="allycs@126.com",
                IdCard = "370000000000000000",
                CreatedOn= timeNow,
                ModifiedOn=timeNow
            };
            try
            {
                await _memberService.NewMemberInfoAsync(entity).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError("LoginModule=regist路由出现后端错误：" + e.Message);
            }
            var passwordFormat = EnumHelper.Random(PasswordFormatType.None);
            var passwordSalt = HashGenerator.Salt();
            var password = HashGenerator.Encode("123456", passwordFormat, passwordSalt);
            await _memberService.NewMemberAccountAsync(new MemberAccount
            {
                Account = "allycs",
                MemberId = entity.Id,
                Password = password,
                PasswordFormat = passwordFormat,
                PasswordSalt = passwordSalt
            });
            return Ok(entity);
        }

    }
}