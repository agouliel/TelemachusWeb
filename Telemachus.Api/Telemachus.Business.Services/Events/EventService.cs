using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Helpers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Telemachus.Business.Interfaces.Authentication;
using Telemachus.Business.Interfaces.Cargo;
using Telemachus.Business.Interfaces.Events;
using Telemachus.Business.Models;
using Telemachus.Business.Models.Events;
using Telemachus.Business.Models.Events.Events;
using Telemachus.Business.Models.Info;
using Telemachus.Business.Services.Mappers;
using Telemachus.Data.Interfaces.Services;
using Telemachus.Data.Models;
using Telemachus.Data.Models.Authentication;
using Telemachus.Data.Models.Events;
using Telemachus.Data.Models.Ports;

namespace Telemachus.Business.Services.Events
{
    public class EventService : IEventService
    {
        private readonly ICargoBusinessService _cargoService;
        private readonly IEventDataService _eventDataService;
        private readonly IEventTypeDataService _eventTypeDataService;
        private readonly IVoyageDataService _voyageDataService;
        private readonly IFileDataService _fileDataService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IConfiguration _configuration;
        private string _attachmentsPath = "";

        public EventService(ICargoBusinessService cargoService, IEventDataService eventDataService, IVoyageDataService voyageDataService, IEventTypeDataService eventTypeDataService, IFileDataService fileDataService, IConfiguration configuration, IAuthenticationService authenticationService)
        {
            _eventDataService = eventDataService;
            _voyageDataService = voyageDataService;
            _eventTypeDataService = eventTypeDataService;
            _fileDataService = fileDataService;
            _configuration = configuration;
            _authenticationService = authenticationService;
            _attachmentsPath = configuration["AttachmentsPath"];
            _cargoService = cargoService;
            if (string.IsNullOrEmpty(_attachmentsPath))
            {
                _attachmentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Telemachus");
            }
        }

        public async Task<CoordsBusinessModel> GetCoords(string userId)
        {
            var coords = await _eventDataService.GetCoords(userId);
            if (coords == null)
            {
                return null;
            }
            return new CoordsBusinessModel()
            {
                Latitude = coords[0],
                Longitude = coords[1]
            };
        }

        public async Task<List<PortBusinessModel>> ListPorts(string userId, string query)
        {
            var ports = await _eventDataService.ListPorts(userId, query);

            return ports.Select(_ => _.ToBusinessModel()).ToList();
        }

        public async Task<List<string>> Search(string target, string query)
        {
            var options = await _eventDataService.Search(target, query);

            return options;
        }

        public async Task<int?> GetRelatedEvent(int eventId)
        {
            var relatedEventId = await _eventDataService.GetRelatedEvent(eventId);
            return relatedEventId;
        }

        public async Task<PortBusinessModel> GetPort(int id)
        {
            var port = await _eventDataService.GetPort(id);

            return port.ToBusinessModel();
        }

        public async Task<EventDataModel> GetUserEventAsync(int eventId)
        {
            var userEvent = await _eventDataService.GetUserEventAsync(eventId);
            return userEvent;
        }
        public async Task<List<ConditionEventsBusinessModel>> GetUserEventsAsync(string userId, int page, int pageSize, List<int> eventTypeIds, List<int> statuses, DateTime? from, DateTime? to)
        {
            var groupedEvents = await _eventDataService.GetUserEventsAsync(userId, page, pageSize, eventTypeIds, statuses, from, to);
            return groupedEvents?.Select(a => a.ToBusinessModel()).ToList();
        }

        public async Task<EventStateBusinessModel> GetEventState(string userId, [FromQuery] DateTimeOffset? timestamp)
        {

            timestamp = timestamp ?? DateTimeOffset.Now.AddDays(1);

            var @event = await _eventDataService.GetEventFromTimestamp(timestamp.Value, userId);

            var currentVoyage = await _voyageDataService.GetCurrentVoyageAsync(userId);

            var latestEvent = await _eventDataService.GetLatestEvent(userId, DateTimeOffset.Now);

            var availableEventTypes = await _eventTypeDataService.GetEventTypesFromTargetEventAsync(@event, timestamp.Value, currentVoyage);

            var condition = @event?.EventCondition ?? currentVoyage.CurrentCondition;

            var availableEventTypesSorted = availableEventTypes.ToBusinessModel().OrderBy(a => a.Available == false).ThenBy(a => a.IsPairedEvent).ThenBy(a => a.Name).ToList();
            var bunkeringPlans = await GetBunkeringPlanList(userId, timestamp);

            var defaultPorts = await _eventDataService.GetDefaultPorts();

            var nearestPorts = new List<Port>();

            if (@event != null)
            {
                if (@event.Lat.HasValue && @event.Lng.HasValue)
                {
                    nearestPorts = await _eventDataService.GetNearestPorts((double)@event.Lat, (double)@event.Lng);
                }
            }

            var suggestedTimestamp = @event?.Timestamp.Value.AddMinutes(1);

            if (latestEvent?.CurrentVoyageConditionKey == @event?.CurrentVoyageConditionKey)
            {
                suggestedTimestamp = latestEvent?.Timestamp.Value.AddMinutes(1);
            }

            var model = new EventStateBusinessModel()
            {
                Timestamp = suggestedTimestamp,
                LatDegrees = @event?.LatDegrees,
                LatMinutes = @event?.LatMinutes,
                LatSeconds = @event?.LatSeconds,
                LongDegrees = @event?.LongDegrees,
                LongMinutes = @event?.LongMinutes,
                LongSeconds = @event?.LongSeconds,
                Lat = @event?.Lat,
                Lng = @event?.Lng,
                Port = @event?.Port.ToBusinessModel(),
                Condition = condition.ToBusinessModel(),
                EventTypes = availableEventTypesSorted,
                BunkeringPlans = bunkeringPlans,
                NearestPorts = nearestPorts.Select(p => p.ToBusinessModel()).ToList(),
                DefaultPorts = defaultPorts.Select(p => p.ToBusinessModel()).ToList()
            };
            return model;
        }

