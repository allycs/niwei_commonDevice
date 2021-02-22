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
        private readonly IMemberService _memberService;
        private readonly ILogger<FarmValidatableService> _logger;

        public FarmValidatableService(
            IOptionsSnapshot<AppSettings> option,
            IFarmService farmService,
            IMemberService memberService,
            ILogger<FarmValidatableService> logger)
            : base(option)
        {
            _settings = option.Value;
            _farmService = farmService;
            _memberService = memberService;
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
        public async Task<string> CheckUpdateFarmInfoCmdValidatableAsync(UpdateFarmInfoCmd cmd)
        {
            if (cmd.Id.IsNullOrWhiteSpace())
                return "请指定所选农场";
           if(!await _farmService.ExistFarmInfoAsync(cmd.Id).ConfigureAwait(false))
                return "农场不存在";
            var farmInfo = await _farmService.GetFarmInfoAsync(cmd.Id).ConfigureAwait(false);
            if (!cmd.Name.IsNullOrWhiteSpace() && cmd.Name != farmInfo.Name && await _farmService.ExistFarmInfoByNameAsync(cmd.Name).ConfigureAwait(false))
                return "农场名称已存在";
            if (!cmd.Telephone.IsNullOrWhiteSpace() && cmd.Telephone != farmInfo.Telephone && await _farmService.ExistFarmInfoByTelephoneAsync(cmd.Name).ConfigureAwait(false))
                return "农场电话已存在";
            if(!cmd.PersonLiable.IsNullOrWhiteSpace())
            {
                if (cmd.PersonLiable.Trim().Length != 24)
                    return "农场责任人不存在";
                if(await _memberService.ExistMemberInfoAsync(cmd.PersonLiable).ConfigureAwait(false))
                    return "农场责任人不存在";
            }
            return null;
        }
    }
}