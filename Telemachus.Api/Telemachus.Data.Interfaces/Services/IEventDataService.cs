using Enums;

using Microsoft.EntityFrameworkCore.Storage;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telemachus.Data.Models;
using Telemachus.Data.Models.Authentication;
using Telemachus.Data.Models.DataTransferModels;
using Telemachus.Data.Models.Events;
using Telemachus.Data.Models.Ports;

namespace Telemachus.Data.Interfaces.Services
{
    public interface IEventDataService
    {
        Task<decimal[]> GetCoords(string userName);
        Task<List<EventDataModel>> GetCospEventRangeAsync(int targetEventId);
        Task ApproveEventAsync(int eventId);
        Task RejectEventAsync(int eventId);
        IQueryable<EventDataModel> GetUserEventQuery(int eventId);
        IQueryable<EventDataModel> GetUserChildEventQuery(int eventId);
        Task<List<ConditionEventsDataModel>> GetUserEventsAsync(string userId, int page, int pageSize, List<int> eventTypeIds, List<int> statuses, DateTime? from, DateTime? to);
        Task<EventDataModel> GetUserEventAsync(int eventId);
        Task<int> GetUserEventsCountAsync(string userId, List<int> eventTypeIds, List<int> statuses, DateTime? from, DateTime? to);
        Task<EventDataModel> CreateEventAsync(EventDataModel eventModel);
        Task<EventDataModel> UpdateEventAsync(EventDataModel eventModel);
        Task<string> DeleteEventAsync(int eventId);
        Task<EventDataModel> GetLatestEvent(string userId, DateTimeOffset? timestamp);
        Task<List<Port>> ListPorts(string userId, string query);
        Task<Port> GetPort(int id);
        Task<int?> GetRelatedEvent(int eventId);
        Task<List<EventDataModel>> GetPrevVoyageEventRangeAsync(int targetEventId);
        IDictionary<SulphurOil, IDictionary<int, double>> GetSteamingTimeAsync(int eventId, List<EventDataModel> cospEvents);
        Task<IDictionary<string, double>> GetTotalConsumptionAsync(int eventId);
        Task<double> GetTotalDistanceOverGroundAsync(int eventId, List<EventDataModel> cospEvents);
        Task<User> GetVesselDetails(string userId);
        Task RejectRelatedNoonEvents(int eventId);
        Task DeleteRelatedNoonEvents(int eventId);
        Task<bool> HasCommenceBunkeringReport(int eventId);
        Task<List<BunkeringDataModel>> GetBunkeringPlanList(string userId, DateTimeOffset? timestamp);
        Task<List<string>> Search(string target, string query);
        Task<List<ImportViewModel>> Import();
        Task<bool> HasNextReports(string userId, DateTimeOffset timestamp);
        Task<List<Port>> GetNearestPorts(double Lat, double Lng);
        Task<EventDataModel> GetEventFromTimestamp(DateTimeOffset timestamp, string userId);
        IQueryable<EventDataModel> GetUserEventQueryFromReportId(int reportId);
        Task<List<Port>> GetDefaultPorts();
        Task<List<Dictionary<string, object>>> GetTaResults(string query);
        Task<string> GetMrvUnmooringEventBusinessIdFromVoyageId(int voyageId);
        Task CreateMrvMis(MrvMisDataModel model);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task UpdateMrvMis(MrvMisDataModel model);
        Task AddStsOperation(StsOperation stsOperation);
    }
}
