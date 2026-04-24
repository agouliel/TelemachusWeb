using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Telemachus.Business.Models.Events;
using Telemachus.Business.Models.Reports;

namespace Telemachus.Business.Interfaces.Reports
{
    public interface IReportService
    {
        Task<List<ReportFieldBusinessModel>> GetReportFieldsAsync(int eventId, string userId);
        Task<int> CreateReportAsync(int eventId, List<ReportFieldValueBusinessModel> fields, ReportingPropsBusinessModel props = null);
        Task UpdateReportAsync(int reportId, List<ReportFieldValueBusinessModel> fields, ReportingPropsBusinessModel props = null);
        Task<ReportBusinessModel> GetReportAsync(int reportId);
        Task<List<ReportBusinessModel>> GetReportHistoryAsync(string userId, string conditionId, int page = 1, int pageSize = 10, int? targetEventId = null);
        Task<int> GetReportHistoryCountAsync(string userId, string conditionId, int? targetEventId = null);
        Task<int?> GetReportIdFromEventId(int eventId);
        Task<ReportPerformanceBusinessModel> GetPerformanceValuesAsync(int eventId, string userId);
        Task<ReportBusinessModel> GetRelatedReportAsync(int eventId);
        Task<ReportBusinessModel> GetRelatedBunkeringReportAsync(int eventId);
        Task<FileInfo> DownloadReportAsync(int reportId, string name, string rank);
        Task<DateTimeOffset?> GetEventTimeStampFromReportId(int reportId);
        Task<DateTimeOffset?> GetEventTimeStampFromEventId(int eventId);
        Task<ReportBusinessModel> GetRelatedBunkeringReport(int fieldValueId, string userId);
        Task<EventAttachmentBusinessModel> UploadDocument(int bunkeringId, string documentCode, List<IFormFile> files, string userId);
        Task DeleteAttachment(int attachmentId);
        Task<EventAttachmentBusinessModel> GetAttachment(int attachmentId);
        Task<List<FileInfo>> GetAttachments(int bunkeringId);
        Task<List<ReportFieldValueBusinessModel>> GetTransferDetailsAsync(string userId);
        Task<bool> isBunkeringPlan(int value);
        Task<int?> GetEventIdFromReportId(int reportId);
        Task UpdateBunkeringSupplier(string userId, int bunkeringId, string supplier);
        Task UpdateBunkeringNamedValue(string userId, int bunkeringId, string namedValue);
        Task<FileInfo> GetWaterConsumptions(DateTime from, DateTime to);
        Task<List<BunkeringDataBusinessModel>> GetBunkeringData(int eventId);
        Task PostProcess();
    }
}
