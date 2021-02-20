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
    using System;
    using System.Linq;

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
        public async Task<string> NewFarmInfoAsync(NewFarmInfoCmd cmd)
        {
            var timeNow = DateTime.Now;
            var extension = cmd.MainImg.Name.Split('.').LastOrDefault();
            var fileName = ObjectId.NewId();
            var fileFullName = fileName + "." + extension;
            var path = Path.Combine(GetUploadDirectory(), fileFullName);
            var MD5 = HashGenerator.GetFileMD5(cmd.ClientFile.Value);

            using (FileStream destinationStream = File.Create(path))
            {
                await cmd.ClientFile.Value.CopyToAsync(destinationStream).ConfigureAwait(false);
            }
            var id = 0;
            if (cmd.Type == SoftClientType.Android)
                fileFullName = @"http://www.radar365.top:28000/rs_app_release/v" + cmd.Version.Replace('.', '_') + @"/app-release.apk";
            using (var conn = CreateConnection())
            {
                id = await conn.InsertAsync<int>(new SoftClientVersion
                {
                    ClientName = cmd.Type == SoftClientType.Android ? "安卓" + cmd.Version + "版" : cmd.Type == SoftClientType.PCClient ? "Win" + cmd.Version + "版" : "服务" + cmd.Version + "版",
                    SoftClientType = cmd.Type,
                    Description = cmd.Description,
                    Version = cmd.Version,
                    VersionCode = cmd.VersionCode,
                    Remark = cmd.Remark,
                    FileSize = cmd.ClientFile.Value.Length,
                    Extension = extension,
                    MessageDigestFive = MD5,
                    FileName = fileName,
                    FileUrl = fileFullName,
                    ForceUpdate = cmd.ForceUpdate.Value,
                    IgnoreableUpdate = cmd.IgnoreableUpdate.Value,
                    ModifiedBy = currentMemberId,
                    ModifiedOn = timeNow,
                    CreatedBy = currentMemberId,
                    CreatedOn = timeNow
                }).ConfigureAwait(false);
            };
            return id;
            var farmInfo = new FarmInfo
            {
                Name = cmd.Name,
                Longitude = cmd.Longitude,
                Latitude = cmd.Latitude,
                Description = cmd.Description,
                Remark = cmd.Remark,
                Address = cmd.Address,
                Telephone = cmd.Telephone,
                Type = cmd.Type,
                PersonLiable = cmd.PersonLiable,
                Status = cmd.Status,
                CreatedBy = CurrentMemberId,
                CreatedOn = timeNow,
                ModifiedOn = timeNow
            };
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