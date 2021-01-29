namespace Allycs.Common.Devices.Modules
{
    using Services;
    using Nancy;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using Allycs.Core;

    public class NancyAuthApiModule : BaseNancyModule
    {
        protected readonly IMemberTokenService _memberTokenService;
        protected readonly IMemberService _memberService;

        public NancyAuthApiModule(IMemberTokenService memberTokenService, IMemberService memberService, string modulePath="") : base("member", "api", modulePath)
        {
            _memberTokenService = memberTokenService;
            _memberService = memberService;
            Before += CheckAuth;
        }

        protected ObjectId CurrentToken
        {
            get;
            private set;
        }
        protected ObjectId CurrentMemberId
        {
            get;
            private set;
        }
        protected MemberType CurrentMemberType
        {
            get;
            private set;
        }
        protected string CurrentMemberMobilePhone
        {
            get;
            private set;
        }
        public ClientType CurrentClientType { get; private set; }

        /// <summary>
        /// 检车Token获取当前用户，并写入属性中
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private Response CheckAuth(NancyContext ctx)
        {
            #region Token
            var token = string.Empty;
            var hasTokenHeader = ctx.Request.Headers.Keys.Contains("X-Token");
            if (hasTokenHeader)
            {
                token = ctx.Request.Headers["X-Token"].FirstOrDefault();
            }
            if (token.IsNullOrWhiteSpace() && ctx.Request.Cookies.Keys.Contains("token"))
            {
                token = ctx.Request.Cookies["token"];
            }
            if (token.IsNullOrWhiteSpace())
            {
                token = Context.Request.Query["token"].Value;
            }
            if (token.IsNullOrWhiteSpace())
                return Unauthorized("请重新获取身份令牌");
            #endregion
            #region ClientType
            var intClientType = (int)ClientType.Mobile;
            var hasClientTypeHeader = ctx.Request.Headers.Keys.Contains("X-ClientType");
            var clientTypeStr = string.Empty;
            if (hasClientTypeHeader)
            {
                clientTypeStr = ctx.Request.Headers["X-ClientType"].FirstOrDefault().ToString();
                int.TryParse(ctx.Request.Headers["X-ClientType"].FirstOrDefault().ToString(), out intClientType);
            }
            if (clientTypeStr.IsNullOrWhiteSpace() && ctx.Request.Cookies.Keys.Contains("clientType"))
            {
                clientTypeStr = ctx.Request.Cookies["clientType"];
                int.TryParse(ctx.Request.Cookies["clientType"], out intClientType);
            }
            if (clientTypeStr.IsNullOrWhiteSpace())
            {
                int.TryParse(Context.Request.Query["clientType"].Value, out intClientType);
            }
            CurrentClientType = (ClientType)intClientType;
            #endregion

            if (_memberTokenService.ExistAvailableTokenAsync(token, (ClientType)intClientType).Result)
            {
                var memberToken = _memberTokenService.GetMemberTokenAsync(token).Result;
                CurrentToken = token;
                var memberEntity = _memberService.GetMemberInfoAsync(memberToken.MemberId).Result;
                CurrentMemberId = memberToken.MemberId;
                CurrentMemberType = memberEntity.Type;
                CurrentMemberMobilePhone = memberEntity.MobilePhone;
            }
            else
                return Unauthorized("身份令牌已失效");
            return null;
        }
    }
}
