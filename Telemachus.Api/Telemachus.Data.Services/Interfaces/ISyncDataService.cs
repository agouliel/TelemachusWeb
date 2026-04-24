using System.Threading.Tasks;
using Telemachus.Data.Models.Sync;

namespace Telemachus.Data.Services.Interfaces
{
    public interface ISyncDataService
    {
        Task<SyncResponseMasterViewModel> GetMasterDataAsync(SyncRequestMasterViewModel data);
        Task<SyncResponseViewModel> GetDataAsync(SyncRequestViewModel data);
        Task<SyncResponseViewModel> GetReadOnlyDataAsync(SyncRequestViewModel data);
        Task SyncMasterValues(string userId);
        Task SyncDataValues(string userId);
        Task<bool> HasValidRemoteAddress(string userId);
        Task PortSync();
        Task<SyncRequestViewModel> GetDataTimestamps(string userPrefix);
        Task SaveData(SyncResponseViewModel vessel);
    }
}
