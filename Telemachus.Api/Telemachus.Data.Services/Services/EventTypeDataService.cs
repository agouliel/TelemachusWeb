using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Telemachus.Data.Interfaces.Services;
using Telemachus.Data.Models.Events;
using Telemachus.Data.Services.Interfaces;

namespace Telemachus.Data.Services.Services
{
    public class EventTypeDataService : IEventTypeDataService
    {
        private readonly IEventTypeRepository _eventTypeRepository;

        public EventTypeDataService(IEventTypeRepository eventTypeRepository)
        {
            _eventTypeRepository = eventTypeRepository;
        }
        public Task<List<EventTypeDataModel>> GetEventTypesAsync()
        {
            return _eventTypeRepository.GetEventTypesAsync();
        }
        public Task<List<EventTypeDataModel>> GetEventTypesFromTargetEventAsync(EventDataModel @event, DateTimeOffset targetTimestamp, VoyageDataModel fallbackVoyage = null)
        {
            return _eventTypeRepository.GetEventTypesFromTargetEventAsync(@event, targetTimestamp, fallbackVoyage);
        }
    }
}
