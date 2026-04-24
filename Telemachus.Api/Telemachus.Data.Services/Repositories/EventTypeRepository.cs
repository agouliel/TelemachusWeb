using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Enums;

using Microsoft.EntityFrameworkCore;

using Telemachus.Data.Models.Events;
using Telemachus.Data.Services.Context;
using Telemachus.Data.Services.Interfaces;

namespace Telemachus.Data.Services.Repositories
{
    public class EventTypeRepository : IEventTypeRepository
    {
        private readonly TelemachusContext _context;

        public EventTypeRepository(TelemachusContext context)
        {
            _context = context;
        }
        public Task<List<EventTypeDataModel>> GetEventTypesAsync()
        {
            return _context.EventTypes
                .Include(t => t.NextCondition)
                .Include(t => t.PairedEventType)
                .ThenInclude(t => t.NextCondition)
                .Select(t => new EventTypeDataModel()
                {
                    Id = t.Id,
                    Name = t.Name,
                    Available = t.Available,
                    BusinessId = t.BusinessId,
                    NextCondition = t.NextCondition != null ? new EventConditionDataModel()
                    {
                        BusinessId = t.NextCondition.BusinessId
                    } : null,
                    PairedEventType = t.PairedEventType != null ? new EventTypeDataModel()
                    {
                        NextCondition = t.PairedEventType.NextCondition != null ? new EventConditionDataModel()
                        {
                            BusinessId = t.PairedEventType.NextCondition.BusinessId
                        } : null
                    } : null
                })
                .ToListAsync();
        }
        private async Task<List<EventTypeDataModel>> WithoutPaired(List<EventTypeDataModel> list, EventDataModel @event)
        {
            var conditionId = @event.ConditionId;
            var topEventTypes = await _context.Events
           .Where(e => e.Timestamp >= DateTime.Now.AddYears(-1) && e.UserId == @event.UserId && e.ConditionId == conditionId)
           .GroupBy(e => e.EventTypeId)
           .Select(g => new
           {
               EventTypeId = g.Key,
               Count = g.Count()
           })
           .OrderByDescending(g => g.Count)
           .Take(5)
           .Select(g => g.EventTypeId)
           .ToListAsync();
            foreach (var item in list)
            {
                if (topEventTypes.Contains(item.Id))
                {
                    item.Relevant = true;
                }

            }
            Guid? relevantEventType = null;

            var conditionKey = @event.CurrentVoyageConditionKey;

            var currentEvents = await _context.Events.Where(e => e.UserId == @event.UserId && e.CurrentVoyageConditionKey == @event.CurrentVoyageConditionKey).Select(e => new EventDataModel()
            {
                EventCondition = new EventConditionDataModel()
                {
                    BusinessId = e.EventCondition.BusinessId
                },
                EventType = new EventTypeDataModel()
                {
                    BusinessId = e.EventType.BusinessId
                },
                Timestamp = e.Timestamp,
                PreviousVoyageConditionKey = e.PreviousVoyageConditionKey,
                CurrentVoyageConditionKey = e.CurrentVoyageConditionKey
            }).ToListAsync();

            var currentConditionId = currentEvents.FirstOrDefault()?.EventCondition.BusinessId ?? Condition.Berthed;

            var prevConditionKey = currentEvents.Where(e => e.PreviousVoyageConditionKey.HasValue).Select(e => e.PreviousVoyageConditionKey).FirstOrDefault();

            var prevConditionId = await _context.Events.Where(e => e.CurrentVoyageConditionKey == prevConditionKey).Select(e => e.EventCondition.BusinessId).FirstOrDefaultAsync();

            if (currentConditionId == Condition.Berthed)
            {
                if (currentEvents.Where(e => e.EventType.BusinessId == EventType.SBE || e.EventType.BusinessId == EventType.FWE).Any())
                {
                    var targetEvent = currentEvents.FirstOrDefault(e => e.EventType.BusinessId == EventType.SBE);
                    if (targetEvent?.Timestamp != null)
                    {
                        relevantEventType = EventType.CommenceUnMooring;
                    }
                }
                else
                {
                    relevantEventType = EventType.FWE;
                }
            }
            else if (currentConditionId == Condition.Maneuvering && prevConditionId == Condition.Berthed)
            {
                relevantEventType = EventType.DropAnchor;
            }
            else if (currentConditionId == Condition.AtAnchor)
            {
                if (currentEvents.Where(e => e.EventType.BusinessId == EventType.CommenceHeavingAnchor).Any())
                {
                    relevantEventType = EventType.AnchorUp;
                }
                else if (currentEvents.Where(e => e.EventType.BusinessId == EventType.SBE || e.EventType.BusinessId == EventType.FWE).Any())
                {
                    var targetEvent = currentEvents.FirstOrDefault(e => e.EventType.BusinessId == EventType.SBE);
                    if (targetEvent?.Timestamp != null)
                    {
                        relevantEventType = EventType.CommenceHeavingAnchor;
                    }
                }
                else if (currentEvents.Where(e => e.EventType.BusinessId == EventType.DropAnchor || e.EventType.BusinessId == EventType.AnchoringCompleted).Any())
                {
                    var targetEvent = currentEvents.FirstOrDefault(e => e.EventType.BusinessId == EventType.AnchoringCompleted);
                    if (targetEvent?.Timestamp != null)
                    {
                        relevantEventType = EventType.FWE;
                    }
                }
            }
            else if (currentConditionId == Condition.Maneuvering && prevConditionId == Condition.AtAnchor)
            {
                relevantEventType = EventType.CommenceDrifting;
            }
            else if (currentConditionId == Condition.Drifting)
            {
                if (currentEvents.Where(e => e.EventType.BusinessId == EventType.SBE || e.EventType.BusinessId == EventType.FWE).Any())
                {
                    var targetEvent = currentEvents.FirstOrDefault(e => e.EventType.BusinessId == EventType.SBE);
                    if (targetEvent?.Timestamp != null)
                    {
                        relevantEventType = EventType.CommenceManeuvering;
                    }
                }
                else
                {
                    relevantEventType = EventType.FWE;
                }
            }
            else if (currentConditionId == Condition.Maneuvering && prevConditionId == Condition.Drifting)
            {
                relevantEventType = EventType.COSP;
            }
            else if (currentConditionId == Condition.AtSea)
            {
                relevantEventType = EventType.EOSP;
            }
            else if (currentConditionId == Condition.Maneuvering && prevConditionId == Condition.AtSea)
            {
                relevantEventType = EventType.CommenceMooring;
            }

            foreach (var item in list)
            {
                if (item.BusinessId == relevantEventType)
                {
                    item.Suggested = true;
                }

            }
            return list;
        }

