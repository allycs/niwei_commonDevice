namespace Allycs.Common.Devices.Services
{
    using Dtos;
    using Entities;
    using Allycs.Core;
    using Dapper;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class FarmService : PostgresService, IFarmService
    {
        private readonly AppSettings _settings;
        private readonly ILogger<FarmService> _logger;

        public FarmService(IOptionsSnapshot<AppSettings> option,
            ILogger<FarmService> logger)
            : base(option)
        {
            _settings = option.Value;
            _logger = logger;
        }

        public async Task<bool> ExistFarmInfoAsync(string id)
        {
            using var conn = CreateConnection();
            return (await conn.RecordCountAsync<FarmInfo>($" WHERE id='{id}'").ConfigureAwait(false)) > 0;
        }

        public async Task<bool> ExistFarmInfoByNameAsync(string farmName)
        {
            using var conn = CreateConnection();
            return (await conn.RecordCountAsync<FarmInfo>($" WHERE name LIKE '%{farmName}%' ").ConfigureAwait(false)) > 0;
        }

        public async Task<bool> ExistFarmInfoByTelephoneAsync(string telephone)
        {
            using var conn = CreateConnection();
            return (await conn.RecordCountAsync<FarmInfo>($" WHERE telephone LIKE '%{telephone}%' ").ConfigureAwait(false)) > 0;
        }

        public async Task<FarmInfo> GetFarmInfoAsync(string id)
        {
            using var conn = CreateConnection();
            return await conn.GetAsync<FarmInfo>(id).ConfigureAwait(false);
        }

        private SqlAndDps FarmInfoFilterSql(GetFarmInfoListCmd cmd)
        {
            var paras = new DynamicParameters();
            var sql = " 1=1 ";
            if (cmd.Type.HasValue && (int)cmd.Type.Value != -1)
            {
                sql += " AND type = @type ";
                paras.Add("type", cmd.Type.Value);
            }
            if (!cmd.Name.IsNullOrWhiteSpace())
            {
                sql += " AND name LIKE @name ";
                paras.Add("name", "%" + cmd.Name.Trim() + "%");
            }
            if (!cmd.Remark.IsNullOrWhiteSpace())
            {
                sql += " AND remark LIKE @remark ";
                paras.Add("remark", "%" + cmd.Remark.Trim() + "%");
            }
            if (!cmd.Address.IsNullOrWhiteSpace())
            {
                sql += " AND address LIKE @address ";
                paras.Add("address", "%" + cmd.Address.Trim() + "%");
            }
            if (!cmd.Telephone.IsNullOrWhiteSpace())
            {
                sql += " AND telephone LIKE @telephone ";
                paras.Add("telephone", "%" + cmd.Telephone.Trim() + "%");
            }

            return new SqlAndDps { Sql = sql, Dps = paras };
        }

        public async Task<IEnumerable<FarmInfo>> GetFarmInfosAsync(GetFarmInfoListCmd cmd, PagedListQuery plQuery)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                var sql = "SELECT * FROM farm_info WHERE ";

                var demo = FarmInfoFilterSql(cmd);
                sql += demo.Sql;
                sql += " ORDER BY created_on DESC ";
                sql += plQuery.PostgresLimitPartialSql();
                return await conn.QueryAsync<FarmInfo>(sql, demo.Dps).ConfigureAwait(false);
            }
        }

        public async Task<bool> NewFarmInfoAsync(FarmInfo entity)
        {
            using var conn = CreateConnection();
            return await conn.InsertAsync<FarmInfo>(entity).ConfigureAwait(false);
        }

        public async Task<bool> UpdateFarmInfoAsync(FarmInfo entity)
        {
            using var conn = CreateConnection();
            return await conn.UpdateAsync(entity).ConfigureAwait(false) > 0;
        }
    }
}