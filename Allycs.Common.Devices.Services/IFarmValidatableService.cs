using Allycs.Common.Devices.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allycs.Common.Devices.Services
{
    public  interface IFarmValidatableService
    {
        Task<string> CheckNewFarmInfoCmdValidatableAsync(NewFarmInfoCmd cmd);
        Task<string> CheckUpdateFarmInfoCmdValidatableAsync(UpdateFarmInfoCmd cmd);
    }
}