        private async Task<EventDataModel> GetClosestEvent(string userId, EventDataModel targetEvent, EventTypeDataModel targetEventType)
        {
            var query = _context.Events
                .Include(e => e.ChildrenEvents)
                .AsNoTracking()
                .Where(e =>
                    e.UserId == userId &&
                    e.EventTypeId == targetEventType.Id &&
                    e.Timestamp.HasValue)
                .AsQueryable();
            if (targetEvent.Id > 0)
            {
                query = query.Where(e => e.Timestamp < targetEvent.Timestamp || (e.Timestamp == targetEvent.Timestamp && e.Id < targetEvent.Id));
            }
            else
            {
                query = query.Where(e => e.Timestamp <= targetEvent.Timestamp);
            }
            return await query
                .OrderByDescending(e => e.Timestamp)
                .ThenByDescending(e => e.Id)
                .FirstOrDefaultAsync();
        }

        private async Task<EventDataModel> GetClosestEvent2(string userId, DateTimeOffset targetTimestamp, EventTypeDataModel targetEventType)
        {
            var query = _context.Events
                .Include(e => e.ChildrenEvents)
                .AsNoTracking()
                .Where(e =>
                    e.UserId == userId &&
                    e.EventTypeId == targetEventType.Id &&
                    e.Timestamp.HasValue)
                .AsQueryable();
            query = query.Where(e => e.Timestamp <= targetTimestamp);
            return await query
                .OrderByDescending(e => e.Timestamp)
                .ThenByDescending(e => e.Id)
                .FirstOrDefaultAsync();
        }
        private async Task<bool> CheckIfLastPairedEventIsCompleted(string userId, int eventTypeId)
        {
            var targetEvent = await _context.Events
                .Include(e => e.ChildrenEvents)
                .Where(e =>
                e.UserId == userId &&
                e.Timestamp.HasValue &&
                e.EventTypeId == eventTypeId
                )
                .OrderByDescending(e => e.Timestamp)
                .ThenByDescending(e => e.Id)
                .FirstOrDefaultAsync();
            return targetEvent == null || targetEvent.ChildEvent == null || targetEvent.ChildEvent.Timestamp != null;
        }
        private async Task<List<EventDataModel>> GetRangeBetweenConditionEvents(string userId, EventDataModel targetEvent)
        {
            var targetConditionKey = targetEvent.CurrentVoyageConditionKey;
            if (targetConditionKey == null || targetConditionKey == Guid.Empty)
            {
                targetConditionKey = await _context.Events
                    .Where(e =>
                    e.UserId == userId &&
                    e.Timestamp.HasValue &&
                    e.Timestamp >= targetEvent.Timestamp
                    )
                    .OrderBy(e => e.Timestamp)
                    .ThenByDescending(e => e.Id)
                    .Select(e => e.CurrentVoyageConditionKey)
                    .FirstOrDefaultAsync();
            }
            var query = _context.Events
                .AsNoTracking()
                .Where(e =>
                    e.UserId == userId &&
                    e.Timestamp.HasValue &&
                    e.CurrentVoyageConditionKey == targetConditionKey
                    )
                .AsQueryable();
            return await query.OrderBy(e => e.Timestamp)
            .ThenBy(e => e.Id)
            .ToListAsync();
        }
        private async Task<List<EventDataModel>> GetRangeBetweenTargetEventAsync(string userId, EventDataModel targetEvent, EventDataModel closestEvent)
        {
            var query = _context.Events
                .AsNoTracking()
                .Where(e =>
                    e.UserId == userId &&
                    e.Timestamp.HasValue
                    )
                .AsQueryable();
            if (targetEvent.Id > 0)
            {
                query = query.Where(e => e.Timestamp < targetEvent.Timestamp || (e.Timestamp == targetEvent.Timestamp && e.Id < targetEvent.Id));
            }
            if (closestEvent?.Timestamp != null)
            {
                query = query.Where(e => e.Timestamp > closestEvent.Timestamp || (e.Timestamp == closestEvent.Timestamp && e.Id > closestEvent.Id));
            }
            return await query.OrderByDescending(e => e.Timestamp)
            .ThenByDescending(e => e.Id)
            .Take(250)
            .ToListAsync();
        }
        private async Task<List<EventDataModel>> GetRangeBetweenPrereqEventAsync(string userId, EventDataModel targetEvent, EventTypeDataModel prereq)
        {
            //TODO: targetEvent.Timestamp, new events timestamp minutes + 1
            if (targetEvent.EventType.Id == 36)
            {
                Console.Write("");
            }
            var query = _context.Events
                .AsNoTracking()
                .Where(e =>
                    e.UserId == userId &&
                    e.Timestamp.HasValue &&
                    e.EventTypeId == prereq.Id);
            if (targetEvent.Id > 0)
            {
                query = query.Where(e => e.Timestamp < targetEvent.Timestamp || (e.Timestamp == targetEvent.Timestamp && e.Id < targetEvent.Id));
            }
            else
            {
                query = query.Where(e => e.Timestamp <= targetEvent.Timestamp);
            }
            var closestEvent = await query
                .OrderByDescending(e => e.Timestamp)
                .ThenByDescending(e => e.Id)
                .FirstOrDefaultAsync();

            var closestTimestamp = closestEvent?.Timestamp ?? DateTimeOffset.MinValue;

            query = _context.Events
                .AsNoTracking()
                .Where(e =>
                    e.UserId == userId &&
                    e.Timestamp.HasValue &&
                    e.EventTypeId == prereq.Id);

            if (targetEvent.Id > 0)
            {
                query = query.Where(e => e.Timestamp > targetEvent.Timestamp || (e.Timestamp == targetEvent.Timestamp && e.Id > targetEvent.Id));
            }
            else
            {
                query = query.Where(e => e.Timestamp >= targetEvent.Timestamp);
            }

            var prevEvent = await query
                .OrderBy(e => e.Timestamp)
                .ThenBy(e => e.Id)
                .FirstOrDefaultAsync();

            var prevTimestamp = prevEvent?.Timestamp ?? DateTimeOffset.MaxValue;

            query = _context.Events
                .AsNoTracking()
                .Where(e =>
                    e.UserId == userId &&
                    e.Timestamp.HasValue
                    )
                .AsQueryable();

            if (closestEvent != null)
            {
                query = query.Where(e => e.Timestamp > closestTimestamp || (e.Timestamp == closestEvent.Timestamp && e.Id > closestEvent.Id));
            }
            else
            {
                query = query.Where(e => e.Timestamp > closestTimestamp);
            }

            if (prevEvent != null)
            {
                query = query.Where(e => e.Timestamp < prevTimestamp || (e.Timestamp == prevEvent.Timestamp && e.Id < prevEvent.Id));
            }
            else
            {
                query = query.Where(e => e.Timestamp < prevTimestamp);
            }

            var eventRange = await query
                .OrderBy(e => e.Timestamp)
                .ThenBy(e => e.Id)
                .Take(250)
                .ToListAsync();
            return eventRange;
        }
        private async Task ValidateRepetition(string userId, EventDataModel targetEvent)
        {
            var requireForRepetition = targetEvent.EventType.Prerequisites.Any(p => p.RequiredForRepetition);

            if (!requireForRepetition)
            {
                return;
            }

            foreach (var prerequisite in targetEvent.EventType.Prerequisites)
            {
                if (!prerequisite.RequiredForRepetition)
                    continue;
                var prereqEventType = prerequisite.AvailableAfterEventType;


                var targetPrerequisite = prereqEventType;

                if (prerequisite.Completed)
                {
                    targetPrerequisite = prereqEventType.PairedEventType;
                }

                var eventRange = await GetRangeBetweenPrereqEventAsync(userId, targetEvent, targetPrerequisite);

                if (eventRange.Any(e => e.EventTypeId == targetEvent.EventType.Id))
                {
                    throw new CustomException($"<span class=\"text-decoration-underline\" title=\"{GetDateRangeExceptionMessage(eventRange.First()?.Timestamp, eventRange.Last()?.Timestamp)}\">{targetEvent.EventType.Name}</span> already happened after {targetPrerequisite.Name}.");
                }
            }
        }

