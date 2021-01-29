using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allycs.Common.Devices.Services
{
    public interface IDevicesService
    {
        Task<bool> ExistDeviceAsync(string id);
    }
}
