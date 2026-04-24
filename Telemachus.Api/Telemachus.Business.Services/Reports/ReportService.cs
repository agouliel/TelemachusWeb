using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Enums;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Telemachus.Business.Interfaces.Cargo;
using Telemachus.Business.Interfaces.Reports;
using Telemachus.Business.Models.Events;
using Telemachus.Business.Models.Reports;
using Telemachus.Business.Services.Mappers;
using Telemachus.Business.Services.Voyages;
using Telemachus.Data.Interfaces.Services;
using Telemachus.Data.Models;
using Telemachus.Data.Models.Reports;

namespace Telemachus.Business.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly IReportDataService _reportDataService;
        private readonly IVoyageService _voyageService;
        private readonly IEventDataService _eventDataService;
        private readonly IWebHostEnvironment _env;
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ILogger<IReportService> _logger;

        private readonly ICargoBusinessService _cargoService;

        public ReportService(ICargoBusinessService cargoService, ILogger<IReportService> logger, ITempDataProvider tempDataProvider, IActionContextAccessor actionContextAccessor, IEventDataService eventDataService, IVoyageService voyageService, IRazorViewEngine viewEngine, IReportDataService reportDataService, IWebHostEnvironment env)
        {
            _reportDataService = reportDataService;
            _voyageService = voyageService;
            _eventDataService = eventDataService;
            _env = env;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _actionContextAccessor = actionContextAccessor;
            _logger = logger;
            _cargoService = cargoService;
        }

        private async Task<ReportExportViewModel> GetReportExportViewModel(int reportId, string name, string rank)
        {
            var report = await GetReportAsync(reportId);
            return await GetReportExportViewModel2(report, name, rank);
        }

        private async Task<ReportExportViewModel> GetReportExportViewModel2(ReportBusinessModel report, string name = "", string rank = "")
        {
            report.RelatedReport = await GetRelatedReportAsync(report.Event.Id);

            var vessel = await _eventDataService.GetVesselDetails(report.Event.UserId);

            var model = new ReportExportViewModel()
            {
                Name = name,
                Rank = rank,
                VesselName = vessel.UserName,
                OperatorName = vessel.Operator == "GRACE" ? "GRACE MANAGEMENT S.A." : "IONIA MANAGEMENT S.A.",
                Timestamp = report.Event.Timestamp.Value.DateTime,
                Remark = report.Event.EventTypeName,
                Weather = report.ReportFields.FirstOrDefault(_ => _.ValidationKey == "observedWeather")?.Value,
                SeaCondition = report.ReportFields.FirstOrDefault(_ => _.ValidationKey == "seaState")?.Value,
                DraftAft = getDouble(report.ReportFields.FirstOrDefault(_ => _.ValidationKey == "draftAft")?.Value),
                DraftFwd = getDouble(report.ReportFields.FirstOrDefault(_ => _.ValidationKey == "draftFwd")?.Value),
                VesselTrim = getDouble(report.ReportFields.FirstOrDefault(_ => _.ValidationKey == "vesselCurrentTrim")?.Value),
                VesselCurrentList = getDouble(report.ReportFields.FirstOrDefault(_ => _.ValidationKey == "vesselCurrentList")?.Value)
            };

            var tanks = report.ReportFields
                .Where(f =>
                    f.Group != null &&
                    (new List<Guid?> { ReportType.RobHfoPoolGroup, ReportType.RobMgoPoolGroup })
                        .Contains(f.Group.BusinessId))
                .GroupBy(f => new
                {
                    f.Group.Id,
                    f.Group.BusinessId,
                    f.TankDisplayOrder,
                    f.TankName
                })
                .Select(g => new ReportExportTankViewModel
                {
                    GroupBusinessId = g.Key.BusinessId,
                    GroupId = g.Key.Id,
                    DisplayOrder = g.Key.TankDisplayOrder,
                    TankName = g.Key.TankName,

                    BdnNumber = g.FirstOrDefault(x => x.ValidationKey == "bdn")?.Value,
                    SulphurContent = getDouble(g.FirstOrDefault(x => x.ValidationKey == "sulphurContent")?.Value),
                    Sounding = getDouble(g.FirstOrDefault(x => x.ValidationKey == "sounding")?.Value),
                    TapeReading = null,
                    BobReading = null,
                    Ullage = null,
                    TankTemp = getDouble(g.FirstOrDefault(x => x.ValidationKey == "tankTemperature")?.Value),
                    Volume = getDouble(g.FirstOrDefault(x => x.ValidationKey == "volume")?.Value),
                    Density = getDouble(g.FirstOrDefault(x => x.ValidationKey == "density")?.Value),
                    Vcf = getDouble(g.FirstOrDefault(x => x.ValidationKey == "vcf")?.Value),
                    Gsv = getDouble(g.FirstOrDefault(x => x.ValidationKey == "gsv")?.Value),
                    Wcf = getDouble(g.FirstOrDefault(x => x.ValidationKey == "wcf")?.Value),
                    Weight = getDouble(g.FirstOrDefault(x => x.ValidationKey == "weight")?.Value),
                    Viscosity = getDouble(g.FirstOrDefault(x => x.ValidationKey == "kinematicViscosity")?.Value),

                    IsSettOrServ = g.Any(x => (x.Settling ?? false) || (x.Serving ?? false))
                })
                .OrderBy(x => x.GroupId)
                .ThenBy(x => x.DisplayOrder)
                .ToList();


            foreach (var tank in tanks)
            {
                if (tank.IsSettOrServ || tank.TankName.ToUpper().Contains("OVFL"))
                {
                    tank.TapeReading = tank.Sounding;
                }
                else
                {
                    tank.Ullage = tank.Sounding;
                }
            }

            model.HfoPerGradesTov = tanks.Sum(_ => _.GroupBusinessId == ReportType.RobHfoPoolGroup && _.Viscosity > 80 ? _.Volume : 0);
            model.HfoPerGradesWeight = tanks.Sum(_ => _.GroupBusinessId == ReportType.RobHfoPoolGroup && _.Viscosity > 80 ? _.Weight : 0);
            model.LfoPerGradesTov = tanks.Sum(_ => _.GroupBusinessId == ReportType.RobHfoPoolGroup && _.Viscosity <= 80 ? _.Volume : 0);
            model.LfoPerGradesWeight = tanks.Sum(_ => _.GroupBusinessId == ReportType.RobHfoPoolGroup && _.Viscosity <= 80 ? _.Weight : 0); //
            model.MdoMgoPerGradesTov = tanks.Sum(_ => _.GroupBusinessId == ReportType.RobMgoPoolGroup ? _.Volume : 0);
            model.MdoMgoPerGradesWeight = tanks.Sum(_ => _.GroupBusinessId == ReportType.RobMgoPoolGroup ? _.Weight : 0);
            model.TotalFuelPerGradesTov = tanks.Sum(_ => _.Volume);
            model.TotalFuelPerGradesWeight = tanks.Sum(_ => _.Weight);
            model.PerSulphurHighTov = tanks.Sum(_ => _.SulphurContent > 0.5 && _.SulphurContent <= 3.5 ? _.Volume : 0);
            model.PerSulphurHighWeight = tanks.Sum(_ => _.SulphurContent > 0.5 && _.SulphurContent <= 3.5 ? _.Weight : 0);
            model.PerSulphurMidTov = tanks.Sum(_ => _.SulphurContent > 0.1 && _.SulphurContent <= 0.5 ? _.Volume : 0);
            model.PerSulphurMidWeight = tanks.Sum(_ => _.SulphurContent > 0.1 && _.SulphurContent <= 0.5 ? _.Weight : 0);
            model.PerSulphurLowTov = tanks.Sum(_ => _.SulphurContent <= 0.1 ? _.Volume : 0);
            model.PerSulphurLowWeight = tanks.Sum(_ => _.SulphurContent <= 0.1 ? _.Weight : 0);
            model.Tanks = tanks;

            var currentReport = report;
            var targetReports = new List<ReportBusinessModel>();
            int loopCount = 0;

            do
            {
                if (currentReport.RelatedReport?.Event?.EventTypeBusinessId == null)
                {
                    targetReports.Add(currentReport);
                    break;
                }

                targetReports.Add(currentReport);

                if (currentReport.RelatedReport.Event.EventTypeBusinessId == EventType.Noon ||
                    currentReport.RelatedReport.Event.EventTypeBusinessId == EventType.CompleteUnmooring)
                {
                    break;
                }

                var nextEventId = currentReport.RelatedReport.Event.Id;
                currentReport = currentReport.RelatedReport;
                currentReport.RelatedReport = await GetRelatedReportAsync(nextEventId);

                loopCount++;
                if (loopCount >= 10)
                {
                    // fallback for safety
                    targetReports.Clear();
                    targetReports.Add(report);
                    break;
                }

            }
            while (true);

            var isLFO = false; // TODO: to be removed

            foreach (var targetReport in targetReports)
            {

                var perf = targetReport.ReportFields
                    .Where(_ => _.Group == null)
                    .ToList();

                #region to be removed 
                var hfoTanks = targetReport.ReportFields
                    .Where(rf => rf.Group?.BusinessId == ReportType.RobHfoPoolGroup)
                    .GroupBy(t => new { t.Group })
                    .Select(g => new
                    {
                        Weight = getDouble(g.FirstOrDefault(fv => fv.ValidationKey == "weight")?.Value),
                        Viscosity = getDouble(g.FirstOrDefault(fv => fv.ValidationKey == "kinematicViscosity")?.Value),
                    })
                    .Sum(fv => fv.Viscosity <= 80 ? fv.Weight : 0);

                var relatedHfoTanks = targetReport.RelatedReport?.ReportFields
                    .Where(rf => rf.Group?.BusinessId == ReportType.RobHfoPoolGroup)
                    .GroupBy(t => new { t.Group })
                    .Select(g => new
                    {
                        Weight = getDouble(g.FirstOrDefault(fv => fv.ValidationKey == "weight")?.Value),
                        Viscosity = getDouble(g.FirstOrDefault(fv => fv.ValidationKey == "kinematicViscosity")?.Value),
                    })
                    .Sum(fv => fv.Viscosity <= 80 ? fv.Weight : 0);

                if (relatedHfoTanks != null && hfoTanks != null)
                {
                    if (relatedHfoTanks.Value - hfoTanks.Value > 0)
                    {
                        isLFO = true;
                    }
                }
                #endregion

                // HFO CONS

                model.ConsHfoMe += getDouble(perf.FirstOrDefault(_ => _.ValidationKey == "vlsfo_pool_consumption_me")?.Value) ?? 0;

                model.ConsHfoGe += perf.Where(v => (new List<string>() {
                "vlsfo_pool_consumption_dg1",
                "vlsfo_pool_consumption_dg2",
                "vlsfo_pool_consumption_dg3",
                "vlsfo_pool_consumption_dg4"
            })
                    .Contains(v.ValidationKey))
                    .Sum(v => getDouble(v.Value)) ?? 0;

                model.ConsHfoBoiler += perf.Where(v => (new List<string>() {
                "vlsfo_pool_consumption_boiler 1",
                "vlsfo_pool_consumption_boiler 2",
                "vlsfo_pool_consumption_composite boiler"
            })
                    .Contains(v.ValidationKey))
                    .Sum(v => getDouble(v.Value)) ?? 0;

                if (isLFO)
                {
                    model.ConsLfoMe = model.ConsHfoMe;
                    model.ConsHfoMe = 0;
                    model.ConsLfoGe = model.ConsHfoGe;
                    model.ConsHfoGe = 0;
                    model.ConsLfoBoiler = model.ConsHfoBoiler;
                    model.ConsHfoBoiler = 0;
                }

                // MGO CONS

                model.ConsMgoMe += getDouble(perf.FirstOrDefault(_ => _.ValidationKey == "lsmgo_pool_consumption_me")?.Value) ?? 0;

                model.ConsMgoGe += perf.Where(v => (new List<string>() {
                "lsmgo_pool_consumption_dg1",
                "lsmgo_pool_consumption_dg2",
                "lsmgo_pool_consumption_dg3",
                "lsmgo_pool_consumption_dg4"
            })
                    .Contains(v.ValidationKey))
                    .Sum(v => getDouble(v.Value)) ?? 0;

                model.ConsMgoBoiler += perf.Where(v => (new List<string>() {
                "lsmgo_pool_consumption_boiler 1",
                "lsmgo_pool_consumption_boiler 2",
                "lsmgo_pool_consumption_composite boiler"
            })
                    .Contains(v.ValidationKey))
                    .Sum(v => getDouble(v.Value)) ?? 0;

                model.ConsMgoInc += getDouble(perf.FirstOrDefault(_ => _.ValidationKey == "lsmgo_pool_consumption_inc")?.Value) ?? 0;

                model.ConsMgoIgg += getDouble(perf.FirstOrDefault(_ => _.ValidationKey == "lsmgo_pool_consumption_igg")?.Value) ?? 0;

                model.ConsMgoEmCy += getDouble(perf.FirstOrDefault(_ => _.ValidationKey == "lsmgo_pool_consumption_dge")?.Value) ?? 0;

                model.ConsMgoCargoCumm += getDouble(perf.FirstOrDefault(_ => _.ValidationKey == "lsmgo_pool_consumption_cummins")?.Value) ?? 0;

            }

            model.ConsHfoMe = model.ConsHfoMe == 0 ? null : model.ConsHfoMe;
            model.ConsHfoGe = model.ConsHfoGe == 0 ? null : model.ConsHfoGe;
            model.ConsHfoBoiler = model.ConsHfoBoiler == 0 ? null : model.ConsHfoBoiler;

            model.ConsLfoMe = model.ConsLfoMe == 0 ? null : model.ConsLfoMe;
            model.ConsLfoGe = model.ConsLfoGe == 0 ? null : model.ConsLfoGe;
            model.ConsLfoBoiler = model.ConsLfoBoiler == 0 ? null : model.ConsLfoBoiler;
            model.ConsMgoMe = model.ConsMgoMe == 0 ? null : model.ConsMgoMe;
            model.ConsMgoGe = model.ConsMgoGe == 0 ? null : model.ConsMgoGe;
            model.ConsMgoBoiler = model.ConsMgoBoiler == 0 ? null : model.ConsMgoBoiler;
            model.ConsMgoInc = model.ConsMgoInc == 0 ? null : model.ConsMgoInc;
            model.ConsMgoIgg = model.ConsMgoIgg == 0 ? null : model.ConsMgoIgg;
            model.ConsMgoEmCy = model.ConsMgoEmCy == 0 ? null : model.ConsMgoEmCy;
            model.ConsMgoCargoCumm = model.ConsMgoCargoCumm == 0 ? null : model.ConsMgoCargoCumm;

            return model;

        }

        private double? getDouble(string value)
        {
            double? doubleVal = null;
            if (value != null)
            {
                double tempValue;
                if (double.TryParse(value, out tempValue))
                {
                    doubleVal = tempValue;
                }
            }
            return doubleVal;
        }

        private int? getInt(string value)
        {
            int? intVal = null;
            if (value != null)
            {
                int tempValue;
                if (int.TryParse(value, out tempValue))
                {
                    intVal = tempValue;
                }
            }
            return intVal;
        }

        public async Task<FileInfo> DownloadReportAsync(int reportId, string name, string rank)
        {

            var model = await GetReportExportViewModel(reportId, name, rank);

            var viewPath = "~/Views/_Export.cshtml";

            var viewEngineResult = _viewEngine.GetView(null, viewPath, false);

            if (!viewEngineResult.Success)
            {
                throw new InvalidOperationException("Invalid view path!");
            }

            var view = viewEngineResult.View;

            var title = $"MRV FORM - {model.VesselName} - {model.Timestamp.Value.ToString("dd-MM-yyyy_HH-mm")} ({model.Remark})";

            using (var output = new StringWriter())
            {
                var tempData = new TempDataDictionary(
                         _actionContextAccessor.ActionContext.HttpContext,
                         _tempDataProvider);
                var viewContext = new ViewContext(
                    _actionContextAccessor.ActionContext,
                    view,
                    new ViewDataDictionary<ReportExportViewModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    { Model = model },
                    tempData,
                    output,
                    new HtmlHelperOptions());

                await view.RenderAsync(viewContext);

                try
                {
                    var fileInfo = await PDFConverter.PDFConverter.Convert(Path.Combine(_env.ContentRootPath), output.ToString());
                    string newfilePath = Path.Combine(fileInfo.Directory.FullName, $"{title}.pdf");

                    if (File.Exists(newfilePath))
                    {
                        File.Delete(newfilePath);
                    }

                    fileInfo.MoveTo(newfilePath);
                    return fileInfo;
                }
                catch (Exception ex)
                {
                    _logger.LogError("PDF:" + ex.Message);
                    throw ex;
                }
            }
        }
        public async Task<List<ReportFieldBusinessModel>> GetReportFieldsAsync(int eventId, string userId)
        {
            var reportEvent = await _eventDataService.GetUserEventAsync(eventId);
            var fields = await _reportDataService.GetReportFieldsAsync(reportEvent.EventTypeId, userId);

            // Not at sea
            if (reportEvent.EventCondition.BusinessId != Guid.Parse("D450DFE8-A736-4DD2-BCFA-14538522350D"))
            {
                var fieldTypesToRemove = new List<string>()
                {
                    "instructedSpeed",
                    "instructedChartererConsumption"
                };
                var fieldsToRemove = fields.Where(_ => fieldTypesToRemove.Contains(_.ValidationKey)).Select(_ => _.Id).ToList();
                fields.RemoveAll(_ => fieldsToRemove.Contains(_.Id));
            }

            var customFields = fields.Select(a => a.ToBusinessModel()).ToList();
            return customFields;
        }

        public async Task<int> CreateReportAsync(int eventId, List<ReportFieldValueBusinessModel> fields, ReportingPropsBusinessModel props = null)
        {

            //TODO: reportcontext

            await using var t = await _eventDataService.BeginTransactionAsync();

            try
            {

                var fieldValues = fields.Select(a => new ReportFieldValueDataModel()
                {
                    ReportFieldId = a.FieldId,
                    Value = a.Value
                }).ToList();

                var report = new ReportDataModel()
                {
                    EventId = eventId,
                    FieldValues = fieldValues.ToList()
                };

                report = await _reportDataService.CreateReportAsync(report);


                report.Event = await _eventDataService.GetUserEventQuery(eventId)
                    .Include(e => e.ParentEvent)
                    .Include(e => e.EventType)
                    .Include(e => e.ChildrenEvents)
                    .FirstOrDefaultAsync();

                await CreateMrvMisReportDataAsync(report.Id);

                try
                {
                    var reportContext = await _reportDataService.GenerateReportContext(report);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"ReportContext generation failed for report id {report.Id} with error: {ex.Message}");
                    throw;
                }

                if (report.Event.EventType.BusinessId == EventType.CommenceInternalTransfer)
                {
                    var pairedReport = new ReportDataModel()
                    {
                        EventId = report.Event.ChildEvent.Id,
                        FieldValues = fieldValues.ToList()
                    };

                    var editedReport = await _reportDataService.InternalTransfer(pairedReport, props.InternalTransferSourceTankId.Value, props.InternalTransferTargetTankId.Value, props.InternalTransferAmount.Value);

                    editedReport = await _reportDataService.CreateReportAsync(editedReport);

                    report.Event = await _eventDataService.GetUserEventQuery(report.Event.ChildEvent.Id)
                        .FirstOrDefaultAsync();

                    await CreateMrvMisReportDataAsync(editedReport.Id);

                    try
                    {
                        var reportContext = await _reportDataService.GenerateReportContext(editedReport);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"ReportContext generation failed for report id {editedReport.Id} with error: {ex.Message}");
                        throw;
                    }
                }

                await t.CommitAsync();

                return report.Id;
            }
            catch (Exception ex)
            {
                await t.RollbackAsync();
                _logger.LogError($"Transaction failed with error: {ex.Message}");
                throw;
            }
        }

        public async Task PostProcess()
        {
            _logger.LogInformation("Post processing...");
            await _reportDataService.PostProcess();
        }
        private async Task<MrvMisDataModel> GetMrvMisDataModel(int reportId)
        {
            var report = await _reportDataService.ParseReport(reportId);
            if (report == null)
            {
                return null;
            }
            var vesselDetails = await _eventDataService.GetVesselDetails(report.UserId);

            var reportModel = report.ToBusinessModel();
            reportModel.Event.UserName = vesselDetails.UserName.ToLower();
            if (ReportType.Performance.Contains(reportModel.Event.ReportTypeId ?? -1))
            {
                var performance = await GetPerformanceValuesAsync(report.EventId, report.UserId);
                reportModel.Performance = performance;
                reportModel.Performance.MainEngineMaxPower = vesselDetails.MainEngineMaxPower;
                reportModel.Performance.PitchPropeller = vesselDetails.PitchPropeller;
            }
            var voyageBusinessId = await _eventDataService.GetMrvUnmooringEventBusinessIdFromVoyageId(report.VoyageId);
            return reportModel.ToMrvDataModel(voyageBusinessId);
        }
        private async Task CreateMrvMisReportDataAsync(int reportId)
        {
            var model = await GetMrvMisDataModel(reportId);
            if (model != null)
            {
                await _eventDataService.CreateMrvMis(model);
            }
        }

        private async Task UpdateMrvMisReportDataAsync(int reportId)
        {
            var model = await GetMrvMisDataModel(reportId);
            if (model != null)
            {
                await _eventDataService.UpdateMrvMis(model);
            }
        }

        public async Task UpdateReportAsync(int reportId, List<ReportFieldValueBusinessModel> fields, ReportingPropsBusinessModel props = null)
        {
            await using var t = await _eventDataService.BeginTransactionAsync();

            try
            {
                var report = new ReportDataModel()
                {
                    Id = reportId,
                    FieldValues = fields.Select(a => new ReportFieldValueDataModel()
                    {
                        ReportFieldId = a.FieldId,
                        Value = a.Value
                    }).ToList()
                };
                await _reportDataService.UpdateReportAsync(report);
                await UpdateMrvMisReportDataAsync(reportId);



                var @event = await _eventDataService.GetUserEventQueryFromReportId(reportId)
                    .Include(e => e.EventType)
                    .Include(e => e.ChildrenEvents)
                    .ThenInclude(e => e.Reports)
                    .FirstOrDefaultAsync();

                if (@event.EventType.BusinessId == EventType.CommenceInternalTransfer)
                {
                    var pairedReport = new ReportDataModel()
                    {
                        Id = @event.ChildEvent.Reports.First().Id,
                        FieldValues = fields.Select(a => new ReportFieldValueDataModel()
                        {
                            ReportFieldId = a.FieldId,
                            Value = a.Value
                        }).ToList()
                    };

                    var editedReport = await _reportDataService.InternalTransfer(pairedReport, props.InternalTransferSourceTankId.Value, props.InternalTransferTargetTankId.Value, props.InternalTransferAmount.Value);

                    await _reportDataService.UpdateReportAsync(pairedReport);
                    //TODO: reportcontext process here
                    await UpdateMrvMisReportDataAsync(pairedReport.Id);
                }
                await t.CommitAsync();
            }
            catch
            {
                await t.RollbackAsync();
                throw;
            }


        }

        public async Task<ReportBusinessModel> GetRelatedReportAsync(int eventId)
        {
            var relatedReport = await _reportDataService.GetRelatedReportAsync(eventId);
            if (relatedReport == null)
                return null;
            var report = relatedReport.ToBusinessModel();

            if (ReportType.Performance.Contains(report.Event.ReportTypeId.Value))
            {
                report.Performance = await GetPerformanceValuesAsync(report.Event.Id, report.Event.UserId);

                var vesselDetails = await _eventDataService.GetVesselDetails(report.Event.UserId);
                report.Performance.MainEngineMaxPower = vesselDetails?.MainEngineMaxPower;
                report.Performance.PitchPropeller = vesselDetails?.PitchPropeller;

            }

            return report;
        }
        public async Task<ReportBusinessModel> GetRelatedBunkeringReportAsync(int eventId)
        {
            var relatedReport = await _reportDataService.GetRelatedBunkeringReportAsync(eventId);
            if (relatedReport == null)
                return null;
            var report = relatedReport.ToBusinessModel();
            return report;
        }

        public async Task<DateTimeOffset?> GetEventTimeStampFromReportId(int reportId)
        {
            return await _reportDataService.GetEventTimeStampFromReportId(reportId);
        }

        public async Task<DateTimeOffset?> GetEventTimeStampFromEventId(int eventId)
        {
            return await _reportDataService.GetEventTimeStampFromEventId(eventId);
        }

        public async Task<ReportBusinessModel> GetReportAsync(int reportId)
        {
            var dbReport = await _reportDataService.GetReportAsync(reportId);

            var report = dbReport.ToBusinessModel();

            if (EventType.CommenceInternalTransfer == dbReport.Event.EventType.BusinessId)
            {
                var pairedEvent = await _eventDataService.GetUserChildEventQuery(dbReport.EventId)
                    .Include(e => e.Reports)
                    .ThenInclude(e => e.FieldValues)
                    .ThenInclude(e => e.ReportField)
                    .ThenInclude(e => e.Group)
                    .Include(e => e.Reports)
                    .ThenInclude(e => e.FieldValues)
                    .ThenInclude(e => e.ReportField)
                    .ThenInclude(e => e.Tank)
                    .FirstOrDefaultAsync();
                var pairedReport = pairedEvent?.Reports.FirstOrDefault();
                if (pairedReport != null)
                {
                    var props = new ReportingPropsBusinessModel();
                    var targetReportIds = new List<int>();
                    var volumeFields = pairedReport.FieldValues.Where(fv => fv.ReportField.ValidationKey == "volume" && ReportType.ActualGroups.Contains(fv.ReportField.Group.BusinessId)).ToList();
                    foreach (var field in volumeFields)
                    {
                        var targetVolumeField = dbReport.FieldValues.Where(fv => fv.ReportFieldId == field.ReportFieldId && fv.Value != field.Value).SingleOrDefault();
                        if (targetVolumeField != null)
                        {
                            if (targetVolumeField.AsDouble() > field.AsDouble())
                            {
                                props.InternalTransferAmount = Math.Round(targetVolumeField.AsDouble() - field.AsDouble(), 4);
                                props.InternalTransferSourceTankId = field.ReportField.TankId;
                            }
                            else
                            {
                                props.InternalTransferTargetTankId = field.ReportField.TankId;
                            }
                        }
                    }
                    report.ReportingProps = props;
                }

            }

            if (ReportType.Performance.Contains(report.Event.ReportTypeId.Value))
            {
                report.Performance = await GetPerformanceValuesAsync(report.Event.Id, report.Event.UserId);
                var vesselDetails = await _eventDataService.GetVesselDetails(report.Event.UserId);
                report.Performance.MainEngineMaxPower = vesselDetails?.MainEngineMaxPower;
                report.Performance.PitchPropeller = vesselDetails?.PitchPropeller;
            }
            return report;
        }

        public async Task<int?> GetReportIdFromEventId(int eventId)
        {
            var reportId = await _reportDataService.GetReportIdFromEventId(eventId);
            return reportId;
        }

        public async Task<List<ReportBusinessModel>> GetReportHistoryAsync(string userId, string conditionId, int page = 1, int pageSize = 10, int? targetEventId = null)
        {
            var dbReports = await _reportDataService.GetReportHistoryAsync(userId, conditionId, page, pageSize, targetEventId);
            var reportList = dbReports.Select(a => a.ToBusinessModel()).ToList();

            foreach (var report in reportList)
            {
                if (ReportType.Performance.Contains(report.Event.ReportTypeId.Value))
                {
                    var performace = await GetPerformanceValuesAsync(report.Event.Id, userId);
                    report.Performance = performace;
                    var vesselDetails = await _eventDataService.GetVesselDetails(report.Event.UserId);
                    report.Performance.MainEngineMaxPower = vesselDetails?.MainEngineMaxPower;
                    report.Performance.PitchPropeller = vesselDetails?.PitchPropeller;
                }
                report.RelatedReport = await GetRelatedReportAsync(report.Event.Id);
            }
            return reportList;
        }

        public Task<int> GetReportHistoryCountAsync(string userId, string conditionId, int? targetEventId = null)
        {
            return _reportDataService.GetReportHistoryCountAsync(userId, conditionId, targetEventId);
        }

        public async Task<ReportPerformanceBusinessModel> GetPerformanceValuesAsync(int eventId, string userId)
        {

            var cospEvents = await _eventDataService.GetCospEventRangeAsync(eventId);

            var steamingTimeDict = _eventDataService.GetSteamingTimeAsync(eventId, cospEvents);

            var details = new List<SteamingTimeBusinessModel>();
            foreach (var parentItem in steamingTimeDict)
            {
                foreach (var childItem in parentItem.Value)
                {
                    details.Add(new SteamingTimeBusinessModel()
                    {
                        EventId = childItem.Key,
                        Oil = parentItem.Key,
                        SteamingTime = childItem.Value
                    });
                }
            }

            var totalConsumption = await _eventDataService.GetTotalConsumptionAsync(eventId);

            var totalConsumptionModel = new TotalConsumptionBusinessModel()
            {
                ActualConsVLSFO = totalConsumption["actualTotalConsumption_hfo"],
                ActualConsLSMGO = totalConsumption["actualTotalConsumption_mgo"],
                PoolConsVLSFO = totalConsumption["poolTotalConsumption_hfo"],
                PoolConsLSMGO = totalConsumption["poolTotalConsumption_mgo"]
            };

            var totalDistanceOverGround = await _eventDataService.GetTotalDistanceOverGroundAsync(eventId, cospEvents);



            var performance = new ReportPerformanceBusinessModel()
            {
                TotalDistanceOverGround = totalDistanceOverGround,
                TotalConsumption = totalConsumptionModel,
                SteamingTime = details
            };
            return performance;
        }
        public async Task<ReportBusinessModel> GetRelatedBunkeringReport(int fieldValueId, string userId)
        {
            var report = await _reportDataService.GetRelatedBunkeringReport(fieldValueId, userId);
            return report.ToBusinessModel();
        }

        public async Task<EventAttachmentBusinessModel> UploadDocument(int bunkeringId, string documentCode, List<IFormFile> files, string userId)
        {
            var attachment = await _reportDataService.UploadDocument(bunkeringId, documentCode, files, userId);
            return attachment.ToBusinessModel();
        }
        public async Task DeleteAttachment(int attachmentId)
        {
            await _reportDataService.DeleteAttachment(attachmentId);
        }
        public async Task<EventAttachmentBusinessModel> GetAttachment(int attachmentId)
        {
            var attachment = await _reportDataService.GetAttachment(attachmentId);
            return attachment.ToBusinessModel();
        }
        public async Task<List<FileInfo>> GetAttachments(int bunkeringId)
        {
            var attachments = await _reportDataService.GetAttachments(bunkeringId);
            return attachments;
        }

        public async Task<List<ReportFieldValueBusinessModel>> GetTransferDetailsAsync(string userId)
        {
            var fieldValues = await _reportDataService.GetTransferDetailsAsync(userId);
            return fieldValues.Select(_ => _.ToBusinessModel()).ToList();
        }

        public async Task<bool> isBunkeringPlan(int eventId)
        {
            return await _reportDataService.isBunkeringPlan(eventId);
        }

        public async Task<int?> GetEventIdFromReportId(int reportId)
        {
            return await _reportDataService.GetEventIdFromReportId(reportId);
        }

        public async Task UpdateBunkeringSupplier(string userId, int bunkeringId, string supplier)
        {
            await _reportDataService.UpdateBunkeringSupplier(userId, bunkeringId, supplier);
        }

        public async Task UpdateBunkeringNamedValue(string userId, int bunkeringId, string namedValue)
        {
            await _reportDataService.UpdateBunkeringNamedValue(userId, bunkeringId, namedValue);
        }

        public async Task<List<BunkeringDataBusinessModel>> GetBunkeringData(int eventId)
        {
            var bunkeringData = await _reportDataService.GetBunkeringData(eventId);
            return bunkeringData.Select(b => b.ToBusinessModel()).ToList();
        }

        public async Task<FileInfo> GetWaterConsumptions(DateTime from, DateTime to)
        {
            var model = await _reportDataService.GetWaterConsumptions(from, to);

            var viewPath = "~/Views/_WaterConsumptions.cshtml";

            var viewEngineResult = _viewEngine.GetView(null, viewPath, false);

            if (!viewEngineResult.Success)
            {
                throw new InvalidOperationException("Invalid view path!");
            }

            var view = viewEngineResult.View;

            var range = model.Min(c => c.Year) == model.Max(c => c.Year) ? $"{model.Min(c => c.Year)}" : $"{model.Min(c => c.Year)} - {model.Max(c => c.Year)}";

            var title = $"Telemachus - Water Consumption ({range})";

            using (var output = new StringWriter())
            {
                var tempData = new TempDataDictionary(
                         _actionContextAccessor.ActionContext.HttpContext,
                         _tempDataProvider);
                tempData["Title"] = "Telemachus - Water Consumption";
                var viewContext = new ViewContext(
                    _actionContextAccessor.ActionContext,
                    view,
                    new ViewDataDictionary<List<WaterConsumptionViewModel>>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    { Model = model },
                    tempData,
                    output,
                    new HtmlHelperOptions());

                await view.RenderAsync(viewContext);

                try
                {
                    var fileInfo = await PDFConverter.PDFConverter.Convert(Path.Combine(_env.ContentRootPath), output.ToString());

                    string newfilePath = Path.Combine(fileInfo.Directory.FullName, $"{title}.pdf");

                    if (File.Exists(newfilePath))
                    {
                        File.Delete(newfilePath);
                    }

                    fileInfo.MoveTo(newfilePath);
                    return fileInfo;
                }
                catch (Exception ex)
                {
                    _logger.LogError("PDF:" + ex.Message);
                    throw ex;
                }
            }
        }

    }
}