        private async Task ValidateAvailability(string userId, EventDataModel targetEvent, DateTimeOffset targetTimestamp)
        {
            //if (targetEvent.EventType.Id == 1136)
            //{
            //    Console.Write("");
            //}
            if (targetEvent.EventType.OnePairPerTime == true && targetEvent.EventType.PairedEventTypeId != null)
            {
                var lastEventIsCompleted = await CheckIfLastPairedEventIsCompleted(userId, targetEvent.EventType.Id);
                if (!lastEventIsCompleted)
                {
                    throw new CustomException($"<div>The last event pair of <span class=\"text-decoration-underline\">{targetEvent.EventType.Name}</span> must be completed before proceeding to the new one.</div>");
                }
            }

            var notFoundPrerequisites = new List<EventTypePrerequisiteDataModel>();

            var notFoundRequiredPrerequisites = new List<string>();

            var shouldNotExistPrerequisites = new List<string>();

            var exceptionMessages = new List<string>();

            foreach (var prerequisite in targetEvent.EventType.Prerequisites)
            {

                var message = "";
                var targetPrerequisite = prerequisite.AvailableAfterEventType;

                if (prerequisite.Override)
                {
                    var closestTargetEvent = await GetClosestEvent(userId, targetEvent, targetEvent.EventType);
                    if (closestTargetEvent != null)
                    {
                        var eventRange = await GetRangeBetweenTargetEventAsync(userId, targetEvent, closestTargetEvent);
                        if (eventRange.Any(e => e.EventTypeId == targetPrerequisite.Id && e.Timestamp.HasValue))
                        {
                            message = GetDateRangeExceptionMessage(closestTargetEvent?.Timestamp, targetEvent?.Timestamp);
                            shouldNotExistPrerequisites.Add(
                                $"<span class=\"text-decoration-underline\" title=\"{message}\">{targetPrerequisite.Name}</span>");
                        }
                    }
                    continue;
                }

                var isAvailable = false;

                var closestPrereqEvent = await GetClosestEvent2(userId, targetTimestamp, targetPrerequisite);

                if (closestPrereqEvent != null)
                {
                    bool prerequisiteMet = prerequisite.Completed
                        ? closestPrereqEvent.ChildEvent?.Timestamp <= targetTimestamp
                        : closestPrereqEvent.Timestamp <= targetTimestamp;

                    if (prerequisiteMet)
                    {
                        isAvailable = true;
                    }
                }

                message = GetDateRangeExceptionMessage(prerequisite.Completed ? closestPrereqEvent?.ChildEvent?.Timestamp : closestPrereqEvent?.Timestamp, targetTimestamp);

                if (!isAvailable)
                {
                    if (prerequisite.Required)
                    {
                        notFoundRequiredPrerequisites.Add(
                            $"<span class=\"text-decoration-underline\" title=\"{message}\">{(prerequisite.Completed ? prerequisite.AvailableAfterEventType.PairedEventType.Name : targetPrerequisite.Name)}</span>");
                    }
                    else
                    {
                        prerequisite.Comment = message;
                        notFoundPrerequisites.Add(prerequisite);
                    }
                }

            }

            if (shouldNotExistPrerequisites.Count > 0)
            {
                exceptionMessages.Add($"{string.Join(", ", shouldNotExistPrerequisites)} event should not exist.");
            }

            if (notFoundRequiredPrerequisites.Count > 0)
            {
                exceptionMessages.Add($"All of the above events are mandatory but are missing: {string.Join(", ", notFoundRequiredPrerequisites)}.");
            }
            else if (targetEvent.EventType.Prerequisites.Count() > 0 && notFoundPrerequisites.Count() > 0 &&
                    targetEvent.EventType.Prerequisites.Where(p => p.Required || (!p.Override && !p.RequiredForRepetition)).All(p => notFoundPrerequisites.Select(nfp => nfp.Id).Contains(p.Id)))
            {
                exceptionMessages.Add(
                    "None of the above events found or completed: " +
                    string.Join(", ", notFoundPrerequisites.Select(et =>
                        $"<span class=\"text-decoration-underline\" title=\"{et.Comment}\">" +
                        $"{(et.Completed ? et.AvailableAfterEventType.PairedEventType.Name : et.AvailableAfterEventType.Name)}</span>"
                    )) + "."
                );
            }

            if (exceptionMessages.Count() > 0)
            {
                throw new CustomException(string.Join("", exceptionMessages.Select(item => $"<div>{item}</div>")));
            }
        }
        private string GetDateRangeExceptionMessage(DateTimeOffset? dateFrom, DateTimeOffset? dateTo)
        {
            if (!dateFrom.HasValue && !dateTo.HasValue)
                return "No date range provided.";

            if (dateFrom.HasValue && dateTo.HasValue)
                return $"Date range: {dateFrom:dd/MM/yyyy HH:mm} to {dateTo:dd/MM/yyyy HH:mm}.";

            return dateFrom.HasValue
                ? $"Starting from {dateFrom:dd/MM/yyyy HH:mm}."
                : $"Up until {dateTo:dd/MM/yyyy HH:mm}.";
        }

