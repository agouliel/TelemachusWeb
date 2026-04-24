using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Enums;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using Telemachus.Data.Interfaces.Services;
using Telemachus.Data.Models.Events;
using Telemachus.Data.Models.Reports;
using Telemachus.Data.Services.Interfaces;

namespace Telemachus.Data.Services.Services
{
    public class ReportDataService : IReportDataService
    {
        private readonly IReportRepository _reportRepository;
        private readonly ICargoDataService _cargoService;
        private readonly IEventRepository _eventRepository;

        public ReportDataService(ICargoDataService cargoService, IReportRepository reportRepository, IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
            _reportRepository = reportRepository;
            _cargoService = cargoService;
        }
        public async Task PostProcess()
        {
            await _reportRepository.PostProcess();
        }

        public Task<List<ReportFieldDataModel>> GetReportFieldsAsync(int eventTypeId, string userId)
        {
            return _reportRepository.GetReportFieldsAsync(eventTypeId, userId);
        }

        public async Task<List<ReportContextDataModel>> GenerateReportContext(ReportDataModel report)
        {
            var reportContext = await _reportRepository.GenerateReportContext(report);
            return reportContext;
        }

        public async Task<ReportDataModel> CreateReportAsync(ReportDataModel report)
        {
            return await _reportRepository.CreateReportAsync(report);
        }
        public async Task<ReportDataModel> InternalTransfer(ReportDataModel report, int sourceTankId, int targetTankId, double amount)
        {
            return await _reportRepository.InternalTransfer(report, sourceTankId, targetTankId, amount);
        }

        public async Task<ReportDataModel> GetRelatedReportAsync(int eventId)
        {
            return await _reportRepository.GetRelatedReportAsync(eventId);
        }
        public async Task<ReportDataModel> GetRelatedBunkeringReportAsync(int eventId)
        {
            return await _reportRepository.GetRelatedBunkeringReportAsync(eventId);
        }

        public async Task<DateTimeOffset?> GetEventTimeStampFromReportId(int reportId)
        {
            return await _reportRepository.GetEventTimeStampFromReportId(reportId);
        }

        public async Task<DateTimeOffset?> GetEventTimeStampFromEventId(int eventId)
        {
            return await _reportRepository.GetEventTimeStampFromEventId(eventId);
        }


        public Task UpdateReportAsync(ReportDataModel report)
        {
            return _reportRepository.UpdateReportAsync(report);
        }

        public async Task<ReportDataModel> GetReportAsync(int reportId)
        {
            var report = await _reportRepository.GetReportAsync(reportId);
            if (report.Event.Timestamp.HasValue)
            {
                report.Event.Cargoes = await _cargoService.GetCargoStatus(report.Event.UserId, report.Event.Timestamp.Value);
            }
            return report;
        }

        public async Task<List<ReportDataModel>> GetReportHistoryAsync(string userId, string conditionId, int page = 1, int pageSize = 10, int? targetEventId = null)
        {
            var reports = await _reportRepository.GetReportHistoryAsync(userId, conditionId, page, pageSize, targetEventId);
            foreach (var report in reports)
            {
                if (report.Event.Timestamp.HasValue)
                {
                    report.Event.Cargoes = await _cargoService.GetCargoStatus(userId, report.Event.Timestamp.Value);
                }
            }
            return reports;
        }

        public Task<int> GetReportHistoryCountAsync(string userId, string conditionId, int? targetEventId = null)
        {
            return _reportRepository.GetReportHistoryCountAsync(userId, conditionId, targetEventId);
        }

        public Task<int?> GetReportIdFromEventId(int eventId)
        {
            return _reportRepository.GetReportIdFromEventId(eventId);
        }

        public async Task<ReportDataModel> GetRelatedBunkeringReport(int fieldValueId, string userId)
        {
            return await _reportRepository.GetRelatedBunkeringReport(fieldValueId, userId);
        }

        public async Task<EventAttachmentDataModel> UploadDocument(int bunkeringId, string documentCode, List<IFormFile> files, string userId)
        {
            return await _reportRepository.UploadDocument(bunkeringId, documentCode, files, userId);
        }
        public async Task DeleteAttachment(int attachmentId)
        {
            await _reportRepository.DeleteAttachment(attachmentId);
        }
        public async Task<EventAttachmentDataModel> GetAttachment(int attachmentId)
        {
            return await _reportRepository.GetAttachment(attachmentId);
        }
        public async Task<List<FileInfo>> GetAttachments(int bunkeringId)
        {
            return await _reportRepository.GetAttachments(bunkeringId);
        }

