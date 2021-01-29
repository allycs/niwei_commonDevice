using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices.Services.Implementations
{
    using Allycs.Common.Devices.Dtos;
    using Allycs.Common.Devices.Entities;
    using Allycs.Core;
    using Dapper;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class MemberService : PostgresService, IMemberService
    {
        private readonly AppSettings _settings;
        private readonly ILogger<MemberService> _logger;
        private readonly IMemoryCache _cache;

        public MemberService(IOptionsSnapshot<AppSettings> option,
            IMemoryCache memoryCache,
            ILogger<MemberService> logger)
            : base(option)
        {
            _settings = option.Value;
            _cache = memoryCache;
            _logger = logger;
        }

        public async Task<bool> ExistPhoneNumberOrTeleNumberAsync(string phoneNumber)
        {
            using (var conn = CreateConnection())
            {
                return (await conn.RecordCountAsync<MemberInfo>($" WHERE telephone='{phoneNumber}' OR mobile_phone='{phoneNumber}'").ConfigureAwait(false)) > 0;
            }
        }
        public async Task<bool> ExistIdCardAsync(string idCard)
        {
            using (var conn = CreateConnection())
            {
                return (await conn.RecordCountAsync<MemberInfo>(new { IdCard = idCard }).ConfigureAwait(false)) > 0;
            }
        }
        public async Task<bool> ExistAccountAsync(string account)
        {
            using (var conn = CreateConnection())
            {
                return (await conn.RecordCountAsync<MemberAccount>(new { Account = account }).ConfigureAwait(false)) > 0;
            }
        }

        public async Task<bool> ExistMobilePhoneAsync(string mobilePhone)
        {
            using (var conn = CreateConnection())
            {
                return (await conn.RecordCountAsync<MemberInfo>(new { MobilePhone = mobilePhone }).ConfigureAwait(false)) > 0;
            }
        }
        public async Task<bool> ExistEmailAsync(string email)
        {
            using (var conn = CreateConnection())
            {
                return (await conn.RecordCountAsync<MemberInfo>(new { Email = email }).ConfigureAwait(false)) > 0;
            }
        }

        public async Task<bool> ExistMemberInfoAsync(string memberId)
        {
            using (var conn = CreateConnection())
            {
                var entity = await conn.GetAsync<MemberInfo>(memberId).ConfigureAwait(false);
                return (entity != null);
            }
        }
        public async Task<bool> ExistSubMemberAsync(string currentMemberId, string subMemberId)
        {
            var count = 0;
            using (var conn = CreateConnection())
            {
                count = await conn.RecordCountAsync<MemberInfo>(new { Id = subMemberId, MainMemberId = currentMemberId }).ConfigureAwait(false);
            }
            return count > 0;
        }
        public bool ExistWechatMemberInfo(string openId)
        {
            using (var conn = CreateConnection())
            {
                return conn.ExecuteScalar<int>("SELECT COUNT(*) FROM wechat_member_info WHERE open_id=@OpenId", new { OpenId = openId }) > 0;
                //return CreateConnection().RecordCount<WechatMemberInfo>(new { OpenId = openId }) > 0;
            }
        }

        public async Task<MemberInfo> GetMemberInfoAsync(string id)
        {
            using (var conn = CreateConnection())
            {
                return await conn.GetAsync<MemberInfo>(id).ConfigureAwait(false);
            }
        }
        public async Task<MemberInfo> GetMemberInfoByPhoneAsync(string phoneNumber)
        {
            using (var conn = CreateConnection())
            {
                return (await conn.GetListAsync<MemberInfo>(new { MobilePhone = phoneNumber }).ConfigureAwait(false)).FirstOrDefault();
            }
        }

        public async Task<MemberAccount> GetMemberAccountAsync(string account)
        {
            using (var conn = CreateConnection())
            {
                return await conn.GetAsync<MemberAccount>(account).ConfigureAwait(false);
            }
        }
        public async Task<IEnumerable<MemberAccount>> GetMemberAccountsByMemberIdAsync(string memberId)
        {
            using (var conn = CreateConnection())
            {
                return await conn.GetListAsync<MemberAccount>(new { MemberId = memberId }).ConfigureAwait(false);
            }
        }
        public async Task<int> CountMemberInfoAsync()
        {
            using (var conn = CreateConnection())
            {
                return await conn.RecordCountAsync<MemberInfo>().ConfigureAwait(false);
            }
        }
        public async Task<bool> UpdateMemberInfoAsync(MemberInfo entity)
        {
            var isOk = false;
            using var conn = CreateConnection();
            return (await conn.UpdateAsync(entity).ConfigureAwait(false)) > 0;
        }
        public async Task<bool> UpdateMemberAccountAsync(MemberAccount entity)
        {
            using (var conn = CreateConnection())
            {
                return (await conn.UpdateAsync(entity).ConfigureAwait(false)) > 0;
            }
        }

        public async Task<bool> UpdateMemberAddressLastAsync(string addressCurrent, ObjectId currentMemberId)
        {
            using (var conn = CreateConnection())
            {
                var memberInfo = conn.Get<MemberInfo>(currentMemberId);
                memberInfo.Address = addressCurrent;
                return (await conn.UpdateAsync(memberInfo).ConfigureAwait(false)) > 0;
            }
        }

        public async Task NewMemberAccountAsync(MemberAccount entity)
        {
            using (var conn = CreateConnection())
            {
                await conn.InsertAsync<MemberAccount>(entity).ConfigureAwait(false);
            }
        }

        public async Task NewMemberInfoAsync(MemberInfo entity)
        {
            using (var conn = CreateConnection())
            {
                await conn.InsertAsync<MemberInfo>(entity).ConfigureAwait(false);
            }
        }

        private SqlAndDps AllMemberInfoSql(GetMemberListCmd cmd)
        {
            var paras = new DynamicParameters();
            var sql = " 1=1 ";
            if (cmd.Type.HasValue && (int)cmd.Type.Value != -1)
            {
                sql += " AND type = @type ";
                paras.Add("type", cmd.Type.Value);
            }
            if (cmd.Level.HasValue)
            {
                sql += " AND levle = @level ";
                paras.Add("level", cmd.Level.Value);
            }
            if (!cmd.RefereeId.IsNullOrWhiteSpace())
            {
                sql += " AND referee_id=@refereeId";
                paras.Add("refereeId", cmd.RefereeId);
            }
            //if (cmd.AdministrativeDivisionsId.HasValue)
            //{
            //    sql += " AND administrative_divisions_id=@administrativeDivisionsId";
            //    paras.Add("administrativeDivisionsId", cmd.AdministrativeDivisionsId);
            //}
            if (!cmd.Realname.IsNullOrEmpty())
            {
                sql += " AND realname LIKE @realname ";
                paras.Add("realname", "%" + cmd.Realname + "%");
            }
            if (!cmd.MobilePhone.IsNullOrEmpty())
            {
                sql += " AND mobile_phone LIKE @mobilePhone ";
                paras.Add("mobilePhone", "%" + cmd.MobilePhone + "%");
            }
            return new SqlAndDps { Sql = sql, Dps = paras };
        }

        public async Task<IEnumerable<MemberInfo>> GetAllMemberInfoAsync(GetMemberListCmd cmd, PagedListQuery plQuery)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                var sql = "SELECT * FROM member_info WHERE ";

                var demo = AllMemberInfoSql(cmd);
                sql += demo.Sql;
                sql += " ORDER BY created_on DESC ";
                sql += plQuery.PostgresLimitPartialSql();
                return await conn.QueryAsync<MemberInfo>(sql, demo.Dps).ConfigureAwait(false);
            }
        }

        public async Task<int> GetAllMemberInfoCountAsync(GetMemberListCmd cmd)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                var sql = "SELECT COUNT(*) FROM member_info WHERE ";

                var demo = AllMemberInfoSql(cmd);
                sql += demo.Sql;
                return await conn.ExecuteScalarAsync<int>(sql, demo.Dps).ConfigureAwait(false);
            }
        }

        public async Task<CommandResult> UpdateMemberPasswordAsync(string memberId, string newPassword)
        {
            Guard.ArgumentNotNullOrEmpty(nameof(memberId), memberId);
            Guard.ArgumentNotNullOrEmpty(nameof(newPassword), newPassword);

            using (var conn = CreateConnection())
            {
                var memberAccount = await conn.QueryFirstOrDefaultAsync<MemberAccount>("select * from member_account where member_id=@memberId", new { memberId = new ObjectId(memberId) }).ConfigureAwait(false);
                if (memberAccount == null)
                {
                    return new CommandResult("尚未创建账号");
                }

                memberAccount.Password = HashGenerator.Encode(newPassword, memberAccount.PasswordFormat, memberAccount.PasswordSalt);
                await conn.UpdateAsync(memberAccount).ConfigureAwait(false);
                return CommandResult.SuccessResult;
            }
        }

    }
}
