using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Interfaces.Services
{
    public interface IEventTypeDataService
    {
        Task<List<EventTypeDataModel>> GetEventTypesAsync();
        Task<List<EventTypeDataModel>> GetEventTypesFromTargetEventAsync(EventDataModel @event, DateTimeOffset targetTimestamp, VoyageDataModel voyage = null);
    }
}
