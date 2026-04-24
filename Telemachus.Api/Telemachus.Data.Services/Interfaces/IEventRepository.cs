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

namespace Telemachus.Data.Services.Interfaces
{
    public interface IEventRepository
    {
        Task DeleteEventAsync2(int eventId);
        Task ApproveEventAsync(int eventId);
        Task RejectEventAsync(int eventId);
        Task<List<ConditionEventsDataModel>> GetUserEventsAsync(string userId, int page, int pageSize, List<int> eventTypeIds, List<int> statuses, DateTime? from, DateTime? to);
        Task<EventDataModel> GetUserEventAsync(int eventId);
        Task<int> GetUserEventsCountAsync(string userId, List<int> eventTypeIds, List<int> statuses, DateTime? from, DateTime? to);
        Task<EventDataModel> CreateEventAsync(EventDataModel eventModel);
        Task<EventDataModel> UpdateEventAsync(EventDataModel eventModel);
        Task<EventDataModel> GetLatestEvent(string userId, DateTimeOffset? timestamp);
        Task<List<Port>> ListPorts(string userId, string query);
        Task<Port> GetPort(int id);
        Task<int?> GetRelatedEvent(int eventId);
        Task<List<EventDataModel>> GetPrevVoyageEventRangeAsync(int targetEventId);
        IDictionary<SulphurOil, IDictionary<int, double>> GetSteamingTimeAsync(int eventId, List<EventDataModel> cospEvents);
        Task<List<EventDataModel>> GetCospEventRangeAsync(int targetEventId);
        Task<IDictionary<string, double>> GetTotalConsumptionAsync(int eventId);
        Task<double> GetTotalDistanceOverGroundAsync(int eventId, List<EventDataModel> cospEvents);
        Task<User> GetVesselDetails(string userId);
        Task<List<int>> GetRelatedNoonEventIds(int bunkeringEventId);
        Task<bool> HasCommenceBunkeringReport(int eventId);
        IQueryable<EventDataModel> GetUserEventQuery(int eventId);
        Task<List<BunkeringDataModel>> GetBunkeringPlanList(string userId, DateTimeOffset? timestamp);
        Task<int> CreateBunkeringData(BunkeringDataModel bunkeringData);
        Task<List<string>> Search(string targtet, string query);
        Task<List<ImportViewModel>> Import();
        Task<bool> HasNextReports(string userId, DateTimeOffset timestamp);
        Task<List<Port>> GetNearestPorts(double lat, double lng);
        Task<EventDataModel> GetEventFromTimestamp(DateTimeOffset timestamp, string userId);
        IQueryable<EventDataModel> GetUserChildEventQuery(int eventId);
        IQueryable<EventDataModel> GetUserEventQueryFromReportId(int reportId);
        IQueryable<EventDataModel> GetUserEventQuery(string userId);
        Task<List<Port>> GetDefaultPorts();
        Task<List<Dictionary<string, object>>> GetTaResults(string query);
        IQueryable<EventDataModel> GetEventVoyageQuery(int voyageId);
        Task CreateMrvMis(MrvMisDataModel model);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task UpdateMrvMis(MrvMisDataModel model);
        Task AddStsOperation(StsOperation stsOperation);
    }
}
