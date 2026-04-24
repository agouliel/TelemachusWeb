using System;
using System.Threading.Tasks;
using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Interfaces.Services
{
    public interface IVoyageDataService
    {
        Task<VoyageDataModel> GetCurrentVoyageAsync(string userId);
        Task<bool> HasAnyFacts(string userId);
        Task UpdateVoyageCurrentConditionAsync(int conditionId, int voyageId);
        Task<bool> FinishCurrentVoyageAsync(string userId, DateTimeOffset? finishDate);
        Task<VoyageDataModel> CreateVoyageAsync(VoyageDataModel voyage);
        Task<int?> GetPrevConditionId(string userId, int voyageId);
        Task<VoyageDataModel> GetVoyageAsync(int eventId);
    }
}
