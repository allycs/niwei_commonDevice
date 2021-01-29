namespace Allycs.Common.Devices.Services
{
    using Entities;
    using System.Threading.Tasks;

    public interface IMemberLoginLogService
    {
        Task<int> NewLogAsync(MemberLoginLog entity);
    }
}