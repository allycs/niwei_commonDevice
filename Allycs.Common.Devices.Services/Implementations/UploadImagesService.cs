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
            if (!cmd.CreatedBy.IsNullOrWhiteSpace())
            {
                sql += " AND created_by = @createdBy ";
                paras.Add("createdBy", cmd.CreatedBy.Trim());
            }
            if (!cmd.RefereeId.IsNullOrWhiteSpace())
            {
                sql += " AND referee_id = @refereeId ";
                paras.Add("refereeId", cmd.RefereeId.Trim());
            }
            if (cmd.Status.HasValue && (int)cmd.Status.Value != -1)
            {
                sql += " AND status = @status ";
                paras.Add("status", cmd.Status.Value);
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
            if (!cmd.Extension.IsNullOrWhiteSpace())
            {
                sql += " AND extension LIKE @extension ";
                paras.Add("extension", "%" + cmd.Extension.Trim() + "%");
            }
            if (!cmd.Description.IsNullOrWhiteSpace())
            {
                sql += " AND description LIKE @description ";
                paras.Add("description", "%" + cmd.Description.Trim() + "%");
            }

            return new SqlAndDps { Sql = sql, Dps = paras };
        }

        public async Task<IEnumerable<UploadImages>> GetFarmInfosAsync(GetUploadImageListCmd cmd, PagedListQuery plQuery)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                var sql = "SELECT * FROM upload_images WHERE ";

                var demo = UploadImageFilterSql(cmd);
                sql += demo.Sql;
                sql += " ORDER BY created_on DESC ";
                sql += plQuery.PostgresLimitPartialSql();
                return await conn.QueryAsync<UploadImages>(sql, demo.Dps).ConfigureAwait(false);
            }
        }
        public async Task<bool> NewUploadImageAsync(UploadImagesCmd cmd, string currentMemberId)
        {
            var timeNow = DateTime.Now;
            foreach (var item in cmd.Images)
            { var id = ObjectId.NewId();
                var extension = item.Name.Split('.').LastOrDefault();
                
                var fileFullName = id + "." + extension;
                var path = Path.Combine(GetUploadDirectory(), fileFullName);
                using FileStream destinationStream = File.Create(path);
                await item.Value.CopyToAsync(destinationStream).ConfigureAwait(false);

                using (var conn = CreateConnection())
                {
                    var farmInfo = new UploadImages
                    {
                        Name = cmd.Name.IsNullOrWhiteSpace()?id.ToString():(cmd.Name+"_"+id),
                        Extension = extension,
                        Description = cmd.Description,
                        Remark = cmd.Remark,
                        Type = cmd.Type,
                        Status = cmd.Status,
                        RefereeId=cmd.RefereeId,
                        CreatedBy = currentMemberId,
                        CreatedOn = timeNow,
                    };
                    await conn.InsertAsync<FarmInfo>(farmInfo).ConfigureAwait(false);
                };
            }
            return true;
        }
    }
}