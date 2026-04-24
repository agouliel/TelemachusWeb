using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using Telemachus.Data.Interfaces.Services;
using Telemachus.Data.Models;
using Telemachus.Data.Models.Authentication;
using Telemachus.Data.Models.Cargo;
using Telemachus.Data.Models.DataTransferModels;
using Telemachus.Data.Models.Events;
using Telemachus.Data.Models.Ports;
using Telemachus.Data.Services.Interfaces;

namespace Telemachus.Data.Services.Services
{
    public class EventDataService : IEventDataService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICargoDataService _cargoService;

        public EventDataService(ICargoDataService cargoService, IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
            _cargoService = cargoService;
        }
        public async Task AddStsOperation(StsOperation stsOperation)
        {
            await _eventRepository.AddStsOperation(stsOperation);
        }
        public async Task<decimal[]> GetCoords(string userId)
        {
            var eventA = await _eventRepository.GetUserEventQuery(userId)
                .Where(e => e.Timestamp.HasValue && e.UserId == userId &&
                    e.Lat != null &&
                    e.Lng != null
                )
                .OrderByDescending(e => e.Timestamp)
                .Select(e => new EventDataModel()
                {
                    Timestamp = e.Timestamp,
                    Lat = e.Lat,
                    Lng = e.Lng
                })
                .FirstOrDefaultAsync();
            var eventB = await _eventRepository.GetUserEventQuery(userId)
                .Where(e => e.Timestamp.HasValue && e.UserId == userId &&
                    e.Port != null && e.Port.Latitude.HasValue && e.Port.Longitude.HasValue
                )
                .OrderByDescending(e => e.Timestamp)
                .Select(e => new EventDataModel()
                {
                    Timestamp = e.Timestamp,
                    Port = new Port()
                    {
                        Latitude = e.Port.Latitude,
                        Longitude = e.Port.Longitude
                    }
                })
                .FirstOrDefaultAsync();

            if (eventB != null && (eventA == null || eventB.Timestamp > eventA.Timestamp))
            {
                return new decimal[] { eventB.Port.Latitude.Value, eventB.Port.Longitude.Value };
            }

            if (eventA == null)
            {
                return null;
            }

            return new decimal[] { eventA.Lat.Value, eventA.Lng.Value };
        }

        public async Task<EventDataModel> GetEventFromTimestamp(DateTimeOffset timestamp, string userId)
        {
            return await _eventRepository.GetEventFromTimestamp(timestamp, userId);
        }
        public Task<Port> GetPort(int id)
        {
            return _eventRepository.GetPort(id);
        }

        public Task<int?> GetRelatedEvent(int eventId)
        {
            return _eventRepository.GetRelatedEvent(eventId);
        }

        public Task<List<Port>> ListPorts(string userId, string query)
        {
            return _eventRepository.ListPorts(userId, query);
        }

        public Task<List<string>> Search(string target, string query)
        {
            return _eventRepository.Search(target, query);
        }

        public Task ApproveEventAsync(int eventId)
        {
            return _eventRepository.ApproveEventAsync(eventId);
        }

        public Task RejectEventAsync(int eventId)
        {
            return _eventRepository.RejectEventAsync(eventId);
        }
        public async Task<List<ConditionEventsDataModel>> GetUserEventsAsync(string userId, int page, int pageSize, List<int> eventTypeIds, List<int> statuses, DateTime? from, DateTime? to)
        {
            var groupedEvents = await _eventRepository.GetUserEventsAsync(userId, page, pageSize, eventTypeIds, statuses, from, to);

            var eventTimestamps = groupedEvents
                .SelectMany(g => g.Events)
                .Where(e => e.Timestamp.HasValue)
                .Select(e => e.Timestamp.Value)
                .ToList();

            if (!eventTimestamps.Any()) return groupedEvents;

            var allCargoDetails = await _cargoService.GetCargoDetailsInRange(userId, eventTimestamps.Min(), eventTimestamps.Max());

            foreach (var groupedEvent in groupedEvents)
            {
                foreach (var userEvent in groupedEvent.Events.Where(e => e.Timestamp.HasValue))
                {
                    var ts = userEvent.Timestamp.Value;
                    userEvent.Cargoes = allCargoDetails
                        .Where(cd =>
                            cd.Cargo.StartedOn <= ts &&
                            (cd.Cargo.CompletedOn == null || cd.Cargo.CompletedOn > ts) &&
                            cd.Timestamp <= ts)
                        .GroupBy(cd => cd.CargoId)
                        .Select(g => new CargoModel
                        {
                            Id = g.First().Cargo.Id,
                            GradeId = g.First().Cargo.GradeId,
                            Grade = g.First().Cargo.Grade,
                            Parcel = g.First().Cargo.Parcel,
                            BusinessId = g.First().Cargo.BusinessId,
                            CargoTonnage = (int)g.Sum(cd => cd.Quantity ?? 0)
                        })
                        .ToList();
                }
            }

            return groupedEvents;
        }

        public Task<EventDataModel> GetUserEventAsync(int eventId)
        {
            return _eventRepository.GetUserEventAsync(eventId);
        }

