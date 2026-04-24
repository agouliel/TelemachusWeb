using Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telemachus.Business.Interfaces;
using Telemachus.Business.Models.Events;
using Telemachus.Business.Services.Mappers;
using Telemachus.Data.Models;
using Telemachus.Data.Models.Events;
using Telemachus.Data.Services.Context;

namespace Telemachus.Business.Services
{
    public class StatementService : IStatementService
    {
        private readonly TelemachusContext _context;

        public StatementService(TelemachusContext context)
        {
            _context = context;
        }
        public async Task<StatementOfFact> CreateStatement(string userId, DocumentViewModelDTO document)
        {
            var hasPending = await _context.StatementOfFact.Where(_ => _.UserId == userId && !_.Completed).AnyAsync();

            if (hasPending)
            {
                throw new Exception("");
            }

            var statement = new StatementOfFact()
            {
                Completed = document.Completed,
                Date = document.Date,
                OperationGrade = document.OperationGrade,
                CharterParty = document.CharterParty,
                PortId = document.PortId,
                Remarks = document.Remarks,
                Terminal = document.Terminal,
                Voyage = document.Voyage,
                UserId = userId
            };

            var eventsIds = Enumerable.Concat(document.EventInclude, document.EventExclude).ToList();

            var events = await _context.Events
                .Where(_ => _.UserId == userId && (eventsIds.Contains(_.Id)))
                .OrderBy(_ => _.Timestamp)
                .ToListAsync();

            foreach (var fact in events)
            {
                if (document.EventExclude.Contains(fact.Id))
                {
                    fact.ExcludeFromStatement = true;
                }
                else if (document.EventInclude.Contains(fact.Id))
                {
                    fact.ExcludeFromStatement = false;
                }
                if (document.HiddenDates.Contains(fact.Id))
                {
                    fact.HiddenDate = true;
                }
                else
                {
                    fact.HiddenDate = false;
                }
            }

            await _context.SaveChangesAsync();

            var FirstEvent = events.Where(_ => !_.ExcludeFromStatement).First();
            var LastEvent = events.Where(_ => !_.ExcludeFromStatement).Last();

            statement.FirstEventId = FirstEvent.Id;
            statement.LastEventId = LastEvent.Id;
            statement.FromDate = document.DateFrom ?? FirstEvent.Timestamp.Value.UtcDateTime;
            statement.ToDate = document.DateTo ?? LastEvent.Timestamp.Value.UtcDateTime;

            _context.StatementOfFact.Add(statement);

            await _context.SaveChangesAsync();

            return statement;

        }
        public async Task<List<StatementOfFact>> GetStatements(string userId)
        {
            var statements = await _context.StatementOfFact.Where(_ => _.UserId == userId).OrderByDescending(_ => _.Id).Take(10).ToListAsync();
            return statements;
        }
        public async Task<StatementOfFact> UpdateStatement(string userId, DocumentViewModelDTO document, int id)
        {

            var statement = await _context.StatementOfFact.Where(_ => _.UserId == userId && _.Id == id).FirstOrDefaultAsync();

            var completed = statement?.Completed ?? false;

            if (statement == null)
            {
                throw new KeyNotFoundException("");
            }

            statement.Completed = document.Completed;
            statement.Date = document.Date;
            statement.OperationGrade = document.OperationGrade;
            statement.PortId = document.PortId;
            statement.Remarks = document.Remarks;
            statement.Terminal = document.Terminal;
            statement.Voyage = document.Voyage;
            statement.UserId = userId;
            statement.CharterParty = document.CharterParty;

            var eventsIds = Enumerable.Concat(document.EventInclude, document.EventExclude).ToList();

            var events = await _context.Events
                .Where(_ => _.UserId == userId && (eventsIds.Contains(_.Id)))
                .OrderBy(_ => _.Timestamp)
                .ToListAsync();

            foreach (var fact in events)
            {
                if (document.EventExclude.Contains(fact.Id))
                {
                    fact.ExcludeFromStatement = true;
                }
                else if (document.EventInclude.Contains(fact.Id))
                {
                    fact.ExcludeFromStatement = false;
                }
                if (document.HiddenDates.Contains(fact.Id))
                {
                    fact.HiddenDate = true;
                }
                else
                {
                    fact.HiddenDate = false;
                }
            }

            await _context.SaveChangesAsync();

            if (!completed)
            {
                var FirstEvent = events.Where(_ => !_.ExcludeFromStatement).First();
                var LastEvent = events.Where(_ => !_.ExcludeFromStatement).Last();
                statement.FirstEventId = FirstEvent.Id;
                statement.LastEventId = LastEvent.Id;
                statement.FromDate = document.DateFrom ?? FirstEvent.Timestamp.Value.UtcDateTime;
                statement.ToDate = document.DateTo ?? LastEvent.Timestamp.Value.UtcDateTime;
            }

            await _context.SaveChangesAsync();

            return statement;
        }
        public async Task PatchStatement(string userId, int id, bool complete)
        {
            var statement = await _context.StatementOfFact.Where(_ => _.UserId == userId && _.Id == id).FirstOrDefaultAsync();

            if (statement == null)
            {
                throw new KeyNotFoundException("");
            }

            if (complete == false)
            {
                var nextStatement = await _context.StatementOfFact.Where(_ => _.UserId == userId && _.Id > id).OrderBy(_ => _.Id).SingleOrDefaultAsync();

                if (nextStatement != null)
                {
                    if (nextStatement.Completed == true)
                    {
                        throw new Exception("");
                    }
                    _context.Remove(nextStatement);
                }
            }

            statement.Completed = complete;

            await _context.SaveChangesAsync();

            return;
        }
        public async Task DeleteStatement(string userId, int id)
        {

            var statement = await _context.StatementOfFact.Where(_ => _.UserId == userId && _.Id == id).FirstOrDefaultAsync();

            if (statement == null)
            {
                throw new KeyNotFoundException("");
            }

            _context.StatementOfFact.Remove(statement);

            await _context.SaveChangesAsync();

            return;
        }
        public async Task<DocumentViewModel> GetDocumentFactsViewModel(string userId, int? statementId, DateTime? from = null, DateTime? to = null)
        {
            var statement = await _context.StatementOfFact.Where(_ => _.UserId == userId && !_.Completed).SingleOrDefaultAsync();

            if (statementId.HasValue)
            {
                statement = await _context.StatementOfFact.Where(_ => _.UserId == userId && _.Id == statementId).SingleOrDefaultAsync();
            }

            var lastStatement = await _context.StatementOfFact.Where(_ => _.UserId == userId && _.Completed).OrderByDescending(_ => _.Id).FirstOrDefaultAsync();

            var eventMarker = await _context.Events.Where(_ => lastStatement != null && _.Id == lastStatement.LastEventId).Select(_ => _.Timestamp.Value.DateTime).FirstOrDefaultAsync();

            var events = _context.Events
                .Include(a => a.EventType)
                .Include(a => a.User)
                .Include(a => a.Status)
                .Include(a => a.EventCondition)
                .Include(a => a.Voyage)
                .Where(_ => _.Timestamp.HasValue && _.UserId == userId && _.Status.BusinessId != Status.Rejected)
                .AsQueryable();

            var document = new DocumentViewModel()
            {
                Id = statement?.Id,
                Completed = statement?.Completed ?? false,
                FirstId = statement?.FirstEventId,
                LastId = statement?.LastEventId,
                DateFrom = from ?? statement?.FromDate ?? lastStatement?.ToDate,
                DateTo = to ?? statement?.ToDate,
                Date = statement?.Date,
                PortId = statement?.PortId,
                CharterParty = statement?.CharterParty,
                Voyage = statement?.Voyage,
                Terminal = statement?.Terminal,
                OperationGrade = statement?.OperationGrade,
                Remarks = statement?.Remarks,
                LastEventDate = eventMarker
            };


            document.PortName = document.PortId != null ? await _context.Port.Where(_ => _.Id == document.PortId).Select(_ => _.CodeOrName).FirstOrDefaultAsync() : null;

            if (document.Completed == false)
            {
                if (document.DateFrom.HasValue)
                {
                    events = events.Where(_ => _.Timestamp.Value.Date >= document.DateFrom.Value.Date);
                }
                if (document.DateTo.HasValue)
                {
                    events = events.Where(_ => _.Timestamp.Value.Date <= document.DateTo.Value.Date);
                }
            }
            else
            {
                events = events.Where(_ => _.Timestamp.Value.Date >= document.DateFrom.Value.Date && _.Timestamp.Value.Date <= document.DateTo.Value.Date);
                if (document.FirstId.HasValue)
                {
                    events = events.Where(_ => _.Id >= document.FirstId);
                }
                if (document.LastId.HasValue)
                {
                    events = events.Where(_ => _.Id <= document.LastId);
                }

            }

            var facts = await events.OrderBy(_ => _.Timestamp).ThenBy(_ => _.Id).ToListAsync();

            var minDate = await _context.Events.Where(_ => _.Timestamp.HasValue && _.UserId == userId && _.Status.BusinessId != Status.Rejected).OrderBy(_ => _.Timestamp).Select(_ => _.Timestamp).FirstOrDefaultAsync();
            var maxDate = await _context.Events.Where(_ => _.Timestamp.HasValue && _.UserId == userId && _.Status.BusinessId != Status.Rejected).OrderByDescending(_ => _.Timestamp).Select(_ => _.Timestamp).FirstOrDefaultAsync();

            if (!document.Completed)
            {
                document.MinDate = minDate?.AddDays(-1).UtcDateTime ?? DateTime.UtcNow.AddDays(1);
                document.MaxDate = maxDate?.AddDays(1).UtcDateTime ?? DateTime.UtcNow.AddDays(1);

                if (lastStatement?.ToDate != null)
                {
                    document.MinDate = lastStatement.ToDate;
                }

                var markers = new List<EventDataModel>();

                var excludeIds = facts.Select(_ => _.Id).ToList();

                var query = _context.Events.Where(_ => _.UserId == userId);

                if (facts.Any())
                {
                    query = query.Where(_ => _.Timestamp < facts.First().Timestamp);
                }
                else if (from.HasValue)
                {
                    query = query.Where(_ => _.Timestamp.Value.Date < from.Value.Date);
                }

                var voyageIds = await query.GroupBy(_ => _.VoyageId)
                    .OrderByDescending(_ => _.Key)
                    .Select(_ => _.Key)
                    .Take(2)
                    .ToListAsync();

                var selectedEvents = await _context.Events
                    .Include(_ => _.EventCondition)
                    .Where(_ => _.UserId == userId)
                    .Where(_ => voyageIds.Contains(_.VoyageId))
                    .Where(_ => facts.Count == 0 || _.Timestamp < facts.First().Timestamp)
                    .Where(_ => (facts.Count == 0 || !from.HasValue) || _.Timestamp.Value.Date < from.Value.Date)
                    .OrderByDescending(_ => _.Timestamp)
                    .ThenByDescending(_ => _.Id)
                    .ToListAsync();

                if (selectedEvents.Any())
                {
                    for (var i = 1; i < selectedEvents.Count; i++)
                    {
                        if (i > 0 && selectedEvents[i - 1].ConditionId != selectedEvents[i].ConditionId)
                        {
                            markers.Add(selectedEvents[i - 1]);
                        }
                    }
                    markers.Add(selectedEvents.LastOrDefault());
                    document.Markers = markers.Select(_ => _.AsEventMarker()).ToList();
                }
                var firstEvent = facts.FirstOrDefault();

                if (firstEvent != null)
                {
                    if (document.MinDate > firstEvent.Timestamp?.AddDays(-1).UtcDateTime)
                    {
                        if (from != null)
                        {
                            if (from > firstEvent.Timestamp.Value.AddDays(-1).UtcDateTime)
                            {
                                document.MinDate = firstEvent.Timestamp.Value.AddDays(-1).UtcDateTime;
                            }
                            else
                            {
                                document.MinDate = (DateTime)from;
                            }
                        }
                        else
                        {
                            document.MinDate = firstEvent.Timestamp.Value.AddDays(-1).UtcDateTime;
                        }
                    }
                }

                if (document.DateFrom == null) document.DateFrom = document.MinDate;
                if (document.DateTo == null) document.DateTo = document.MaxDate;
            }
            else
            {
                document.MinDate = document.DateFrom ?? DateTime.UtcNow.AddDays(1);
                document.MaxDate = document.DateTo ?? DateTime.UtcNow.AddDays(1);

            }

            if (document.MinDate > document.MaxDate)
            {
                document.MinDate = document.MaxDate;
            }

            if (document.DateFrom > document.DateTo)
            {
                document.DateFrom = document.DateTo;
            }

            document.Facts = facts.Select(_ => _.AsDocumentViewModel()).ToList();

            return document;
        }
    }
}
