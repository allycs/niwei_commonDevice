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
    using System.IO;

    public class UploadImagesService : PostgresService, IUploadImagesService
    {
        private readonly AppSettings _settings;
        private readonly ILogger<UploadImagesService> _logger;

        public UploadImagesService(IOptionsSnapshot<AppSettings> option,
            ILogger<UploadImagesService> logger)
            : base(option)
        {
            _settings = option.Value;
            _logger = logger;
        }
        public string GetUploadDirectory()
        {
            var uploadDirectory = Path.Combine(Environment.CurrentDirectory, _settings.UploadImageDirectory);

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            return uploadDirectory;
        }
        public async Task<UploadImages> GetUploadImageAsync(string id)
        {
            using var conn = CreateConnection();
            return await conn.GetAsync<UploadImages>(id).ConfigureAwait(false);
        }

        private SqlAndDps UploadImageFilterSql(GetUploadImageListCmd cmd)
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

                var demo = UploadImageFilterSql(cmd);
                sql += demo.Sql;
                sql += " ORDER BY created_on DESC ";
                sql += plQuery.PostgresLimitPartialSql();
                return await conn.QueryAsync<FarmInfo>(sql, demo.Dps).ConfigureAwait(false);
            }
        }
        public async Task<string> NewFarmInfoAsync(NewFarmInfoCmd cmd, string currentMemberId)
        {
            var timeNow = DateTime.Now;
            var extension = cmd.MainImg.Name.Split('.').LastOrDefault();
            var fileName = ObjectId.NewId();
            var fileFullName = fileName + "." + extension;
            var path = Path.Combine(GetUploadDirectory(), fileFullName);
            using (FileStream destinationStream = File.Create(path))
            {
                await cmd.MainImg.Value.CopyToAsync(destinationStream).ConfigureAwait(false);
            }

            using (var conn = CreateConnection())
            {
                var farmInfo = new FarmInfo
                {
                    Name = cmd.Name,
                    Longitude = cmd.Longitude,
                    Latitude = cmd.Latitude,
                    Description = cmd.Description,
                    MainImg= fileName,
                    Extension=extension,
                    Remark = cmd.Remark,
                    Address = cmd.Address,
                    Telephone = cmd.Telephone,
                    Type = cmd.Type,
                    PersonLiable = cmd.PersonLiable,
                    Status = cmd.Status,
                    CreatedBy = currentMemberId,
                    ModifiedBy=currentMemberId,
                    CreatedOn = timeNow,
                    ModifiedOn = timeNow
                };
                await conn.InsertAsync<FarmInfo>(farmInfo).ConfigureAwait(false);

                return farmInfo.Id;
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
        public async Task<string> UpdateFarmInfoAsync(UpdateFarmInfoCmd cmd, string currentMemberId)
        {
            var timeNow = DateTime.Now;
            var farmInfo = await GetFarmInfoAsync(cmd.Id).ConfigureAwait(false);
            farmInfo.ModifiedOn = timeNow;
            farmInfo.ModifiedBy = currentMemberId;
            if(cmd.MainImg!=null)
            {
                var extension = cmd.MainImg.Name.Split('.').LastOrDefault();
                var fileName = ObjectId.NewId();
                var fileFullName = fileName + "." + extension;
                var path = Path.Combine(GetUploadDirectory(), fileFullName);
                using (FileStream destinationStream = File.Create(path))
                {
                    await cmd.MainImg.Value.CopyToAsync(destinationStream).ConfigureAwait(false);
                }
                farmInfo.MainImg = fileName;
                farmInfo.Extension = extension;
            }
            if (!cmd.Name.IsNullOrWhiteSpace())
                farmInfo.Name = cmd.Name.Trim();
            if (cmd.Longitude.HasValue)
                farmInfo.Longitude = cmd.Longitude.Value;
            if (cmd.Latitude.HasValue)
                farmInfo.Latitude = cmd.Latitude.Value;
            if (!cmd.Description.IsNullOrWhiteSpace() & cmd.Description.Trim() != farmInfo.Description)
                farmInfo.Description = cmd.Description.Trim();
            if (!cmd.Remark.IsNullOrWhiteSpace() && cmd.Remark.Trim() != farmInfo.Remark)
                farmInfo.Remark = cmd.Remark.Trim();
            if (!cmd.Address.IsNullOrWhiteSpace() && cmd.Address.Trim() != farmInfo.Address)
                farmInfo.Address = cmd.Address.Trim();
            if (!cmd.Telephone.IsNullOrWhiteSpace() && cmd.Telephone.Trim() != farmInfo.Telephone)
                farmInfo.Telephone = cmd.Telephone.Trim();
            if (cmd.Type.HasValue && cmd.Type != farmInfo.Type)
                farmInfo.Type = cmd.Type.Value;
            if (!cmd.PersonLiable.IsNullOrWhiteSpace() && cmd.PersonLiable.Trim() != farmInfo.PersonLiable)
                farmInfo.PersonLiable = cmd.PersonLiable.Trim();
            if (cmd.Status.HasValue && cmd.Status.Value != farmInfo.Status)
                farmInfo.Status = cmd.Status.Value;
            using (var conn = CreateConnection())
            {
                await conn.UpdateAsync(farmInfo).ConfigureAwait(false);
                return farmInfo.Id;
            };

        }
    }
}