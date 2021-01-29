namespace Allycs.Common.Devices.Services
{
    using Allycs.Core;
    using Dapper;
    using Entities;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Threading.Tasks;

    public class MemberTokenService : PostgresService, IMemberTokenService
    {
        private readonly AppSettings _settings;
        private readonly ILogger<MemberTokenService> _logger;

        public MemberTokenService(IOptionsSnapshot<AppSettings> option, ILogger<MemberTokenService> logger) : base(option)
        {
            _settings = option.Value;
            _logger = logger;
        }

        public async Task<MemberToken> GetMemberTokenAsync(string token)
        {
            return await CreateConnection().GetAsync<MemberToken>(token).ConfigureAwait(false);
        }

        public async Task<int> CountTodayTokenAsync()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var day = DateTime.Now.Day;
            var startTime = new DateTime(year, month, day);
            using (var conn = CreateConnection())
            {
                return await conn.RecordCountAsync<MemberToken>($"WHERE created_on > '{startTime}' AND created_on < '{startTime.AddDays(1)}'").ConfigureAwait(false);
            }
        }

        public async Task<int> CountDeviceOnlineAsync()
        {
            await UpdateMemberTokenToDisabledByTimeAsync().ConfigureAwait(false);
            using (var conn = CreateConnection())
            {
                return await conn.RecordCountAsync<MemberToken>(new { IsDisabled = false }).ConfigureAwait(false);
            }
        }

        public async Task<ObjectId> NewMemberTokenAsync(string memberId, ClientType type, string clientIp)
        {
            var timeNow = DateTime.Now;
            var disableOn = timeNow.AddSeconds(_settings.MemberTokenDisableTime);
            var token = ObjectId.NewId();
            var existAvailableToken = await ExistAvailableTokenAsync(token, type).ConfigureAwait(false);
            while (existAvailableToken)
            {
                token = ObjectId.NewId();
                existAvailableToken = await ExistAvailableTokenAsync(token, type).ConfigureAwait(false);
            }
            await UpdateMemberTokenToDisabledByNewCode(memberId, type).ConfigureAwait(false);
            using (var conn = CreateConnection())
            {
                //await conn.DeleteListAsync<MemberToken>(new { ClientType = type, MemberId = memberId.ToString() }).ConfigureAwait(false);
                if (!(await conn.InsertAsync(new MemberToken
                {
                    Token = token,
                    MemberId = memberId,
                    ClientIp = clientIp,
                    ClientType = type,
                    IsDisabled = false,
                    DisabledOn = disableOn,
                    CreatedOn = timeNow
                })))
                    _logger.LogError($"用户：{memberId}插入Token出错时间：{timeNow}");

                return token;
            }
        }

        public async Task<bool> ExistAvailableTokenAsync(string token, ClientType type)
        {
            var timeNow = DateTime.Now;
            using (var conn = CreateConnection())
            {
                await UpdateMemberTokenToDisabledByTimeAsync().ConfigureAwait(false);
                return (await conn.RecordCountAsync<MemberToken>($"WHERE token = '{token}' AND is_disabled=false AND client_type ={(int)type} AND disabled_on>'{timeNow}'").ConfigureAwait(false)) > 0;
            }
        }

        public async Task<bool> ExistAvailableTokenAsync(string token)
        {
            var timeNow = DateTime.Now;
            using (var conn = CreateConnection())
            {
                await UpdateMemberTokenToDisabledByTimeAsync().ConfigureAwait(false);
                return (await conn.RecordCountAsync<MemberToken>($"WHERE token = '{token}' AND disabled_on>'{timeNow}'").ConfigureAwait(false)) > 0;
            }
        }

        public bool ExistWechatUserToken(string openId)
        {
            return CreateConnection().ExecuteScalar<int>("SELECT COUNT(*) FROM wechat_user_token WHERE open_id=@OpenId", new { OpenId = openId }) > 0;
        }

        public async Task UpdateMemberTokenToDisabledByTimeAsync()
        {
            using (var conn = CreateConnection())
            {
                await conn.ExecuteAsync($"UPDATE member_token SET is_disabled=true , reason = '时间失效' WHERE disabled_on<'{DateTime.Now}' AND is_disabled=false AND reason IS NULL").ConfigureAwait(false);
            }
        }

        public async Task UpdateMemberTokenToDisabledByNewCode(string memberId, ClientType type)
        {
            using (var conn = CreateConnection())
            {
                await conn.ExecuteAsync($"UPDATE member_token SET is_disabled=true , reason='生成新令牌' WHERE member_id='{memberId}' AND client_type ={(int)type} AND is_disabled=false AND reason IS NULL ").ConfigureAwait(false);
            }
        }

        public async Task UpdateMemberTokenToDisabledByMemberId(string memberId)
        {
            using (var conn = CreateConnection())
            {
                await conn.ExecuteAsync($"UPDATE member_token SET is_disabled=true , reason='用户强制下线' WHERE member_id='{memberId}' AND is_disabled=false ").ConfigureAwait(false);
            }
        }

        public async Task DoLogoutAsync(string token)
        {
            var exist = await ExistAvailableTokenAsync(token).ConfigureAwait(false);
            if (exist)
            {
                using (var conn = CreateConnection())
                {
                    await conn.ExecuteAsync($"UPDATE member_token SET is_disabled=true , reason='登出' WHERE token='{token}'AND is_disabled=false AND reason IS NULL ").ConfigureAwait(false);
                }
            }
        }
    }
}