        private async Task ValidateInCondition(string userId, EventTypeDataModel eventType, DateTimeOffset targetTimestamp)
        {
            var targetEvent = await _context.Events
                .Include(e => e.EventCondition)
            .Where(e =>
                e.UserId == userId &&
                e.Timestamp.HasValue &&
                e.Timestamp >= targetTimestamp)
                .OrderBy(e => e.Timestamp)
                .ThenBy(e => e.Id)
                .FirstOrDefaultAsync();
            await ValidateInCondition(userId, eventType, targetEvent.EventCondition);
        }
        private async Task ValidateInCondition(string userId, EventTypeDataModel eventType, EventConditionDataModel condition)
        {
            var inCondition = await _context.EventTypesConditions.AnyAsync(ec => ec.ConditionId == condition.Id && ec.EventTypeId == eventType.Id);
            if (!inCondition)
            {
                throw new CustomException($"<div>The event type <span class=\"text-decoration-underline\">{eventType.Name}</span> cannot be used in <span class=\"text-decoration-underline\">{condition.Name}</span> condition.</div>");
            }
        }

        /*
         * Optional Id
         * Requires EventType, EventType.Prerequisites, EventType.Prerequisites.AvailableAfterEventType, EventType.Prerequisites.AvailableAfterEventType.PairedEventType, Timestamp
         */
        private async Task ValidatePrerequisites(string userId, EventDataModel targetEvent, DateTimeOffset targetTimestamp)
        {
            await ValidateInCondition(userId, targetEvent.EventType, targetEvent.EventCondition);
            await ValidateAvailability(userId, targetEvent, targetTimestamp);
            await ValidateRepetition(userId, targetEvent);
        }

