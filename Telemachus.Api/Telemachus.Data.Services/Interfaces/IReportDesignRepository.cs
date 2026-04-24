using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Storage;

using Telemachus.Data.Models.Authentication;
using Telemachus.Data.Models.Events;
using Telemachus.Data.Models.Reports;

namespace Telemachus.Data.Services.Interfaces
{
    public interface IReportDesignRepository
    {
        Task<List<string>> GetReportFieldNamesAsync();
        Task<ReportFieldDataModel> GetField(string fieldName);
        Task<List<int>> GetReportTypeIds(string fieldName);
        Task<List<int>> GetGroupIds(string fieldName);
        Task<List<ReportFieldGroupDataModel>> GetGroups();
        Task<List<ReportTypeDataModel>> GetReportTypes();
        Task CreateField(string name, string validationKey, string description, List<int> groups, List<int> reportTypes);
        Task DeleteField(string fieldName, bool setHiddenOnly);
        Task UpdateField(string fieldName, string newFieldName, string validationKey, string description, List<int> groups, List<int> reportTypes);
        Task<bool> HasValues(string fieldName);
        Task<bool> IsHidden(string fieldName);
        IQueryable<EventTypeDataModel> GetEventTypes();
        Task<List<EventConditionDataModel>> GetEventConditionsAsync();
        Task UpdateEventTypes(List<EventTypeDataModel> eventTypes);
        Task CreateEventType(EventTypeDataModel eventType);
        Task UpdateEventTypesConditions(int eventTypeId, List<int> conditionIds);
        Task UpdateEventTypePrerequisites(int eventTypeId, List<EventTypePrerequisiteDataModel> prerequisites);
        Task<List<string>> GetCustomEventNames();
        Task processCorrectionFactors2();
        IQueryable<TankUserSpecsDataModel> GetTankQuery(string userId = null);
        IQueryable<FuelTypeDataModel> GetFuelTypes();
        IQueryable<User> GetVessels();
        Task SaveChangesAsync();
        Task CreateTank(TankUserSpecsDataModel tank);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CreateFieldContext(int tankId);
        IQueryable<TankUserSpecsDataModel> GetUserTank(int userTankId);
        IQueryable<ReportFieldValueDataModel> GetTankReportFieldValues(int tankId);
        Task DeleteTank(int tankId);
    }
}