        public IQueryable<EventDataModel> GetUserEventQuery(int eventId)
        {
            return _eventRepository.GetUserEventQuery(eventId);
        }

        public IQueryable<EventDataModel> GetUserEventQueryFromReportId(int reportId)
        {
            return _eventRepository.GetUserEventQueryFromReportId(reportId);
        }

        public IQueryable<EventDataModel> GetUserChildEventQuery(int eventId)
        {
            return _eventRepository.GetUserChildEventQuery(eventId);
        }

        public Task<int> GetUserEventsCountAsync(string userId, List<int> eventTypeIds, List<int> statuses, DateTime? from, DateTime? to)
        {
            return _eventRepository.GetUserEventsCountAsync(userId, eventTypeIds, statuses, from, to);
        }

        public async Task<EventDataModel> CreateEventAsync(EventDataModel eventModel)
        {
            return await _eventRepository.CreateEventAsync(eventModel);
        }

        public async Task<EventDataModel> UpdateEventAsync(EventDataModel eventModel)
        {
            var model = await _eventRepository.UpdateEventAsync(eventModel);
            return model;
        }

        public async Task<string> DeleteEventAsync(int eventId)
        {
            try
            {
                await _eventRepository.DeleteEventAsync2(eventId);
                return null;
            }
            catch (CustomException e)
            {
                return e.Message;
            }
        }

        public Task<List<EventDataModel>> GetPrevVoyageEventRangeAsync(int targetEventId)
        {
            return _eventRepository.GetPrevVoyageEventRangeAsync(targetEventId);
        }

        public IDictionary<SulphurOil, IDictionary<int, double>> GetSteamingTimeAsync(int eventId, List<EventDataModel> cospEvents)
        {
            return _eventRepository.GetSteamingTimeAsync(eventId, cospEvents);
        }

        public async Task<IDictionary<string, double>> GetTotalConsumptionAsync(int eventId)
        {
            return await _eventRepository.GetTotalConsumptionAsync(eventId);
        }

        public async Task<double> GetTotalDistanceOverGroundAsync(int eventId, List<EventDataModel> cospEvents)
        {
            return await _eventRepository.GetTotalDistanceOverGroundAsync(eventId, cospEvents);
        }

        public async Task<User> GetVesselDetails(string userId)
        {
            return await _eventRepository.GetVesselDetails(userId);
        }
        public async Task RejectRelatedNoonEvents(int eventId)
        {
            var eventIds = await _eventRepository.GetRelatedNoonEventIds(eventId);
            foreach (var id in eventIds)
            {
                await _eventRepository.RejectEventAsync(id);
            }
        }
        public async Task DeleteRelatedNoonEvents(int eventId)
        {
            var eventIds = await _eventRepository.GetRelatedNoonEventIds(eventId);
            foreach (var id in eventIds)
            {
                await _eventRepository.DeleteEventAsync2(id);
            }
        }
        public async Task<EventDataModel> GetLatestEvent(string userId, DateTimeOffset? timestamp)
        {
            return await _eventRepository.GetLatestEvent(userId, timestamp);
        }
        public async Task<bool> HasCommenceBunkeringReport(int eventId)
        {
            return await _eventRepository.HasCommenceBunkeringReport(eventId);
        }

        public async Task<List<EventDataModel>> GetCospEventRangeAsync(int targetEventId)
        {
            return await _eventRepository.GetCospEventRangeAsync(targetEventId);
        }

        public async Task<List<BunkeringDataModel>> GetBunkeringPlanList(string userId, DateTimeOffset? timestamp)
        {
            return await _eventRepository.GetBunkeringPlanList(userId, timestamp);
        }

        public async Task<List<ImportViewModel>> Import()
        {
            return await _eventRepository.Import();
        }

        public async Task<bool> HasNextReports(string userId, DateTimeOffset timestamp)
        {
            return await _eventRepository.HasNextReports(userId, timestamp);
        }

        public async Task<List<Port>> GetNearestPorts(double lat, double lng)
        {
            var ports = await _eventRepository.GetNearestPorts(lat, lng);
            return ports;
        }
        public async Task<List<Port>> GetDefaultPorts()
        {
            var ports = await _eventRepository.GetDefaultPorts();
            return ports;
        }

        public async Task<List<Dictionary<string, object>>> GetTaResults(string query)
        {
            return await _eventRepository.GetTaResults(query);
        }

        public async Task<string> GetMrvUnmooringEventBusinessIdFromVoyageId(int voyageId)
        {
            var eventBusinessId = await _eventRepository.GetEventVoyageQuery(voyageId)
                .Where(e => e.EventType.BusinessId == EventType.CompleteUnmooring)
                .OrderBy(e => e.Timestamp)
                .Select(e => e.BusinessId)
                .FirstOrDefaultAsync();
            return eventBusinessId;
        }

        public async Task CreateMrvMis(MrvMisDataModel model)
        {
            await _eventRepository.CreateMrvMis(model);
        }

        public async Task UpdateMrvMis(MrvMisDataModel model)
        {
            await _eventRepository.UpdateMrvMis(model);
        }


        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _eventRepository.BeginTransactionAsync();
        }


    }
}