        public async Task<List<EventTypeDataModel>> GetEventTypesFromTargetEventAsync(EventDataModel @event, DateTimeOffset targetTimestamp, VoyageDataModel fallbackVoyage)
        {

            if (fallbackVoyage == null)
            {
                throw new CustomException("Error loading data. Sync data before continuing.");
            }

            var userId = @event?.UserId ?? fallbackVoyage.UserId;
            var voyageId = @event?.VoyageId ?? fallbackVoyage.Id;

            var pairedEventTypeIds = await _context.EventTypes.Where(e => e.PairedEventTypeId.HasValue).Select(e => (int)e.PairedEventTypeId).Distinct().ToListAsync();

            var eventTypes = await _context.EventTypes
                .Include(t => t.Prerequisites)
                .ThenInclude(t => t.AvailableAfterEventType)
                .ThenInclude(t => t.PairedEventType)
                .Include(t => t.NextCondition)
                .Include(t => t.PairedEventType)
                .ThenInclude(t => t.NextCondition)
                .Where(t => !pairedEventTypeIds.Contains(t.Id))
                .Select(t => new EventTypeDataModel()
                {
                    Id = t.Id,
                    Name = t.Name,
                    PairedEventTypeId = t.PairedEventTypeId,
                    NextConditionId = t.NextConditionId,
                    Transit = t.Transit,
                    OnePairPerTime = t.OnePairPerTime,
                    EventType = t.EventType,
                    ReportTypeId = t.ReportTypeId,
                    BusinessId = t.BusinessId,
                    NextCondition = t.NextCondition != null ? new EventConditionDataModel()
                    {
                        BusinessId = t.NextCondition.BusinessId,
                        Name = t.NextCondition.Name
                    } : null,
                    PairedEventType = t.PairedEventType != null ? new EventTypeDataModel()
                    {
                        NextCondition = t.PairedEventType.NextCondition != null ? new EventConditionDataModel()
                        {
                            BusinessId = t.PairedEventType.NextCondition.BusinessId,
                            Name = t.PairedEventType.NextCondition.Name
                        } : null
                    } : null,
                    Prerequisites = t.Prerequisites
                })
                .ToListAsync();

            foreach (var eventType in eventTypes)
            {
                var targetEvent = new EventDataModel()
                {
                    EventTypeId = eventType.Id,
                    EventCondition = @event?.EventCondition ?? fallbackVoyage.CurrentCondition,
                    ConditionId = @event?.ConditionId ?? fallbackVoyage.CurrentCondition.Id,
                    EventType = eventType,
                    Timestamp = @event?.Timestamp ?? DateTimeOffset.MaxValue
                };
                if (eventType.IsCustom())
                {
                    eventType.Available = true;

                    var currentConditionKey = @event?.CurrentVoyageConditionKey ?? fallbackVoyage.CurrentVoyageConditionKey;

                    if (currentConditionKey != null)
                    {
                        var customEventsCount = await _context.Events.Where(e => e.UserId == userId && e.CurrentVoyageConditionKey == currentConditionKey &&
                        e.EventType.BusinessId == EventType.Custom)
                            .CountAsync();
                        if (customEventsCount > 7)
                        {
                            eventType.Available = false;
                            eventType.Comment = "Maximum number (8) of available custom events for current condition exceeded!";
                            continue;
                        }
                    }
                    continue;
                }
                try
                {
                    await ValidatePrerequisites(userId, targetEvent, targetTimestamp);
                    eventType.Available = true;
                }
                catch (CustomException e)
                {
                    eventType.Available = false;
                    eventType.Comment = e.Message;
                }
                if (eventType.IsCommenceBunkering) // onyl if @event is not null
                {
                    var hasPendingBunkeringEvent = await _context.BunkeringData
                        .Where(b => b.UserId == userId && b.Events.Where(e => e.VoyageId == voyageId && EventType.CommenceBunkering == e.EventType.BusinessId).Any() && string.IsNullOrEmpty(b.Bdn)).AnyAsync();
                    if (hasPendingBunkeringEvent)
                    {
                        eventType.Comment = "Please complete the last bunkering event before applying a new one.";
                        eventType.Available = false;
                    }
                }
            }

            if (@event == null)
            {
                return eventTypes;
            }

            return await WithoutPaired(eventTypes, @event);
        }
    }
}
