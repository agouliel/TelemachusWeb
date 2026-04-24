using System.Collections.Generic;
using System.Threading.Tasks;

using Telemachus.Business.Models.Events.Design;
using Telemachus.Business.Models.Reports.Design;

namespace Telemachus.Business.Interfaces.Reports.Design
{
    public interface IReportDesignService
    {
        Task<FieldDesignViewModel> GetReportFieldsAsync();
        Task<bool> CreateField(FieldDesignModel field);
        Task<bool> UpdateField(string fieldName, FieldDesignModel field);
        Task<bool> DeleteField(string fieldName, bool setHiddenOnly = true);
        Task<EventDesignViewModel> GetEventTypesAsync();
        Task UpdateEventTypes(List<EventTypeBusinessModel> eventTypes);
        Task CreateEventType(EventTypeBusinessModel eventType);
        Task ProcessCorrectionFactors2();
        Task<List<TankViewModel>> GetTanks(string userId);
        Task<TankDataViewModel> GetTankData();
        Task UpdateTanks(List<TankViewModel> tanks);
        Task CreateTank(TankViewModel tank);
        Task ArchiveTank(int userTankId);
        Task DeleteTank(int userTankId);
    }
}