        public Task<int> GetUserEventsCountAsync(string userId, List<int> eventTypeIds, List<int> statuses, DateTime? from, DateTime? to)
        {
            return _eventDataService.GetUserEventsCountAsync(userId, eventTypeIds, statuses, from, to);
        }

        public Task ApproveEventAsync(int eventId)
        {
            return _eventDataService.ApproveEventAsync(eventId);
        }

        public async Task RejectEventAsync(int eventId)
        {
            await _eventDataService.RejectEventAsync(eventId);
        }

        public async Task<InfoBusinessModel> CreateCurrentEventAsync(EventBaseBusinessModel eventModel)
        {
            await using var transaction = await _eventDataService.BeginTransactionAsync();

            var coords = Converters.DMSToDecimalDegrees(
                eventModel.LatDegrees,
                eventModel.LatMinutes,
                eventModel.LatSeconds,
                eventModel.LongDegrees,
                eventModel.LongMinutes,
                eventModel.LongSeconds);

            eventModel.Lat = coords?[0];
            eventModel.Lng = coords?[1];

            try
            {
                var @event = await _eventDataService.CreateEventAsync(eventModel.ToDataModel());
                if (eventModel.StsOperation != null)
                {
                    eventModel.StsOperation.EventId = @event.Id;
                    var stsOperation = eventModel.StsOperation.ToDataModel();
                    await _eventDataService.AddStsOperation(stsOperation);

                }
                var cargoDetails = eventModel.ToCargoDetailsDataModel(eventModel.UserId);
                if (cargoDetails != null)
                {
                    await _cargoService.CreateCargoDetails(@event.Id, cargoDetails);
                }
                await transaction.CommitAsync();
                return new InfoBusinessModel() { };
            }
            catch (CustomException e)
            {
                await transaction.RollbackAsync();
                return new InfoBusinessModel()
                {
                    Error = e.Message
                };
            }
        }

        public async Task<InfoBusinessModel> DeleteEventAsync(int eventId)
        {

            var error = await _eventDataService.DeleteEventAsync(eventId);

            if (string.IsNullOrEmpty(error))
            {
                return new InfoBusinessModel()
                {
                    Info = "The fact has been deleted successfully!"
                };
            }

            return new InfoBusinessModel()
            {
                Error = error
            };

        }

        public async Task<InfoBusinessModel> UpdateEventAsync(EventUpdateBusinessModel eventViewModel)
        {
            var eventDataModel = new EventDataModel()
            {
                Id = eventViewModel.Id,
                Comment = eventViewModel.Comment,
                PortId = eventViewModel.PortId,
                CustomEventName = eventViewModel.CustomEventName,
                Timestamp = eventViewModel.Timestamp,
                LatDegrees = eventViewModel.LatDegrees,
                LatMinutes = eventViewModel.LatMinutes,
                LatSeconds = eventViewModel.LatSeconds,
                LongDegrees = eventViewModel.LongDegrees,
                LongMinutes = eventViewModel.LongMinutes,
                LongSeconds = eventViewModel.LongSeconds,
                Lat = eventViewModel.Lat,
                Lng = eventViewModel.Lng,
                StsOperation = eventViewModel.StsOperation.ToDataModel()
            };

            await using var transaction = await _eventDataService.BeginTransactionAsync();

            try
            {
                var updatedEvent = await _eventDataService.UpdateEventAsync(eventDataModel);
                var cargoDetails = eventViewModel.ToCargoDetailsDataModel(updatedEvent.UserId);
                if (cargoDetails != null)
                {
                    await _cargoService.UpdateCargoDetails(updatedEvent.Id, cargoDetails);
                }
                await transaction.CommitAsync();
                return new InfoBusinessModel() { };
            }
            catch (CustomException e)
            {
                await transaction.RollbackAsync();
                return new InfoBusinessModel()
                {
                    Error = e.Message
                };
            }

        }

        public async Task RejectRelatedNoonEvents(int eventId)
        {
            await _eventDataService.RejectRelatedNoonEvents(eventId);
        }
        public async Task DeleteRelatedNoonEvents(int eventId)
        {
            await _eventDataService.DeleteRelatedNoonEvents(eventId);
        }
        public async Task<bool> HasCommenceBunkeringReport(int eventId)
        {
            return await _eventDataService.HasCommenceBunkeringReport(eventId);
        }

        public async Task<List<BunkeringDataBusinessModel>> GetBunkeringPlanList(string userId, DateTimeOffset? timestamp)
        {
            var bunkeringDataList = await _eventDataService.GetBunkeringPlanList(userId, timestamp);
            return bunkeringDataList.Select(a => a.ToBusinessModel()).ToList();
        }
        public async Task<List<ImportViewModel>> Import()
        {
            return await _eventDataService.Import();
        }
        public async Task<List<Dictionary<string, object>>> GetTaResults(string query)
        {
            return await _eventDataService.GetTaResults(query);
        }

        public async Task<User> GetVesselDetails(string userId)
        {
            return await _eventDataService.GetVesselDetails(userId);
        }

    }
}
