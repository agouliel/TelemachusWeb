using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Services.Interfaces
{
    public interface IEventTypeRepository
    {
        Task<List<EventTypeDataModel>> GetEventTypesAsync();
        Task<List<EventTypeDataModel>> GetEventTypesFromTargetEventAsync(EventDataModel @event, DateTimeOffset targetTimestamp, VoyageDataModel fallbackVoyage = null);
    }
}
