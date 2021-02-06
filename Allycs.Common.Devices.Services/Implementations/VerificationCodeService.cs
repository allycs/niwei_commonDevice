using System;
using System.Collections.Generic;
using System.Text;

namespace Allycs.Common.Devices.Services
{
    using Allycs.Common.Devices.Entities;
    using Allycs.Core;
    using Dapper;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class VerificationCodeService : PostgresService, IVerificationCodeService
    {
        private readonly AppSettings _settings;
        private readonly ILogger<VerificationCodeService> _logger;

        public VerificationCodeService(IOptionsSnapshot<AppSettings> option,
            ILogger<VerificationCodeService> logger)
            : base(option)
        {
            _settings = option.Value;
            _logger = logger;
        }

        public async Task<int> NewVerificationCodeAsync(VerificationCode entity)
        {
            using var conn = CreateConnection();
            return await conn.InsertAsync<int>(entity).ConfigureAwait(false);
        }

        public async Task<VerificationCode> NewVerificationCodeAsync(string code, CodeType type, DateTime timeNow, ObjectId? memberId = null, string nationCode = "86")
        {
            var entity = new VerificationCode
            {
                MemberId = memberId,
                Code = code,
                Type = type,
                CreatedOn = timeNow,
                IsDisabled = false,
                DisabledOn = timeNow.AddSeconds(_settings.VerificationCodeDisableTime)
            };
            entity.Id = await NewVerificationCodeAsync(entity).ConfigureAwait(false);
            return entity;
        }
        public async Task UpdateVerificationCodeToDisabledByTimeAsync()
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync($"UPDATE verification_code SET is_disabled=true , reason = '时间失效' WHERE disabled_on<'{DateTime.Now}' AND is_disabled=false AND reason IS NULL").ConfigureAwait(false);
        }
        public async Task UpdateVerificationCodeToDisabledByNewCode(string memberId, CodeType type)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync($"UPDATE verification_code SET is_disabled=true , reason='生成新验证码' WHERE member_id='{memberId}' AND type ={(int)type} AND is_disabled=false AND reason IS NULL ").ConfigureAwait(false);
        }
        public async Task UpdateVerificationCodeToDisabledByUsed(int id)
        {
            using var conn = CreateConnection();
            var entity = await conn.GetAsync<VerificationCode>(id).ConfigureAwait(false);
            entity.IsDisabled = true;
            entity.Reason = "使用验证码";
            await conn.UpdateAsync(entity).ConfigureAwait(false);
        }

        public async Task<bool> ExistAvailableCode(string memberId,string code, CodeType type)
        {
            using var conn = CreateConnection();
            var whereSql = $"WHERE is_disabled=false " + (memberId.IsNullOrWhiteSpace() ? " AND member_id IS NULL " : " AND member_id='{memberId}' ") + " AND code='{code.ToUpper()}' AND type ={(int)type} ";
            return (await conn.RecordCountAsync<VerificationCode>(whereSql).ConfigureAwait(false)) > 0;
        }
        public async Task<bool> ExistAvailableCodeWithoutMemberAsync(string clientIp, string code, CodeType type)
        {
            using var conn = CreateConnection();
            var whereSql = $"WHERE is_disabled=false  AND member_id IS NULL  AND code='{code.ToUpper()}' AND type ={(int)type} AND client_ip='{clientIp}'";
            return (await conn.RecordCountAsync<VerificationCode>(whereSql).ConfigureAwait(false)) > 0;
        }
        public async Task<bool> ExistAvailableRenewPasswordCodeAsync(string memberId, string clientIp)
        {
            using var conn = CreateConnection();
            var whereSql = $"WHERE is_disabled=false  AND member_id ='{memberId}'  AND type ={(int)CodeType.RenewPassword} AND client_ip='{clientIp}'";
            return (await conn.RecordCountAsync<VerificationCode>(whereSql).ConfigureAwait(false)) > 0;
        }
        public async Task<bool> ExistAvailableRenewPasswordCodeAsync(string memberId, string clientIp, string code)
        {
            using var conn = CreateConnection();
            var whereSql = $"WHERE is_disabled=false  AND member_id ='{memberId}' AND code='{code}'  AND type ={(int)CodeType.RenewPassword} AND client_ip='{clientIp}'";
            return (await conn.RecordCountAsync<VerificationCode>(whereSql).ConfigureAwait(false)) > 0;

        }
        public async Task<bool> ExistAvailableRegistCodeByClientIpAsync(string clientIp)
        {
            using var conn = CreateConnection();
            var whereSql = $"WHERE is_disabled=false  AND member_id IS NULL   AND type ={(int)CodeType.Regist} AND client_ip='{clientIp}'";
            return (await conn.RecordCountAsync<VerificationCode>(whereSql).ConfigureAwait(false)) > 0;
        }
        public async Task<VerificationCode> GetAvailableCode(string memberId, string code, CodeType type)
        {
            using var conn = CreateConnection();
            var whereSql = $"WHERE is_disabled=false " + (memberId.IsNullOrWhiteSpace() ? " AND member_id IS NULL " : $" AND member_id='{memberId}' ") + " AND code='{code.ToUpper()}' AND type ={(int)type} ORDER BY created_on DESC ";

            return (await conn.GetListAsync<VerificationCode>(whereSql).ConfigureAwait(false)).FirstOrDefault();
        }
        public async Task<VerificationCode> GetAvailableRegistCodeByClientIpAsync(string clientIp)
        {
            await UpdateVerificationCodeToDisabledByTimeAsync().ConfigureAwait(false);
            using var conn = CreateConnection();
            var whereSql = $"WHERE is_disabled=false  AND member_id IS NULL  AND client_ip='{clientIp}' AND type ={(int)CodeType.Regist} ORDER BY created_on DESC ";
            return (await conn.GetListAsync<VerificationCode>(whereSql).ConfigureAwait(false)).FirstOrDefault();
        }
        public async Task<VerificationCode> GetAvailableRenewPasswordCodeByClientIpAsync(string memberId, string clientIp)
        {
            await UpdateVerificationCodeToDisabledByTimeAsync().ConfigureAwait(false);
            using var conn = CreateConnection();
            var whereSql = $"WHERE is_disabled=false  AND member_id ='{memberId}'  AND client_ip='{clientIp}' AND type ={(int)CodeType.RenewPassword} ORDER BY created_on DESC ";
            return (await conn.GetListAsync<VerificationCode>(whereSql).ConfigureAwait(false)).FirstOrDefault();
        }
    }
}