        public async Task<List<ReportFieldValueDataModel>> GetTransferDetailsAsync(string userId)
        {
            return await _reportRepository.GetTransferDetailsAsync(userId);
        }

        public async Task<bool> isBunkeringPlan(int eventId)
        {
            return await _reportRepository.isBunkeringPlan(eventId);
        }

        public async Task<int?> GetEventIdFromReportId(int reportId)
        {
            return await _reportRepository.GetEventIdFromReportId(reportId);
        }

        public async Task UpdateBunkeringSupplier(string userId, int bunkeringId, string supplier)
        {
            await _reportRepository.UpdateBunkeringSupplier(userId, bunkeringId, supplier);
        }
        public async Task UpdateBunkeringNamedValue(string userId, int bunkeringId, string namedAmount)
        {
            await _reportRepository.UpdateBunkeringNamedValue(userId, bunkeringId, namedAmount);
        }
        public async Task<List<WaterConsumptionViewModel>> GetWaterConsumptions(DateTime from, DateTime to)
        {
            return await _reportRepository.GetWaterConsumptions(from, to);
        }
        public async Task<List<BunkeringDataModel>> GetBunkeringData(int eventId)
        {
            return await _reportRepository.GetBunkeringData(eventId);
        }
        public async Task<List<int>> GetVesselReportIds(string userId, DateTime? from = null, DateTime? to = null, int? offset = null)
        {
            ///&& r.Event.VoyageId == 5907 
            var query = _reportRepository.GetVesselReportQuery()
                .Where(r => r.Event.UserId == userId && r.Event.Timestamp.HasValue && !Enums.EventType.BunkeringPlanGroup.Contains(r.Event.EventType.BusinessId));

            if (offset.HasValue)
            {
                var timestamp = await _reportRepository.GetVesselReportQuery().Where(r => r.Event.Id == offset).Select(r => r.Event.Timestamp.Value).FirstAsync();
                query = query.Where(r => r.Event.Timestamp >= timestamp);
            }
            else if (from.HasValue)
            {
                query = query.Where(r => r.Event.Timestamp >= from);
            }

            if (to.HasValue)
            {
                query = query.Where(r => r.Event.Timestamp < to);
            }

            return await query
                .OrderBy(e => e.Event.Timestamp)
                .ThenBy(e => e.Event.Id)
                .Select(e => e.Id)
                .ToListAsync();
        }

        public async Task<ReportCustomDataModel> ParseReport(int reportId)
        {

            var report = await _reportRepository.GetVesselReportQuery()
                .Include(a => a.Event)
                .ThenInclude(a => a.EventType)
                .Include(a => a.Event)
                .ThenInclude(a => a.EventCondition)
                .Include(a => a.Event)
                .ThenInclude(a => a.Port)
                .Include(a => a.Event)
                .ThenInclude(a => a.BunkeringData)
                .Include(a => a.FieldValues)
                .ThenInclude(a => a.ReportField)
                .ThenInclude(a => a.Group)
                .Include(a => a.FieldValues)
                .ThenInclude(a => a.ReportField)
                .ThenInclude(a => a.Tank)
                .Where(r => r.Id == reportId && r.Event.Timestamp.HasValue && !EventType.BunkeringPlanGroup.Contains(r.Event.EventType.BusinessId))
                .Select(r => new ReportCustomDataModel()
                {
                    UserId = r.Event.UserId,
                    EventId = r.EventId,
                    Timestamp = r.Event.Timestamp.Value,
                    StatusId = r.Event.StatusId,
                    EventTypeBusinessId = r.Event.EventType.BusinessId,
                    EventTypeName = r.Event.EventType.Name,
                    ReportTypeId = r.Event.EventType.ReportTypeId,
                    EventConditionName = r.Event.EventCondition.Name,
                    PortName = r.Event.Port != null ? r.Event.Port.Name : null,
                    ReportId = r.Id,
                    PortBusinessId = r.Event.Port != null ? r.Event.Port.BusinessId : null,
                    PortIsEuInt = r.Event.Port != null ? r.Event.Port.IsEuInt : null,
                    Lat = r.Event.Lat,
                    Lng = r.Event.Lng,
                    BunkeringData = r.Event.BunkeringData,
                    FieldValues = r.FieldValues.ToList(),
                    VoyageId = r.Event.VoyageId
                })
                .FirstOrDefaultAsync();
            ;
            if (report == null)
            {
                return null;
            }
            var cargoes = await _cargoService.GetCargoStatus(report.UserId, report.Timestamp);

            report.Cargoes = cargoes;
            return report;

        }
    }
}
