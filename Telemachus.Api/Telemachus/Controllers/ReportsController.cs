using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

using Enums;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

using Telemachus.Business.Interfaces.Events;
using Telemachus.Business.Interfaces.Reports;
using Telemachus.Business.Models.Events;
using Telemachus.Business.Models.Reports;
using Telemachus.Business.Services.Mappers;
using Telemachus.Business.Services.Voyages;
using Telemachus.Data.Services.Interfaces;
using Telemachus.Helpers;
using Telemachus.Mappers;
using Telemachus.Models;
using Telemachus.Models.Reports;

namespace Telemachus.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IEventService _eventService;
        private readonly IVoyageService _voyageService;
        private readonly ICargoDataService _cargoService;

        public ReportsController(ICargoDataService cargoService, IReportService reportService, IEventService eventservice, IVoyageService voyageService)
        {
            _reportService = reportService;
            _eventService = eventservice;
            _voyageService = voyageService;
            _cargoService = cargoService;
        }

        [HttpGet("{eventId}/fields")]
        public async Task<IActionResult> GetFactReportFields(int eventId)
        {
            var userId = ClaimHelper.GetUserId(User);
            var reportFields = await _reportService.GetReportFieldsAsync(eventId, userId);
            if (reportFields.Count == 0)
            {
                return NotFound();
            }
            var userEvent = await _eventService.GetUserEventAsync(eventId);

            if (userEvent.HasBunkeringData && userEvent.EventType.BelongsToBunkeringPlanGroup)
            {
                if (userEvent.BunkeringData.HasFuelTypeVLSFO)
                {
                    var fieldsToRemove = reportFields.Where(f => !ReportType.HfoGroups.Contains(f.Group.BusinessId)).Select(f => f.FieldId).ToList();
                    reportFields.RemoveAll(f => fieldsToRemove.Contains(f.FieldId));
                }

                if (userEvent.BunkeringData.HasFuelTypeLSMGO)
                {
                    var fieldsToRemove = reportFields.Where(f => !ReportType.MgoGroups.Contains(f.Group.BusinessId)).Select(f => f.FieldId).ToList();
                    reportFields.RemoveAll(f => fieldsToRemove.Contains(f.FieldId));
                }
            }

            var response = new ReportMatchBusinessModel();

            if (userEvent.Timestamp.HasValue)
            {
                userEvent.Cargoes = await _cargoService.GetCargoStatus(userId, userEvent.Timestamp.Value);
            }

            response.Event = userEvent.ToBusinessModel();

            response.ReportFields = reportFields;

            if (userEvent.EventType.BelongsToPerformanceGroup)
            {
                var performance = await _reportService.GetPerformanceValuesAsync(eventId, userId);
                response.Performance = performance;
                var vesselDetails = await _eventService.GetVesselDetails(userId);
                response.Performance.MainEngineMaxPower = vesselDetails?.MainEngineMaxPower;
                response.Performance.PitchPropeller = vesselDetails?.PitchPropeller;
            }

            var relatedReport = await _reportService.GetRelatedReportAsync(eventId);

            response.RelatedReport = relatedReport;

            if (relatedReport != null)
            {
                var bunkeringData = await _reportService.GetBunkeringData(relatedReport.Event.Id);

                response.BunkeringData = bunkeringData;
            }

            return Ok(response);
        }

        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetFactReport(int reportId)
        {
            var report = await _reportService.GetReportAsync(reportId);

            report.RelatedReport = await _reportService.GetRelatedReportAsync(report.Event.Id);

            if (report.RelatedReport != null)
            {
                var bunkeringData = await _reportService.GetBunkeringData(report.RelatedReport.Event.Id);

                report.BunkeringData = bunkeringData;
            }

            return Ok(report);
        }

        [HttpGet("bunkering/{fieldValueId}")]
        public async Task<IActionResult> GetRelatedBunkeringReport(int fieldValueId)
        {
            var userId = ClaimHelper.GetUserId(User);
            var bunkeringReport = await _reportService.GetRelatedBunkeringReport(fieldValueId, userId);
            if (bunkeringReport == null)
            {
                return NoContent();
            }
            return Ok(bunkeringReport);
        }

        [HttpDelete("attachment/{attachmentId}")]
        public async Task<IActionResult> DeleteAttachment(int attachmentId)
        {
            await _reportService.DeleteAttachment(attachmentId);
            return Ok();
        }

        [HttpGet("attachment/{attachmentId}")]
        public async Task<IActionResult> GetAttachment(int attachmentId)
        {
            var attachment = await _reportService.GetAttachment(attachmentId);

            var file = new FileInfo(attachment.FilePath);

            if (!file.Exists)
            {
                return NotFound("File not found");
            }

            var stream = file.OpenRead();

            return File(stream, attachment.MimeType, file.Name);
        }

        [HttpGet("attachment/bunkering/{bunkeringId}")]
        public async Task<IActionResult> GetAttachments(int bunkeringId)
        {
            var attachments = await _reportService.GetAttachments(bunkeringId);
            if (attachments.Count == 0)
            {
                return NotFound();
            }
            var id = Guid.NewGuid().ToString();
            var tempPathA = Path.Combine(Path.GetTempPath(), "Telemachus");
            var tempPathB = Path.Combine(tempPathA, id);
            var archivePath = Path.Combine(tempPathA, $"{id}.zip");
            if (Directory.Exists(tempPathB))
            {
                Directory.Delete(tempPathB, true);
            }
            Directory.CreateDirectory(tempPathB);
            if (System.IO.File.Exists(archivePath))
            {
                System.IO.File.Delete(archivePath);
            }
            attachments.ForEach(file => file.CopyTo(Path.Combine(tempPathB, file.Name)));
            ZipFile.CreateFromDirectory(tempPathB, archivePath);
            System.IO.Directory.Delete(tempPathB, true);
            var provider = new PhysicalFileProvider(tempPathA);
            var fileInfo = provider.GetFileInfo($"{id}.zip");
            var readStream = fileInfo.CreateReadStream();
            HttpContext.Response.OnCompleted(() =>
            {
                if (System.IO.File.Exists(archivePath))
                {
                    System.IO.File.Delete(archivePath);
                }
                return Task.CompletedTask;
            });
            return File(readStream, "application/zip", "download.zip");
        }

        [Middlewares.RequestSizeLimit(20971520)]
        [HttpPost("attachment/{bunkeringId}/{documentCode}")]
        public async Task<IActionResult> UploadDocument(int bunkeringId, string documentCode, List<IFormFile> files)
        {
            var userId = ClaimHelper.GetUserId(User);
            var attachment = await _reportService.UploadDocument(bunkeringId, documentCode, files, userId);
            if (attachment == null)
            {
                return BadRequest();
            }
            return Ok(attachment);
        }

        [HttpGet("{reportId}/download")]
        public async Task<IActionResult> Download(int reportId, [FromQuery] string name, [FromQuery] string rank)
        {
            var fileInfo = await _reportService.DownloadReportAsync(reportId, name, rank);

            var bytes = new byte[0];
            var fileName = "";

            using (FileStream fs = fileInfo.OpenRead())
            {
                using (var memoryStream = new MemoryStream())
                {
                    fs.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
                    fileName = fileInfo.Name;

                }
            }

            fileInfo.Delete();
            return File(bytes, "application/pdf", fileName);
        }

        [HttpPost("{eventId}")]
        public async Task<IActionResult> CreateFactReport(int eventId, [FromBody] ReportCreateViewModel report)
        {
            var fields = report.ToBusinessModel();
            var props = report.ToReportingPropsBusinessModel();
            var reportId = await _reportService.CreateReportAsync(eventId, fields, props);

            return Ok(reportId);
        }

        [HttpPatch("{reportId}")]
        public async Task<IActionResult> UpdateFactReport(int reportId, [FromBody] ReportCreateViewModel report)
        {
            var props = report.ToReportingPropsBusinessModel();
            await _reportService.UpdateReportAsync(reportId, report.ToBusinessModel(), props);

            return NoContent();
        }


        [HttpGet("")]
        public IActionResult Index()
        {
            return NoContent();
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetReportHistory(string conditionId, int? reportId, int? eventId, int page = 1, int pageSize = 10)
        {
            var userId = ClaimHelper.GetUserId(User);
            int? targetEventId = eventId;

            if (reportId.HasValue)
            {
                targetEventId = await _reportService.GetEventIdFromReportId((int)reportId);
            }

            var reports = await _reportService.GetReportHistoryAsync(userId, conditionId, page, pageSize, targetEventId);
            var reportCount = await _reportService.GetReportHistoryCountAsync(userId, conditionId, targetEventId);
            return Ok(new PagedResult<ReportBusinessModel>()
            {
                Items = reports,
                TotalCount = reportCount
            });
        }

        [HttpGet("transfer")]
        public async Task<IActionResult> GetTransferDetailsAsync()
        {
            var userId = ClaimHelper.GetUserId(User);
            var transfer = await _reportService.GetTransferDetailsAsync(userId);
            return Ok(transfer);
        }

        [HttpPatch("bunkering/{bunkeringId}/supplier")]
        public async Task<IActionResult> UpdateBunkeringSupplier(int bunkeringId, [FromBody] BunkeringDataBusinessModel data)
        {
            var userId = ClaimHelper.GetUserId(User);
            try
            {
                await _reportService.UpdateBunkeringSupplier(userId, bunkeringId, data.Supplier);
                return NoContent();
            }

            catch (ArgumentNullException ex)
            {
                return NotFound();
            }
        }
        [HttpPatch("bunkering/{bunkeringId}/namedAmount")]
        public async Task<IActionResult> UpdateBunkeringNamedValue(int bunkeringId, [FromBody] BunkeringDataBusinessModel data)
        {
            var userId = ClaimHelper.GetUserId(User);
            try
            {
                await _reportService.UpdateBunkeringNamedValue(userId, bunkeringId, data.NamedAmount);
                return NoContent();
            }

            catch (ArgumentNullException ex)
            {
                return NotFound();
            }
        }




    }
}
