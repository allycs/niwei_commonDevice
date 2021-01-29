namespace Allycs.Common.Devices.Services
{
    using Entities;
    using Dapper;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System.Threading.Tasks;

    public class MemberLoginLogService : PostgresService, IMemberLoginLogService
    {
        private readonly AppSettings _settings;
        private readonly ILogger<MemberLoginLogService> _logger;

        public MemberLoginLogService(
            IOptionsSnapshot<AppSettings> option,
            ILogger<MemberLoginLogService> logger)
            : base(option)
        {
            _settings = option.Value;
            _logger = logger;
        }

        public async Task<int> NewLogAsync(MemberLoginLog entity)
        {
            using (var conn = CreateConnection())
            {
                return await conn.InsertAsync<int>(entity).ConfigureAwait(false);
            }
        }
    }
}