using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Enums;

using Helpers;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using Telemachus.Data.Models.Authentication;
using Telemachus.Data.Models.Events;
using Telemachus.Data.Models.Reports;
using Telemachus.Data.Services.Context;
using Telemachus.Data.Services.Interfaces;

namespace Telemachus.Data.Services.Repositories
{
    public class ReportDesignRepository : IReportDesignRepository
    {
        private readonly TelemachusContext _context;
        private readonly ILogger<ReportDesignRepository> _logger;

        public ReportDesignRepository(TelemachusContext context, ILogger<ReportDesignRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> HasValues(string fieldName)
        {
            var targetFields = await _context.ReportFields.Where(_ => _.Name == fieldName).Select(_ => _.Id).ToListAsync();
            return await _context.ReportFieldValues.Where(_ => targetFields.Contains(_.ReportFieldId)).AnyAsync();
        }
        public async Task<bool> IsHidden(string fieldName)
        {
            var targetFields = await _context.ReportFields.Where(_ => _.Name == fieldName).Select(_ => _.Id).ToListAsync();
            var hasRelations = await _context.ReportFieldRelations.Where(_ => targetFields.Contains(_.ReportFieldId)).AnyAsync();
            return !hasRelations;
        }
        public async Task<List<string>> GetReportFieldNamesAsync()
        {

            var list = await _context.ReportFields
                .GroupBy(_ => new { _.Name })
                .OrderBy(_ => _.Key.Name)
                .Select(_ => _.Key.Name)
                .ToListAsync();
            return list;
        }
        public async Task<ReportFieldDataModel> GetField(string fieldName)
        {
            var field = await _context.ReportFields.Where(_ => _.Name == fieldName).OrderBy(_ => _.Name).FirstOrDefaultAsync();
            return field;
        }
        public async Task<List<int>> GetReportTypeIds(string fieldName)
        {
            var fieldIds = await _context.ReportFields.Where(_ => _.Name == fieldName).Select(_ => _.Id).ToListAsync();

            var list = await _context.ReportFieldRelations
                .Where(_ => fieldIds.Contains(_.ReportFieldId))
                .Select(_ => _.ReportTypeId)
                .Distinct().ToListAsync();
            return list;
        }
        public async Task<List<int>> GetGroupIds(string fieldName)
        {
            var groupIds = await _context.ReportFields
                .Where(_ => _.Name == fieldName && _.GroupId.HasValue)
                .Select(_ => _.GroupId.Value)
                .Distinct()
                .ToListAsync();
            var enabledGroupIds = new List<int>();
            foreach (var groupId in groupIds)
            {
                var relatedFieldIds = await _context.ReportFields
                    .Where(_ => _.Name == fieldName && _.GroupId == groupId)
                    .Select(_ => _.Id)
                    .ToListAsync();
                var hasRelations = await _context.ReportFieldRelations
                    .Where(_ => relatedFieldIds.Contains(_.ReportFieldId))
                    .AnyAsync();
                if (hasRelations)
                {
                    enabledGroupIds.Add(groupId);
                }
            }
            return enabledGroupIds;
        }
        public async Task<List<ReportFieldGroupDataModel>> GetGroups()
        {
            return await _context.ReportFieldGroups.ToListAsync();
        }
        public async Task<List<ReportTypeDataModel>> GetReportTypes()
        {
            return await _context.ReportTypes.ToListAsync();
        }
        public async Task DeleteField(string fieldName, bool setHiddenOnly = true)
        {
            var reportFieldIds = await _context.ReportFields.Where(_ => _.Name == fieldName).Select(_ => _.Id).ToListAsync();

            if (reportFieldIds.Count > 0)
            {

                if (!setHiddenOnly)
                {
                    var reportFieldValuesIds = _context.ReportFieldValues.Where(_ => reportFieldIds.Contains(_.ReportFieldId)).Select(_ => _.Id).ToList();

                    if (reportFieldValuesIds.Count > 0)
                    {
                        _context.RemoveRange(reportFieldValuesIds.Select(_ => new ReportFieldValueDataModel { Id = _ }));
                        await _context.SaveChangesAsync();
                    }
                }

                var reportFieldRelationIds = _context.ReportFieldRelations.Where(_ => reportFieldIds.Contains(_.ReportFieldId)).Select(_ => _.Id).ToList();

                if (reportFieldRelationIds.Count > 0)
                {
                    _context.RemoveRange(reportFieldRelationIds.Select(_ => new ReportFieldRelationDataModel { Id = _ }));
                    await _context.SaveChangesAsync();
                }

                if (!setHiddenOnly)
                {
                    _context.RemoveRange(reportFieldIds.Select(_ => new ReportFieldDataModel { Id = _ }));
                    await _context.SaveChangesAsync();
                }
            }
        }
        public async Task UpdateField(string fieldName, string newFieldName, string validationKey, string description, List<int> groups, List<int> reportTypes)
        {

            var reportFields = await _context.ReportFields
                .Include(_ => _.FieldValues)
                .Where(_ => _.Name == fieldName)
                .ToListAsync();

            if (reportFields.Count == 0)
            {
                return;
            }

            foreach (var reportField in reportFields)
            {
                reportField.Name = newFieldName;
                reportField.ValidationKey = validationKey;
                reportField.Description = description;
            }
            await _context.SaveChangesAsync();

            //groups to remove

            var fieldsToRemoveFromGroups = reportFields
                .Where(_ => _.GroupId.HasValue && !groups.Contains(_.GroupId.Value))
                .ToList();

            var fieldGroupIdsToRemoveFrom = fieldsToRemoveFromGroups.Select(_ => _.GroupId.Value).Distinct().ToList();

            foreach (var groupId in fieldGroupIdsToRemoveFrom)
            {
                var targetFields = fieldsToRemoveFromGroups
                    .Where(_ => _.GroupId == groupId)
                    .ToList();
                var hasRelatedFieldValues = await _context.ReportFieldValues.Where(_ => targetFields.Select(_ => _.Id).Contains(_.ReportFieldId)).AnyAsync();

                var targetRelationsToRemove = await _context.ReportFieldRelations.Where(_ => targetFields.Select(_ => _.Id).Contains(_.ReportFieldId)).ToListAsync();
                _context.RemoveRange(targetRelationsToRemove);
                await _context.SaveChangesAsync();

                //completely remove from fields table if no history data in reportfieldvalues
                if (!hasRelatedFieldValues)
                {
                    _context.RemoveRange(targetFields);
                    await _context.SaveChangesAsync();
                }
            }

            //groups to add

            var reportFieldsToAdd = new List<ReportFieldDataModel>();

            foreach (var groupId in groups)
            {
                var groupTankIds = await _context.ReportFields.Where(_ => _.GroupId == groupId).OrderBy(_ => _.TankId).Select(_ => _.TankId).Distinct().ToListAsync();
                foreach (var groupTankId in groupTankIds)
                {
                    var alreadyExists = await _context.ReportFields.Where(_ => _.Name == fieldName && _.GroupId == groupId && _.TankId == groupTankId).AnyAsync();
                    if (alreadyExists)
                        continue;
                    var reportField = new ReportFieldDataModel
                    {
                        Name = newFieldName,
                        ValidationKey = validationKey,
                        GroupId = groupId,
                        TankId = groupTankId,
                        IsSubgroupMain = false,
                        Description = description
                    };
                    reportFieldsToAdd.Add(reportField);
                }
            }

            if (reportFieldsToAdd.Count > 0)
            {
                _context.ReportFields.AddRange(reportFieldsToAdd);
                await _context.SaveChangesAsync();
            }

            // relations

            reportFields = await _context.ReportFields.Where(_ => _.Name == newFieldName).ToListAsync();

            // field removed from groups, move to performance tab without groups
            if (reportFields.Count == 0 && groups.Count == 0 && reportTypes.Count > 0)
            {
                var reportField = new ReportFieldDataModel
                {
                    Name = newFieldName,
                    ValidationKey = validationKey,
                    GroupId = null,
                    TankId = null,
                    IsSubgroupMain = false,
                    Description = description
                };
                reportFields.Add(reportField);
                _context.ReportFields.AddRange(reportFields);
                await _context.SaveChangesAsync();
            }

            var relations = await _context.ReportFieldRelations
               .Where(_ => reportFields.Select(_ => _.Id).Contains(_.ReportFieldId)).ToListAsync();

            var relationsToRemove = relations.Where(_ => !reportTypes.Contains(_.ReportTypeId)).ToList();

            if (relationsToRemove.Count > 0)
            {
                _context.RemoveRange(relationsToRemove);
                await _context.SaveChangesAsync();
            }

            var targetRelationFields = reportFields
                .Where(_ => !relations.Select(_ => _.ReportFieldId).Contains(_.Id) && (!_.GroupId.HasValue || groups.Contains(_.GroupId.Value)))
                .ToList();

            foreach (var reportType in reportTypes)
            {
                var relationsToAdd = targetRelationFields.Select(_ => new ReportFieldRelationDataModel
                {
                    ReportFieldId = _.Id,
                    ReportTypeId = reportType
                }).ToList();
                if (relationsToAdd.Count > 0)
                {
                    _context.ReportFieldRelations.AddRange(relationsToAdd);
                    await _context.SaveChangesAsync();
                }
            }

        }
        public async Task CreateField(string name, string validationKey, string description, List<int> groups, List<int> reportTypes)
        {
            var reportFields = new List<ReportFieldDataModel>();

            if (groups.Count > 0)
            {
                foreach (var groupId in groups)
                {
                    var group = await _context.ReportFieldGroups.Where(_ => _.Id == groupId).FirstOrDefaultAsync();
                    if (group == null)
                        continue;
                    var groupTankIds = await _context.ReportFields.Where(_ => _.GroupId == groupId).OrderBy(_ => _.TankId).Select(_ => _.TankId).Distinct().ToListAsync();
                    foreach (var groupTankId in groupTankIds)
                    {
                        var reportField = new ReportFieldDataModel
                        {
                            Name = name,
                            ValidationKey = validationKey,
                            GroupId = groupId,
                            TankId = groupTankId,
                            IsSubgroupMain = false,
                            Description = description
                        };
                        reportFields.Add(reportField);
                    }
                }
            }
            else
            {
                var reportField = new ReportFieldDataModel
                {
                    Name = name,
                    ValidationKey = validationKey,
                    GroupId = null,
                    TankId = null,
                    IsSubgroupMain = false,
                    Description = description
                };
                reportFields.Add(reportField);
            }
            _context.ReportFields.AddRange(reportFields);
            await _context.SaveChangesAsync();
            var relations = new List<ReportFieldRelationDataModel>();
            foreach (var reportField in reportFields)
            {
                foreach (var reportTypeId in reportTypes)
                {
                    var relation = new ReportFieldRelationDataModel()
                    {
                        ReportFieldId = reportField.Id,
                        ReportTypeId = reportTypeId
                    };
                    relations.Add(relation);
                }
            }
            if (relations.Count > 0)
            {
                _context.ReportFieldRelations.AddRange(relations);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<EventTypeDataModel> GetEventTypes()
        {
            return _context.EventTypes
                .AsQueryable();
        }
        public async Task<List<EventConditionDataModel>> GetEventConditionsAsync()
        {
            return await _context.EventConditions
                .OrderBy(t => t.Name)
                .ToListAsync();
        }
        public async Task<List<string>> GetCustomEventNames()
        {
            var customEvents = await _context.Events
                .Where(e => !string.IsNullOrEmpty(e.CustomEventName))
                .Select(e => e.CustomEventName)
                .Distinct()
                .ToListAsync();
            customEvents.Sort();
            return customEvents;
        }
        public async Task UpdateEventTypes(List<EventTypeDataModel> eventTypes)
        {
            var data = await _context.EventTypes
                .Include(_ => _.PairedEventType)
                .Where(t => eventTypes.Select(t => t.Id).Contains(t.Id)).ToListAsync();
            foreach (var eventType in data)
            {
                var item = eventTypes.Where(t => t.Id == eventType.Id).First();
                eventType.Name = item.Name;
                eventType.OnePairPerTime = item.OnePairPerTime;
                if (!eventType.PairedConditionChange)
                {
                    eventType.NextConditionId = item.NextConditionId;
                }

                if (eventType.PairedEventType != null && item.PairedEventType != null)
                {
                    eventType.PairedEventType.Name = item.PairedEventType.Name;

                    eventType.PairedEventType.Transit = item.PairedEventType.Transit;
                    if (eventType.PairedConditionChange)
                    {
                        eventType.PairedEventType.NextConditionId = item.NextConditionId;
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
        public async Task CreateEventType(EventTypeDataModel eventType)
        {
            _context.EventTypes.Add(eventType);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateEventTypesConditions(int eventTypeId, List<int> conditionIds)
        {
            var conditionsToRemove = await _context.EventTypesConditions
                .Where(t => t.EventTypeId == eventTypeId && !conditionIds.Contains(t.ConditionId))
                .ToListAsync();
            _context.RemoveRange(conditionsToRemove);

            var currentConditionIds = await _context.EventTypesConditions.Where(c => c.EventTypeId == eventTypeId).Select(c => c.ConditionId).ToListAsync();

            var conditionsToAdd = conditionIds.Where(c => !currentConditionIds.Contains(c)).ToList();

            foreach (var conditionId in conditionsToAdd)
            {
                var condition = new EventTypesConditionsDataModel()
                {
                    ConditionId = conditionId,
                    EventTypeId = eventTypeId
                };
                _context.EventTypesConditions.Add(condition);
            }

            await _context.SaveChangesAsync();

        }
        public async Task UpdateEventTypePrerequisites(int eventTypeId, List<EventTypePrerequisiteDataModel> prerequisites)
        {
            var prerequisiteToRemove = await _context.EventTypePrerequisites
                .Where(t => t.EventTypeId == eventTypeId && !prerequisites.Select(p => p.AvailableAfterEventTypeId).Contains(t.AvailableAfterEventTypeId))
                .ToListAsync();
            _context.RemoveRange(prerequisiteToRemove);

            foreach (var prerequisite in prerequisites)
            {
                var item = await _context.EventTypePrerequisites.Where(p => p.EventTypeId == prerequisite.EventTypeId && p.AvailableAfterEventTypeId == prerequisite.AvailableAfterEventTypeId).IgnoreQueryFilters().FirstOrDefaultAsync();
                if (item == null)
                {
                    item = new EventTypePrerequisiteDataModel();
                }
                item.EventTypeId = prerequisite.EventTypeId == 0 ? eventTypeId : prerequisite.EventTypeId;
                item.AvailableAfterEventTypeId = prerequisite.AvailableAfterEventTypeId;
                item.Completed = prerequisite.Completed;
                item.Override = prerequisite.Override;
                item.Required = prerequisite.Required;
                item.RequiredForRepetition = prerequisite.RequiredForRepetition;
                item.IsDeleted = false;
                if (item.Id == 0)
                {
                    _context.EventTypePrerequisites.Add(item);
                }
            }

            await _context.SaveChangesAsync();



        }

        public IQueryable<User> GetVessels()
        {
            var query = _context.Users
                .OrderBy(u => u.UserName);
            return query;
        }

        public IQueryable<FuelTypeDataModel> GetFuelTypes()
        {
            var query = _context.FuelTypes
                .OrderBy(u => u.Name);
            return query;
        }


        public IQueryable<TankUserSpecsDataModel> GetUserTank(int userTankId)
        {
            var query = _context.TankUserSpecs
                .Include(t => t.Tank)
                .Where(t => t.Id == userTankId)
                ;

            return query;
        }

        public IQueryable<ReportFieldValueDataModel> GetTankReportFieldValues(int tankId)
        {
            var query = _context.ReportFieldValues
                .Where(rfv => rfv.ReportField.TankId == tankId && rfv.IsDeleted == false && rfv.Report.IsDeleted == false && rfv.Report.Event.IsDeleted == false)
                ;

            return query;
        }

        public async Task DeleteTank(int tankId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var reportFields = await _context.ReportFields.Where(rf => rf.TankId == tankId).ToListAsync();

                var reportFieldIds = reportFields.Select(rf => rf.Id).ToList();

                var relations = await _context.ReportFieldRelations.Where(r => reportFieldIds.Contains(r.ReportFieldId)).ToListAsync();

                var userTank = await _context.TankUserSpecs.Where(t => t.Tank.Id == tankId).FirstOrDefaultAsync();

                var tank = await _context.Tanks.Where(t => t.Id == tankId).FirstOrDefaultAsync();

                _context.ReportFieldRelations.RemoveRange(relations);
                _context.ReportFields.RemoveRange(reportFields);
                _context.TankUserSpecs.Remove(userTank);
                _context.Tanks.Remove(tank);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public IQueryable<TankUserSpecsDataModel> GetTankQuery(string userId = null)
        {
            var query = _context.TankUserSpecs
                .Include(t => t.User)
                .Include(t => t.Tank)
                .ThenInclude(t => t.FuelType)
                .AsQueryable();

            if (userId != null)
            {
                query = query.Where(t => t.UserId == userId);
            }
            return query;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task CreateFieldContext(int userTankId)
        {

            var targetTank = await _context.TankUserSpecs
                .AsNoTracking()
                .Include(t => t.Tank)
                .Where(t => t.Id == userTankId).FirstAsync();

            var tankToCopyFrom = await _context
                .TankUserSpecs
                .Include(t => t.Tank)
                .ThenInclude(t => t.ReportFields)
                .AsNoTracking()
                .Where(t => t.IsActive == true && t.Tank.FuelTypeId == targetTank.Tank.FuelTypeId && t.Tank.ReportFields.Any())
                .Select(t => t.Tank)
                .FirstAsync()
                ;

            var fieldsToAdd = new List<ReportFieldDataModel>();

            foreach (var field in tankToCopyFrom.ReportFields)
            {
                var newField = new ReportFieldDataModel()
                {
                    Name = field.Name,
                    GroupId = field.GroupId,
                    IsSubgroupMain = field.IsSubgroupMain,
                    TankId = targetTank.Tank.Id,
                    ValidationKey = field.ValidationKey,
                    Description = field.Description,
                    DateModified = DateTime.Now,
                    BusinessId = Guid.NewGuid(),
                };
                fieldsToAdd.Add(newField);
            }

            _context.ReportFields.AddRange(fieldsToAdd);
            _context.SaveChanges();

            var relationsToAdd = new List<ReportFieldRelationDataModel>();

            var rels = await _context.ReportFieldRelations
                .AsNoTracking()
                .Include(r => r.ReportField)
                .Where(r => r.ReportField.TankId == tankToCopyFrom.Id).ToListAsync();

            foreach (var rel in rels)
            {
                var targetReportFieldId = await _context.ReportFields
                    .Where(rf => rf.GroupId == rel.ReportField.GroupId && rf.ValidationKey == rel.ReportField.ValidationKey && rf.TankId == targetTank.Tank.Id)
                    .Select(rf => rf.Id)
                    .SingleAsync();

                var newRel = new ReportFieldRelationDataModel()
                {
                    ReportFieldId = targetReportFieldId,
                    ReportTypeId = rel.ReportTypeId,
                    DateModified = DateTime.Now,
                    BusinessId = Guid.NewGuid()
                };
                relationsToAdd.Add(newRel);
            }
            _context.ReportFieldRelations.AddRange(relationsToAdd);
            _context.SaveChanges();
        }

        public async Task CreateTank(TankUserSpecsDataModel tank)
        {
            _context.TankUserSpecs.Add(tank);
            await _context.SaveChangesAsync();
        }

        public async Task processCorrectionFactors2()
        {
            var userIds = await _context.Users.Select(u => u.Id).ToListAsync();

            var excludedEventTypes = new List<int>()
            {
                1111,
                1112,
                60
            };

            foreach (var userId in userIds)
            {
                var query = _context.Reports
                    .Include(r => r.FieldValues)
                    .ThenInclude(fv => fv.ReportField)
                    .ThenInclude(rf => rf.Group)
                    .Where(r => r.Event.UserId == userId && !excludedEventTypes.Contains(r.Event.EventTypeId))
                    .AsQueryable();
                query = query.Where(r => r.Event.Timestamp >= DateTime.UtcNow.AddDays(-30));
                var reports = await query
                    .OrderByDescending(r => r.Event.Timestamp)
                    .ThenByDescending(r => r.Event.Id)
                    .ToListAsync();
                foreach (var report in reports)
                {
                    var groupIds = report.FieldValues.Where(fv => fv.ReportField.Group != null).Select(fv => fv.ReportField.Group.BusinessId).Distinct().ToList();

                    foreach (var groupId in groupIds)
                    {
                        var tankIds = report.FieldValues.Where(fv => fv.ReportField.Group != null && fv.ReportField.Group.BusinessId == groupId && fv.ReportField.TankId != null).Select(fv => fv.ReportField.TankId).Distinct().ToList();

                        foreach (var tankId in tankIds)
                        {
                            var targetFieldValues = report.FieldValues.Where(fv => fv.ReportField.Group != null && fv.ReportField.Group.BusinessId == groupId && fv.ReportField.TankId == tankId).ToList();

                            var density = targetFieldValues.First(_ => _.ReportField.ValidationKey == "density");
                            var temp = targetFieldValues.First(_ => _.ReportField.ValidationKey == "tankTemperature");
                            var volume = targetFieldValues.First(_ => _.ReportField.ValidationKey == "volume");
                            var weight = targetFieldValues.First(_ => _.ReportField.ValidationKey == "weight");

                            var wcf = targetFieldValues.First(_ => _.ReportField.ValidationKey == "wcf");
                            var gsv = targetFieldValues.First(_ => _.ReportField.ValidationKey == "gsv");
                            var vcf = targetFieldValues.First(_ => _.ReportField.ValidationKey == "vcf");

                            if (weight.ReportField.GroupId == 7 || weight.ReportField.GroupId == 8)
                            {
                                continue;
                            }

                            var correctionFactors = ReportType.BunkerGroups.Contains(groupId) ? BunkeringTools.GetVolumeCorrectionFactor(density.Value, temp.Value, weight.Value) : BunkeringTools.GetWeightCorrectionFactor(density.Value, temp.Value, volume.Value);

                            var formattedValue = BunkeringTools.GetDouble(weight.Value).ToString("F3");

                            var diff = Math.Abs(BunkeringTools.GetDouble(formattedValue) - BunkeringTools.GetDouble(correctionFactors.Weight.ToString("F3"))).ToString("F3");

                            if (formattedValue != correctionFactors.Weight.ToString("F3") && diff != "0.001" && BunkeringTools.GetDouble(volume.Value) != 0)
                            {
                                _logger.LogError($"Weight correction factor mismatch for report {weight.ReportId} tank {tankId} in group {weight.ReportField.GroupId}. Diff: {diff}, Value:{formattedValue}, Expected: {correctionFactors.Weight.ToString("F3")}, Volume: {correctionFactors.Volume.ToString("F3")}, VCF: {correctionFactors.VolumeCorrectionFactor.ToString("F4")}, WCF: {correctionFactors.WeightCorrectionFactor.ToString("F4")}, GSV: {correctionFactors.GrossStandardVolume.ToString()}.");
                                //wcf.Value = correctionFactors.WeightCorrectionFactor.ToString("F4");
                                //vcf.Value = correctionFactors.VolumeCorrectionFactor.ToString("F4");
                                //gsv.Value = correctionFactors.GrossStandardVolume.ToString("F4");
                                //weight.Value = correctionFactors.Weight.ToString("F3");
                            }
                        }
                    }
                }
                //await _context.SaveChangesAsync();
            }
        }



    }
}
