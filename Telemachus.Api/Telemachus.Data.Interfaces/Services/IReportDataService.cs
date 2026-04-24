using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Telemachus.Data.Models.Events;
using Telemachus.Data.Models.Reports;

namespace Telemachus.Data.Interfaces.Services
{
    public interface IReportDataService
    {
        Task<List<ReportFieldDataModel>> GetReportFieldsAsync(int eventTypeId, string userId);
        Task<ReportDataModel> CreateReportAsync(ReportDataModel report);
        Task UpdateReportAsync(ReportDataModel report);
        Task<ReportDataModel> GetReportAsync(int reportId);
        Task<List<ReportDataModel>> GetReportHistoryAsync(string userId, string conditionId, int page = 1, int pageSize = 10, int? targetEventId = null);
        Task<int> GetReportHistoryCountAsync(string userId, string conditionId, int? targetEventId = null);
        Task<int?> GetReportIdFromEventId(int eventId);
        Task<ReportDataModel> GetRelatedReportAsync(int eventId);
        Task<ReportDataModel> GetRelatedBunkeringReportAsync(int eventId);
        Task<DateTimeOffset?> GetEventTimeStampFromReportId(int reportId);
        Task<DateTimeOffset?> GetEventTimeStampFromEventId(int eventId);
        Task<ReportDataModel> GetRelatedBunkeringReport(int fieldValueId, string userId);
        Task<EventAttachmentDataModel> UploadDocument(int fieldValueId, string documentCode, List<IFormFile> files, string userId);
        Task DeleteAttachment(int attachmentId);
        Task<EventAttachmentDataModel> GetAttachment(int attachmentId);
        Task<List<FileInfo>> GetAttachments(int bunkeringId);
        Task<List<ReportFieldValueDataModel>> GetTransferDetailsAsync(string userId);
        Task<bool> isBunkeringPlan(int eventId);
        Task<int?> GetEventIdFromReportId(int reportId);
        Task UpdateBunkeringSupplier(string userId, int bunkeringId, string supplier);
        Task<ReportDataModel> InternalTransfer(ReportDataModel report, int sourceTankId, int targetTankId, double amount);
        Task UpdateBunkeringNamedValue(string userId, int bunkeringId, string namedAmount);
        Task<List<WaterConsumptionViewModel>> GetWaterConsumptions(DateTime from, DateTime to);
        Task<List<BunkeringDataModel>> GetBunkeringData(int eventId);
        Task<List<int>> GetVesselReportIds(string userId, DateTime? from = null, DateTime? to = null, int? offset = null);
        Task<ReportCustomDataModel> ParseReport(int reportId);
        Task PostProcess();
        Task<List<ReportContextDataModel>> GenerateReportContext(ReportDataModel report);
    }
}
