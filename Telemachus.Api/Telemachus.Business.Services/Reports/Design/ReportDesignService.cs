using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Telemachus.Business.Interfaces.Reports.Design;
using Telemachus.Business.Models.Events.Design;
using Telemachus.Business.Models.Login;
using Telemachus.Business.Models.Reports.Design;
using Telemachus.Business.Services.Mappers;
using Telemachus.Data.Models.Reports;
using Telemachus.Data.Services.Interfaces;

namespace Telemachus.Business.Services.Reports.Design
{
    public class ReportDesignService : IReportDesignService
    {
        private readonly IReportDesignRepository _reportDesignRepository;
        public ReportDesignService(IReportDesignRepository reportDesignRepository)
        {
            _reportDesignRepository = reportDesignRepository;
        }
        public async Task<bool> UpdateField(string fieldName, FieldDesignModel field)
        {
            try
            {
                await _reportDesignRepository.UpdateField(fieldName, field.Name, field.ValidationKey, field.Description, field.Groups, field.ReportTypes);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> CreateField(FieldDesignModel field)
        {
            try
            {
                await _reportDesignRepository.CreateField(field.Name, field.ValidationKey, field.Description, field.Groups, field.ReportTypes);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> DeleteField(string fieldName, bool setHiddenOnly = true)
        {
            try
            {
                await _reportDesignRepository.DeleteField(fieldName, setHiddenOnly);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<FieldDesignViewModel> GetReportFieldsAsync()
        {
            var fieldList = new List<FieldDesignModel>();
            var fieldNames = await _reportDesignRepository.GetReportFieldNamesAsync();
            foreach (var fieldName in fieldNames)
            {
                var reportTypeIds = await _reportDesignRepository.GetReportTypeIds(fieldName);
                var reportField = await _reportDesignRepository.GetField(fieldName);
                var groupIds = await _reportDesignRepository.GetGroupIds(fieldName);
                var field = new FieldDesignModel
                {
                    Name = fieldName,
                    Description = reportField.Description,
                    ValidationKey = reportField.ValidationKey,
                    ReportTypes = reportTypeIds,
                    Groups = groupIds,
                    HasValues = await _reportDesignRepository.HasValues(fieldName),
                    Hidden = await _reportDesignRepository.IsHidden(fieldName)
                };
                fieldList.Add(field);
            }
            var groups = await _reportDesignRepository.GetGroups();
            var reportTypes = await _reportDesignRepository.GetReportTypes();

            var viewModel = new FieldDesignViewModel
            {
                Groups = groups,
                ReportTypes = reportTypes,
                Fields = fieldList,
            };
            return viewModel;
        }

        public async Task<EventDesignViewModel> GetEventTypesAsync()
        {
            var eventTypes = await _reportDesignRepository
                .GetEventTypes()
                .Include(et => et.PairedEventType)
                .ThenInclude(et => et.EventTypesConditions)
                .Include(et => et.PairedEventType)
                .ThenInclude(et => et.Prerequisites)
                .Include(et => et.Prerequisites)
                .OrderBy(t => t.Name)
                .Select(t => new EventTypeBusinessModel()
                {
                    EventTypeId = t.Id,
                    Name = t.Name,
                    OnePairPerTime = t.OnePairPerTime,
                    PairedEventTypeId = t.PairedEventTypeId,
                    PairedEventType = t.PairedEventType != null ? new EventTypeBusinessModel()
                    {
                        EventTypeId = t.PairedEventType.Id,
                        Name = t.PairedEventType.Name,
                        NextConditionId = t.PairedEventType.NextConditionId,
                        Transit = t.PairedEventType.Transit,
                        ReportTypeId = t.PairedEventType.ReportTypeId,
                    } : null,
                    NextConditionId = t.NextConditionId,
                    Transit = t.Transit,
                    ReportTypeId = t.ReportTypeId,
                    EventTypesConditions = t.EventTypesConditions != null ? t.EventTypesConditions.Select(c => c.ConditionId).ToList() : null,
                    Prerequisites = t.Prerequisites != null ? t.Prerequisites.ToBusinessModel() : null
                })
                .ToListAsync();
            var conditions = await _reportDesignRepository.GetEventConditionsAsync();
            var customEvents = await _reportDesignRepository.GetCustomEventNames();
            var viewModel = new EventDesignViewModel() { EventTypes = eventTypes, Conditions = conditions, CustomEvents = customEvents };
            return viewModel;
        }
        public async Task UpdateEventTypes(List<EventTypeBusinessModel> eventTypes)
        {
            await _reportDesignRepository.UpdateEventTypes(eventTypes.ToDataModel());

            foreach (var eventType in eventTypes)
            {
                if (eventType.EventTypeId == null)
                {
                    await CreateEventType(eventType);
                    continue;
                }
                await _reportDesignRepository.UpdateEventTypesConditions(eventType.EventTypeId.Value, eventType.EventTypesConditions);
                await _reportDesignRepository.UpdateEventTypePrerequisites(eventType.EventTypeId.Value, eventType.Prerequisites.ToDataModel());
            }

        }
        public async Task CreateEventType(EventTypeBusinessModel eventType)
        {
            var item = eventType.ToDataModel();
            await _reportDesignRepository.CreateEventType(item);
            await _reportDesignRepository.UpdateEventTypesConditions(item.Id, eventType.EventTypesConditions);
            await _reportDesignRepository.UpdateEventTypePrerequisites(item.Id, eventType.Prerequisites.ToDataModel());
        }

        public async Task ProcessCorrectionFactors2()
        {
            await _reportDesignRepository.processCorrectionFactors2();
        }
        public async Task<TankDataViewModel> GetTankData()
        {
            var vessels = await _reportDesignRepository.GetVessels().Select(
                  v => new UserBusinessModel()
                  {
                      Id = v.Id,
                      Name = v.UserName.ToUpper(),
                      Prefix = v.Prefix
                  }
                ).ToListAsync();
            var fuelTypes = await _reportDesignRepository.GetFuelTypes().ToListAsync();
            var model = new TankDataViewModel()
            {
                Vessels = vessels,
                FuelTypes = fuelTypes
            };
            return model;
        }
        public async Task<List<TankViewModel>> GetTanks(string userId)
        {
            var tanks = await _reportDesignRepository.GetTankQuery(userId)
                .OrderBy(t => t.User.UserName)
                .ThenBy(t => t.DateArchived)
                .ThenBy(t => t.Tank.FuelTypeId)
                .ThenBy(t => t.DisplayOrder)
                .Select(t => t.ToBusinessModel())
                .ToListAsync();
            return tanks;
        }
        public async Task UpdateTanks(List<TankViewModel> tanks)
        {
            var userId = tanks.Select(t => t.VesselId).First();

            var dbTanks = await _reportDesignRepository.GetTankQuery()
                .Where(t => t.UserId == userId && t.IsActive == true).ToListAsync();
            foreach (var dbTank in dbTanks)
            {
                var tank = tanks.FirstOrDefault(t => t.TankId == dbTank.Id);
                if (tank == null)
                {
                    continue;
                }
                dbTank.TankName = tank.TankName;
                dbTank.MaxCapacity = tank.MaxCapacity.ToString();
                dbTank.DisplayOrder = tank.DisplayOrder;
                dbTank.Tank.TankType = tank.TankTypeId;
                //dbTank.Tank.FuelTypeId = tank.FuelTypeId;

            }

            var groupedTanks = dbTanks.GroupBy(t => t.Tank.FuelTypeId).ToList();


            foreach (var group in groupedTanks)
            {
                int order = 10;
                int step = 10;

                foreach (var dbTank in group.OrderBy(t => t.DisplayOrder))
                {
                    dbTank.DisplayOrder = order;
                    order += step;
                }
            }
            await _reportDesignRepository.SaveChangesAsync();

        }
        public async Task ArchiveTank(int userTankId)
        {
            var tank = await _reportDesignRepository.GetTankQuery()
                .Where(t => t.Id == userTankId)
                .FirstAsync();
            if (tank.IsActive == true)
            {
                tank.DateArchived = DateTime.UtcNow;
                tank.IsActive = false;
                await _reportDesignRepository.SaveChangesAsync();
            }
        }
        public async Task DeleteTank(int userTankId)
        {
            var userTank = await _reportDesignRepository.GetUserTank(userTankId).FirstAsync();

            var hasFieldValues = await _reportDesignRepository.GetTankReportFieldValues(userTank.Tank.Id).ToListAsync();

            if (hasFieldValues.Count > 0)
            {
                throw new Exception("");
            }

            await _reportDesignRepository.DeleteTank(userTank.Tank.Id);

        }
        public async Task CreateTank(TankViewModel tank)
        {
            await using var transaction = await _reportDesignRepository.BeginTransactionAsync();

            try
            {
                var dbTank = new TankUserSpecsDataModel()
                {
                    TankName = tank.TankName,
                    UserId = tank.VesselId,
                    MaxCapacity = tank.MaxCapacity.ToString(),
                    IsActive = true,
                    DateArchived = null,
                    DisplayOrder = tank.DisplayOrder,
                    Tank = new TankDataModel()
                    {
                        Name = tank.TankName,
                        TankType = tank.TankTypeId,
                        FuelTypeId = tank.FuelTypeId
                    }
                };

                await _reportDesignRepository.CreateTank(dbTank);

                await _reportDesignRepository.CreateFieldContext(dbTank.Id);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


    }
}
