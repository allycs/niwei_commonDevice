namespace Allycs.Common.Devices.Services
{
    using Dtos;
    using Allycs.Core;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System.Threading.Tasks;

    public class FarmValidatableService : PostgresService, IFarmValidatableService
    {
        private readonly AppSettings _settings;
        private readonly IFarmService _farmService;
        private readonly ILogger<FarmValidatableService> _logger;

        public FarmValidatableService(
            IOptionsSnapshot<AppSettings> option,
            IFarmService farmService,
            ILogger<FarmValidatableService> logger)
            : base(option)
        {
            _settings = option.Value;
            _farmService = farmService;
            _logger = logger;
        }

        public async Task<string> CheckNewFarmInfoCmdValidatableAsync(NewFarmInfoCmd cmd)
        {
            if (cmd.Name.IsNullOrWhiteSpace())
                return "农场名称必填";
            if (await _farmService.ExistFarmInfoByNameAsync(cmd.Name).ConfigureAwait(false))
                return "农场名称已存在";
            if (cmd.Telephone.IsNullOrWhiteSpace())
                return "农场电话必填";
            if (await _farmService.ExistFarmInfoByTelephoneAsync(cmd.Telephone).ConfigureAwait(false))
                return "农场电话已存在";
            if (cmd.Address.IsNullOrWhiteSpace())
                return "农场地址必填";
            return null;
        }
    }
}