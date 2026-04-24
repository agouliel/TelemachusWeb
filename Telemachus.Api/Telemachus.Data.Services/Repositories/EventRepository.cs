using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Enums;

using Helpers;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using NetTopologySuite.Geometries;

using Newtonsoft.Json;

using Telemachus.Data.Models;
using Telemachus.Data.Models.Authentication;
using Telemachus.Data.Models.DataTransferModels;
using Telemachus.Data.Models.Events;
using Telemachus.Data.Models.Ports;
using Telemachus.Data.Models.Reports;
using Telemachus.Data.Services.Context;
using Telemachus.Data.Services.Interfaces;

namespace Telemachus.Data.Services.Repositories
{
    public class EventRepository : IEventRepository
    {
        //private static int EOSP = 53;
        //private static int COSP = 29;
        //private static int NOON = 1;
        private readonly TelemachusContext _context;
        private readonly IHostingEnvironment _env;
        private readonly ICargoDataService _cargoService;
        private readonly ILogger<EventRepository> _logger;
        private readonly IConfiguration _config;
        public EventRepository(TelemachusContext context, IHostingEnvironment env, ICargoDataService cargoService, ILogger<EventRepository> logger, IConfiguration config)
        {
            _env = env;
            _context = context;
            _cargoService = cargoService;
            _logger = logger;
            _config = config;
        }
        public async Task AddStsOperation(StsOperation stsOperation)
        {
            _context.StsOperations.Add(stsOperation);
            await _context.SaveChangesAsync();
        }
        public async Task<int?> GetRelatedEvent(int eventId)
        {
            var relatedEventId = await _context
                .Events.Where(_ =>
                _.ParentEventId == eventId)
                .Select(_ => _.Id)
                .FirstOrDefaultAsync();
            return relatedEventId;
        }

