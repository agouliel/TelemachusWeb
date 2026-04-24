using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telemachus.Data.Models.Events;
using Telemachus.Data.Services.Context;
using Telemachus.Data.Services.Interfaces;

namespace Telemachus.Data.Services.Repositories
{
    public class VoyageRepository : IVoyageRepository
    {
        private readonly TelemachusContext _context;

        public VoyageRepository(TelemachusContext context)
        {
            _context = context;
        }

        public async Task<VoyageDataModel> GetCurrentVoyageAsync(string userId)
        {
            var currentVoyage =
            await _context.Voyages
            .Include(_ => _.CurrentCondition)
            .OrderByDescending(_ => _.StartDate)
            .ThenByDescending(_ => _.Id)
            .FirstOrDefaultAsync(a => a.UserId == userId && !a.IsFinished);
            return currentVoyage;
        }

        public Task<VoyageDataModel> GetVoyageAsync(int eventId)
        {
            return _context.Events.Include(_ => _.Voyage).ThenInclude(_ => _.Events).ThenInclude(_ => _.EventType).Where(_ => _.Id == eventId).Select(_ => _.Voyage).FirstOrDefaultAsync();
        }

        public async Task<List<ConditionDtoModel>> GetConditionsAsync(string userId, DateTimeOffset eventDate)
        {
            // TODO loop through event range only
            var voyages = await _context.Voyages
                .Include(a => a.Events)
                .Where(a => a.UserId == userId)
                .ToListAsync();
            var conditions = new List<ConditionDtoModel>();
            foreach (var voyage in voyages)
            {
                var groupedEvents = voyage.Events.GroupBy(a => new { a.CurrentVoyageConditionKey, a.ConditionId });
                foreach (var groupedEvent in groupedEvents)
                {
                    var maxDate = voyage.CurrentVoyageConditionKey == groupedEvent.Key.CurrentVoyageConditionKey && !voyage.IsFinished ? DateTimeOffset.MaxValue : groupedEvent.Max(a => a.Timestamp);
                    var minDate = groupedEvent.Min(a => a.Timestamp);
                    if (maxDate >= eventDate && minDate <= eventDate)
                    {
                        var condition =
                            await _context.EventConditions.FirstOrDefaultAsync(
                                a => a.Id == groupedEvent.Key.ConditionId);
                        conditions.Add(new ConditionDtoModel()
                        {
                            Id = groupedEvent.Key.ConditionId,
                            ConditionKey = groupedEvent.Key.CurrentVoyageConditionKey,
                            EndDate = maxDate,
                            StartDate = minDate,
                            VoyageId = voyage.Id,
                            Name = condition?.Name
                        });
                    }
                }
            }

            return conditions.OrderByDescending(a => a.StartDate).ToList();

        }

        public async Task UpdateVoyageCurrentConditionAsync(int conditionId, int voyageId)
        {
            var dbVoyage = await _context.Voyages.FirstOrDefaultAsync(a => a.Id == voyageId);
            if (dbVoyage != null)
            {
                dbVoyage.CurrentConditionId = conditionId;
                dbVoyage.CurrentVoyageConditionKey = Guid.NewGuid();
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> FinishCurrentVoyageAsync(string userId, DateTimeOffset? finishDate)
        {
            var voyage = await GetCurrentVoyageAsync(userId);

            if (voyage != null)
            {
                voyage.IsFinished = true;
                voyage.EndDate = finishDate;
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<VoyageDataModel> CreateVoyageAsync(VoyageDataModel voyage)
        {
            var dbVoyage = _context.Voyages.Add(voyage);
            await _context.SaveChangesAsync();
            return dbVoyage.Entity;
        }

        public Task<bool> HasAnyFacts(string userId)
        {
            return _context.Events.AnyAsync(a => a.UserId == userId);
        }
        public async Task<int?> GetPrevConditionId(string userId, int voyageId)
        {
            var conditionIds = await _context.Events
                .Where(_ => _.VoyageId == voyageId && _.UserId == userId)
                .OrderByDescending(_ => _.Timestamp)
                .ThenByDescending(_ => _.Id)
                .Select(_ => _.ConditionId)
                .ToListAsync();
            if (conditionIds.Count < 2)
                return null;
            for (var i = 1; i < conditionIds.Count; i++)
            {
                if (conditionIds[i] != conditionIds[i - 1])
                    return conditionIds[i];
            }
            return null;
        }
    }
}
