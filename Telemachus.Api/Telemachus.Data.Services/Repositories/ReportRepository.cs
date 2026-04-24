using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Enums;

using Helpers;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Telemachus.Data.Models.Authentication;
using Telemachus.Data.Models.Events;
using Telemachus.Data.Models.Reports;
using Telemachus.Data.Services.Context;
using Telemachus.Data.Services.Interfaces;


namespace Telemachus.Data.Services.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly TelemachusContext _context;
        private readonly IConfiguration _config;
        private string _attachmentsPath = "";
        private readonly ILogger<ReportRepository> _logger;
        public ReportRepository(TelemachusContext context, IConfiguration config, ILogger<ReportRepository> logger)
        {
            _context = context;
            _config = config;
            _attachmentsPath = _config["AttachmentsPath"];
            if (string.IsNullOrEmpty(_attachmentsPath))
            {
                _attachmentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Telemachus");
            }
            _logger = logger;

        }
        public async Task PostProcess3()
        {
            var e = await _context.Reports
                .Where(r => r.Id == 14049)
                .Select(r => new EventDataModel()
                {
                    Id = r.Event.Id,
                    UserId = r.Event.UserId,
                    Timestamp = r.Event.Timestamp,
                    ParentEvent = r.Event.ParentEvent != null ? new EventDataModel()
                    {
                        Timestamp = r.Event.ParentEvent.Timestamp
                    } : null
                })
                .FirstAsync();
            var prevReport = await GetPrevReportBase(e.Id, e.UserId, e.Timestamp.Value);
            var targetTimestamp = e.Timestamp ?? e.ParentEvent?.Timestamp;
            var targetBunkeringId = await _context
                .Events
                .Where(ev => ev.UserId == e.UserId &&
                    ev.EventTypeId == 61 &&
                    ev.Timestamp.HasValue &&
                    ev.Timestamp < targetTimestamp.Value &&
                    ev.BunkeringDataId.HasValue &&
                    ev.BunkeringData.IsDeleted == false &&
                    ev.BunkeringData.Bdn == "BRMNWE2025WIL-204")
                .OrderByDescending(ev => ev.Timestamp)
                .Select(ev => ev.BunkeringDataId)
                .FirstOrDefaultAsync();
        }
        public async Task PostProcess()
        {
            //await PostProcess3();
            //return;
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var exportPath = Path.Combine(documentsPath, "log.txt");
            var filePath = Path.Combine(documentsPath, "report_field_values_202603021206.json");

            // 1️⃣ Read JSON
            var json = await File.ReadAllTextAsync(filePath);
            using var doc = JsonDocument.Parse(json);
            var array = doc.RootElement.GetProperty("report_field_values");

            //int total = array.GetArrayLength();
            //int index = 0;

            // 2️⃣ Collect only the keys needed from JSON
            var keys = new HashSet<(int ReportId, int ReportFieldId)>();
            foreach (var element in array.EnumerateArray())
            {
                int reportId = element.GetProperty("ReportId").GetInt32();
                int reportFieldId = element.GetProperty("ReportFieldId").GetInt32();
                keys.Add((reportId, reportFieldId));
            }

            var reportIds = keys.Select(k => k.ReportId).Distinct().ToList();
            var reportFieldIds = keys.Select(k => k.ReportFieldId).Distinct().ToList();

            // 3️⃣ Load matching rows from DB (tracked entities for updating)
            var dbValues = await _context.ReportFieldValues
                .Where(rfv => reportIds.Contains(rfv.ReportId) &&
                              reportFieldIds.Contains(rfv.ReportFieldId))
                .Include(rfv => rfv.ReportField) // to get ValidationKey
                .ToListAsync();

            // 4️⃣ Build in-memory lookup
            var lookup = dbValues.ToDictionary(
                x => (x.ReportId, x.ReportFieldId));

            //var messages = new List<string>();

            // 5️⃣ Process JSON, log changes, and update DB entities

            //var targetFields = new List<string>()
            //        {
            //            "sulphurContent",
            //            "density",
            //            "volume",
            //            "weight"
            //        };


            foreach (var element in array.EnumerateArray())
            {
                //index++;

                int reportId = element.GetProperty("ReportId").GetInt32();
                int reportFieldId = element.GetProperty("ReportFieldId").GetInt32();
                string value = element.GetProperty("Value").GetString();

                if (lookup.TryGetValue((reportId, reportFieldId), out var targetField))
                {
                    //var message = $"[{targetField.ReportField.ValidationKey}] {reportId}, {reportFieldId}, {targetField.Value}, {value}";

                    //if (targetFields.Contains(targetField.ReportField.ValidationKey) &&
                    //    targetField.Value != value)
                    //{
                    //    messages.Add(message);
                    //}

                    if (targetField.Value != value)
                    {
                        //Update the value
                        //targetField.Value = value;
                    }


                }
            }

            // 6️⃣ Persist changes to DB in one SaveChangesAsync call
            //await _context.SaveChangesAsync();

            // 7️⃣ Write log once
            //if (messages.Count > 0)
            //{
            //    await File.WriteAllLinesAsync(exportPath, messages);
            //}
        }
        public async Task<List<ReportFieldDataModel>> GetReportFieldsAsync(int eventTypeId, string userId)
        {

            var eventType = await _context.EventTypes.SingleAsync(et => et.Id == eventTypeId);

            var user = await _context.Users.Where(u => u.Id == userId).Select(u => new User
            {
                Id = u.Id,
                NonHafnia = u.NonHafnia,
                NonPool = u.NonPool
            }).FirstAsync();

            var reportFields = await _context
                .EventTypes
                .Include(a => a.ReportType)
                .ThenInclude(a => a.AvailableReportFields)
                .ThenInclude(a => a.ReportField)
                .ThenInclude(a => a.Group)
                .Include(a => a.ReportType)
                .ThenInclude(a => a.AvailableReportFields)
                .ThenInclude(a => a.ReportField)
                .ThenInclude(a => a.Tank)
                .Where(a => a.Id == eventType.Id)
                .SelectMany(a => a.ReportType.AvailableReportFields.Select(a => a.ReportField))
                .ToListAsync();

            var fieldsToRemove = new List<int>();

            var userTanks = await _context.TankUserSpecs
                    .Include(_ => _.Tank)
                    .Where(_ => _.UserId == userId && _.IsActive)
                    .ToListAsync();

            if (user.NonHafnia)
            {
                //remove oop
                var id = reportFields.Where(rf => rf.ValidationKey == "oop").Select(rf => rf.Id).FirstOrDefault();
            }

            if (user.NonPool)
            {
                var id = reportFields.Where(rf => rf.ValidationKey == "instructedChartererConsumption").Select(rf => rf.Id).FirstOrDefault();
                fieldsToRemove.Add(id);
            }

            foreach (var field in reportFields)
            {
                if (field.Tank == null)
                    continue;
                field.UserTank = userTanks.Where(_ => _.TankId == field.Tank.Id)
                    .SingleOrDefault();
                if (field.UserTank != null && !(EventType.BunkeringPlanGroup.Contains(eventType.BusinessId) && !field.Tank.Storage))
                {
                    continue;
                }
                fieldsToRemove.AddRange(
                    reportFields.Where(_ => _.TankId == field.TankId)
                    .Select(_ => _.Id)
                    .ToList()
                    );
            }


            reportFields.RemoveAll(field => fieldsToRemove.Contains(field.Id));

            return reportFields;
        }

        /*
         * Requires ReportFieldValue.ReportField.TankId, ReportFieldValue.ReportField.Group.BusinessId
         */
        private void processCorrectionFactors(ReportDataModel report, bool safelyUpdateWeight = true)
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
                    var gsv = targetFieldValues.First(_ => _.ReportField.ValidationKey == "gsv");
                    var vcf = targetFieldValues.First(_ => _.ReportField.ValidationKey == "vcf");
                    var wcf = targetFieldValues.First(_ => _.ReportField.ValidationKey == "wcf");

                    var correctionFactors = ReportType.BunkerGroups.Contains(groupId) ? BunkeringTools.GetVolumeCorrectionFactor(density.Value, temp.Value, weight.Value) : BunkeringTools.GetWeightCorrectionFactor(density.Value, temp.Value, volume.Value);

                    volume.Value = correctionFactors.Volume.ToString("F3");

                    if (safelyUpdateWeight)
                    {
                        gsv.Value = correctionFactors.GrossStandardVolume.ToString("F4");
                        vcf.Value = correctionFactors.VolumeCorrectionFactor.ToString("F4");
                        wcf.Value = correctionFactors.WeightCorrectionFactor.ToString("F4");
                        weight.Value = correctionFactors.Weight.ToString("F3");
                    }
                }
            }
        }

        private void processBunkering(ReportDataModel bunkeringCompleteReport, ReportDataModel bunkeringReport, BunkeringDataModel bunkeringData)
        {
            var fuelGroups = bunkeringData.FuelType == 1 ? ReportType.HfoGroups : ReportType.MgoGroups;

            double totalWeight = 0;

            foreach (var tankContext in bunkeringData.Tanks)
            {
                foreach (var group in fuelGroups)
                {
                    var fields = bunkeringCompleteReport.FieldValues.Where(v => v.ReportField.Group != null && v.ReportField.Group.BusinessId == group && v.ReportField.TankId == tankContext.TankId).ToList();
                    if (!fields.Any())
                        continue;
                    var parentFields = bunkeringReport.FieldValues.Where(v => v.ReportField.Group != null && v.ReportField.Group.BusinessId == group && v.ReportField.TankId == tankContext.TankId).ToList();
                    var densityField = fields.Where(f => f.ReportField.ValidationKey == "density").First();
                    var sulphurField = fields.Where(f => f.ReportField.ValidationKey == "sulphurContent").First();
                    var volumeField = fields.Where(f => f.ReportField.ValidationKey == "volume").First();
                    var bdnField = fields.Where(f => f.ReportField.ValidationKey == "bdn").First();
                    var temperatureField = fields.Where(f => f.ReportField.ValidationKey == "tankTemperature").First();

                    bdnField.Value = bdnField.Value?.Trim().ToUpper() ?? "";
                    if (string.IsNullOrEmpty(tankContext.Amount))
                    {
                        var parentField = parentFields.Where(f => f.ReportField.ValidationKey == volumeField.ReportField.ValidationKey).First();
                        var actualVolume = BunkeringTools.GetDouble(volumeField.Value) - BunkeringTools.GetDouble(parentField.Value);
                        var correctionFactor = BunkeringTools.GetWeightCorrectionFactor(densityField.Value, temperatureField.Value, actualVolume.ToString("F3"));
                        tankContext.Amount = correctionFactor.Weight.ToString("F3");
                        totalWeight += BunkeringTools.GetDouble(correctionFactor.Weight.ToString("F3"));
                    }
                    if (string.IsNullOrEmpty(bunkeringData.Bdn))
                    {
                        bunkeringData.Bdn = bdnField.Value;
                    }
                    if (string.IsNullOrEmpty(bunkeringData.SulphurContent))
                    {
                        bunkeringData.SulphurContent = BunkeringTools.GetDouble(sulphurField.Value).ToString("F2");
                    }
                    if (string.IsNullOrEmpty(bunkeringData.Density))
                    {
                        bunkeringData.Density = BunkeringTools.GetDouble(densityField.Value).ToString("F4");
                    }
                    if (string.IsNullOrEmpty(bunkeringData.Viscosity))
                    {
                        var viscosityField = fields.Where(f => f.ReportField.ValidationKey == "kinematicViscosity").First();
                        bunkeringData.Viscosity = BunkeringTools.GetDouble(viscosityField.Value).ToString("F2");
                    }
                    if (tankContext.ComminglingData != null && string.IsNullOrEmpty(bunkeringData.TotalAmount)) // initial
                    {
                        bdnField.Value = $"{tankContext.ComminglingData.Bdn}, {bdnField.Value}";
                        var parentDensityField = parentFields.Where(f => f.ReportField.ValidationKey == "density").First();
                        var newDensityValue = (BunkeringTools.GetDouble(densityField.Value) + BunkeringTools.GetDouble(parentDensityField.Value)) / 2;
                        densityField.Value = newDensityValue.ToString("F4");
                        var weightField = fields.Where(f => f.ReportField.ValidationKey == "weight").First();
                        var parentSulphurField = parentFields.Where(f => f.ReportField.ValidationKey == "sulphurContent").First();
                        var newSulphurValue = (BunkeringTools.GetDouble(sulphurField.Value) + BunkeringTools.GetDouble(parentSulphurField.Value)) / 2;
                        sulphurField.Value = newSulphurValue.ToString("F2");
                        var wcfField = fields.Where(f => f.ReportField.ValidationKey == "wcf").First();
                        var vcfField = fields.Where(f => f.ReportField.ValidationKey == "vcf").First();
                        var gsvField = fields.Where(f => f.ReportField.ValidationKey == "gsv").First();

                        var correctionFactors = ReportType.BunkerGroups.Contains(group) ?
                            BunkeringTools.GetVolumeCorrectionFactor(densityField.Value, temperatureField.Value, weightField.Value) :
                            BunkeringTools.GetWeightCorrectionFactor(densityField.Value, temperatureField.Value, volumeField.Value);

                        vcfField.Value = correctionFactors.VolumeCorrectionFactor.ToString("F4");
                        gsvField.Value = correctionFactors.GrossStandardVolume.ToString("F4");
                        wcfField.Value = correctionFactors.WeightCorrectionFactor.ToString("F4");
                        weightField.Value = correctionFactors.Weight.ToString("F3");
                        volumeField.Value = correctionFactors.Volume.ToString("F3");

                    }
                }
            }
            if (string.IsNullOrEmpty(bunkeringData.TotalAmount))
            {
                bunkeringData.TotalAmount = totalWeight.ToString("F3");
                if (string.IsNullOrEmpty(bunkeringData.NamedAmount))
                {
                    bunkeringData.NamedAmount = bunkeringData.TotalAmount;
                }
            }

        }
        private async Task ifUpdateManagedFieldValues(ReportDataModel report)
        {
            var targetReport = await GetReportAsync(report.Id);
            if (!targetReport.Event.EventType.IsCompleteBunkering)
                return;

            var parentEventReportId = await _context.Reports
                .Where(r => r.EventId == report.Event.ParentEventId.Value)
                .Select(r => r.Id)
                .FirstAsync();

            var parentEventReport = await GetReportAsync(parentEventReportId);

            var bunkeringData = await _context.BunkeringData
                .Include(b => b.Tanks)
                .ThenInclude(b => b.ComminglingData)
                .FirstAsync(b => b.Id == targetReport.Event.BunkeringDataId);

            processBunkering(targetReport, parentEventReport, bunkeringData);
            await _context.SaveChangesAsync();

            var nextBunkeringTimestamp = await _context.Events
                .Where(e => e.EventType.BusinessId == EventType.CommenceBunkering &&
                            e.UserId == report.Event.UserId &&
                            e.Timestamp > report.Event.Timestamp)
                .OrderByDescending(e => e.Timestamp)
                .ThenByDescending(e => e.Id)
                .Select(e => e.Timestamp)
                .FirstOrDefaultAsync();

            var query = _context.Reports
                .Include(r => r.FieldValues)
                    .ThenInclude(fv => fv.ReportField)
                        .ThenInclude(f => f.Tank)
                .Where(r => r.Event.UserId == report.Event.UserId &&
                            !EventType.BunkeringPlanGroup.Contains(r.Event.EventType.BusinessId) &&
                            ReportType.Rob.Contains(r.Event.EventType.ReportTypeId ?? -1) &&
                            r.Event.Timestamp > targetReport.Event.Timestamp);

            if (nextBunkeringTimestamp.HasValue)
                query = query.Where(r => r.Event.Timestamp < nextBunkeringTimestamp);

            var nextReports = await query.ToListAsync();

            var fieldValuesToUpdate = new List<ReportFieldValueDataModel>();

            foreach (var nextReport in nextReports)
            {
                var nextReportTanks = nextReport.FieldValues
                    .Where(fv => fv.ReportField.Tank != null)
                    .GroupBy(fv => fv.ReportField.Group)
                    .Select(group => new
                    {
                        Group = group.Key,
                        Tanks = group.GroupBy(fv => fv.ReportField.Tank.Id)
                    });

                foreach (var group in nextReportTanks)
                {
                    foreach (var tankGroup in group.Tanks)
                    {

                        var tankId = tankGroup.Key;
                        //if (tankId == 10 && targetReport.Id == 12769)
                        //{
                        //    Console.Write("");
                        //}
                        var targetBdn = tankGroup.FirstOrDefault(fv => fv.ReportField.ValidationKey == "bdn");

                        if (targetBdn == null)
                        {
                            continue;
                        }

                        var targetDensity = tankGroup.FirstOrDefault(fv => fv.ReportField.ValidationKey == "density");
                        var targetSulphur = tankGroup.FirstOrDefault(fv => fv.ReportField.ValidationKey == "sulphurContent");
                        var targetViscosity = tankGroup.FirstOrDefault(fv => fv.ReportField.ValidationKey == "kinematicViscosity");
                        var targetCalorifer = tankGroup.FirstOrDefault(fv => fv.ReportField.ValidationKey == "lowerCalorifer");


                        var sourceTankId = targetReport.FieldValues
                            .Where(fv =>
                                fv.ReportField.GroupId == group.Group.Id &&
                                fv.ReportField.TankId.HasValue &&
                                fv.ReportField.ValidationKey == "bdn" &&
                                fv.Value == targetBdn.Value)
                            .OrderByDescending(fv => fv.ReportField.TankId == tankId) // top priority source and target has the same tankId
                            .ThenByDescending(fv => fv.ReportField.Tank.Storage) // second priority source is from storage tank
                            .Select(fv => fv.ReportField.TankId)
                            .FirstOrDefault();

                        if (sourceTankId == null)
                        {
                            continue;
                        }

                        var sourceFields = targetReport.FieldValues.Where(fv => fv.ReportField.GroupId == group.Group.Id && fv.ReportField.TankId == sourceTankId).ToList();
                        var sourceDensity = sourceFields.FirstOrDefault(fv => fv.ReportField.ValidationKey == "density");
                        var sourceSulphur = sourceFields.FirstOrDefault(fv => fv.ReportField.ValidationKey == "sulphurContent");
                        var sourceViscosity = sourceFields.FirstOrDefault(fv => fv.ReportField.ValidationKey == "kinematicViscosity");
                        var sourceCalorifer = sourceFields.FirstOrDefault(fv => fv.ReportField.ValidationKey == "lowerCalorifer");

                        void UpdateIfDifferent(ReportFieldValueDataModel target, ReportFieldValueDataModel source)
                        {
                            if (target != null && source != null && target.Value != source.Value)
                            {
                                target.Value = source.Value;
                                fieldValuesToUpdate.Add(target);
                            }
                        }

                        UpdateIfDifferent(targetDensity, sourceDensity);
                        UpdateIfDifferent(targetSulphur, sourceSulphur);
                        UpdateIfDifferent(targetViscosity, sourceViscosity);
                        UpdateIfDifferent(targetCalorifer, sourceCalorifer);
                    }
                }

            }

            if (fieldValuesToUpdate.Any())
            {
                _context.UpdateRange(fieldValuesToUpdate);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<ReportDataModel> InternalTransfer(ReportDataModel report, int sourceTankId, int targetTankId, double amount)
        {
            var targetFieldIds = report.FieldValues.Select(fv => fv.ReportFieldId).Distinct().ToList();

            var targetFields = (await _context.ReportFields
                .Include(f => f.Group)
                .Where(f => targetFieldIds.Contains(f.Id) && f.TankId.HasValue && (f.TankId == sourceTankId || f.TankId == targetTankId))
                .ToListAsync())
                .GroupBy(f => (f.GroupId, f.TankId.Value))
                .ToDictionary(g => g.Key, g => g.ToDictionary(f => f.ValidationKey, f => f));

            var fieldsToAdd = new List<ReportFieldValueDataModel>();

            foreach (var ((groupId, tankId), fields) in targetFields)
            {
                var densityField = report.FieldValues.FirstOrDefault(fv => fv.ReportFieldId == fields["density"].Id);
                var tempField = report.FieldValues.FirstOrDefault(fv => fv.ReportFieldId == fields["tankTemperature"].Id);
                var volumeField = report.FieldValues.FirstOrDefault(fv => fv.ReportFieldId == fields["volume"].Id);

                if (densityField == null || tempField == null || volumeField == null)
                    continue;

                var volume = volumeField.AsDouble() + (tankId == targetTankId ? amount : -amount);
                var correctionFactors = BunkeringTools.GetWeightCorrectionFactor(densityField.Value, tempField.Value, volumeField.Value);

                fieldsToAdd.AddRange(new[]
                {
            new ReportFieldValueDataModel
            {
                ReportFieldId = fields["vcf"].Id,
                Value = correctionFactors.VolumeCorrectionFactor.ToString("F4")
            },
            new ReportFieldValueDataModel
            {
                ReportFieldId = fields["gsv"].Id,
                Value = correctionFactors.GrossStandardVolume.ToString("F4")
            },
            new ReportFieldValueDataModel
            {
                ReportFieldId = fields["wcf"].Id,
                Value = correctionFactors.WeightCorrectionFactor.ToString("F4")
            },
            new ReportFieldValueDataModel
            {
                ReportFieldId = fields["volume"].Id,
                Value = volume.ToString("F3")
            },
            new ReportFieldValueDataModel
            {
                ReportFieldId = fields["weight"].Id,
                Value = correctionFactors.Weight.ToString("F3")
            }
        });
            }

            foreach (var field in fieldsToAdd)
            {
                var targetField = report.FieldValues.FirstOrDefault(f => f.ReportFieldId == field.ReportFieldId);
                if (targetField != null)
                {
                    targetField.Value = field.Value;
                }
            }

            return report;
        }
        private async Task<ReportDataModel> GetPrevReportBase(int eventId, string userId, DateTimeOffset timestamp)
        {
            var prevReport = await _context.Events
                .AsNoTracking()
                .Include(_ => _.Reports)
                .ThenInclude(_ => _.ReportContext)
                .Where(_ =>
                    _.UserId == userId &&
                    _.Id != eventId &&
                    _.Timestamp.HasValue &&
                    (_.Timestamp < timestamp || (_.Timestamp == timestamp && _.Id < eventId)) &&
                    _.EventTypeId != 1111 &&
                    _.EventTypeId != 1112 &&
                    _.EventType.ReportTypeId != 5 &&
                    _.Reports.Any())
                .OrderByDescending(_ => _.Timestamp)
                .ThenByDescending(_ => _.Id)
                .Select(r => new ReportDataModel()
                {
                    Id = r.Reports.First().Id,
                    ReportContext = r.Reports.First().ReportContext.Select(rc => new ReportContextDataModel()
                    {
                        Id = rc.Id,
                        TankId = rc.TankId,
                        BunkeringId = rc.BunkeringId
                    }).ToList()
                })
                .FirstOrDefaultAsync()
                ;
            return prevReport;
        }

        public async Task<List<ReportContextDataModel>> GenerateReportContext(ReportDataModel report)
        {
            //TODO: reportcontext

            //if (e.EventTypeId == 1111 || e.EventTypeId == 1112) // avoid bunkering plan
            //{
            //    return;
            //}
            // TODO log warning if some conditions has to be true but fails. ex. timestamps are null tank not found but has to be exists
            var reportFieldIds = report.FieldValues.Select(fv => fv.ReportFieldId).ToList();

            var tanks = await _context.ReportFields
                .Where(f => f.TankId != null &&
                            reportFieldIds.Contains(f.Id) &&
                            f.ValidationKey == "bdn" &&
                            (f.GroupId != 1 && f.GroupId != 3))
                .Select(g => new
                {
                    TankId = g.TankId.Value,
                    ReportFieldId = g.Id
                })
                .OrderBy(t => t.TankId)
                .ToListAsync()
                ;
            tanks = tanks.GroupBy(t => t.TankId)
            .Select(g => g.First())
            .ToList();

            ReportDataModel prevReport = null;

            var hasParentReports = report.Event.ParentEventId.HasValue ? await _context.Reports.Where(r => r.EventId == report.Event.ParentEventId).AnyAsync() : false;

            if (hasParentReports)
            {
                prevReport = await _context.Reports
                    .AsNoTracking()
                    .Include(_ => _.ReportContext)
                    .Where(r => r.EventId == report.Event.ParentEventId)
                    .Select(r => new ReportDataModel()
                    {
                        Id = r.Id,
                        ReportContext = r.ReportContext.Select(rc => new ReportContextDataModel()
                        {
                            Id = rc.Id,
                            TankId = rc.TankId,
                            BunkeringId = rc.BunkeringId
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();
            }
            else if (report.Event.Timestamp.HasValue)
            {
                prevReport = await GetPrevReportBase(report.Event.Id, report.Event.UserId, report.Event.Timestamp.Value);
            }



            if (prevReport != null)
            {
                var trackedReport = _context.Set<ReportDataModel>()
                    .Local
                    .FirstOrDefault(r => r.Id == report.Id);

                if (trackedReport != null)
                {
                    trackedReport.PrevReportId = prevReport.Id;
                }
                else
                {
                    _context.Attach(report);
                    report.PrevReportId = prevReport.Id;
                }

                await _context.SaveChangesAsync();
            }

            // TODO: reportcontext : when is implemented in production, improve this function:
            // select previous report tanks instead or else instead of checking the bunkering data table

            // TODO: ask gg, if tank is deleted and new created. instead of hardcode bdn, select from the last bunkering table bdns and match

            // TODO: comminglings, handle comminlings from internal transfer

            //TODO: internal transfer and update report (if on update cannot chnage data from create then skip this comment)

            //TODO: when delete event, delete report then delete report context rows!!

            //TODO: debug tanks are not included, test on Estia

            //TODO: remove trye catch on parent method

            //TODO: viscosity, prevreportcontextid



            var reportContextToAdd = new List<ReportContextDataModel>();

            foreach (var tank in tanks)
            {
                var prevContext = prevReport?.ReportContext.FirstOrDefault(rc => rc.TankId == tank.TankId);
                var prevContextId = prevContext?.Id;
                int? targetBunkeringId = null;
                if (report.Event.BunkeringDataId.HasValue)
                {
                    var isBunkeringTank = await _context.BunkeringData
                        .Include(b => b.Tanks)
                        .Where(b => b.Id == report.Event.BunkeringDataId)
                        .AnyAsync(b => b.Tanks.Any(t => t.TankId == tank.TankId));
                    if (isBunkeringTank && report.Event.ParentEvent != null)
                    {
                        reportContextToAdd.Add(new ReportContextDataModel()
                        {
                            ReportId = report.Id,
                            TankId = tank.TankId,
                            BunkeringId = report.Event.BunkeringDataId,
                            PrevContextId = null
                        });
                        continue;
                    }
                }
                var targetBdn = report.FieldValues
                    .Where(fv => fv.ReportFieldId == tank.ReportFieldId)
                    .Select(rfv => rfv.Value)
                    .FirstOrDefault()?.Trim().ToUpper();
                var targetFuelType = await _context.ReportFields
                    .Include(rf => rf.Group)
                    .Where(f => f.Id == tank.ReportFieldId && f.GroupId != null)
                    .Select(f => (int?)f.Group.FuelTypeId)
                    .FirstOrDefaultAsync();
                if (!string.IsNullOrWhiteSpace(targetBdn) && targetFuelType.HasValue) // TODO: if plan, non fuel target fields are removed
                {
                    var targetTimestamp = report.Event.Timestamp ?? report.Event.ParentEvent?.Timestamp;
                    targetBunkeringId = await _context
                        .Events
                        .Where(ev => ev.UserId == report.Event.UserId &&
                            ev.EventTypeId == 61 &&
                            ev.Timestamp.HasValue &&
                            ev.Timestamp < targetTimestamp.Value &&
                            ev.BunkeringDataId.HasValue &&
                            ev.BunkeringData.IsDeleted == false &&
                            ev.BunkeringData.FuelType == targetFuelType.Value &&
                            ev.BunkeringData.Bdn == targetBdn)
                        .OrderByDescending(ev => ev.Timestamp)
                        .Select(ev => ev.BunkeringDataId)
                        .FirstOrDefaultAsync();
                }
                if (targetBunkeringId != prevContext.BunkeringId)
                {
                    prevContextId = null;
                }
                reportContextToAdd.Add(new ReportContextDataModel()
                {
                    ReportId = report.Id,
                    TankId = tank.TankId,
                    BunkeringId = targetBunkeringId,
                    PrevContextId = prevContextId
                });

            }

            _context.ReportContext.AddRange(reportContextToAdd);
            await _context.SaveChangesAsync();

            //if (prevReport != null)
            //{

            //    var prevContextsByTankId = prevReport.ReportContext
            //        .ToDictionary(rc => rc.TankId);

            //    var prevContextIds = prevContextsByTankId.Values
            //        .Select(rc => rc.Id)
            //        .ToList();

            //    var contextsToFix = await _context.ReportContext
            //        .Where(rc => rc.PrevContextId != null && prevContextIds.Contains(rc.PrevContextId.Value))
            //        .ToListAsync();

            //    var contextsToFixByPrevId = contextsToFix
            //        .GroupBy(rc => rc.PrevContextId)
            //        .ToDictionary(g => g.Key, g => g.ToList());

            //    foreach (var context in reportContextToAdd)
            //    {
            //        if (!prevContextsByTankId.TryGetValue(context.TankId, out var prevContext))
            //            continue;

            //        if (!contextsToFixByPrevId.TryGetValue(prevContext.Id, out var fixes))
            //            continue;

            //        if (fixes.Any(f => f.BunkeringId != context.BunkeringId))
            //        {
            //            throw new InvalidOperationException(
            //                $"BunkeringId mismatch for TankId {context.TankId}. Operation aborted.");
            //        }

            //        foreach (var contextToFix in fixes)
            //        {
            //            contextToFix.PrevContextId = context.Id;
            //        }
            //    }

            //    await _context.SaveChangesAsync();
            //}



            foreach (var rfv in report.FieldValues)
            {
                var contextId = reportContextToAdd.First(c => c.TankId == rfv.ReportField.TankId).Id;
                rfv.ReportContextId = contextId;
            }
            await _context.SaveChangesAsync();
            return reportContextToAdd;
        }

        public async Task<ReportDataModel> CreateReportAsync(ReportDataModel report)
        {

            _context.Database.SetCommandTimeout(180);

            var hasReports = await _context.Events.Where(_ => _.Id == report.EventId && _.Reports.Any()).AnyAsync();
            if (hasReports)
            {
                return await UpdateReportAsync(report);
            }
            var fieldValues = report.FieldValues.ToList();
            report.FieldValues.Clear();

            var dateModified = DateTime.UtcNow;
            var userPrefix = await _context.Events.Where(_ => _.Id == report.EventId).Select(_ => _.User.Prefix).SingleAsync();

            userPrefix = userPrefix.ToUpper();

            var lastRecordBusinessId = await _context.ReportFieldValues
                .Where(_ => _.BusinessId.StartsWith(userPrefix))
                .OrderByDescending(_ => _.BusinessId.Length)
                .ThenByDescending(_ => _.BusinessId)
                .IgnoreQueryFilters()
                .Select(rfv => rfv.BusinessId)
                .FirstOrDefaultAsync();

            var lastId = 0;
            if (lastRecordBusinessId != null)
            {
                lastId = int.Parse(Regex.Match(lastRecordBusinessId, @"\d+").Value);
            }

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            var sqlBuilder = new StringBuilder();

            foreach (var entity in fieldValues)
            {
                lastId++;

                var valueParam = entity.Value?.Replace("'", "''") ?? "";
                var reportIdParam = report.Id;
                var reportFieldIdParam = entity.ReportFieldId;
                var dateModifiedParam = dateModified.ToString("yyyy-MM-dd HH:mm:ss.fff");
                var businessIdParam = $"{userPrefix}-{lastId}";

                sqlBuilder.Append($@"INSERT INTO [report_field_values] (Value, ReportId, ReportFieldId, DateModified, BusinessId) VALUES ('{valueParam}', {reportIdParam}, {reportFieldIdParam}, '{dateModifiedParam}', '{businessIdParam}');");


            }

            var sql = sqlBuilder.ToString();
            _context.Database.ExecuteSqlRaw(sql);

            var fields = await _context.ReportFieldValues
                .Include(v => v.ReportField)
                .ThenInclude(v => v.Group)
                .Where(v => v.ReportId == report.Id && v.ReportField.GroupId != null)
                .ToListAsync();

            report.FieldValues = fields;

            var bunkeringData = await _context.Events
                .Where(e => e.Id == report.EventId && e.EventType.BusinessId == EventType.BunkeringPlan).Select(e => e.BunkeringData).SingleOrDefaultAsync();

            if (bunkeringData != null)
            {
                await UpdateBunkeringPlanDataAsync(fields, bunkeringData);
            }

            await UpdateStsData(report.Id);

            return report;

        }

        private async Task UpdateStsData(int reportId)
        {
            var eventId = await _context.Reports.Where(r => r.Id == reportId).Select(r => (int?)r.EventId).FirstOrDefaultAsync();
            if (eventId == null)
            {
                return;
            }
            var @event = await _context.Events
                .Include(e => e.StsOperation)
                .Include(e => e.ParentEvent)
                .ThenInclude(e => e.StsOperation)
                .Where(e => e.Id == eventId)
                .FirstOrDefaultAsync();

            if (@event == null)
            {
                return;
            }
            var targetStsData = @event.ParentEvent?.StsOperation ?? @event.StsOperation;
            if (targetStsData == null)
            {
                return;
            }
            var targetEventId = @event.ParentEventId ?? @event.Id;
            var seaState = await _context.ReportFieldValues
                .AsNoTracking()
                .Where(rfv => rfv.Report.EventId == targetEventId && rfv.ReportField.ValidationKey == "seaState")
                .Select(rfv => rfv.Value)
                .FirstOrDefaultAsync();
            bool roughSeaState = false;
            if (!string.IsNullOrWhiteSpace(seaState))
            {
                switch (seaState.Trim().ToUpper())
                {
                    case "ROUGH":
                    case "VERY ROUGH":
                    case "HIGH":
                    case "VERY HIGH":
                    case "PHENOMENAL":
                        roughSeaState = true;
                        break;
                    default:
                        break;
                }
            }
            targetStsData.RoughSeaState = roughSeaState;
            await _context.SaveChangesAsync();

        }

        private async Task UpdateBunkeringPlanDataAsync(List<ReportFieldValueDataModel> fields, BunkeringDataModel bunkeringData)
        {

            var tankIds = fields.Where(f => f.ReportField.Group.BusinessId == bunkeringData.BunkerGroup && f.ReportField.ValidationKey == "weight" && !string.IsNullOrEmpty(f.Value))
                .Select(f => f.ReportField.TankId.Value)
                .Distinct()
                .ToList();

            foreach (var tankId in tankIds)
            {
                var tank = new BunkeringTankDataModel()
                {
                    BunkeringDataId = bunkeringData.Id,
                    TankId = tankId
                };
                var isCommigling = fields.Where(f => f.ReportField.Group.BusinessId == bunkeringData.BunkerGroup && f.ReportField.ValidationKey == "commingling" && f.Value == "1" && f.ReportField.TankId == tankId)
                    .Any();
                if (isCommigling)
                {
                    var targetTimestamp = await _context.BunkeringData
                        .Where(b => b.Id == bunkeringData.Id)
                        .Select(e => e.Events.Where(e => e.EventType.BusinessId == EventType.BunkeringPlan).Single().Timestamp).FirstAsync();
                    var comminglingId = _context.BunkeringDataTanks
                        .Where(b =>
                        b.BunkeringData.UserId == bunkeringData.UserId &&
                        b.TankId == tankId &&
                        b.BunkeringData.Events
                            .Where(e => EventType.BunkeringPlan == e.EventType.BusinessId).Single().Timestamp < targetTimestamp)
                        .OrderByDescending(b => b.BunkeringData.Events.Where(e => EventType.BunkeringPlan == e.EventType.BusinessId).Single().Timestamp)
                        .Select(b => (int?)b.BunkeringDataId)
                        .FirstOrDefault();
                    if (comminglingId != bunkeringData.Id)
                    {
                        tank.ComminglingId = comminglingId;
                    }
                }
                _context.BunkeringDataTanks.Add(tank);
                await _context.SaveChangesAsync();
                _context.Entry(tank).State = EntityState.Detached;
            }

        }

        public async Task<ReportDataModel> UpdateReportAsync(ReportDataModel report)
        {
            var dbReport = await _context.Reports
                .Include(a => a.FieldValues)
                .ThenInclude(a => a.ReportField)
                .ThenInclude(a => a.Group)
                .FirstOrDefaultAsync(a => a.Id == report.Id);

            if (dbReport == null)
            {
                return null;
            }

            foreach (var fieldValue in report.FieldValues)
            {
                var target = dbReport.FieldValues.Single(a => a.ReportFieldId == fieldValue.ReportFieldId);
                if (target == null)
                {
                    // TODO add unique index @fieldvalues reportId-reportFieldId
                    dbReport.FieldValues.Add(new ReportFieldValueDataModel
                    {
                        ReportFieldId = fieldValue.ReportFieldId,
                        ReportId = dbReport.Id,
                        Value = fieldValue.Value
                    });
                }
                else
                {
                    target.Value = fieldValue.Value;
                }
            }

            await _context.SaveChangesAsync();
            await ifUpdateManagedFieldValues(dbReport);

            await UpdateStsData(report.Id);

            return dbReport;
        }

        public async Task<ReportDataModel> GetRelatedReportAsync(int eventId)
        {

            var fact = await _context.Events.Where(_ => _.Id == eventId && EventType.BunkeringPlanProjected != _.EventType.BusinessId && EventType.CommenceBunkeringComplete != _.EventType.BusinessId)
                .FirstOrDefaultAsync();
            if (fact == null)
                return null;

            var prevFacts = _context.Events
                .AsNoTracking()
                .Include(_ => _.EventType)
                .Include(_ => _.Reports)
                .Where(_ => _.UserId == fact.UserId && _.Id != fact.Id && _.Timestamp.HasValue && (_.Timestamp < fact.Timestamp || (_.Timestamp == fact.Timestamp && _.Id < fact.Id)) && !EventType.BunkeringPlanGroup.Contains(_.EventType.BusinessId) && _.Reports.Any())
                .OrderByDescending(_ => _.Timestamp)
                .ThenByDescending(_ => _.Id);

            var targetFact = await prevFacts.FirstOrDefaultAsync(_ => ReportType.Performance.Contains(_.EventType.ReportTypeId ?? -1));

            if (targetFact == null)
            {
                targetFact = await prevFacts.FirstOrDefaultAsync(_ => ReportType.Rob.Contains(_.EventType.ReportTypeId ?? -1));
                if (targetFact != null)
                {
                    var targetReportId = targetFact.Reports.FirstOrDefault()?.Id;
                    var targetReport = await GetReportAsync((int)targetReportId);
                    return targetReport;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var targetReportId = targetFact.Reports.FirstOrDefault()?.Id;
                var targetReport = await GetReportAsync((int)targetReportId);
                targetReport = await MergeRobData(targetReport, fact.Timestamp.Value);
                return targetReport;
            }
        }

        private async Task<ReportDataModel> MergeRobData(ReportDataModel targetReport, DateTimeOffset timestamp)
        {
            // Include rob data to event
            var eventId = targetReport.Event.Id;
            var sourceReportId = await _context.Events
                .Include(_ => _.EventType)
                .Include(_ => _.Reports)
                .Where(_ => !EventType.BunkeringPlanGroup.Contains(_.EventType.BusinessId) && ReportType.Rob.Contains(_.EventType.ReportTypeId ?? -1) && _.UserId == targetReport.Event.UserId && _.Timestamp < timestamp && _.Reports.Any())
                .OrderByDescending(_ => _.Timestamp)
                .ThenByDescending(_ => _.Id)
                .Select(e => e.Reports.Any() ? (int?)e.Reports.First().Id : null)
                .FirstOrDefaultAsync();
            if (sourceReportId == null)
                return targetReport;
            var sourceReport = await GetReportAsync(sourceReportId.Value);
            var sourceFields = sourceReport.FieldValues.ToList();
            if (!sourceFields.Any())
                return targetReport;
            targetReport.FieldValues.Clear();
            foreach (var fieldValue in sourceFields)
            {
                targetReport.FieldValues.Add(fieldValue);
            }
            return targetReport;
        }
        public async Task<ReportDataModel> GetRelatedBunkeringReportAsync(int eventId)
        {

            var targetEvent = await _context.Events.FindAsync(eventId);

            if (targetEvent == null)
                return null;

            var targetBunkeringEvent = await _context.Events
                .Include(_ => _.EventType)
                .Include(_ => _.Reports)
                .Where(_ => _.UserId == targetEvent.UserId && _.Timestamp.HasValue && _.Timestamp < targetEvent.Timestamp && EventType.CommenceBunkeringComplete == _.EventType.BusinessId && _.Reports.Any())
                .OrderByDescending(_ => _.Timestamp)
                .ThenByDescending(_ => _.Id)
                .FirstOrDefaultAsync();

            return targetBunkeringEvent?.Reports.FirstOrDefault();

        }
        public async Task<DateTimeOffset?> GetEventTimeStampFromReportId(int reportId)
        {
            return await _context.Reports
                .Where(a => a.Id == reportId)
                .Select(a => a.Event.Timestamp)
                .FirstOrDefaultAsync();
        }

        public async Task<DateTimeOffset?> GetEventTimeStampFromEventId(int eventId)
        {
            return await _context.Events
                .Where(a => a.Id == eventId)
                .Select(a => a.Timestamp)
                .FirstOrDefaultAsync();
        }

        public async Task<int?> GetEventIdFromReportId(int reportId)
        {
            var eventId = await _context.Reports
                .Where(a => a.Id == reportId)
                .Select(a => a.EventId)
                .FirstOrDefaultAsync();
            return eventId;
        }

        public async Task<ReportDataModel> GetReportAsync(int reportId)
        {
            var report = await _context.Reports
                .Include(a => a.Event)
                .ThenInclude(a => a.User)
                .Include(a => a.Event)
                .ThenInclude(a => a.EventType)
                .ThenInclude(a => a.PairedEventType)
                .Include(a => a.Event)
                .ThenInclude(e => e.EventType)
                .ThenInclude(e => e.ReportType)
                .Include(a => a.Event)
                .ThenInclude(a => a.EventCondition)
                .Include(a => a.Event)
                .ThenInclude(a => a.Port)
                .Include(a => a.Event)
                .ThenInclude(a => a.BunkeringData)
                .Include(a => a.Event)
                .ThenInclude(a => a.Status)
                .Include(a => a.Event)
                .ThenInclude(a => a.Voyage)
                .Include(a => a.FieldValues)
                .ThenInclude(a => a.ReportField)
                .ThenInclude(a => a.Group)
                .Include(a => a.FieldValues)
                .ThenInclude(a => a.ReportField)
                .ThenInclude(a => a.Tank)
                .FirstAsync(a => a.Id == reportId);

            var userTanks = await _context.TankUserSpecs
                .Where(_ => _.UserId == report.Event.UserId)
                .ToListAsync();

            foreach (var field in report.FieldValues)
            {
                if (field.ReportField.Tank == null)
                    continue;
                field.ReportField.UserTank = userTanks.Where(_ => _.TankId == field.ReportField.Tank.Id)
                    .SingleOrDefault();
            }

            var lastReportId = await _context.Reports
                .Where(_ => _.Event.UserId == report.Event.UserId)
                .OrderByDescending(_ => _.Event.Timestamp)
                .ThenByDescending(_ => _.Id)
                .Select(_ => _.Id)
                .FirstOrDefaultAsync();

            if (lastReportId == report.Id && report.Event.EventType.BusinessId == EventType.Noon)
            {
                // TODO: Dont remove this
                report.ReadOnly = false;
            }

            return report;

        }

        public async Task<List<ReportDataModel>> GetReportHistoryAsync(string userId, string conditionId, int page = 1, int pageSize = 10, int? targetEventId = null)
        {

            var targetEvent = await _context.Events
                .Where(e => e.Id == targetEventId && e.UserId == userId)
                .Select(e => new EventDataModel()
                {
                    Id = e.Id,
                    Timestamp = e.Timestamp,
                    EventType = new EventTypeDataModel()
                    {
                        BusinessId = e.EventType.BusinessId
                    }
                })
                .FirstOrDefaultAsync();

            if (targetEvent != null && EventType.BunkeringPlanGroup.Contains(targetEvent.EventType.BusinessId))
            {
                return new List<ReportDataModel>();
            }

            var query = _context.Reports.AsQueryable();

            query = query
                //.AsNoTracking()
                .Include(a => a.Event)
                .ThenInclude(e => e.EventType)
                .ThenInclude(e => e.PairedEventType)
                .Include(a => a.Event)
                .ThenInclude(e => e.EventType)
                .ThenInclude(e => e.ReportType)
                .Include(a => a.Event)
                .ThenInclude(e => e.EventCondition)
                .Include(e => e.Event)
                .ThenInclude(e => e.BunkeringData)
                .Include(a => a.Event)
                .ThenInclude(a => a.Status)
                .Include(a => a.FieldValues)
                .ThenInclude(fv => fv.ReportField)
                .Include(a => a.FieldValues)
                .ThenInclude(fv => fv.ReportField)
                .ThenInclude(fv => fv.Group)
                ;

            query = query.Where(a => a.Event.UserId == userId && !EventType.BunkeringPlanGroup.Contains(a.Event.EventType.BusinessId));

            if (targetEvent?.Timestamp != null)
            {
                query = query.Where(a => a.Event.Timestamp <= targetEvent.Timestamp.Value && a.Event.Id != targetEvent.Id);
            }

            if (!string.IsNullOrEmpty(conditionId))
            {
                var conditionGuid = Guid.Parse(conditionId);
                query = query.Where(a => a.Event.EventCondition.BusinessId == conditionGuid);
            }

            query = query.OrderByDescending(a => a.Event.Timestamp)
                         .ThenByDescending(a => a.Event.Id)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize);

            var reports = await query.ToListAsync();

            var userTanks = await _context.TankUserSpecs
                .Include(t => t.Tank)
                .Where(t => t.UserId == userId)
                .ToListAsync();

            var userTanksDict = userTanks.ToDictionary(ut => ut.TankId);
            foreach (var report in reports)
            {
                foreach (var fieldValue in report.FieldValues)
                {
                    var field = fieldValue.ReportField;
                    if (field.Tank != null && userTanksDict.TryGetValue(field.Tank.Id, out var userTankSpec))
                    {
                        field.UserTank = userTankSpec;
                    }
                }
            }

            return reports;
        }

        public async Task<int> GetReportHistoryCountAsync(string userId, string conditionId, int? targetEventId = null)
        {

            DateTimeOffset? targetEventDate = null;

            if (targetEventId.HasValue)
            {
                targetEventDate = await GetEventTimeStampFromEventId((int)targetEventId);
            }

            var query = _context.Reports.Where(a => a.Event.UserId == userId && !EventType.BunkeringPlanGroup.Contains(a.Event.EventType.BusinessId));
            if (targetEventDate.HasValue)
            {
                query = query.Where(a => a.Event.Timestamp <= targetEventDate && a.Event.Id != targetEventId);
            }
            if (!string.IsNullOrEmpty(conditionId))
            {
                query = query.Where(_ => _.Event.EventCondition.BusinessId == Guid.Parse(conditionId));
            }
            return await query.CountAsync();
        }

        public Task<int?> GetReportIdFromEventId(int eventId)
        {
            return _context.Events.Where(a => a.Id == eventId).Select(a => a.EventType.ReportTypeId).FirstOrDefaultAsync();
        }
        public async Task<ReportDataModel> GetRelatedBunkeringReport(int fieldValueId, string userId)
        {
            var fieldValue = await _context.ReportFieldValues
                .AsNoTracking()
                .Where(a => a.Id == fieldValueId && a.Report.Event.UserId == userId)
                .Select(a => new
                {
                    IsCommenceBunkering = EventType.CommenceBunkering == a.Report.Event.EventType.BusinessId,
                    a.Report.Event.Timestamp,
                    a.ReportField.TankId,
                    a.ReportFieldId,
                    a.Value,
                    a.ReportField.GroupId,
                    ReportId = a.Report.Id,
                    EventId = a.Report.Event.Id
                })
            .FirstAsync();
            if (fieldValue == null)
            {
                return null;
            }
            var targetBdn = fieldValue.Value?.Split(',').Last().Trim().ToUpper();
            var query = _context.Events
                .Where(e => e.UserId == userId && e.BunkeringData != null && e.BunkeringData.Tanks.Any(t => t.TankId == fieldValue.TankId) && e.BunkeringData.Bdn.ToUpper() == targetBdn)
                .AsQueryable();

            if (fieldValue.IsCommenceBunkering)
            {
                query = query
                    .Where(e => e.Timestamp < fieldValue.Timestamp);
            }
            else
            {
                query = query
                    .Where(e => e.Timestamp <= fieldValue.Timestamp);
            }

            var targetEvent = await query
                .OrderByDescending(b => b.BunkeringData.Timestamp)
                .ThenByDescending(b => b.BunkeringData.Id)
                .Select(b => b.BunkeringData.Events.Where(e => EventType.CommenceBunkeringComplete == e.EventType.BusinessId && e.Reports.Any()).FirstOrDefault())
                .FirstOrDefaultAsync();

            if (targetEvent == null)
                return null;

            var targetReport = await _context.Reports
                .Include(b => b.Event)
                .ThenInclude(b => b.BunkeringData)
                .ThenInclude(b => b.Port)
                .Include(b => b.Event)
                .ThenInclude(b => b.BunkeringData)
                .ThenInclude(b => b.Tanks)
                .ThenInclude(b => b.ComminglingData)
                .Where(r => r.EventId == targetEvent.Id)
                .Select(r => new ReportDataModel
                {
                    Id = r.Id,
                    EventId = r.EventId,
                    Event = new EventDataModel()
                    {
                        Timestamp = r.Event.Timestamp,
                        BunkeringData = r.Event.BunkeringData,
                        Attachments = r.Event.Attachments
                    }
                })
            .FirstAsync();

            if (targetReport.Event.HasBunkeringData)
            {

                var totalRobWeight = await GetRobWeight(fieldValue.ReportId, fieldValue.GroupId.Value, targetBdn);

                targetReport.Event.BunkeringData.RobAmount = totalRobWeight.ToString("F3");

                var relatedReport = await GetRelatedReportAsync(fieldValue.EventId);

                if (relatedReport != null)
                {
                    var relatedTotalRobWeight = await GetRobWeight(relatedReport.Id, fieldValue.GroupId.Value, targetBdn);

                    var diff = relatedTotalRobWeight - totalRobWeight;

                    targetReport.Event.BunkeringData.RobAmountDiff = diff.ToString("F3");
                    targetReport.Event.BunkeringData.RobAmountDiffTimestamp = relatedReport.Event.Timestamp.Value;
                }

            }

            var attachments = await _context.EventAttachments
                .Include(a => a.DocumentType)
                .Where(a => a.EventId == targetReport.EventId && a.BunkeringDataId == targetReport.Event.BunkeringData.Id)
                .ToListAsync();

            targetReport.Event.Attachments = attachments;

            return targetReport;
        }

        private async Task<double> GetRobWeight(int reportId, int groupId, string targetBdn)
        {
            var targetTankIds = await _context.Reports
                .Where(r => r.Id == reportId)
                .SelectMany(r => r.FieldValues
                    .Where(fv => fv.ReportField.ValidationKey == "bdn"
                                 && fv.ReportField.GroupId == groupId
                                 && fv.Value != null
                                 && fv.Value.Contains(targetBdn) && fv.ReportField.TankId.HasValue)
                    .Select(fv => (int)fv.ReportField.TankId))
                .ToListAsync();

            var robWeightFields = await _context.ReportFieldValues
                .Include(fv => fv.ReportField)
                .ThenInclude(rf => rf.Group)
                .Where(fv => fv.Report.Id == reportId
                             && fv.ReportField.ValidationKey == "weight"
                             && fv.ReportField.TankId.HasValue
                             && fv.ReportField.GroupId == groupId
                             && targetTankIds.Contains(fv.ReportField.TankId.Value))
                .ToListAsync();

            double totalRobWeight = 0;

            var values = robWeightFields
                .Select(fv => fv.Value)
                .ToList();

            foreach (var value in values)
            {
                totalRobWeight += BunkeringTools.GetDouble2(value);
            }

            return totalRobWeight;
        }

        public async Task<EventAttachmentDataModel> UploadDocument(int bunkeringId, string documentCode, List<IFormFile> files, string userId)
        {
            var file = files.FirstOrDefault();
            if (file == null || file.Length == 0)
                return null;
            var bunkeringData = await _context.BunkeringData.Where(b => b.Id == bunkeringId).FirstOrDefaultAsync();
            var documentType = await _context.DocumentType.Where(d => d.Code == documentCode).FirstOrDefaultAsync();
            if (documentType == null)
            {
                return null;
            }
            var businessFile = new BusinessFile(_attachmentsPath)
            {
                BunkeringId = bunkeringData.BusinessId,
                DocumentTypeId = documentType.BusinessId.ToString(),
            };
            var eventId = _context.Events.Where(e => EventType.CommenceBunkeringComplete == e.EventType.BusinessId && e.BunkeringDataId == bunkeringId).Select(e => e.Id).FirstOrDefault();
            using (var fs = businessFile.GetStream(file.FileName))
            {
                await file.CopyToAsync(fs);
                var attachment = new EventAttachmentDataModel()
                {
                    EventId = eventId,
                    FileName = file.FileName,
                    FileSize = file.Length,
                    MimeType = file.ContentType,
                    BunkeringDataId = bunkeringId,
                    DocumentTypeId = documentType.Id,
                };
                await _context.EventAttachments.AddAsync(attachment);
                await _context.SaveChangesAsync();
                return attachment;
            }

        }

        public async Task DeleteAttachment(int attachmentId)
        {
            var attachment = await _context.EventAttachments
                .Include(a => a.DocumentType)
                .FirstAsync(a => a.Id == attachmentId);

            var bunkeringBusinessId = await _context.BunkeringData.Where(b => b.Id == attachment.BunkeringDataId).Select(b => b.BusinessId).FirstOrDefaultAsync();

            var businessFile = new BusinessFile(_attachmentsPath)
            {
                BunkeringId = bunkeringBusinessId,
                DocumentTypeId = attachment.DocumentType?.BusinessId.ToString(),
            };

            var file = businessFile.GetFile();

            if (file != null)
            {
                file.Delete();
            }

            _context.EventAttachments.Remove(attachment);
            await _context.SaveChangesAsync();

        }
        public async Task<EventAttachmentDataModel> GetAttachment(int attachmentId)
        {
            var attachment = await _context.EventAttachments
               .Include(a => a.DocumentType)
               .FirstAsync(a => a.Id == attachmentId);

            var bunkeringBusinessId = await _context.BunkeringData.Where(b => b.Id == attachment.BunkeringDataId).Select(b => b.BusinessId).FirstOrDefaultAsync();

            var businessFile = new BusinessFile(_attachmentsPath)
            {
                BunkeringId = bunkeringBusinessId,
                DocumentTypeId = attachment.DocumentType?.BusinessId.ToString(),
            };

            var file = businessFile.GetFile();

            if (file.Exists)
            {
                attachment.FilePath = file.FullName;
            }

            return attachment;
        }
        public async Task<List<FileInfo>> GetAttachments(int bunkeringId)
        {
            var fileList = new List<FileInfo>();
            var bunkeringBusinessId = await _context.BunkeringData.Where(b => b.Id == bunkeringId).Select(b => b.BusinessId).FirstOrDefaultAsync();
            if (bunkeringBusinessId == null)
            {
                return fileList;
            }
            var businessFile = new BusinessFile(_attachmentsPath)
            {
                BunkeringId = bunkeringBusinessId
            };
            return businessFile.GetFiles();
        }
        public async Task<List<ReportFieldValueDataModel>> GetTransferDetailsAsync(string userId)
        {
            var latestReport = await _context.Reports
                .Where(e =>
                    e.Event.UserId == userId &&
                    e.Event.Timestamp.HasValue &&
                    !ReportType.ExcludedEventTypes.Contains(e.Event.EventType.BusinessId))
                .OrderByDescending(e => e.Event.Timestamp)
                .FirstOrDefaultAsync();

            if (latestReport == null)
            {
                return new List<ReportFieldValueDataModel>();
            }

            var fieldValues = await _context.ReportFieldValues
                .Include(_ => _.ReportField)
                .ThenInclude(_ => _.Group)
                .Where(_ =>
                    _.Report.Event.Id == latestReport.EventId &&
                    _.ReportField.ValidationKey == "volume" &&
                    _.ReportField.Group != null &&
                    ReportType.ActualGroups.Contains(_.ReportField.Group.BusinessId))
                .ToListAsync();

            foreach (var fieldValue in fieldValues)
            {
                fieldValue.ReportField.UserTank = await _context.TankUserSpecs
                    .Include(_ => _.Tank)
                    .Where(_ => _.TankId == fieldValue.ReportField.TankId && _.UserId == userId)
                    .FirstAsync();
            }

            return fieldValues;
        }

        public async Task<bool> isBunkeringPlan(int eventId)
        {
            return await _context.Events
                .Where(e => e.Id == eventId && EventType.BunkeringPlanGroup.Contains(e.EventType.BusinessId))
                .AnyAsync();
        }

        public async Task UpdateBunkeringSupplier(string userId, int bunkeringId, string supplier)
        {
            var bunkeringData = await _context.BunkeringData
                .Where(b => b.Id == bunkeringId && b.UserId == userId)
                .FirstOrDefaultAsync();
            if (bunkeringData == null)
            {
                throw new ArgumentNullException();
            }
            bunkeringData.Supplier = supplier.ToUpper();
            await _context.SaveChangesAsync();
        }
        public async Task UpdateBunkeringNamedValue(string userId, int bunkeringId, string namedValue)
        {
            var bunkeringData = await _context.BunkeringData
                .Where(b => b.Id == bunkeringId && b.UserId == userId)
                .FirstOrDefaultAsync();
            if (bunkeringData == null)
            {
                throw new ArgumentNullException();
            }
            bunkeringData.NamedAmount = string.IsNullOrEmpty(namedValue) ? null : namedValue;
            await _context.SaveChangesAsync();
        }
        public async Task<List<WaterConsumptionViewModel>> GetWaterConsumptions(DateTime from, DateTime to)
        {

            var userAnnualAmounts = new List<WaterConsumptionViewModel>();
            var targetFields = new List<string>()
            {
                "freshWaterDomestic",
                "freshWaterPotable",
                "freshWaterTankCleaning",
                "distilledWater",
            };
            var targetConditions = new List<int>()
            {
                3, 6
            };
            var users = await _context.Users.AsNoTracking().OrderBy(u => u.UserName)
                .Select(u => new User()
                {
                    Id = u.Id,
                    UserName = u.UserName.ToUpper(),
                    Prefix = u.Prefix.ToUpper(),
                })
                .ToListAsync();
            foreach (var user in users)
            {
                var reports = await _context.Reports
                    .AsNoTracking()
                    .Where(r => r.Event.UserId == user.Id &&
                                r.Event.EventType.ReportTypeId == 1 &&
                                r.Event.Timestamp.HasValue &&
                                r.Event.Timestamp >= from &&
                                r.Event.Timestamp <= to)
                    .OrderBy(r => r.Event.Timestamp)
                    .Select(r => new ReportDataModel()
                    {
                        Event = new EventDataModel()
                        {
                            ConditionId = r.Event.ConditionId,
                            Timestamp = r.Event.Timestamp,
                            User = new User()
                            {
                                Id = r.Event.UserId,
                                UserName = r.Event.User.UserName.ToUpper(),
                                Prefix = r.Event.User.Prefix.ToUpper()
                            }
                        },
                        FieldValues = r.FieldValues
                            .Where(fv => !fv.ReportField.GroupId.HasValue && targetFields.Contains(fv.ReportField.ValidationKey))
                            .Select(fv => new ReportFieldValueDataModel()
                            {
                                Value = fv.Value,
                                ReportField = new ReportFieldDataModel()
                                {
                                    ValidationKey = fv.ReportField.ValidationKey
                                }
                            })
                            .ToList(),
                    })
                    .ToListAsync();
                WaterConsumptionViewModel item = null;
                for (var i = 0; i < reports.Count; i++)
                {
                    var prevReport = i > 0 ? reports[i - 1] : null;
                    var report = reports[i];
                    var prevReportDate = prevReport?.Event.Timestamp.Value.DateTime.ToUniversalTime();
                    var reportDate = report.Event.Timestamp.Value.DateTime.ToUniversalTime();
                    if (item == null || (prevReportDate.HasValue && prevReportDate.Value.Year != reportDate.Year))
                    {
                        item = new WaterConsumptionViewModel()
                        {
                            AmountConsumed = 0,
                            AmountLoaded = 0,
                            AmountLoadedAnchor = 0,
                            VesselId = user.Id,
                            VesselName = user.UserName,
                            VesselPrefix = user.Prefix,
                            Year = reportDate.Year
                        };
                        userAnnualAmounts.Add(item);
                    }
                    foreach (var targetField in targetFields)
                    {
                        var currentStr = report.FieldValues.FirstOrDefault(fv => fv.ReportField.ValidationKey == targetField)?.Value;
                        var prevStr = prevReport?.FieldValues.FirstOrDefault(fv => fv.ReportField.ValidationKey == targetField)?.Value;

                        if (double.TryParse(currentStr, out var currentValue) &&
                            double.TryParse(prevStr, out var prevValue))
                        {
                            if (prevValue > currentValue)
                            {
                                item.AmountConsumed += prevValue - currentValue;
                            }
                            else if (prevValue < currentValue)
                            {
                                item.AmountLoaded += currentValue - prevValue;
                                if (targetConditions.Contains(report.Event.ConditionId))
                                {
                                    item.AmountLoadedAnchor += currentValue - prevValue;
                                }
                            }
                        }
                    }

                }
            }
            return userAnnualAmounts;
        }


        // TODO: reportcontext general info dont make changes
        public async Task<List<BunkeringDataModel>> GetBunkeringData(int eventId)
        {

            var userEvent = await _context.Events.Where(e => e.Id == eventId).FirstAsync();

            var targetGroups = new List<int?>() { 2, 4 };

            var fieldValues = await _context
                .ReportFieldValues
                .Include(rfv => rfv.ReportField)
                .Where(rfv => rfv.Report.EventId == eventId && rfv.ReportField.ValidationKey == "bdn" && targetGroups.Contains(rfv.ReportField.GroupId))
                .ToListAsync();

            var grouped = fieldValues
                .GroupBy(rfv => rfv.ReportField.GroupId.Value)
                .Select(g => new
                {
                    GroupId = g.Key,
                    Values = g
                        .SelectMany(rfv => (rfv.Value ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        .Select(val => val.Trim().ToUpperInvariant())
                        .ToHashSet()
                })
                .ToList();

            var bunkeringDataList = new List<BunkeringDataModel>();

            foreach (var group in grouped)
            {
                var fuelType = group.GroupId == 2 ? 1 : 2;

                foreach (var bdn in group.Values)
                {
                    var targetBunkeringEvent = await _context.Events
                        .Where(e =>
                            e.UserId == userEvent.UserId &&
                            e.EventType.BusinessId == EventType.CommenceBunkeringComplete &&
                            e.Timestamp.HasValue &&
                            e.Timestamp <= userEvent.Timestamp &&
                            e.BunkeringDataId != null &&
                            e.BunkeringData.FuelType == fuelType &&
                            e.BunkeringData.Bdn == bdn.Trim().ToUpper()
                            )
                        .Select(e => new EventDataModel()
                        {
                            Id = e.Id,
                            UserId = e.UserId,
                            EventTypeId = e.EventTypeId,
                            Timestamp = e.Timestamp,
                            BunkeringData = e.BunkeringData
                        })
                        .OrderByDescending(e => e.Timestamp)
                        .FirstOrDefaultAsync();
                    if (targetBunkeringEvent == null)
                    {
                        continue;
                    }
                    var tankIds = await _context
                        .ReportFieldValues
                        .Include(rfv => rfv.ReportField)
                        .Where(rfv =>
                            rfv.Report.Event.UserId == targetBunkeringEvent.UserId &&
                            rfv.Report.EventId == userEvent.Id &&
                            rfv.ReportField.ValidationKey == "bdn" &&
                            rfv.Value.ToUpper() == targetBunkeringEvent.BunkeringData.Bdn.ToUpper() &&
                            rfv.ReportField.GroupId == group.GroupId
                            )
                        .Select(rfv => rfv.ReportField.TankId)
                        .Distinct()
                        .ToListAsync();
                    var stringValues = await _context
                        .ReportFieldValues
                        .Include(rfv => rfv.ReportField)
                        .Where(rfv =>
                            rfv.Report.Event.UserId == targetBunkeringEvent.UserId &&
                            rfv.Report.EventId == userEvent.Id &&
                            //rfv.Report.Event.Id < userEvent.Id &&
                            //rfv.Report.Event.Id > targetBunkeringEvent.Id &&
                            //rfv.Report.Event.Timestamp <= userEvent.Timestamp &&
                            //rfv.Report.Event.Timestamp >= targetBunkeringEvent.Timestamp &&
                            tankIds.Contains(rfv.ReportField.TankId) &&
                            rfv.ReportField.ValidationKey == "weight" &&
                            rfv.ReportField.GroupId == group.GroupId
                            )
                        .Select(rfv => rfv.Value)
                        .ToListAsync();
                    double total = stringValues
                      .Select(s => double.TryParse(s, out var val) ? val : (double?)null)
                      .Where(val => val.HasValue)
                      .Sum(val => val.Value);
                    var targetAmount = targetBunkeringEvent.BunkeringData.NamedAmount ?? targetBunkeringEvent.BunkeringData.TotalAmount;
                    var dblTargetAmount = double.TryParse(targetAmount, out var d) ? d : 0.0;
                    targetBunkeringEvent.BunkeringData.RobAmount = (total).ToString("F3");
                    bunkeringDataList.Add(targetBunkeringEvent.BunkeringData);
                }
            }
            return bunkeringDataList;

        }

        public IQueryable<ReportDataModel> GetVesselReportQuery()
        {

            return _context.Reports
                .AsNoTracking()
                .AsQueryable();
        }



    }

}
