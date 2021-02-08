namespace Allycs.Common.Devices.Services
{
    using Allycs.Common.Devices.Dtos;
    using Allycs.Common.Devices.Entities;
    using Allycs.Core;
    using Dapper;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class DevicesService : PostgresService, IDevicesService
    {
        private readonly AppSettings _settings;
        private readonly ILogger<DevicesService> _logger;

        public DevicesService(IOptionsSnapshot<AppSettings> option,
            ILogger<DevicesService> logger)
            : base(option)
        {
            _settings = option.Value;
            _logger = logger;
        }

        public async Task<bool> ExistDeviceInfoAsync(string id)
        {
            using var conn = CreateConnection();
            return (await conn.RecordCountAsync<DeviceInfo>($" WHERE id='{id}'").ConfigureAwait(false)) > 0;
        }
        public async Task<bool> ExistDeviceInfoByDeviceCodeAsync(string deviceCode)
        {
            using var conn = CreateConnection();
            return (await conn.RecordCountAsync<DeviceInfo>($" WHERE device_code='{deviceCode}'").ConfigureAwait(false)) > 0;
        }
        public async Task<bool> ExistDeviceInfoByDeviceSerialNumberAsync(string serialNumber)
        {
            using var conn = CreateConnection();
            return (await conn.RecordCountAsync<DeviceInfo>($" WHERE serial_number='{serialNumber}'").ConfigureAwait(false)) > 0;
        }
        public async Task<bool> NewDeviceInfoAsync(DeviceInfo entity)
        {
            using var conn = CreateConnection();
            return await conn.InsertAsync<DeviceInfo>(entity).ConfigureAwait(false);
        }

        public async Task<DeviceInfo> GetDeviceInfoAsync(string id)
        {
            using var conn = CreateConnection();
            return await conn.GetAsync<DeviceInfo>(id).ConfigureAwait(false);
        }
        public async Task<bool> UpdateDeviceInfoAsync(DeviceInfo entity)
        {
            using var conn = CreateConnection();
            return await conn.UpdateAsync(entity).ConfigureAwait(false) > 0;
        }
        private SqlAndDps DeviceDataFilterSql(GetDeviceDataListCmd cmd)
        {
            var paras = new DynamicParameters();
            var sql = " 1=1 ";
            if (cmd.Type.HasValue && (int)cmd.Type.Value != -1)
            {
                sql += " AND sensor_type = @type ";
                paras.Add("type", cmd.Type.Value);
            }
            if (cmd.StartOn.HasValue)
            {
                sql += " AND update_on > @upOn ";
                paras.Add("upOn", cmd.StartOn.Value);
            }
            if (cmd.EndOn.HasValue)
            {
                sql += " AND update_on > @endOn ";
                paras.Add("endOn", cmd.EndOn.Value);
            }
            if (!cmd.DeviceId.IsNullOrWhiteSpace())
            {
                sql += " AND device_id=@deviceId";
                paras.Add("deviceId", cmd.DeviceId.Trim());
            }

            return new SqlAndDps { Sql = sql, Dps = paras };
        }
        public async Task<IEnumerable<DeviceData>> GetDeviceDataSAsync(GetDeviceDataListCmd cmd, PagedListQuery plQuery)
        {

            using (var conn = CreateConnection())
            {
                conn.Open();
                var sql = "SELECT * FROM device_data WHERE ";

                var demo = DeviceDataFilterSql(cmd);
                sql += demo.Sql;
                sql += " ORDER BY created_on DESC ";
                sql += plQuery.PostgresLimitPartialSql();
                return await conn.QueryAsync<DeviceData>(sql, demo.Dps).ConfigureAwait(false);
            }
        }
        public async Task<bool> NewDeviceDataAsync(DeviceData entity)
        {
            using var conn = CreateConnection();
            return await conn.InsertAsync<DeviceData>(entity).ConfigureAwait(false);
        }
        public async Task NewMultisensorDeviceDataAsync(SensorDataDto dto)
        {
            var timeNow = DateTime.Now;
            if (dto.Temperature.HasValue)
                await NewDeviceDataAsync(new DeviceData
                {
                    DeviceId = dto.Id,
                    SensorType = SensorType.Temperature,
                    Data = dto.Temperature.Value,
                    DataStr = dto.Temperature.Value.ToString(),
                    UnitName = "℃",
                    Remark = dto.Remark,
                    Information = dto.Information,
                    UpdateOn = timeNow
                }).ConfigureAwait(false);
            if (dto.Humidity.HasValue)
                await NewDeviceDataAsync(new DeviceData
                {
                    DeviceId = dto.Id,
                    SensorType = SensorType.Humidity,
                    Data = dto.Humidity.Value,
                    DataStr = dto.Humidity.Value.ToString(),
                    UnitName = "%",
                    Remark = dto.Remark,
                    Information = dto.Information,
                    UpdateOn = timeNow
                }).ConfigureAwait(false);

            if (dto.CarbonDioxide.HasValue)
                await NewDeviceDataAsync(new DeviceData
                {
                    DeviceId = dto.Id,
                    SensorType = SensorType.CarbonDioxide,
                    Data = dto.CarbonDioxide.Value,
                    DataStr = dto.CarbonDioxide.Value.ToString(),
                    UnitName = "%",
                    Remark = dto.Remark,
                    Information = dto.Information,
                    UpdateOn = timeNow
                }).ConfigureAwait(false);
            if (dto.AcidBase.HasValue)
                await NewDeviceDataAsync(new DeviceData
                {
                    DeviceId = dto.Id,
                    SensorType = SensorType.AcidBase,
                    Data = dto.AcidBase.Value,
                    DataStr = dto.AcidBase.Value.ToString(),
                    UnitName = "PH",
                    Remark = dto.Remark,
                    Information = dto.Information,
                    UpdateOn = timeNow
                }).ConfigureAwait(false);
            if (dto.Nitrogen.HasValue)
                await NewDeviceDataAsync(new DeviceData
                {
                    DeviceId = dto.Id,
                    SensorType = SensorType.Nitrogen,
                    Data = dto.Nitrogen.Value,
                    DataStr = dto.Nitrogen.Value.ToString(),
                    UnitName = "%",
                    Remark = dto.Remark,
                    Information = dto.Information,
                    UpdateOn = timeNow
                }).ConfigureAwait(false);
            if (dto.Phosphorus.HasValue)
                await NewDeviceDataAsync(new DeviceData
                {
                    DeviceId = dto.Id,
                    SensorType = SensorType.Phosphorus,
                    Data = dto.Phosphorus.Value,
                    DataStr = dto.Phosphorus.Value.ToString(),
                    UnitName = "%",
                    Remark = dto.Remark,
                    Information = dto.Information,
                    UpdateOn = timeNow
                }).ConfigureAwait(false);
            if (dto.Potassium.HasValue)
                await NewDeviceDataAsync(new DeviceData
                {
                    DeviceId = dto.Id,
                    SensorType = SensorType.Potassium,
                    Data = dto.Potassium.Value,
                    DataStr = dto.Potassium.Value.ToString(),
                    UnitName = "%",
                    Remark = dto.Remark,
                    Information = dto.Information,
                    UpdateOn = timeNow
                }).ConfigureAwait(false);
            if (dto.Illuminance.HasValue)
                await NewDeviceDataAsync(new DeviceData
                {
                    DeviceId = dto.Id,
                    SensorType = SensorType.Illuminance,
                    Data = dto.Illuminance.Value,
                    DataStr = dto.Illuminance.Value.ToString(),
                    UnitName = "lux",
                    Remark = dto.Remark,
                    Information = dto.Information,
                    UpdateOn = timeNow
                }).ConfigureAwait(false);
        }
    }
}