        public async Task<Port> GetPort(int id)
        {
            return await _context.Port
                .Include(_ => _.Country)
                .ThenInclude(_ => _.Region)
                .ThenInclude(_ => _.Area)
                .Where(_ => _.Id == id).FirstOrDefaultAsync();
        }
        public async Task<EventDataModel> GetLatestEvent(string userId, DateTimeOffset? timestamp)
        {
            var result = await _context.Events
                .AsNoTracking()
                .Include(_ => _.Port)
                .ThenInclude(_ => _.Country)
                .ThenInclude(_ => _.Region)
                .ThenInclude(_ => _.Area)
                .Where(_ => _.UserId == userId && _.Status.BusinessId != Status.InProgress && _.Timestamp != null && _.Timestamp <= DateTime.UtcNow)
                .OrderByDescending(_ => _.Timestamp)
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<List<Port>> ListPorts(string userId, string query)
        {
            var targetEvent = await GetLatestEvent(userId, null);

            query = query.ToLower();

            var ports = await _context.Port
                .Include(_ => _.Country)
                .ThenInclude(_ => _.Region)
                .ThenInclude(_ => _.Area)
                .Where(_ =>
                    _.BusinessId.ToLower() != "96159e74-1d0e-4ac8-a2a4-a5f08eba159f" &&
                    _.BusinessId.ToLower() != "aa501eba-e43f-4171-8830-1925e8decd35" &&
                    _.BusinessId.ToLower() != "7e6459a7-8eeb-45ef-b567-0c78abbb9eb3" &&
                    (_.Name.ToLower().StartsWith(query) || (query.Length > 2 && _.Code.ToLower().StartsWith(query)))
                )
                .OrderBy(_ => _.Name).ThenBy(_ => _.Code).Take(20)
                .ToListAsync();

            if (targetEvent?.Point != null)
            {
                var inputPoint = new Point((double)targetEvent.Lng.Value, (double)targetEvent.Lat.Value) { SRID = 4326 };
                foreach (var port in ports)
                {
                    if (port.Point != null)
                    {
                        double distanceInDegrees = port.Point.Distance(inputPoint);

                        double distanceInMeters = distanceInDegrees * 111000;

                        double distanceInMiles = distanceInMeters / 1609.34;

                        port.Distance = Math.Round(distanceInMiles, 0);
                    }
                }
            }

            return ports;
        }

        public async Task<List<string>> Search(string target, string query)
        {

            var options = new List<string>();

            if (target == "bdn")
            {
                options = await _context.BunkeringData
                    .Where(b => !string.IsNullOrEmpty(b.Bdn) && b.Bdn.ToUpper().StartsWith(query.ToUpper()))
                    .Select(b => b.Bdn)
                    .Distinct()
                    .Take(10)
                    .ToListAsync();
            }
            else if (target == "supplier")
            {
                options = await _context.BunkeringData
                   .Where(b => !string.IsNullOrEmpty(b.Supplier) && b.Supplier.ToUpper().StartsWith(query.ToUpper()))
                   .Select(b => b.Supplier)
                   .Distinct()
                   .Take(10)
                   .ToListAsync();
            }

            return options;
        }

        private async Task<EventDataModel> GetEventFromTimestampWithCoords(EventDataModel targetEvent)
        {
            if (targetEvent?.Timestamp == null || targetEvent.Lat != null)
                return targetEvent;
            var eventWithCoords = await _context.Events
                .Where(e => e.UserId == targetEvent.UserId && e.Timestamp.HasValue && e.Lat.HasValue && e.Timestamp.Value <= targetEvent.Timestamp.Value)
                .OrderByDescending(e => e.Timestamp)
                .ThenByDescending(e => e.Id)
                .FirstOrDefaultAsync();
            targetEvent.LatDegrees = eventWithCoords?.LatDegrees;
            targetEvent.LatMinutes = eventWithCoords?.LatMinutes;
            targetEvent.LatSeconds = eventWithCoords?.LatSeconds;
            targetEvent.LongDegrees = eventWithCoords?.LongDegrees;
            targetEvent.LongMinutes = eventWithCoords?.LongMinutes;
            targetEvent.LongSeconds = eventWithCoords?.LongSeconds;
            targetEvent.Lat = eventWithCoords?.Lat;
            targetEvent.Lng = eventWithCoords?.Lng;
            return targetEvent;
        }

        public async Task<EventDataModel> GetEventFromTimestamp(DateTimeOffset timestamp, string userId)
        {

            var query = _context.Events
                .AsNoTracking()
                .Include(e => e.EventCondition)
                .Include(e => e.EventType)
                .Include(_ => _.Port)
                .ThenInclude(_ => _.Country)
                .ThenInclude(_ => _.Region)
                .ThenInclude(_ => _.Area)
                .Include(_ => _.ParentEvent);

            var targetEvent = await query.Where(e => e.UserId == userId && e.Timestamp.HasValue && e.Timestamp >= timestamp)
                .OrderBy(e => e.Timestamp)
                .ThenBy(e => e.Id)
                .FirstOrDefaultAsync();

            var condition1 = targetEvent != null && targetEvent.EventType.NextConditionId.HasValue && targetEvent.Timestamp > timestamp;
            if (condition1)
            {
                targetEvent = await query
                    .Where(e => e.UserId == userId && e.Timestamp.HasValue && e.Timestamp <= targetEvent.Timestamp && e.Id < targetEvent.Id)
                    .OrderByDescending(e => e.Timestamp)
                    .ThenByDescending(e => e.Id)
                    .FirstOrDefaultAsync();
            }

            if (targetEvent == null)
            {
                var orderedQuery = query
                    .Where(e => e.UserId == userId && e.Timestamp.HasValue)
                    .OrderByDescending(e => e.Timestamp)
                    .ThenByDescending(e => e.Id)
                    .AsQueryable();
                var latestEvent = await orderedQuery.FirstOrDefaultAsync();
                var oldestEvent = await orderedQuery.LastOrDefaultAsync();
                if (timestamp < oldestEvent?.Timestamp)
                {
                    return await GetEventFromTimestampWithCoords(oldestEvent);
                }
                else if (timestamp > latestEvent?.Timestamp)
                {
                    return await GetEventFromTimestampWithCoords(latestEvent);
                }
            }

            return await GetEventFromTimestampWithCoords(targetEvent);
        }

        private async Task CleanBunkeringDataBeforeEventDelete(int eventId)
        {

            var bunkeringDataList = await _context.Events
                .Include(b => b.BunkeringData)
                .ThenInclude(b => b.Tanks)
                .Where(b => b.Id == eventId && b.EventType.BusinessId == EventType.CommenceBunkering && b.BunkeringData != null)
                .Select(b => b.BunkeringData)
                .ToListAsync();
            foreach (var bunkeringData in bunkeringDataList)
            {
                bunkeringData.Bdn = null;
                bunkeringData.Supplier = null;
                bunkeringData.TotalAmount = null;
                bunkeringData.Density = null;
                bunkeringData.SulphurContent = null;
                bunkeringData.Viscosity = null;
                foreach (var tank in bunkeringData.Tanks)
                {
                    tank.Amount = null;
                }
                var attachments = await _context.EventAttachments.Where(a => a.BunkeringDataId == bunkeringData.Id).ToListAsync();
                foreach (var attachment in attachments)
                {
                    _context.EventAttachments.Remove(attachment);
                }

            }
            await _context.SaveChangesAsync();

            bunkeringDataList = await _context.Events
                .Include(b => b.BunkeringData)
                .ThenInclude(b => b.Tanks)
                .Where(b => b.Id == eventId && b.EventType.BusinessId == EventType.BunkeringPlan && b.BunkeringData != null)
                .Select(b => b.BunkeringData)
                .ToListAsync();

            foreach (var bunkeringData in bunkeringDataList)
            {
                _context.BunkeringDataTanks.RemoveRange(bunkeringData.Tanks);
            }

            _context.BunkeringData.RemoveRange(bunkeringDataList);

            await _context.SaveChangesAsync();
            return;
        }

        private async Task PreDelete(int eventId)
        {
            await _cargoService.DeleteCargo(eventId);

            await CleanBunkeringDataBeforeEventDelete(eventId);

            await ProcessReportContextPreEventDeletion(eventId);

            var reports = await _context.Reports
                .Include(r => r.ReportContext)
                .Include(r => r.FieldValues)
                .Where(r => r.EventId == eventId)
                .ToListAsync();

            var fieldValues = reports.SelectMany(r => r.FieldValues).ToList();
            var reportContexts = reports
                .Where(r => r.ReportContext != null)
                .SelectMany(r => r.ReportContext)
                .ToList();

            foreach (var fieldValue in fieldValues)
            {
                fieldValue.ReportContextId = null;
            }
            _context.ReportFieldValues.RemoveRange(fieldValues);
            _context.ReportContext.RemoveRange(reportContexts);
            _context.Reports.RemoveRange(reports);

            await _context.SaveChangesAsync();
        }

        private async Task<List<int>> DeleteEvent(int eventId)
        {
            var deletedEventIds = new List<int>();
            var dbEvent = await _context.Events
                .Include(_ => _.Voyage)
                .Include(_ => _.EventType)
                .ThenInclude(_ => _.Prerequisites)
                .Include(_ => _.ChildrenEvents)
                .ThenInclude(_ => _.EventType)
                .Include(_ => _.ChildrenEvents)
                .ThenInclude(_ => _.Voyage)
                .Where(_ => _.Id == eventId)
            .FirstOrDefaultAsync();

            if (dbEvent == null)
            {
                dbEvent = await _context.Events.FindAsync(eventId);
                if (dbEvent == null)
                {

                    throw new CustomException($"Event with Id {eventId} not found.");
                }
                await PreDelete(dbEvent.Id);
                deletedEventIds.Add(dbEvent.Id);
                _context.Events.Remove(dbEvent);
                await _context.SaveChangesAsync();
                return deletedEventIds;
            }

            if (dbEvent.ParentEventId != null)
            {
                throw new CustomException($"To delete that fact - first of all delete the parent one.");
            }

            if (dbEvent.EventType.IsBunkeringPlan)
            {
                if (dbEvent.BunkeringDataId.HasValue)
                {
                    var hasPairedEvents = await _context.Events
                        .Where(e => e.BunkeringDataId == dbEvent.BunkeringDataId && e.EventType.BusinessId == EventType.CommenceBunkering)
                        .AnyAsync();
                    if (hasPairedEvents)
                    {
                        throw new CustomException($"To delete that fact - first of all delete the Commence Bunkering event.");
                    }
                }
            }

            //var targetEvents = await _context.Events
            //.Include(e => e.EventType)
            //.Where(e => e.UserId == dbEvent.UserId && dbEvent.CurrentVoyageConditionKey == e.CurrentVoyageConditionKey &&
            //            e.EventType.Prerequisites.Any() &&
            //            ((e.EventType.Prerequisites.Any(p => p.AvailableAfterEventTypeId == dbEvent.EventType.Id) && e.Timestamp > dbEvent.Timestamp) ||
            //             (dbEvent.ChildEvent != null && e.EventType.Prerequisites.Any(p => p.AvailableAfterEventTypeId == dbEvent.ChildEvent.EventType.Id) && e.Timestamp > dbEvent.ChildEvent.Timestamp)))
            //.OrderBy(e => e.Timestamp)
            //.ThenBy(e => e.Id)
            //.ToListAsync();

            //if (targetEvents.Any())
            //{
            //    var conflictingEvent = targetEvents.First();
            //    throw new CustomException($"To delete that fact - first of all delete {conflictingEvent.EventType.Name} event in current condition.");
            //}

            var eventIdsChanged = new HashSet<int>();

            if (dbEvent.ChangedCondition())
            {
                var conditionChangeEvent = dbEvent.ChildEvent?.EventType.NextConditionId != null ? dbEvent.ChildEvent : dbEvent;
                var conditionChangeVoyageId = conditionChangeEvent.VoyageId;
                var eventsToMove = await _context
                    .Events
                    .Include(e => e.EventType)
                    .Where(e => e.UserId == conditionChangeEvent.UserId && e.CurrentVoyageConditionKey == conditionChangeEvent.CurrentVoyageConditionKey).ToListAsync();
                var prevEvent = await _context.Events
                    .Include(e => e.Voyage)
                    .Include(e => e.EventCondition)
                    .Where(e => e.UserId == dbEvent.UserId && e.CurrentVoyageConditionKey == conditionChangeEvent.PreviousVoyageConditionKey && e.EventType.NextConditionId.HasValue)
                    .FirstAsync();
                foreach (var eventToMove in eventsToMove)
                {
                    if (eventToMove.Id == dbEvent.Id || eventToMove.Id == dbEvent.ChildEvent?.Id || eventToMove.ParentEventId != null)
                    {
                        continue;
                    }
                    var canBeMoved = await _context.EventTypesConditions.Where(e => e.ConditionId == prevEvent.ConditionId && e.EventTypeId == eventToMove.EventTypeId).AnyAsync();
                    if (!canBeMoved)
                    {
                        throw new CustomException($"{eventToMove.EventType.Name} cannot be moved to {prevEvent.EventCondition.Name} condition.");
                    }
                }

                foreach (var eventToMove in eventsToMove)
                {
                    if (eventToMove.Id == dbEvent.Id || eventToMove.Id == dbEvent.ChildEvent?.Id)
                    {
                        continue;
                    }
                    eventIdsChanged.Add(eventToMove.Id);
                    eventToMove.CurrentVoyageConditionKey = prevEvent.CurrentVoyageConditionKey;
                    eventToMove.ConditionId = prevEvent.ConditionId;
                }
                if (conditionChangeEvent.Voyage.CurrentVoyageConditionKey == conditionChangeEvent.CurrentVoyageConditionKey)
                {
                    conditionChangeEvent.Voyage.CurrentVoyageConditionKey = prevEvent.CurrentVoyageConditionKey;
                    conditionChangeEvent.Voyage.CurrentConditionId = prevEvent.ConditionId;
                }
                var nextConditionEvent = await _context.Events.Where(e => e.UserId == dbEvent.UserId && e.EventType.NextConditionId.HasValue && e.PreviousVoyageConditionKey == conditionChangeEvent.CurrentVoyageConditionKey).SingleOrDefaultAsync();
                if (nextConditionEvent != null)
                {
                    nextConditionEvent.PreviousVoyageConditionKey = conditionChangeEvent.PreviousVoyageConditionKey;
                }
                if (dbEvent.EventType.IsCommenceUnMooring())
                {
                    var nextTargetVoyage = await _context.Voyages.Where(v => v.UserId == conditionChangeEvent.UserId && v.StartDate > prevEvent.Voyage.StartDate && v.Id != conditionChangeEvent.Voyage.Id).AnyAsync();

                    if (!nextTargetVoyage)
                    {
                        prevEvent.Voyage.IsFinished = false;
                        prevEvent.Voyage.EndDate = null;
                        foreach (var eventToMove in eventsToMove)
                        {
                            if (eventToMove.Id == dbEvent.Id || eventToMove.Id == dbEvent.ChildEvent?.Id)
                            {
                                continue;
                            }
                            eventIdsChanged.Add(eventToMove.Id);
                            eventToMove.VoyageId = prevEvent.VoyageId;
                        }
                    }
                    else
                    {
                        var prevVoyageEvents = await _context.Events.Where(e => e.UserId == dbEvent.UserId && e.VoyageId == conditionChangeEvent.VoyageId).ToListAsync();
                        foreach (var voyageEvent in prevVoyageEvents)
                        {
                            eventIdsChanged.Add(voyageEvent.Id);
                            voyageEvent.VoyageId = prevEvent.VoyageId;
                        }
                        var latestEvent = prevVoyageEvents.OrderByDescending(e => e.Timestamp).First();
                        prevEvent.Voyage.EndDate = latestEvent.Timestamp;
                        prevEvent.Voyage.CurrentVoyageConditionKey = latestEvent.CurrentVoyageConditionKey;
                        prevEvent.Voyage.CurrentConditionId = latestEvent.ConditionId;
                    }

                    await _context.SaveChangesAsync();

                    var voyageToDelete = await _context.Voyages
                        .Include(v => v.Events)
                        .Where(v => v.Id == conditionChangeVoyageId).FirstAsync();
                    // TODO: debug 
                    foreach (var v in voyageToDelete.Events)
                    {
                        v.VoyageId = prevEvent.VoyageId;
                    }
                    _context.Voyages.Remove(voyageToDelete);


                }
            }

            if (dbEvent.ChildEvent != null)
            {
                var childEvent = await _context.Events.Where(e => e.Id == dbEvent.ChildEvent.Id).FirstAsync();
                var childVoyageEvent = await _context.MrvMisData.Where(e => e.EventId == dbEvent.ChildEvent.Id).FirstOrDefaultAsync();
                if (childVoyageEvent != null)
                {
                    _context.MrvMisData.Remove(childVoyageEvent);
                }
                await PreDelete(childEvent.Id);
                deletedEventIds.Add(childEvent.Id);
                _context.Events.Remove(childEvent);
                await _context.SaveChangesAsync();
            }



            var userEvent = await _context.Events.Where(e => e.Id == dbEvent.Id).FirstAsync();
            var userVoyageEvent = await _context.MrvMisData.Where(e => e.EventId == dbEvent.Id).FirstOrDefaultAsync();
            if (userVoyageEvent != null)
            {
                _context.MrvMisData.Remove(userVoyageEvent);
            }
            var stsData = await _context.StsOperations.Where(e => e.EventId == dbEvent.Id).FirstOrDefaultAsync();
            if (stsData != null)
            {
                _context.StsOperations.Remove(stsData);
            }
            await PreDelete(userEvent.Id);
            deletedEventIds.Add(userEvent.Id);
            _context.Events.Remove(userEvent);

            //eventIdsChanged

            var voyageDetailEvents = await _context.Events.AsNoTracking().Where(e => eventIdsChanged.Contains(e.Id)).ToListAsync();
            foreach (var voyageDetail in voyageDetailEvents)
            {
                await UpdateVoyageDetails(voyageDetail);
            }
            await _context.SaveChangesAsync();
            return deletedEventIds;
        }
        //private async Task ProcessReportContext(int eventId)
        //{
        //    var report = await _context.Reports
        //        .Include(r => r.ReportContext)
        //        .Where(r => r.EventId == eventId)
        //        .FirstOrDefaultAsync();
        //    foreach (var rc in report.ReportContext)
        //    {
        //        var contextsToUpdate = await _context.ReportContext.Where(r => r.PrevContextId == rc.Id).ToListAsync();
        //        foreach (var contextToUpdate in contextsToUpdate)
        //        {
        //            contextToUpdate.PrevContextId = rc.PrevContextId;
        //        }
        //    }
        //}
        private async Task ProcessReportContextPreEventDeletion(int eventId)
        {
            var report = await _context.Reports
                .Include(r => r.ReportContext)
                .FirstOrDefaultAsync(r => r.EventId == eventId);

            if (report?.ReportContext == null || !report.ReportContext.Any())
                return;

            var contextMap = report.ReportContext.ToDictionary(rc => rc.Id);

            var contextIds = contextMap.Keys.ToList();

            var contextsToUpdate = await _context.ReportContext
                .Where(rc => rc.PrevContextId != null && contextIds.Contains(rc.PrevContextId.Value))
                .ToListAsync();

            foreach (var context in contextsToUpdate)
            {
                if (context.PrevContextId.HasValue &&
                    contextMap.TryGetValue(context.PrevContextId.Value, out var parent))
                {
                    context.PrevContextId = parent.PrevContextId;
                }
            }
        }


        public async Task DeleteEventAsync2(int eventId)
        {

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {


                var deletedEventIds = await DeleteEvent(eventId);

                // TODO: Update voyage_details

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public async Task ApproveEventAsync(int eventId)
        {
            var dbEvent = await _context.Events.Include(_ => _.EventType).FirstOrDefaultAsync(a => a.Id == eventId);
            if (dbEvent != null)
            {
                dbEvent.StatusId = 4;
                await UpdateVoyageDetails(dbEvent);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RejectEventAsync(int eventId)
        {
            var dbEvent = await _context.Events.Include(_ => _.EventType).SingleAsync(a => a.Id == eventId);

            dbEvent.StatusId = 3;
            await UpdateVoyageDetails(dbEvent);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUserEventsCountAsync(string userId, List<int> eventTypeIds, List<int> statuses, DateTime? from, DateTime? to)
        {

            var query = _context.Events
                .Where(e => e.UserId == userId && e.Timestamp.HasValue)
                .AsQueryable();

            if (eventTypeIds != null && eventTypeIds.Any())
            {
                query = query.Where(a => eventTypeIds.Contains(a.EventTypeId));
            }

            if (statuses != null && statuses.Any())
            {
                var statusesInt = statuses.Select(a => (int)a).ToList();
                query = query.Where(a => statusesInt.Contains(a.StatusId));
            }

            if (from.HasValue)
            {
                query = query.Where(e => e.Timestamp >= from);
            }

            if (to.HasValue)
            {
                query = query.Where(e => e.Timestamp <= to);
            }

            return await query
                .CountAsync();

        }
        private IQueryable<EventDataModel> GetSelectableEventList(IQueryable<EventDataModel> list)
        {
            return list
                .Select(e => new EventDataModel()
                {
                    Id = e.Id,
                    ParentEventId = e.ParentEventId,
                    Timestamp = e.Timestamp,
                    EventTypeId = e.EventTypeId,
                    CustomEventName = e.CustomEventName,
                    VoyageId = e.VoyageId,
                    Comment = e.Comment,
                    StatusId = e.StatusId,
                    PortId = e.PortId,
                    Terminal = e.Terminal,
                    Lat = e.Lat,
                    Lng = e.Lng,
                    LatDegrees = e.LatDegrees,
                    LatMinutes = e.LatMinutes,
                    LatSeconds = e.LatSeconds,
                    LongDegrees = e.LongDegrees,
                    LongMinutes = e.LongMinutes,
                    LongSeconds = e.LongSeconds,
                    ConditionId = e.ConditionId,
                    CurrentVoyageConditionKey = e.CurrentVoyageConditionKey,
                    CargoDetailId = e.CargoDetailId,
                    BunkeringDataId = e.BunkeringDataId,
                    StsOperation = e.StsOperation,
                    Port = e.Port != null ? new Port()
                    {
                        Name = e.Port.Name,
                        Country = e.Port.Country != null ? new Country()
                        {
                            Alpha2 = e.Port.Country.Alpha2
                        } : null,
                        BusinessId = e.Port.BusinessId
                    } : null,
                    Status = new EventStatusDataModel()
                    {
                        BusinessId = e.Status.BusinessId
                    },
                    EventCondition = new EventConditionDataModel()
                    {
                        Id = e.EventCondition.Id,
                        Name = e.EventCondition.Name,
                        BusinessId = e.EventCondition.BusinessId
                    },
                    EventType = new EventTypeDataModel()
                    {
                        Name = e.EventType.Name,
                        ReportTypeId = e.EventType.ReportTypeId,
                        PairedEventTypeId = e.EventType.PairedEventTypeId,
                        BusinessId = e.EventType.BusinessId
                    },
                    BunkeringData = e.BunkeringData != null ? new BunkeringDataModel()
                    {
                        Id = e.BunkeringData.Id,
                        Timestamp = e.BunkeringData.Timestamp,
                        FuelType = e.BunkeringData.FuelType,
                        Port = e.BunkeringData.Port != null ? new Port()
                        {
                            Name = e.BunkeringData.Port.Name,
                        } : null
                    } : null,
                    ParentEvent = e.ParentEvent != null ? new EventDataModel()
                    {
                        BunkeringDataId = e.ParentEvent.BunkeringDataId,
                        CargoDetailId = e.ParentEvent.CargoDetailId,
                        CargoDetail = e.ParentEvent.CargoDetail != null ? new Models.Cargo.CargoDetailModel()
                        {
                            Id = e.ParentEvent.CargoDetail.Id,
                            Quantity = e.ParentEvent.CargoDetail.Quantity,
                            Cargo = new Models.Cargo.CargoModel()
                            {
                                Id = e.ParentEvent.CargoDetail.Cargo.Id,
                                GradeId = e.ParentEvent.CargoDetail.Cargo.GradeId,
                                Parcel = e.ParentEvent.CargoDetail.Cargo.Parcel,
                                Grade = new Models.Cargo.GradeModel()
                                {
                                    Name = e.ParentEvent.CargoDetail.Cargo.Grade.Name
                                }
                            }
                        } : null,
                        Timestamp = e.ParentEvent.Timestamp,
                        EventTypeId = e.ParentEvent.EventTypeId,
                        EventType = new EventTypeDataModel()
                        {
                            Name = e.ParentEvent.EventType.Name
                        },
                        StsOperation = e.ParentEvent.StsOperation,
                    } : null,
                    CargoDetail = e.CargoDetail != null ? new Models.Cargo.CargoDetailModel()
                    {
                        Id = e.CargoDetail.Id,
                        Quantity = e.CargoDetail.Quantity,
                        Cargo = new Models.Cargo.CargoModel()
                        {
                            Id = e.CargoDetail.Cargo.Id,
                            GradeId = e.CargoDetail.Cargo.GradeId,
                            Parcel = e.CargoDetail.Cargo.Parcel,
                            Grade = new Models.Cargo.GradeModel()
                            {
                                Name = e.CargoDetail.Cargo.Grade.Name
                            }
                        }
                    } : null,
                    Reports = e.Reports.Select(r => new Models.Reports.ReportDataModel()
                    {
                        Id = r.Id
                    }).ToList()
                });
        }
        public async Task<List<ConditionEventsDataModel>> GetUserEventsAsync(string userId, int page, int pageSize, List<int> eventTypeIds, List<int> statuses, DateTime? from, DateTime? to)
        {

            var query = _context.Events
                .Where(e => e.UserId == userId && e.Timestamp.HasValue);

            if (eventTypeIds != null && eventTypeIds.Any())
            {
                query = query.Where(a => eventTypeIds.Contains(a.EventTypeId));
            }

            if (statuses != null && statuses.Any())
            {
                var statusesInt = statuses.Select(a => (int)a).ToList();
                query = query.Where(a => statusesInt.Contains(a.StatusId));
            }

            if (from.HasValue)
            {
                query = query.Where(e => e.Timestamp >= from);
            }

            if (to.HasValue)
            {
                query = query.Where(e => e.Timestamp <= to);
            }

            query = GetSelectableEventList(query);

            var events = await query
                .OrderByDescending(e => e.Timestamp)
                .ThenByDescending(e => e.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var conditionKeys = events.Select(e => e.CurrentVoyageConditionKey).Distinct().ToList();

            query = _context.Events
                .Where(e => e.UserId == userId && !e.Timestamp.HasValue && conditionKeys.Contains(e.CurrentVoyageConditionKey));

            if (eventTypeIds != null && eventTypeIds.Any())
            {
                query = query.Where(a => eventTypeIds.Contains(a.EventTypeId));
            }

            if (statuses != null && statuses.Any())
            {
                var statusesInt = statuses.Select(a => (int)a).ToList();
                query = query.Where(a => statusesInt.Contains(a.StatusId));
            }

            var inProgressEvents = await GetSelectableEventList(query).AsNoTracking().ToListAsync();

            var groupedEvents = events.GroupBy(a => new { a.EventCondition.Id, a.EventCondition.Name, a.CurrentVoyageConditionKey })
                .Select(e => new ConditionEventsDataModel()
                {
                    ConditionKey = e.Key.CurrentVoyageConditionKey,
                    ConditionId = e.Key.Id,
                    ConditionName = e.Key.Name,
                    StartDate = e.Min(b => b.Timestamp),
                    EndDate = e.Max(b => b.Timestamp),
                    Events = e
                        .Concat(inProgressEvents.Where(ie => ie.CurrentVoyageConditionKey == e.Key.CurrentVoyageConditionKey))
                        .OrderByDescending(b => b.Timestamp == null)
                        .ThenByDescending(b => b.Timestamp)
                        .ThenByDescending(b => b.Id)
                        .ToList()
                })
                .ToList();

            return groupedEvents;
        }
        public IQueryable<EventDataModel> GetUserEventQueryFromReportId(int reportId)
        {
            return _context.Events
                .Where(e => e.Reports.Any(r => r.Id == reportId)).AsQueryable();
        }
        public IQueryable<EventDataModel> GetUserEventQuery(int eventId)
        {
            return _context.Events
                .Where(e => e.Id == eventId).AsQueryable();
        }
        public IQueryable<EventDataModel> GetUserEventQuery(string userId)
        {
            return _context.Events
                .Where(e => e.UserId == userId).AsQueryable();
        }
        public IQueryable<EventDataModel> GetUserChildEventQuery(int eventId)
        {
            return _context.Events
                .Where(e => e.ParentEventId == eventId).AsQueryable();
        }
        public async Task<EventDataModel> GetUserEventAsync(int eventId)
        {
            var @event = await _context.Events
                    .Include(a => a.BunkeringData)
                    .ThenInclude(a => a.Tanks)
                    .ThenInclude(a => a.ComminglingData)
                    .Include(a => a.BunkeringData)
                    .ThenInclude(a => a.Port)
                .Include(_ => _.Port)
                .Include(_ => _.EventCondition)
                .Include(_ => _.EventType)
                .Include(_ => _.Attachments)
                .Include(a => a.ChildrenEvents)
                .Include(a => a.ParentEvent)
                .FirstOrDefaultAsync(a => a.Id == eventId);

            if (@event == null)
                return null;

            return @event;
        }
        public async Task<List<EventDataModel>> GetPrevVoyageEventRangeAsync(int targetEventId)
        {
            var targetEvent = await _context.Events.Include(_ => _.Voyage).FirstOrDefaultAsync(_ => _.Id == targetEventId);
            if (targetEvent == null)
            {
                return new List<EventDataModel>();
            }
            var voyageEndDate = targetEvent.Voyage?.EndDate ?? DateTime.MaxValue;
            return await _context.Voyages
                .Include(_ => _.Events)
                .ThenInclude(_ => _.Reports)
                .ThenInclude(_ => _.FieldValues)
                .ThenInclude(_ => _.ReportField)
                .Include(_ => _.Events)
                .ThenInclude(_ => _.EventType)
                .Include(_ => _.Events)
                .ThenInclude(_ => _.ChildrenEvents)
                .Where(_ => _.UserId == targetEvent.UserId && (_.EndDate ?? DateTime.MaxValue) <= voyageEndDate)
                .OrderByDescending(_ => _.EndDate == null)
                .ThenByDescending(_ => _.EndDate)
                .ThenByDescending(_ => _.Id)
                .Take(2)
                .SelectMany(_ => _.Events)
                .Where(_ => _.Timestamp.HasValue && _.Timestamp < targetEvent.Timestamp)
                .OrderByDescending(_ => _.Timestamp)
                .ThenByDescending(_ => _.Id)
                .ToListAsync();
        }
        public Task<User> GetVesselDetails(string userId)
        {
            return _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new User
                {
                    Id = u.Id,
                    UserName = u.UserName.ToUpper(),
                    MainEngineMaxPower = u.MainEngineMaxPower,
                    PitchPropeller = u.PitchPropeller
                })
                .FirstOrDefaultAsync();
        }

        #region Cascade remove
        private void Remove(EventDataModel eventDataModel)
        {
            var eventAttachments = _context.EventAttachments.Where(_ => _.EventId == eventDataModel.Id);
            _context.EventAttachments.RemoveRange(eventAttachments);
            var reports = _context.Reports.Include(_ => _.FieldValues).Where(_ => _.EventId == eventDataModel.Id);
            _context.Reports.RemoveRange(reports);
            _context.Events.Remove(eventDataModel);
        }
        private void Remove(List<EventDataModel> eventDataModelList)
        {
            if (eventDataModelList.Count == 0)
                return;
            var eventAttachments = _context.EventAttachments.Where(_ => eventDataModelList.Select(e => e.Id).Contains(_.EventId));
            _context.EventAttachments.RemoveRange(eventAttachments);
            var reports = _context.Reports.Include(_ => _.FieldValues).Where(_ => eventDataModelList.Select(e => e.Id).Contains(_.EventId));
            _context.Reports.RemoveRange(reports);
            _context.Events.RemoveRange(eventDataModelList);
        }
        #endregion
        public async Task<List<int>> GetRelatedNoonEventIds(int bunkeringEventId)
        {
            var dbEvent = await _context.Events.Where(_ => _.Id == bunkeringEventId).FirstOrDefaultAsync();
            if (dbEvent == null)
            {
                return new List<int>();
            }
            var eventIds = await _context.Events
                .Include(_ => _.EventType)
                .Where(_ =>
                _.UserId == dbEvent.UserId &&
                _.EventType.ReportTypeId.HasValue &&
                _.Timestamp >= dbEvent.Timestamp)
                .Select(_ => _.Id)
                .ToListAsync();
            return eventIds;
        }
        public async Task<bool> HasCommenceBunkeringReport(int eventId)
        {
            return await _context.Events
                .Include(_ => _.Reports)
                .Where(_ => _.Id == eventId && EventType.CommenceBunkeringGroup.Contains(_.EventType.BusinessId))
                .Select(_ => _.Reports)
                .AnyAsync();

        }

        public async Task<EventDataModel> CreateEventAsync(EventDataModel @event)
        {

            var eventType = await _context.EventTypes.Include(et => et.Prerequisites).Where(et => et.Id == @event.EventTypeId).FirstOrDefaultAsync();

            //if (eventType.HasPairedEventBehaviour())
            //{
            //    throw new CustomException("Paired event can't be created manually.");
            //}

            if (eventType.ReportTypeId.HasValue && EventType.BunkeringGroup.Contains(eventType.BusinessId))
            {

                // TODO: Manage this restriction

                var hasNextReports = await HasNextReports(@event.UserId, @event.Timestamp.Value);

                if (hasNextReports)
                {
                    throw new CustomException("A new bunkering report event cannot be created while other reports are pending or in progress.");
                }
            }

            var targetEvent = await GetEventFromTimestamp(@event.Timestamp.Value, @event.UserId);
            //TODO: reportcontext
            //var hasReport = dbModel.EventType.ReportTypeId.HasValue && await _context.Reports.Where(r => r.EventId == dbModel.Id).AnyAsync();
            //if (hasReport)
            //{
            //    if (dbModel.Timestamp != eventModel.Timestamp)
            //    {
            //        var reportContext = await _context.ReportContext
            //            .Where(rc => rc.Report.EventId == dbModel.Id)
            //            .Select(rc => new
            //            {
            //                rc.Id,
            //                rc.PrevContextId
            //            })
            //            .FirstOrDefaultAsync()
            //            ;
            //        if (reportContext != null)
            //        {
            //            var prevReportTimestamp = await _context.ReportContext
            //                .Where(rc => rc.Id == reportContext.PrevContextId)
            //                .Select(rc => rc.Report.Event.Timestamp)
            //                .FirstOrDefaultAsync()
            //                ;
            //            var nextReportTimestamp = await _context.ReportContext
            //                .Where(rc => rc.PrevContextId == reportContext.Id)
            //                .Select(rc => rc.Report.Event.Timestamp)
            //                .FirstOrDefaultAsync()
            //                ;
            //            if (prevReportTimestamp != null && prevReportTimestamp >= eventModel.Timestamp)
            //            {
            //                throw new CustomException("The report event time cannot be set earlier than the previous report event.");
            //            }
            //            if (nextReportTimestamp != null && nextReportTimestamp <= eventModel.Timestamp)
            //            {
            //                throw new CustomException("The report event time cannot be set later than the next report event.");
            //            }

            //        }

            //    }

            //    var voyageEvents = await GetPrevVoyageEventRangeAsync(dbModel.Id);

            //    var voyageEvent = voyageEvents.Where(e => e.Reports.Any()).FirstOrDefault();

            //    if (voyageEvent != null)
            //    {
            //        if (eventModel.Timestamp <= voyageEvent.Timestamp && dbModel.Id < voyageEvent.Id)
            //        {
            //            throw new CustomException("The event time must be later than the previous report event.");
            //        }
            //    }
            //}

            var currentVoyageFallback = await _context.Voyages.Include(v => v.CurrentCondition).FirstOrDefaultAsync(a => !a.IsFinished && a.UserId == @event.UserId);

            var conditionId = targetEvent?.ConditionId ?? currentVoyageFallback.CurrentConditionId;
            var conditionKey = targetEvent?.CurrentVoyageConditionKey ?? currentVoyageFallback.CurrentVoyageConditionKey;
            var voyageId = targetEvent?.VoyageId ?? currentVoyageFallback.Id;

            var targetEvents = await _context.Events
                .Include(e => e.EventType)
                .Include(e => e.ChildrenEvents)
                .ThenInclude(e => e.EventType)
                .Where(e => e.VoyageId == voyageId && e.ConditionId == conditionId && e.CurrentVoyageConditionKey == conditionKey)
                .ToListAsync();

            var targetConditionChangeEvent = targetEvents.Where(e => e.EventType.NextConditionId.HasValue).FirstOrDefault();

            if (eventType.ChangesCondition())
            {
                if (targetEvents.Any(e => !e.Timestamp.HasValue && e.EventType.NextConditionId.HasValue))
                {
                    throw new CustomException("A change condition event is already in progress for the current condition.");
                }
            }

            if (eventType.NextConditionId.HasValue && targetEvent != null && targetEvent.EventCondition.BusinessId == Condition.Berthed)
            {
                var exists = await _context.Events.Where(e => e.PreviousVoyageConditionKey == targetEvent.CurrentVoyageConditionKey).AnyAsync();
                // TODO: check B condition
                if (exists)
                {
                    throw new CustomException("Cannot break voyage pair.");
                }
            }

            var statusBusinessId = Status.Approved;

            if (eventType.ReportTypeId.HasValue)
            {
                statusBusinessId = Status.Completed;
            }

            var status = await _context.EventStatuses.ToListAsync();

            @event.ConditionId = conditionId;
            @event.StatusId = status.Single(s => s.BusinessId == statusBusinessId).Id;
            @event.VoyageId = voyageId;
            @event.CurrentVoyageConditionKey = conditionKey;

            if (EventType.CommenceMooring != eventType.BusinessId)
            {
                // remove this and set it as terminal placeholder

            }

            _context.Events.Add(@event);
            await _context.SaveChangesAsync();

            if (@event.StsOperation != null && @event.StsOperation.EventId <= 0)
            {
                @event.StsOperation.EventId = @event.Id;
            }

            if (eventType.BusinessId == EventType.BunkeringPlan)
            {
                var bunkeringData = new BunkeringDataModel()
                {
                    UserId = @event.UserId,
                    FuelType = @event.FuelType.Value,
                    PortId = @event.PortId.Value,
                    Timestamp = @event.Timestamp.Value
                };
                //TODO: reportcontext
                @event.BunkeringDataId = await CreateBunkeringData(bunkeringData);
                await _context.SaveChangesAsync();
            }

            if (eventType.NextConditionId.HasValue)
            {

                var newConditionKey = Guid.NewGuid();
                @event.CurrentVoyageConditionKey = newConditionKey;
                @event.ConditionId = eventType.NextConditionId.Value;
                @event.VoyageId = voyageId;
                @event.ConditionStartedDate = @event.Timestamp;

                if (targetConditionChangeEvent == null || (@event.Timestamp > targetConditionChangeEvent.Timestamp || (@event.Timestamp == targetConditionChangeEvent.Timestamp && targetEvent.Id >= targetConditionChangeEvent.Id)))
                {
                    @event.PreviousVoyageConditionKey = conditionKey;
                    var eventPoint = targetEvent ?? @event;

                    var eventsToMove = targetEvents.Where(e => e.Timestamp.HasValue && (e.Timestamp > @event.Timestamp || e.Timestamp == @event.Timestamp && e.Id > eventPoint.Id)).ToList();

                    if (targetEvent != null)
                    {
                        var selectedEvent = await _context.Events
                            .Where(e => e.UserId == @event.UserId && e.EventType.NextConditionId.HasValue && e.PreviousVoyageConditionKey == targetEvent.CurrentVoyageConditionKey)
                            .FirstOrDefaultAsync();
                        if (selectedEvent != null)
                        {
                            selectedEvent.PreviousVoyageConditionKey = @event.CurrentVoyageConditionKey;
                        }
                    }
                    foreach (var eventToMove in eventsToMove)
                    {
                        if (eventToMove.ParentEventId.HasValue && eventToMove.EventType.Transit != true)
                        {
                            if (!eventsToMove.Any(e => e.Id == eventToMove.ParentEventId))
                            {
                                _context.Events.Remove(@event);
                                await _context.SaveChangesAsync();
                                throw new Exception($"{eventToMove.ParentEvent.EventType.Name} must be in the same condition group with {eventToMove.EventType.Name}.");
                            }
                        }
                        eventToMove.CurrentVoyageConditionKey = @event.CurrentVoyageConditionKey;
                        eventToMove.ConditionId = @event.ConditionId;
                        var childEvent = eventToMove.ChildrenEvents.Where(e => !e.Timestamp.HasValue && !e.EventType.Transit).FirstOrDefault();
                        if (childEvent != null)
                        {
                            childEvent.CurrentVoyageConditionKey = @event.CurrentVoyageConditionKey;
                            childEvent.ConditionId = @event.ConditionId;
                        }
                    }
                    var incompleteEventsWithTransit = await _context.Events.Where(e => e.UserId == @event.UserId && e.CurrentVoyageConditionKey == conditionKey && !e.Timestamp.HasValue && e.EventType.Transit && e.EventType.EventTypesConditions.Any(e => e.ConditionId == @event.ConditionId)).ToListAsync();
                    foreach (var incompleteEventWithTransit in incompleteEventsWithTransit)
                    {
                        incompleteEventWithTransit.CurrentVoyageConditionKey = @event.CurrentVoyageConditionKey;
                        incompleteEventWithTransit.ConditionId = @event.ConditionId;
                    }
                }
                else
                {
                    _context.Events.Remove(@event);
                    await _context.SaveChangesAsync();
                    throw new Exception("Invalid date and time.");
                }

                await _context.SaveChangesAsync();

                var currentVoyage = await _context.Voyages
                    .FirstAsync(v => v.Id == voyageId);

                var latestVoyageEvent = await _context.Events.Where(e => e.VoyageId == voyageId && e.EventType.NextConditionId.HasValue && e.Timestamp.HasValue)
                    .OrderByDescending(e => e.Timestamp)
                    .ThenByDescending(e => e.Id)
                    .FirstOrDefaultAsync();

                if (latestVoyageEvent != null && currentVoyage.CurrentVoyageConditionKey != latestVoyageEvent.CurrentVoyageConditionKey)
                {
                    currentVoyage.CurrentVoyageConditionKey = latestVoyageEvent.CurrentVoyageConditionKey;
                    currentVoyage.CurrentConditionId = latestVoyageEvent.ConditionId;
                }

                await _context.SaveChangesAsync();
            }

            if (eventType.PairedEventTypeId.HasValue)
            {
                var pairedEvent = new EventDataModel()
                {
                    UserId = @event.UserId,
                    ParentEventId = @event.Id,
                    EventTypeId = eventType.PairedEventTypeId.Value,
                    PortId = @event.PortId,
                    Terminal = @event.Terminal,
                    StatusId = status.Single(s => s.BusinessId == Status.InProgress).Id,
                    VoyageId = voyageId,
                    CurrentVoyageConditionKey = @event.CurrentVoyageConditionKey,
                    ConditionId = @event.ConditionId,
                    BunkeringDataId = @event.BunkeringDataId

                };
                _context.Events.Add(pairedEvent);
                await _context.SaveChangesAsync();
            }

            return @event;
        }

        private async Task UpdateVoyageDetails(EventDataModel dbModel)
        {
            var voyageDetail = await _context.MrvMisData.Where(e => e.EventId == dbModel.Id).FirstOrDefaultAsync();

            if (voyageDetail == null)
            {
                return;
            }
            var conditionName = await _context.EventConditions.Where(c => c.Id == dbModel.ConditionId).Select(c => c.Name).FirstOrDefaultAsync();


            voyageDetail.ConditionName = conditionName;

            var voyageEventBusinessId = await GetEventVoyageQuery(dbModel.VoyageId)
                .Where(e => e.EventType.BusinessId == EventType.CompleteUnmooring)
                .OrderBy(e => e.Timestamp)
                .Select(e => e.BusinessId)
                .FirstOrDefaultAsync();

            voyageDetail.LegId = voyageEventBusinessId;
            voyageDetail.Voyage = voyageEventBusinessId;

            var port = await _context.Port.AsNoTracking().Where(p => p.Id == dbModel.PortId).FirstOrDefaultAsync();

            voyageDetail.PortName = port?.Name;
            voyageDetail.PortBusinessId = port?.BusinessId;
            voyageDetail.IsEu = port?.IsEuInt;

            voyageDetail.Lat = (double?)dbModel.Lat;
            voyageDetail.Long = (double?)dbModel.Lng;
            voyageDetail.EventTimestamp = dbModel.Timestamp?.DateTime;
            voyageDetail.EventTimestampUtc = dbModel.Timestamp?.UtcDateTime;
            voyageDetail.StatusId = dbModel.StatusId;

        }

        public async Task<EventDataModel> UpdateEventAsync(EventDataModel eventModel)
        {
            var dbModel = await GetUserEventQuery(eventModel.Id)
                .Include(a => a.StsOperation)
                .Include(a => a.EventType)
                .Include(a => a.ParentEvent)
                .ThenInclude(a => a.StsOperation)
                .Include(a => a.ChildrenEvents)
                .Include(a => a.EventCondition)
            .FirstAsync();

            if (dbModel == null)
            {
                throw new CustomException("Event not found.");
            }

            if (dbModel.Timestamp != eventModel.Timestamp)
            {

                if (dbModel.ParentEvent != null)
                {
                    if (eventModel.Timestamp < dbModel.ParentEvent.Timestamp)
                    {
                        throw new CustomException("The date cannot be set earlier than its parent fact.");
                    }
                }

                if (dbModel.ChildrenEvents.Any(a => a.Timestamp < eventModel.Timestamp))
                {
                    throw new CustomException("The date cannot be set earlier than its paired fact.");
                }

                var targetEventTimestamps = await _context.Events
                    .Where(e =>
                        e.UserId == dbModel.UserId &&
                        e.Timestamp.HasValue &&
                        e.CurrentVoyageConditionKey == dbModel.CurrentVoyageConditionKey &&
                        !e.EventType.NextConditionId.HasValue &&
                        (e.EventType.PairedEventType == null || !e.EventType.PairedEventType.NextConditionId.HasValue))
                    .OrderByDescending(e => e.Timestamp)
                    .ThenByDescending(e => e.Id)
                    .Select(e => (DateTimeOffset?)e.Timestamp.Value)
                    .ToListAsync();

                if (dbModel.EventType.NextConditionId != null && dbModel.ConditionId == dbModel.EventType.NextConditionId)
                {
                    var previousConditionKey = dbModel.PreviousVoyageConditionKey.Value;
                    var prevEvent = await _context.Events.Where(e => e.UserId == dbModel.UserId && e.CurrentVoyageConditionKey == previousConditionKey && e.Timestamp.HasValue)
                        .OrderByDescending(e => e.Timestamp)
                        .ThenByDescending(e => e.Id)
                        .FirstOrDefaultAsync();
                    if (prevEvent != null)
                    {
                        if (prevEvent.Timestamp > eventModel.Timestamp)
                        {
                            throw new CustomException("The condition change event group cannot be moved outside its boundaries.");
                        }
                    }
                    var lastEventTimestamp = targetEventTimestamps.LastOrDefault();
                    if (lastEventTimestamp != null && lastEventTimestamp < eventModel.Timestamp)
                    {
                        throw new CustomException("The condition change event group must be the first event in the condition group.");
                    }
                }
                else if (dbModel.EventType.NextConditionId != null && dbModel.ConditionId != dbModel.EventType.NextConditionId)
                {
                    var lastEventTimestamp = targetEventTimestamps.LastOrDefault();

                    if (lastEventTimestamp != null && eventModel.Timestamp < lastEventTimestamp)
                    {
                        throw new CustomException("The condition change event group cannot be moved outside its boundaries.");
                    }
                }
                else
                {

                    var targetEvent = await GetEventFromTimestamp(eventModel.Timestamp.Value, dbModel.UserId);

                    if (targetEvent.CurrentVoyageConditionKey != dbModel.CurrentVoyageConditionKey)
                    {
                        var hasCondition = await _context.EventTypesConditions
                            .Where(a => (a.ConditionId == targetEvent.ConditionId || (a.EventType.NextConditionId == null && targetEvent.ParentEventId != null && a.ConditionId == targetEvent.ParentEvent.ConditionId)) && a.EventTypeId == dbModel.EventTypeId)
                            .AnyAsync();
                        if (!hasCondition)
                        {
                            throw new CustomException($"{dbModel.EventType.Name} is not allowed in {targetEvent.EventCondition.Name} condition.");
                        }
                    }

                    dbModel.ConditionId = targetEvent.ConditionId;
                    dbModel.CurrentVoyageConditionKey = targetEvent.CurrentVoyageConditionKey;
                    dbModel.VoyageId = targetEvent.VoyageId;


                    // TODO: add the behaviour logic
                }
            }

            var hasReport = dbModel.EventType.ReportTypeId.HasValue && await _context.Reports.Where(r => r.EventId == dbModel.Id).AnyAsync();
            if (hasReport)
            {
                if (dbModel.Timestamp != eventModel.Timestamp)
                {
                    var reportContext = await _context.ReportContext
                        .Where(rc => rc.Report.EventId == dbModel.Id)
                        .Select(rc => new
                        {
                            rc.Id,
                            rc.PrevContextId
                        })
                        .FirstOrDefaultAsync()
                        ;
                    if (reportContext != null)
                    {
                        var prevReportTimestamp = await _context.ReportContext
                            .Where(rc => rc.Id == reportContext.PrevContextId)
                            .Select(rc => rc.Report.Event.Timestamp)
                            .FirstOrDefaultAsync()
                            ;
                        var nextReportTimestamp = await _context.ReportContext
                            .Where(rc => rc.PrevContextId == reportContext.Id)
                            .Select(rc => rc.Report.Event.Timestamp)
                            .FirstOrDefaultAsync()
                            ;
                        if (prevReportTimestamp != null && prevReportTimestamp >= eventModel.Timestamp)
                        {
                            throw new CustomException("The report event time cannot be set earlier than the previous report event.");
                        }
                        if (nextReportTimestamp != null && nextReportTimestamp <= eventModel.Timestamp)
                        {
                            throw new CustomException("The report event time cannot be set later than the next report event.");
                        }

                    }

                }

                var voyageEvents = await GetPrevVoyageEventRangeAsync(dbModel.Id);

                var voyageEvent = voyageEvents.Where(e => e.Reports.Any()).FirstOrDefault();

                if (voyageEvent != null)
                {
                    if (eventModel.Timestamp <= voyageEvent.Timestamp && dbModel.Id < voyageEvent.Id)
                    {
                        throw new CustomException("The event time must be later than the previous report event.");
                    }
                }
            }

            dbModel.PortId = eventModel.PortId;
            dbModel.Comment = eventModel.Comment;
            dbModel.LatDegrees = eventModel.LatDegrees;
            dbModel.LatMinutes = eventModel.LatMinutes;
            dbModel.LatSeconds = eventModel.LatSeconds;
            dbModel.LongDegrees = eventModel.LongDegrees;
            dbModel.LongMinutes = eventModel.LongMinutes;
            dbModel.LongSeconds = eventModel.LongSeconds;
            dbModel.Lat = eventModel.Lat;
            dbModel.Lng = eventModel.Lng;
            dbModel.CustomEventName = eventModel.CustomEventName;

            var targetModel = dbModel.ParentEvent ?? dbModel;

            var existing = await _context.StsOperations
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(x => x.EventId == targetModel.Id);



            if (eventModel.StsOperation != null)
            {
                if (existing == null)
                {
                    _context.StsOperations.Add(new StsOperation
                    {
                        EventId = targetModel.Id,
                        ReverseLightering = eventModel.StsOperation.ReverseLightering,
                        CompanyParticipatingVesselId = eventModel.StsOperation.CompanyParticipatingVesselId,
                        ParticipatingVessel = eventModel.StsOperation.ParticipatingVessel,
                        SameSizeParticipatingVessel = eventModel.StsOperation.SameSizeParticipatingVessel,
                        Comments = eventModel.StsOperation.Comments,
                        IsDeleted = false
                    });
                }
                else
                {
                    existing.IsDeleted = false;

                    existing.ReverseLightering = eventModel.StsOperation.ReverseLightering;
                    existing.CompanyParticipatingVesselId = eventModel.StsOperation.CompanyParticipatingVesselId;
                    existing.ParticipatingVessel = eventModel.StsOperation.ParticipatingVessel;
                    existing.SameSizeParticipatingVessel = eventModel.StsOperation.SameSizeParticipatingVessel;
                    existing.Comments = eventModel.StsOperation.Comments;
                }
            }
            else
            {
                if (existing != null)
                {
                    existing.IsDeleted = true;
                }
            }

            // TODO: 24/2/26 check sea state update, upate old events from table, deploy


            await _context.SaveChangesAsync();

            var childEvents = await _context.Events.Where(e => e.ParentEventId == dbModel.Id).ToListAsync();
            var voyageDetailsEventIds = childEvents.Where(e => !e.PortId.HasValue).Select(e => e.Id).ToList();
            foreach (var childEvent in childEvents)
            {
                if (!childEvent.PortId.HasValue)
                {
                    childEvent.PortId = eventModel.PortId;
                }
            }



            var voyageDetailsEvents = await _context.MrvMisData.Where(e => voyageDetailsEventIds.Contains(e.EventId.Value)).ToListAsync();
            var port = await _context.Port.Where(p => p.Id == eventModel.PortId).FirstOrDefaultAsync();
            foreach (var v in voyageDetailsEvents)
            {
                v.PortBusinessId = port?.BusinessId;
                v.PortName = port?.Name;
            }
            await _context.SaveChangesAsync();

            var status = await _context.EventStatuses.ToListAsync();

            if (EventType.BunkeringPlan == dbModel.EventType.BusinessId)
            {
                var bunkeringData = await _context.Events.Include(_ => _.BunkeringData).Where(b => b.Id == dbModel.Id).FirstOrDefaultAsync();
                if (bunkeringData != null)
                {
                    bunkeringData.PortId = eventModel.PortId.Value;
                    await _context.SaveChangesAsync();
                }
            }



            if (dbModel.Timestamp.HasValue)
            {
                dbModel.Timestamp = eventModel.Timestamp;

                await _context.SaveChangesAsync();
                await UpdateVoyageDetails(dbModel);
                await _context.SaveChangesAsync();
                return dbModel;
            }
            else
            {
                dbModel.StatusId = status.Single(s => s.BusinessId == Status.Completed).Id;

            }

            dbModel.Timestamp = eventModel.Timestamp;

            await _context.SaveChangesAsync();

            var currentVoyage = await _context.Voyages.FirstAsync(a => a.Id == dbModel.VoyageId);
            var conditionId = dbModel.ConditionId;
            var conditionKey = new Guid(dbModel.CurrentVoyageConditionKey.ToString());

            if (dbModel.EventType.NextConditionId != null && dbModel.ConditionId != dbModel.EventType.NextConditionId)
            {

                dbModel.ConditionStartedDate = eventModel.Timestamp;
                dbModel.ConditionId = dbModel.EventType.NextConditionId.Value;

                dbModel.PreviousVoyageConditionKey = new Guid(dbModel.CurrentVoyageConditionKey.ToString());
                dbModel.CurrentVoyageConditionKey = Guid.NewGuid();

                if (dbModel.EventType.IsCompleteUnMooring())
                {
                    var targetEventType = await _context.Events.Where(e =>
                        (e.EventType.BusinessId == Enums.EventType.AnchorUp ||
                        e.EventType.BusinessId == Enums.EventType.DropAnchor) &&
                        e.UserId == dbModel.UserId && e.Timestamp.HasValue
                    ).OrderByDescending(e => e.Timestamp)
                    .Select(e => e.EventType)
                    .FirstOrDefaultAsync();

                    if (targetEventType != null)
                    {
                        if (targetEventType.IsDropAnchor())
                        {
                            dbModel.ConditionId = 3;
                        }
                        else if (targetEventType.IsAnchorUp())
                        {
                            dbModel.ConditionId = 2;
                        }
                    }

                }

                if (!dbModel.EventType.IsCompleteUnMooring())
                {
                    currentVoyage.CurrentConditionId = dbModel.ConditionId;
                    currentVoyage.CurrentVoyageConditionKey = dbModel.CurrentVoyageConditionKey;
                    dbModel.VoyageId = currentVoyage.Id;
                }

                await _context.SaveChangesAsync();

                var eventsToMove = await _context.Events
                    .Where(e =>
                    e.UserId == dbModel.UserId &&
                    e.VoyageId == currentVoyage.Id &&
                    e.CurrentVoyageConditionKey == conditionKey &&
                    e.ConditionId == conditionId &&
                    ((!e.Timestamp.HasValue && e.ParentEventId != null && e.EventType.Transit && e.EventType.EventTypesConditions.Any(e => e.ConditionId == dbModel.EventType.NextConditionId.Value)) || (
                    e.Timestamp > eventModel.Timestamp || (e.Timestamp == eventModel.Timestamp && e.Id > dbModel.Id)
                )))
                .ToListAsync();

                var eventsToMoveIds = eventsToMove.Select(e => e.Id).ToList();
                var voyageDetailEvents = await _context.MrvMisData.Where(e => eventsToMoveIds.Contains(e.Id)).ToListAsync();

                foreach (var eventDataModel in eventsToMove)
                {
                    // Transit
                    eventDataModel.ConditionId = dbModel.ConditionId;
                    eventDataModel.CurrentVoyageConditionKey = dbModel.CurrentVoyageConditionKey;
                }
                var condition = await _context.EventConditions.Where(c => c.Id == dbModel.ConditionId).FirstOrDefaultAsync();
                foreach (var v in voyageDetailEvents)
                {
                    v.ConditionName = condition?.Name;
                }

                await _context.SaveChangesAsync();

                if (dbModel.EventType.IsCompleteUnMooring())
                {
                    currentVoyage.IsFinished = true;
                    currentVoyage.EndDate = eventModel.Timestamp.Value;
                    dbModel.PreviousVoyageConditionKey = currentVoyage.CurrentVoyageConditionKey;
                    await _context.SaveChangesAsync();
                    currentVoyage = new VoyageDataModel()
                    {
                        CurrentConditionId = dbModel.ConditionId,
                        CurrentVoyageConditionKey = dbModel.CurrentVoyageConditionKey,
                        StartDate = eventModel.Timestamp.Value,
                        UserId = dbModel.UserId
                    };

                    _context.Voyages.Add(currentVoyage);
                    await _context.SaveChangesAsync();
                    dbModel.VoyageId = currentVoyage.Id;
                    foreach (var eventDataModel in eventsToMove)
                    {
                        eventDataModel.VoyageId = currentVoyage.Id;
                    }

                    await _context.SaveChangesAsync();

                    var voyageEventBusinessId = await GetEventVoyageQuery(currentVoyage.Id)
                        .Where(e => e.EventType.BusinessId == EventType.CompleteUnmooring)
                        .OrderBy(e => e.Timestamp)
                        .Select(e => e.BusinessId)
                        .FirstOrDefaultAsync();

                    foreach (var v in voyageDetailEvents)
                    {

                        v.Voyage = voyageEventBusinessId;
                        v.LegId = voyageEventBusinessId;
                    }
                    await _context.SaveChangesAsync();
                }
            }

            await UpdateVoyageDetails(dbModel);
            await _context.SaveChangesAsync();
            return dbModel;
        }


        #region steaming time

        public async Task<double> GetTotalDistanceOverGroundAsync(int eventId, List<EventDataModel> eventRange)
        {
            var eventIds = eventRange.Select(e => e.Id).ToList();

            var distanceSum = await _context.Reports
                .Where(report => eventIds.Contains(report.EventId))
                .SelectMany(report => report.FieldValues
                    .Where(fieldValue => fieldValue.ReportField.ValidationKey == "distanceOverGround")
                    .Select(fieldValue => fieldValue.Value))
                .ToListAsync();

            var totalDistance = distanceSum
                .Select(value => double.TryParse(value, out var number) ? number : 0.0)
                .Sum();

            return totalDistance;
        }

        public async Task<List<EventDataModel>> GetCospEventRangeAsync(int targetEventId)
        {

            var targetEvent = await _context.Events
                .AsNoTracking()
                .Include(e => e.EventType)
                .Include(_ => _.Voyage)
                .FirstOrDefaultAsync(_ => _.Id == targetEventId);

            if (targetEvent == null)
            {
                return new List<EventDataModel>();
            }

            var from = await _context.Events
                .AsNoTracking()
                .Where(_ => _.UserId == targetEvent.UserId && _.EventType.BusinessId == EventType.COSP && _.Timestamp.HasValue && _.Timestamp < targetEvent.Timestamp)
                .OrderByDescending(_ => _.Timestamp)
                .ThenByDescending(_ => _.Id)
                // TODO: debug
                .Where(_ => !_context.Events.Any(e => e.UserId == targetEvent.UserId && _.EventType.BusinessId == EventType.EOSP && e.Timestamp >= _.Timestamp && e.Timestamp < targetEvent.Timestamp))
                .Select(_ => _.Timestamp)
                .FirstOrDefaultAsync();

            if (from == null)
            {
                return new List<EventDataModel>();
            }

            var to = await _context.Events
                .AsNoTracking()
            .Include(e => e.EventType)
            .Where(_ => _.UserId == targetEvent.UserId && _.EventType.BusinessId == EventType.EOSP && _.Timestamp.HasValue && _.Timestamp >= targetEvent.Timestamp && _.Id != targetEvent.Id)
            .OrderBy(_ => _.Timestamp)
            .ThenBy(_ => _.Id)
            .FirstOrDefaultAsync();

            if (to == null)
            {
                to = await _context.Events.Include(e => e.EventType).Where(_ => _.VoyageId == targetEvent.VoyageId && _.Timestamp.HasValue).OrderByDescending(_ => _.Timestamp).FirstOrDefaultAsync();
            }
            if (to == null)
            {
                return new List<EventDataModel>();
            }

            var targetTimestamp = to.Timestamp;
            var targetEventTypeId = to.EventType.BusinessId;

            if (targetEvent.EventType.BusinessId == EventType.EOSP)
            {
                targetTimestamp = targetEvent.Timestamp;
                targetEventTypeId = targetEvent.EventType.BusinessId;
            }

            var events = _context.Events
                .AsNoTracking()
                .Include(_ => _.ChildrenEvents)
                .Include(_ => _.EventType)
                .Where(_ => _.UserId == targetEvent.UserId && _.Timestamp.HasValue && _.Timestamp >= from);
            if (targetEventTypeId == EventType.COSP)
            {
                events = events.Where(_ => _.Timestamp < targetTimestamp);
            }
            else
            {
                events = events.Where(_ => _.Timestamp <= targetTimestamp);
            }
            var eventList = await events.Where(e => EventType.ChangeOverGroup.Contains(e.EventType.BusinessId) || ReportType.Performance.Contains(e.EventType.ReportTypeId.Value)).ToListAsync();

            var orderedList = eventList
                .OrderBy(_ => _.Timestamp)
                .ThenBy(_ => _.Id)
                .ToList();
            return orderedList;
        }

        private IDictionary<int, double> GetSteamingTimeTypeAsync(List<EventDataModel> cospEvents, SulphurOil targetOil)
        {
            var eventDurations = new Dictionary<int, double>();
            double totalHours = 0;
            bool eosp = false;
            DateTimeOffset? initialTimestamp = null;



            SulphurOil? currentOil = null;

            var hfo = new Guid("C7191E21-6330-45F5-8109-24E88F004A00");
            var lsmgo = new Guid("873F491A-FCC8-4245-8548-ECFD0ADEA69B");
            var generic = new Guid("0FF21E47-0E33-4D44-87FF-3F3D9BE57535");

            var targetEventTypes = new List<Guid>() { hfo, lsmgo, generic };

            if (targetOil != SulphurOil.LSMGO)
            {
                throw new NotImplementedException();
            }

            var targetEventId = lsmgo;

            foreach (var eventData in cospEvents)
            {
                if (eventData.EventType.BusinessId == targetEventId || (currentOil != targetOil && eventData.EventType.BusinessId == generic))
                {
                    if (!eosp)
                    {
                        initialTimestamp = eventData.Timestamp;
                    }
                    currentOil = targetOil;
                }
                else if (targetEventTypes.Contains(eventData.EventType.BusinessId))
                {
                    if (initialTimestamp.HasValue)
                    {
                        totalHours += (eventData.Timestamp.Value - initialTimestamp.Value).TotalHours;
                        initialTimestamp = null;
                    }
                    currentOil = null;
                }
                else if (eventData.EventType.ReportTypeId.HasValue)
                {
                    if (initialTimestamp.HasValue)
                    {
                        totalHours += (eventData.Timestamp.Value - initialTimestamp.Value).TotalHours;
                        initialTimestamp = eventData.Timestamp;
                    }
                    if (totalHours > 0)
                    {
                        eventDurations[eventData.Id] = totalHours;
                        totalHours = 0;
                    }
                }
                else if (eventData.EventType.BusinessId == EventType.EOSP)
                {
                    if (initialTimestamp.HasValue)
                    {
                        totalHours += (eventData.Timestamp.Value - initialTimestamp.Value).TotalHours;
                        initialTimestamp = null;
                    }
                    eosp = true;
                }
            }

            return eventDurations;
        }

        public IDictionary<SulphurOil, IDictionary<int, double>> GetSteamingTimeAsync(int eventId, List<EventDataModel> cospEvents)
        {
            var result = new Dictionary<SulphurOil, IDictionary<int, double>>();

            var total = GetSteamingTimeAsync(cospEvents, eventId);
            var totalLSMGO = GetSteamingTimeTypeAsync(cospEvents, SulphurOil.LSMGO);
            var totalVLSFO = new Dictionary<int, double>();

            result.Add(SulphurOil.LSMGO, totalLSMGO);
            result.Add(SulphurOil.VLSFO, totalVLSFO);
            result.Add(SulphurOil.Unspecified, total);
            return result;
        }

        private IDictionary<int, double> GetSteamingTimeAsync(List<EventDataModel> cospEvents, int? eventId)
        {
            var eventList = new Dictionary<int, double>();
            EventDataModel prevEvent = null;

            foreach (var currEvent in cospEvents)
            {
                if (currEvent.EventType.ReportTypeId.HasValue)
                {
                    if (prevEvent != null)
                    {
                        var diff = currEvent.Timestamp.Value - prevEvent.Timestamp.Value;
                        eventList.Add(currEvent.Id, diff.TotalHours);
                        if (currEvent.EventType.BusinessId == EventType.EOSP)
                        {
                            return eventList;
                        }
                    }

                    prevEvent = currEvent;
                }
            }

            return eventList;
        }

        private double GetTotalWeight(List<ReportFieldValueDataModel> fieldValues, Guid groupId)
        {

            var tankIds = fieldValues.Where(_ => _.ReportField.Group != null && _.ReportField.Group.BusinessId == groupId && _.ReportField.Tank != null).Select(fv => fv.ReportField.Tank.Id).Distinct().ToList();

            double totalAmount = 0;

            foreach (var tankId in tankIds)
            {

                var targetFields = fieldValues.Where(_ => _.ReportField.Group != null && _.ReportField.Group.BusinessId == groupId && _.ReportField.Tank.Id == tankId).ToList();

                //var density = targetFields.FirstOrDefault(_ => _.ReportField.ValidationKey == "density");
                //var sulphurContent = targetFields.FirstOrDefault(_ => _.ReportField.ValidationKey == "sulphurContent");
                //var temp = targetFields.FirstOrDefault(_ => _.ReportField.ValidationKey == "tankTemperature");
                //var volume = targetFields.FirstOrDefault(_ => _.ReportField.ValidationKey == "volume");
                var weight = targetFields.FirstOrDefault(_ => _.ReportField.ValidationKey == "weight");

                //var correctionFactors = BunkeringTools.GetWeightCorrectionFactor(density.Value, temp.Value, volume.Value);

                //totalAmount += correctionFactors.Weight;
                //if (weight.ReportId == 7794 || weight.ReportId == 7795)
                //{
                //    if (weight.ReportField.Group.Id == 1)
                //    {
                //        _logger.LogError($"{weight.ReportId}, GroupId: {weight.ReportField.Group.Id}, Weight Actual: {BunkeringTools.GetDouble(weight.Value).ToString("F3")}, Weight Expected: {correctionFactors.Weight.ToString("F3")}, Weight Complete: {correctionFactors.Weight.ToString()}");
                //    }
                //}
                totalAmount += BunkeringTools.GetDouble2(weight.Value);
            }

            return totalAmount;
        }

        private double GetTotalWeightDiff(List<ReportFieldValueDataModel> prevFieldValues, List<ReportFieldValueDataModel> currFieldValues, Guid groupId)
        {

            double currTotalWeight = GetTotalWeight(currFieldValues, groupId);

            double prevTotalWeight = GetTotalWeight(prevFieldValues, groupId);

            return prevTotalWeight - currTotalWeight;
        }

        public async Task<IDictionary<string, double>> GetTotalConsumptionAsync(int eventId)
        {
            var result = new Dictionary<string, double>();
            result.Add(key: "actualTotalConsumption_hfo", value: 0);
            result.Add(key: "actualTotalConsumption_mgo", value: 0);
            result.Add(key: "poolTotalConsumption_hfo", value: 0);
            result.Add(key: "poolTotalConsumption_mgo", value: 0);
            var targetEvent = await _context.Events
            .Where(e => e.Id == eventId && e.Reports.Any())
            .Select(e => new EventDataModel()
            {
                Id = e.Id,
                UserId = e.UserId,
                Timestamp = e.Timestamp,
                Reports = e.Reports.Select(r => new ReportDataModel()
                {
                    FieldValues = r.FieldValues.Select(fv => new ReportFieldValueDataModel()
                    {
                        ReportId = r.Id,
                        Value = fv.Value,
                        ReportField = new ReportFieldDataModel()
                        {
                            Id = fv.ReportField.Id,
                            Name = fv.ReportField.Name,
                            ValidationKey = fv.ReportField.ValidationKey,
                            Tank = new TankDataModel()
                            {
                                Id = fv.ReportField.Tank.Id,
                                Name = fv.ReportField.Tank.Name
                            },
                            Group = new ReportFieldGroupDataModel()
                            {
                                Id = fv.ReportField.Group.Id,
                                FieldGroupName = fv.ReportField.Group.FieldGroupName,
                                BusinessId = fv.ReportField.Group.BusinessId
                            }
                        }
                    }).ToList()
                }).ToList()
            })
            .FirstOrDefaultAsync();

            if (targetEvent == null)
                return result;

            var prevEvent = await _context.Events
                .Where(_ =>
                _.UserId == targetEvent.UserId &&
                _.Timestamp.HasValue &&
                _.Timestamp <= targetEvent.Timestamp &&
                _.Id != targetEvent.Id &&
                ReportType.Rob.Contains((int)_.EventType.ReportTypeId) &&
                !EventType.BunkeringPlanGroup.Contains(_.EventType.BusinessId) &&
                _.Reports.Any())
                .Select(e => new EventDataModel()
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    Timestamp = e.Timestamp,
                    Reports = e.Reports.Select(r => new ReportDataModel()
                    {
                        FieldValues = r.FieldValues.Select(fv => new ReportFieldValueDataModel()
                        {
                            ReportId = r.Id,
                            Value = fv.Value,
                            ReportField = new ReportFieldDataModel()
                            {
                                Id = fv.ReportField.Id,
                                Name = fv.ReportField.Name,
                                ValidationKey = fv.ReportField.ValidationKey,
                                Tank = new TankDataModel()
                                {
                                    Id = fv.ReportField.Tank.Id,
                                    Name = fv.ReportField.Tank.Name
                                },
                                Group = new ReportFieldGroupDataModel()
                                {
                                    Id = fv.ReportField.Group.Id,
                                    FieldGroupName = fv.ReportField.Group.FieldGroupName,
                                    BusinessId = fv.ReportField.Group.BusinessId
                                }
                            }
                        }).ToList()
                    }).ToList()
                })
                .OrderByDescending(_ => _.Timestamp)
                .ThenByDescending(_ => _.Id)
                .FirstOrDefaultAsync();

            if (prevEvent == null)
                return result;

            var targetReport = targetEvent.Reports.FirstOrDefault();
            var prevReport = prevEvent.Reports.FirstOrDefault();

            if (targetReport == null || prevReport == null)
                return result;

            var currFieldValues = targetReport.FieldValues.ToList();
            var prevFieldValues = prevReport.FieldValues.ToList();

            //HFO ACTUAL

            double actualTotalConsumption_VLSFO = GetTotalWeightDiff(prevFieldValues, currFieldValues, ReportType.RobHfoActualGroup);

            //MGO ACTUAL

            double actualTotalConsumption_LSMGO = GetTotalWeightDiff(prevFieldValues, currFieldValues, ReportType.RobMgoActualGroup);

            //HFO POOL

            double poolTotalConsumption_VLSFO = GetTotalWeightDiff(prevFieldValues, currFieldValues, ReportType.RobHfoPoolGroup);

            //MGO POOL

            double poolTotalConsumption_LSMGO = GetTotalWeightDiff(prevFieldValues, currFieldValues, ReportType.RobMgoPoolGroup);

            result["actualTotalConsumption_hfo"] = actualTotalConsumption_VLSFO;
            result["actualTotalConsumption_mgo"] = actualTotalConsumption_LSMGO;
            result["poolTotalConsumption_hfo"] = poolTotalConsumption_VLSFO;
            result["poolTotalConsumption_mgo"] = poolTotalConsumption_LSMGO;

            return result;
        }

        #endregion

        public async Task<List<BunkeringDataModel>> GetBunkeringPlanList(string userId, DateTimeOffset? timestamp)
        {

            var query = _context.BunkeringData
                .Include(e => e.Port)
                .Include(e => e.Tanks)
                .ThenInclude(e => e.ComminglingData)
                .Where(b => b.UserId == userId &&
                    !b.Events.Any(e => e.EventType.BusinessId == EventType.CommenceBunkering) &&
                    b.Events.Any(e => e.EventType.BusinessId == EventType.BunkeringPlan && e.Reports.Any())
                )
                .AsQueryable();

            if (timestamp.HasValue)
            {
                query = query.Where(e => e.Timestamp < timestamp);
            }

            return await query
                .OrderByDescending(e => e.Timestamp)
                .Take(10)
                .ToListAsync();
        }
        public async Task<int> CreateBunkeringData(BunkeringDataModel bunkeringData)
        {
            await _context.BunkeringData.AddAsync(bunkeringData);
            await _context.SaveChangesAsync();
            return bunkeringData.Id;
        }

        private async Task<EventTypeDataModel> getEventTypeName(string value)
        {
            value = value.ToLower();

            if (value == "arrival")
            {
                value = "eosp";
            }
            else if (value == "departure")
            {
                value = "cosp";
            }
            var eventType = await _context.EventTypes
                .Where(_ => _.Name.ToLower() == value)
                .FirstAsync();
            return new EventTypeDataModel()
            {
                Id = eventType.Id,
                Name = eventType.Name
            };
        }

        public async Task<List<ImportViewModel>> Import()
        {
            var jsonFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "reports.json");
            var jsonString = await System.IO.File.ReadAllTextAsync(jsonFile);
            var vessels = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonString);

            var list = new List<ImportViewModel>();

            foreach (var vessel in vessels)
            {
                var vesselName = vessel["Name"].ToString();
                var vesselId = await _context.Users.Where(u => u.UserName == vesselName.ToLower()).Select(u => u.Id).FirstAsync();
                var reports = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(vessel["Reports"].ToString());
                var item = new ImportViewModel()
                {
                    VesselId = vesselId,
                    Reports = new List<ImportReportViewModel>()
                };

                foreach (var report in reports)
                {
                    var parser = new DictionaryParser(report);

                    string portName = parser.GetString("port");
                    var port = _context.Port
                                       .Where(p => p.Name == portName && portName != null)
                                       .Select(p => new { p.Id, p.Name, p.TimeZone })
                                       .FirstOrDefault();
                    var eventType = await getEventTypeName(parser.GetString("type"));
                    var timestamp = new DateTimeOffset(DateTime.Parse(parser.GetString("date")), TimeSpan.FromHours(port?.TimeZone ?? 0));
                    var reportItem = new ImportReportViewModel()
                    {
                        Id = (int)parser.GetDouble("id"),
                        Timestamp = timestamp,
                        PortId = port?.Id,
                        PortName = port?.Name ?? portName,
                        TimeZone = port?.TimeZone.ToString(),
                        EventTypeId = eventType.Id,
                        EventTypeName = eventType.Name,
                        EventTypeBusinessId = eventType.BusinessId,
                        InstructedSpeed = parser.GetDouble("Instructed speed"),
                        RobHfo = parser.GetDouble("ROB HFO POOL", "ROB HFO  DECLARED"),
                        RobMgo = parser.GetDouble("ROB MGO POOL", "ROB MGO DECLARED2"),
                        DistanceToGo = parser.GetDouble("Distance to Go"),
                        DistanceOverGround = parser.GetDouble("Distance over ground"),
                        SteamingTimeHfo = parser.GetDouble("HFO Period Hours") ?? 0,
                        SteamingTimeMgo = parser.GetDouble("MGO Period Hours") ?? 0,
                        MeConsHfo = parser.GetDouble("HFO POOL M/E CONS"),
                        MeConsMgo = parser.GetDouble("MGO POOL M/E CONS"),
                        OutOfPerformance = parser.GetDouble("Out of Performance"),
                        Slip = parser.GetDouble("Slip Declared", "Slip Pool"),
                        SpeedOverGround = parser.GetDouble("Avg, Speed (over ground)"),
                        Rpm = parser.GetDouble("RPM DECLARED", "RPM Pool"),
                        TotalMeCons = parser.GetDouble("Total Pool M/E Cons")
                    };
                    item.Reports.Add(reportItem);
                }

                for (var i = 0; i < item.Reports.Count; i++)
                {
                    var report = item.Reports[i];

                    var nextReport = i + 1 < item.Reports.Count ? item.Reports[i + 1] : null;
                    var prevReport = i - 1 >= 0 ? item.Reports[i - 1] : null;
                    var offset = report.Timestamp.Offset;
                    var totalSteamingTime = report.SteamingTimeMgo.Value + report.SteamingTimeHfo.Value;
                    if (string.IsNullOrEmpty(report.TimeZone))
                    {
                        if (prevReport != null)
                        {
                            offset = prevReport.Timestamp.Offset;
                        }
                        if (totalSteamingTime == 25)
                        {
                            offset = offset.Add(TimeSpan.FromHours(-1));
                            if (offset.Hours < -11)
                            {
                                offset = TimeSpan.FromHours(12);
                            }
                        }
                        else if (totalSteamingTime == 23) //&& report.EventTypeId == 1 && prevReport?.EventTypeId == 1
                        {
                            offset = offset.Add(TimeSpan.FromHours(+1));
                            if (offset.Hours > 12)
                            {
                                offset = TimeSpan.FromHours(-11);
                            }
                        }
                    }
                    if (totalSteamingTime == 0)
                    {
                        totalSteamingTime = nextReport.SteamingTimeMgo.Value + nextReport.SteamingTimeHfo.Value;
                        var nextTimestamp = new DateTimeOffset(
                            nextReport.Timestamp.Year,
                            nextReport.Timestamp.Month,
                            nextReport.Timestamp.Day,
                            12,
                            0,
                            0,
                            offset
                        );
                        if (totalSteamingTime == 23)
                        {
                            totalSteamingTime += 1;
                        }
                        else if (totalSteamingTime == 25)
                        {
                            totalSteamingTime -= 1;
                        }
                        report.Timestamp = new DateTimeOffset(nextTimestamp.AddHours(-totalSteamingTime).DateTime, offset);
                    }
                    else
                    {
                        if (prevReport == null)
                        {
                            offset = item.Reports.Where(r => !string.IsNullOrEmpty(r.TimeZone)).OrderBy(r => r.Timestamp).Select(r => r.Timestamp.Offset).First();
                            report.Timestamp = new DateTimeOffset(
                                report.Timestamp.Year,
                                report.Timestamp.Month,
                                report.Timestamp.Day,
                                12,
                                0,
                                0,
                                offset
                            );
                        }
                        else
                        {
                            if (totalSteamingTime == 23)
                            {
                                totalSteamingTime += 1;
                            }
                            else if (totalSteamingTime == 25)
                            {
                                totalSteamingTime -= 1;
                            }
                            report.Timestamp = new DateTimeOffset(prevReport.Timestamp.AddHours(totalSteamingTime).DateTime, offset);
                        }
                    }
                }
                var eventsToAdd = new List<ImportReportViewModel>();

                for (var i = 0; i < item.Reports.Count; i++)
                {
                    var report = item.Reports[i];
                    if (report.SteamingTimeMgo > 0 && (i == 0 || item.Reports[i - 1].SteamingTimeMgo == 0))
                    {
                        var timestamp = report.Timestamp.AddHours(-report.SteamingTimeMgo.Value);
                        eventsToAdd.Add(new ImportReportViewModel()
                        {
                            Id = report.Id,
                            Timestamp = timestamp,
                            EventTypeId = 112,
                            EventTypeName = "Commence Change Over to LSMGO",
                        });
                        eventsToAdd.Add(new ImportReportViewModel()
                        {
                            Id = report.Id,
                            Timestamp = timestamp,
                            EventTypeId = 113,
                            EventTypeName = "Complete Change Over to LSMGO",
                        });
                    }
                    else if (report.SteamingTimeHfo > 0 && i > 0 && item.Reports[i - 1].SteamingTimeMgo > 0 && item.Reports[i - 1].SteamingTimeHfo == 0) //
                    {
                        var timestamp = report.Timestamp.AddHours(-report.SteamingTimeHfo.Value);
                        eventsToAdd.Add(new ImportReportViewModel()
                        {
                            Id = report.Id,
                            Timestamp = timestamp,
                            EventTypeId = 114,
                            EventTypeName = "Commence Change Over to VLSFO",
                        });
                        eventsToAdd.Add(new ImportReportViewModel()
                        {
                            Id = report.Id,
                            Timestamp = timestamp,
                            EventTypeId = 115,
                            EventTypeName = "Complete Change Over to VLSFO",
                        });
                    }
                }
                item.Reports.AddRange(eventsToAdd);

                int conditionId = 1;
                DateTimeOffset? conditionChangeDate = null;
                var currentVoyageConditionKey = Guid.NewGuid().ToString();
                DateTimeOffset? voyageStartDate = null;
                DateTimeOffset? voyageEndDate = null;

                item.Reports = item.Reports.OrderBy(r => r.Timestamp).ThenBy(r => r.Id).ToList();

                foreach (var report in item.Reports)
                {

                    if (report.EventTypeBusinessId == EventType.EOSP)
                    {
                        voyageEndDate = report.Timestamp;

                        if (conditionId != 2)
                        {
                            conditionChangeDate = report.Timestamp;
                            conditionId = 2;
                            currentVoyageConditionKey = Guid.NewGuid().ToString();
                        }

                    }
                    else if (report.EventTypeBusinessId == EventType.COSP)
                    {
                        if (voyageStartDate == null)
                        {
                            voyageStartDate = report.Timestamp;
                        }
                        if (conditionId != 1)
                        {
                            conditionChangeDate = report.Timestamp;
                            conditionId = 1;
                            currentVoyageConditionKey = Guid.NewGuid().ToString();

                        }
                    }
                    report.ConditionId = conditionId;
                    report.ConditionStartedDate = conditionChangeDate ?? report.Timestamp;
                    report.CurrentVoyageConditionKey = new Guid(currentVoyageConditionKey);
                }
                var lastReport = item.Reports.OrderBy(r => r.Timestamp).ThenBy(r => r.Id).Last();
                if (voyageEndDate == null)
                {
                    voyageEndDate = lastReport.Timestamp;
                }
                var voyage = new VoyageDataModel()
                {
                    StartDate = voyageStartDate.Value,
                    EndDate = voyageEndDate.Value,
                    UserId = item.VesselId,
                    IsFinished = true,
                    CurrentConditionId = lastReport.ConditionId,
                    CurrentVoyageConditionKey = lastReport.CurrentVoyageConditionKey.Value
                };
                item.Voyage = voyage;
                list.Add(item);
            }
            return list;
        }

        public async Task<bool> HasNextReports(string userId, DateTimeOffset timestamp)
        {
            var hasNextReports = await _context.Events.Where(e => e.UserId == userId &&
                EventType.BunkeringGroup.Contains(e.EventType.BusinessId) &&
                e.Timestamp.HasValue &&
                e.Timestamp > timestamp &&
                e.Reports.Any()
            ).AnyAsync();
            return hasNextReports;

        }

        public async Task<List<Port>> GetDefaultPorts()
        {
            var targetPorts = new List<string>()
            {
                "AA501EBA-E43F-4171-8830-1925E8DECD35",
                "96159E74-1D0E-4AC8-A2A4-A5F08EBA159F"
            };
            var defaultPorts = await _context.Port
                .Where(p => targetPorts.Contains(p.BusinessId))
                .ToListAsync();
            return defaultPorts;
        }
        public async Task<List<Port>> GetNearestPorts(double lat, double lng)
        {
            var inputPoint = new Point(lng, lat) { SRID = 4326 };

            var ports = await _context.Port
                .Include(p => p.Country)
                .ThenInclude(p => p.Region)
                .ThenInclude(p => p.Area)
                //.Where(m => m.Point.Distance(inputPoint) <= 100000)
                .Where(p => p.Id != 1 && p.Point != null)
                .OrderBy(m => m.Point.Distance(inputPoint))
                .Take(20)
                .ToListAsync();




            foreach (var port in ports)
            {
                if (port.Point != null)
                {
                    double distanceInDegrees = port.Point.Distance(inputPoint);

                    double distanceInMeters = distanceInDegrees * 111000;

                    double distanceInMiles = distanceInMeters / 1609.34;



                    port.Distance = Math.Round(distanceInMiles, 0);
                }
            }
            return ports.OrderBy(p => p.Distance).ToList();

        }
        public async Task<List<Dictionary<string, object>>> GetTaResults(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new Exception("Query parameter 'q' is required.");
            query = "SELECT " + Regex.Replace(query.Trim(), "^SELECT", "", RegexOptions.IgnoreCase);
            var connectionString = _config.GetConnectionString("BIONIA");
            await using var conn = new SqlConnection(connectionString);
            await using var cmd = new SqlCommand(query, conn);
            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            var results = new List<Dictionary<string, object?>>();
            var schema = reader.GetColumnSchema();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object?>();
                foreach (var col in schema)
                {
                    var value = reader[col.ColumnName];
                    row[col.ColumnName] = value is DBNull ? null : value;
                }
                results.Add(row);
            }

            return results;

        }

        public IQueryable<EventDataModel> GetEventVoyageQuery(int voyageId)
        {
            return _context.Events.AsNoTracking().Where(e => e.VoyageId == voyageId);
        }

        public async Task CreateMrvMis(MrvMisDataModel model)
        {
            _context.MrvMisData.Add(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMrvMis(MrvMisDataModel model)
        {
            var dbModel = await _context.MrvMisData
                .FirstOrDefaultAsync(e => e.EventId == model.EventId);

            if (dbModel == null)
                return;

            var props = typeof(MrvMisDataModel).GetProperties();

            foreach (var prop in props)
            {
                var excludeProperties = new List<string>()
                {
                    nameof(MrvMisDataModel.Id),
                    nameof(MrvMisDataModel.Vessel),
                    nameof(MrvMisDataModel.EventId),
                    nameof(MrvMisDataModel.EventName),
                    nameof(MrvMisDataModel.ReportId),
                };

                if (excludeProperties.Contains(prop.Name))
                    continue;

                if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
                {
                    var newValue = prop.GetValue(model);
                    var oldValue = prop.GetValue(dbModel);

                    if (!Equals(newValue, oldValue))
                    {
                        prop.SetValue(dbModel, newValue);
                    }
                }
            }


            await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }


}
