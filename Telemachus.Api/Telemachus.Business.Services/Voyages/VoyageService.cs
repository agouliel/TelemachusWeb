using Telemachus.Data.Interfaces.Services;

namespace Telemachus.Business.Services.Voyages
{
    public class VoyageService : IVoyageService
    {
        private readonly IVoyageDataService _voyageDataService;
        private readonly IEventDataService _eventDataService;
        public VoyageService(IVoyageDataService voyageDataService, IEventDataService eventDataService)
        {
            _voyageDataService = voyageDataService;
            _eventDataService = eventDataService;
        }

    }
}
