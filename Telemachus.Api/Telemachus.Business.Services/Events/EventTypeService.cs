using System.Collections.Generic;
using System.Threading.Tasks;
using Telemachus.Business.Interfaces.Events;
using Telemachus.Business.Models.Events;
using Telemachus.Business.Services.Mappers;
using Telemachus.Data.Interfaces.Services;

namespace Telemachus.Business.Services.Events
{
    public class EventTypeService : IEventTypeService
    {
        private readonly IEventTypeDataService _eventTypeDataService;

        public EventTypeService(IEventTypeDataService eventTypeDataService)
        {
            _eventTypeDataService = eventTypeDataService;
        }

        public async Task<List<EventTypeBusinessModel>> GetEventTypesAsync()
        {
            var eventTypes = await _eventTypeDataService.GetEventTypesAsync();
            return eventTypes.ToBusinessModel();
        }

    }
}
