using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telemachus.Business.Models;
using Telemachus.Business.Models.Events;
using Telemachus.Business.Models.Events.Events;
using Telemachus.Business.Models.Info;
using Telemachus.Data.Models;
using Telemachus.Data.Models.Authentication;
using Telemachus.Data.Models.Events;

namespace Telemachus.Business.Interfaces.Events
{
    public interface IEventService
    {
        Task ApproveEventAsync(int eventId);
        Task RejectEventAsync(int eventId);
        Task<EventDataModel> GetUserEventAsync(int eventId);
        Task<List<ConditionEventsBusinessModel>> GetUserEventsAsync(string userId, int page, int pageSize, List<int> eventTypeIds, List<int> statuses, DateTime? from, DateTime? to);
        Task<int> GetUserEventsCountAsync(string userId, List<int> eventTypeIds, List<int> statuses, DateTime? from, DateTime? to);
        Task<InfoBusinessModel> CreateCurrentEventAsync(EventBaseBusinessModel eventModel);
        Task<InfoBusinessModel> DeleteEventAsync(int eventId);
        Task<InfoBusinessModel> UpdateEventAsync(EventUpdateBusinessModel eventModel);
        Task<EventStateBusinessModel> GetEventState(string userId, DateTimeOffset? timestamp);
        Task<List<PortBusinessModel>> ListPorts(string userId, string query);
        Task<PortBusinessModel> GetPort(int id);
        Task<int?> GetRelatedEvent(int eventId);
        Task RejectRelatedNoonEvents(int eventId);
        Task DeleteRelatedNoonEvents(int eventId);
        Task<bool> HasCommenceBunkeringReport(int eventId);
        Task<List<BunkeringDataBusinessModel>> GetBunkeringPlanList(string userId, DateTimeOffset? timestamp);
        Task<List<string>> Search(string target, string query);
        Task<List<ImportViewModel>> Import();
        Task<CoordsBusinessModel> GetCoords(string userId);
        Task<List<Dictionary<string, object>>> GetTaResults(string query);
        Task<User> GetVesselDetails(string userId);
    }
}
