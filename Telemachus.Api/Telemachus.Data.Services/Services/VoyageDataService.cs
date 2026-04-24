using System;
using System.Threading.Tasks;
using Telemachus.Data.Interfaces.Services;
using Telemachus.Data.Models.Events;
using Telemachus.Data.Services.Interfaces;

namespace Telemachus.Data.Services.Services
{
    public class VoyageDataService : IVoyageDataService
    {
        private readonly IVoyageRepository _voyageRepository;
        private readonly IEventTypeRepository _eventTypeRepository;

        public VoyageDataService(IVoyageRepository voyageRepository, IEventTypeRepository eventTypeRepository)
        {
            _voyageRepository = voyageRepository;
            _eventTypeRepository = eventTypeRepository;
        }
        public Task<VoyageDataModel> GetCurrentVoyageAsync(string userId)
        {
            return _voyageRepository.GetCurrentVoyageAsync(userId);
        }
        public Task<bool> HasAnyFacts(string userId)
        {
            return _voyageRepository.HasAnyFacts(userId);
        }

        // to be removed
        public Task UpdateVoyageCurrentConditionAsync(int conditionId, int voyageId)
        {
            return _voyageRepository.UpdateVoyageCurrentConditionAsync(conditionId, voyageId);
        }

        public Task<bool> FinishCurrentVoyageAsync(string userId, DateTimeOffset? finishDate)
        {
            return _voyageRepository.FinishCurrentVoyageAsync(userId, finishDate);
        }

        public Task<VoyageDataModel> CreateVoyageAsync(VoyageDataModel voyage)
        {
            return _voyageRepository.CreateVoyageAsync(voyage);
        }

        public Task<int?> GetPrevConditionId(string userId, int voyageId)
        {
            return _voyageRepository.GetPrevConditionId(userId, voyageId);
        }

        public async Task<VoyageDataModel> GetVoyageAsync(int eventId)
        {
            return await _voyageRepository.GetVoyageAsync(eventId);
        }
    }
}
