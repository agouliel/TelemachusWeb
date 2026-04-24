using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Services.Interfaces
{
    public interface IVoyageRepository
    {
        Task<VoyageDataModel> GetVoyageAsync(int eventId);
        Task<VoyageDataModel> GetCurrentVoyageAsync(string userId);
        Task<List<ConditionDtoModel>> GetConditionsAsync(string userId, DateTimeOffset eventDate);
        Task UpdateVoyageCurrentConditionAsync(int conditionId, int voyageId);
        Task<bool> FinishCurrentVoyageAsync(string userId, DateTimeOffset? finishDate);
        Task<VoyageDataModel> CreateVoyageAsync(VoyageDataModel voyage);
        Task<bool> HasAnyFacts(string userId);
        Task<int?> GetPrevConditionId(string userId, int voyageId);
    }
}
