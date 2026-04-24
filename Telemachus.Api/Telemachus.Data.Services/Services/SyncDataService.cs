using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using EFCore.BulkExtensions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using Operations.Data;

using Telemachus.Data.Models.Authentication;
using Telemachus.Data.Models.Cargo;
using Telemachus.Data.Models.Events;
using Telemachus.Data.Models.Ports;
using Telemachus.Data.Models.Reports;
using Telemachus.Data.Models.Sync;
using Telemachus.Data.Models.Sync.Data;
using Telemachus.Data.Services.Context;
using Telemachus.Data.Services.Interfaces;
using Telemachus.Models;

using Sync = Telemachus.Data.Models.Sync.MasterData;

namespace Telemachus.Data.Services.Services
{
    public class SyncDataService : ISyncDataService
    {
        private readonly TelemachusContext _context;
        private readonly OperationsDbContext _operationsDbContext;
        private readonly HttpClient _httpClient;
        private bool _isInHouse = false;
        private readonly UserManager<User> _userManager;
        private readonly string _remoteIp;
        public SyncDataService(IHttpContextAccessor httpContext, UserManager<User> userManager, HttpClient httpClient, TelemachusContext context, IConfiguration config, OperationsDbContext operationsContext)
        {
            _context = context;
            _operationsDbContext = operationsContext;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _userManager = userManager;
            var passcode = config.GetValue<string>("SyncCredentials:Passcode");
            _httpClient.DefaultRequestHeaders.Add("X-Passcode", passcode);
            _remoteIp = httpContext.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            _isInHouse = !config.GetSection("VesselDetails").Exists();
        }
        public async Task PortSync()
        {

            if (!_isInHouse)
            {
                throw new NotImplementedException();
            }
            var areasLast = (await _context.Area.MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;

            var areas = await _operationsDbContext
                .Area
                .Where(_ => _.DateModified > areasLast)
                .OrderBy(_ => _.Id)
                .ToListAsync();

            var areaCoordinateLast = (await _context.AreaCoordinate.MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;

            var areaCoordinates = await _operationsDbContext
                .AreaCoordinate
                .Include(_ => _.Area)
                .Where(_ => _.DateModified > areaCoordinateLast)
                .OrderBy(_ => _.Id)
                .ToListAsync();

            var regionLast = (await _context.Region.MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;

            var regions = await _operationsDbContext
                .Region
                .Include(_ => _.Area)
                .Where(_ => _.DateModified > regionLast)
                .OrderBy(_ => _.Id)
                .ToListAsync();

            var countryLast = (await _context.Country.MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;

            var countries = await _operationsDbContext
                .Country
                .Include(_ => _.Region)
                .Where(_ => _.DateModified > countryLast)
                .OrderBy(_ => _.Id)
                .ToListAsync();

            var portLast = (await _context.Port.MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;

            var ports = await _operationsDbContext
                .Port
                .Include(_ => _.Region)
                .Include(_ => _.Area)
                .Include(_ => _.Country)
                .Where(_ => _.DateModified > portLast)
                .OrderBy(_ => _.Id)
                .ToListAsync();

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var areasToAdd = new List<Area>();
                    foreach (var item in areas.OrderBy(_ => _.Id))
                    {
                        var val = await _context.Area.SingleOrDefaultAsync(_ => _.BusinessId == item.BusinessId);
                        if (val == null)
                        {
                            val = new Area()
                            {
                                Name = item.Name,
                                Code = item.Code,
                                DateModified = item.DateModified,
                                BusinessId = item.BusinessId
                            };
                            areasToAdd.Add(val);
                        }
                        else
                        {
                            val.Name = item.Name;
                            val.Code = item.Code;
                            val.DateModified = item.DateModified;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (areasToAdd.Any())
                        await _context.BulkInsertAsync(areasToAdd, options => options.PreserveInsertOrder = true);
                    var coordinatesToAdd = new List<AreaCoordinate>();
                    foreach (var item in areaCoordinates.OrderBy(_ => _.Id))
                    {
                        var val = await _context.AreaCoordinate
                            .SingleOrDefaultAsync(_ => _.BusinessId == item.BusinessId);

                        var areaId = await _context.Area.Where(_ => _.BusinessId == item.Area.BusinessId).Select(_ => _.Id).SingleAsync();

                        if (val == null)
                        {
                            val = new AreaCoordinate()
                            {
                                AreaId = areaId,
                                Lng = item.Lng,
                                Lat = item.Lat,
                                PointIndex = item.PointIndex,
                                DateModified = item.DateModified,
                                BusinessId = item.BusinessId
                            };
                            coordinatesToAdd.Add(val);
                        }
                        else
                        {
                            val.AreaId = areaId;
                            val.Lng = item.Lng;
                            val.Lat = item.Lat;
                            val.PointIndex = item.PointIndex;
                            val.DateModified = item.DateModified;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (coordinatesToAdd.Any())
                        await _context.BulkInsertAsync(coordinatesToAdd, options => options.PreserveInsertOrder = true);
                    var regionsToAdd = new List<Region>();
                    foreach (var item in regions.OrderBy(_ => _.Id))
                    {
                        var val = await _context.Region.SingleOrDefaultAsync(_ => _.BusinessId == item.BusinessId);

                        int? areaId = null;

                        if (item.Area != null)
                            areaId = await _context.Area.Where(_ => _.BusinessId == item.Area.BusinessId).Select(_ => _.Id).SingleAsync();

                        if (val == null)
                        {
                            val = new Region()
                            {
                                Name = item.Name,
                                AreaId = areaId,
                                DateModified = item.DateModified,
                                BusinessId = item.BusinessId
                            };
                            regionsToAdd.Add(val);
                        }
                        else
                        {
                            val.Name = item.Name;
                            val.AreaId = areaId;
                            val.DateModified = item.DateModified;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (regionsToAdd.Any())
                        await _context.BulkInsertAsync(regionsToAdd, options => options.PreserveInsertOrder = true);
                    var countriesToAdd = new List<Country>();
                    foreach (var item in countries.OrderBy(_ => _.Id))
                    {
                        var val = await _context.Country.SingleOrDefaultAsync(_ => _.BusinessId == item.BusinessId);

                        int? regionId = null;

                        if (item.Region != null)
                            regionId = await _context.Region.Where(_ => _.BusinessId == item.Region.BusinessId).Select(_ => _.Id).SingleAsync();

                        if (val == null)
                        {
                            val = new Country()
                            {
                                Numerical = item.Numerical,
                                Name = item.Name ?? "",
                                Alpha2 = item.Alpha2,
                                Alpha3 = item.Alpha3,
                                Nationality = item.Nationality,
                                RegionId = regionId,
                                LloydsCode = item.LloydsCode,
                                PhoneCode = item.PhoneCode,
                                DateModified = item.DateModified,
                                BusinessId = item.BusinessId
                            };
                            countriesToAdd.Add(val);
                        }
                        else
                        {
                            val.Numerical = item.Numerical;
                            val.Name = item.Name ?? "";
                            val.Alpha2 = item.Alpha2;
                            val.Alpha3 = item.Alpha3;
                            val.Nationality = item.Nationality;
                            val.RegionId = regionId;
                            val.LloydsCode = item.LloydsCode;
                            val.PhoneCode = item.PhoneCode;
                            val.DateModified = item.DateModified;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (countriesToAdd.Any())
                        await _context.BulkInsertAsync(countriesToAdd, options => options.PreserveInsertOrder = true);
                    var portsToAdd = new List<Models.Ports.Port>();
                    foreach (var item in ports.OrderBy(_ => _.Id))
                    {
                        var val = await _context.Port.SingleOrDefaultAsync(_ => _.BusinessId == item.BusinessId);

                        var regionId = await _context.Region.Where(_ => _.BusinessId == item.Region.BusinessId).Select(_ => _.Id).SingleAsync();
                        var countryId = await _context.Country.Where(_ => _.BusinessId == item.Country.BusinessId).Select(_ => _.Id).SingleAsync();
                        var areaId = await _context.Area.Where(_ => _.BusinessId == item.Area.BusinessId).Select(_ => _.Id).SingleAsync();

                        var hasCoords = item.Lat.HasValue && item.Lng.HasValue && !(item.Lat == 0 && item.Lng == 0);

                        if (val == null)
                        {
                            val = new Telemachus.Data.Models.Ports.Port()
                            {
                                Name = item.Name ?? "",
                                Latitude = hasCoords ? Math.Round((decimal)item.Lat, 6) : (decimal?)null,
                                Longitude = hasCoords ? Math.Round((decimal)item.Lng, 6) : (decimal?)null,
                                Code = item.Code,
                                CountryId = countryId,
                                RegionId = regionId,
                                AreaId = areaId,
                                TimeZone = item.TimeZone,
                                IsEuInt = item.IsEuInt,
                                DateModified = item.DateModified,
                                BusinessId = item.BusinessId
                            };
                            portsToAdd.Add(val);
                        }
                        else
                        {
                            val.Name = item.Name ?? "";
                            val.Latitude = hasCoords ? Math.Round((decimal)item.Lat, 6) : (decimal?)null;
                            val.Longitude = hasCoords ? Math.Round((decimal)item.Lng, 6) : (decimal?)null;
                            val.Code = item.Code;
                            val.CountryId = countryId;
                            val.RegionId = regionId;
                            val.AreaId = areaId;
                            val.TimeZone = item.TimeZone;
                            val.IsEuInt = item.IsEuInt;
                            val.DateModified = item.DateModified;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (portsToAdd.Any())
                        await _context.BulkInsertAsync(portsToAdd, options => options.PreserveInsertOrder = true);
                    await transaction.CommitAsync();

                }
                catch (Exception)
                {
                    // Rollback the transaction if an exception occurs
                    await transaction.RollbackAsync();
                    throw; // Rethrow the exception to handle it at a higher level
                }
            }
        }
        public async Task<bool> HasValidRemoteAddress(string userId)
        {
            var hasValidRemoteAddress = await _context.Users.Where(_ => _.Id == userId && !string.IsNullOrEmpty(_.RemoteAddress)).AnyAsync();
            return hasValidRemoteAddress;
        }
        private async Task<VesselDetails> GetVesselDetails(string userPrefix)
        {
            var vesselDetails = await _context.Users.Where(_ => _.Prefix.ToUpper() == userPrefix.ToUpper()).Select(_ => new VesselDetails()
            {
                Prefix = _.Prefix,
                Operator = _.Operator,
                PitchPropeller = _.PitchPropeller,
                MainEngineMaxPower = _.MainEngineMaxPower,
                AvailablePasscodeSlots = _.AvailablePasscodeSlots,
                Name = _.UserName,
                RemoteAddress = _.RemoteAddress,
                RemotePort = _.RemotePort,
                NonHafnia = _.NonHafnia,
                NonPool = _.NonPool
            }).SingleOrDefaultAsync();
            return vesselDetails;
        }
        public async Task<SyncRequestViewModel> GetDataTimestamps(string userPrefix)
        {
            var userId = await _context.Users.Where(_ => _.Prefix.ToUpper() == userPrefix.ToUpper()).Select(_ => _.Id).SingleAsync();

            var voyages = (await _context.Voyages.Where(_ => _.UserId == userId).IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var events = (await _context.Events.Where(_ => _.UserId == userId).IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var eventAttachments = (await _context.EventAttachments.Include(_ => _.Event).Where(_ => _.Event.UserId == userId).IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var reports = (await _context.Reports.Include(_ => _.Event).Where(_ => _.Event.UserId == userId).IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var reportFieldValues = (await _context.ReportFieldValues.Include(_ => _.Report).ThenInclude(_ => _.Event).Where(_ => _.Report.Event.UserId == userId).IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var statementOfFacts = (await _context.StatementOfFact.Where(_ => _.UserId == userId).IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var bunkeringData = (await _context.BunkeringData.Where(_ => _.UserId == userId).IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var bunkeringTanks = (await _context.BunkeringDataTanks.Where(_ => _.BunkeringData.UserId == userId).IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var cargoes = (await _context.Cargoes.Where(_ => _.UserId == userId).IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var cargoDetails = (await _context.CargoDetails.Where(_ => _.Cargo.UserId == userId).IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;


            var viewModel = new SyncRequestViewModel()
            {
                UserPrefix = userPrefix.ToUpper(),
                Voyages = voyages,
                Events = events,
                EventAttachments = eventAttachments,
                Reports = reports,
                ReportFieldValues = reportFieldValues,
                StatementOfFacts = statementOfFacts,
                BunkeringData = bunkeringData,
                BunkeringDataTanks = bunkeringTanks,
                Cargoes = cargoes,
                CargoDetails = cargoDetails
            };
            return viewModel;
        }
        private async Task<SyncRequestViewModel> FetchDataTimestamps(string userPrefix)
        {

            var jsonData = JsonConvert.SerializeObject(userPrefix.ToUpper());

            var vesselDetails = await GetVesselDetails(userPrefix);

            UriBuilder uriBuilder = new UriBuilder
            {
                Scheme = "http",
                Port = vesselDetails.RemotePort,
                Host = vesselDetails.RemoteAddress,
                Path = "/api/sync/timestamps",
                Query = null
            };

            var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.ToString())
            {
                Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
            };

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var downloadedData = await new StreamReader(responseStream).ReadToEndAsync();
                    var responseObject = JsonConvert.DeserializeObject<SyncRequestViewModel>(downloadedData);
                    return responseObject;
                }
            }
        }
        private async Task<SyncResponseViewModel> SendData(SyncResponseViewModel vessel)
        {
            var jsonData = JsonConvert.SerializeObject(vessel);

            var vesselDetails = await GetVesselDetails(vessel.User);

            UriBuilder uriBuilder = new UriBuilder
            {
                Scheme = "http",
                Port = vesselDetails.RemotePort,
                Host = vesselDetails.RemoteAddress,
                Path = "/api/sync/data",
                Query = null
            };

            var request = new HttpRequestMessage(HttpMethod.Post, uriBuilder.ToString())
            {
                Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
            };

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var downloadedData = await new StreamReader(responseStream).ReadToEndAsync();
                    try
                    {
                        var responseObject = JsonConvert.DeserializeObject<SyncResponseViewModel>(downloadedData);
                        return responseObject;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
        }
        public async Task SyncDataValues(string userId)
        {

            if (_isInHouse)
            {
                throw new NotImplementedException();
            }
            var userPrefix = await _context.Users.Where(_ => _.Id == userId).Select(_ => _.Prefix.ToUpper()).SingleAsync();
            var isInitialized = await _context.Voyages.IgnoreQueryFilters().Where(u => u.UserId == userId).AnyAsync();
            if (!isInitialized)
            {
                var timestamps = await GetDataTimestamps(userPrefix);
                var vessel = await FetchDataAsync(timestamps);
                await SaveData(vessel);
            }
            else
            {
                var remoteTimestamps = await FetchDataTimestamps(userPrefix);
                var data = await GetDataAsync(remoteTimestamps);
                var localTimestamps = await GetDataTimestamps(userPrefix);
                data.LocalTimestamps = localTimestamps;
                var newData = await SendData(data);
                await SaveReadOnlyData(newData);
            }
        }
        private async Task SaveReadOnlyData(SyncResponseViewModel vessel)
        {

            _context.SoftDeleteEnabled = false;
            _context.UpdateTimestamps = false;
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var userId = await _context.Users.Where(_ => _.Prefix.ToUpper() == vessel.User.ToUpper()).Select(_ => _.Id).SingleAsync();

                    var data = vessel.Data;
                    foreach (var item in data.Events.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.Events.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);

                        if (currentValue == null)
                        {
                            continue;
                        }

                        var statusId = await _context.EventStatuses.IgnoreQueryFilters().Where(_ => _.BusinessId == item.StatusId).Select(_ => _.Id).SingleAsync();

                        if (currentValue.StatusId == statusId)
                        {
                            continue;
                        }
                        currentValue.StatusId = statusId;
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            _context.SoftDeleteEnabled = true;
            _context.UpdateTimestamps = true;
        }
        public async Task SaveData(SyncResponseViewModel vessel)
        {

            _context.SoftDeleteEnabled = false;
            _context.UpdateTimestamps = false;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var userId = await _context.Users.Where(_ => _.Prefix.ToUpper() == vessel.User.ToUpper()).Select(_ => _.Id).SingleAsync();

                    var data = vessel.Data;

                    var cargoes = new List<CargoModel>();
                    foreach (var item in data.Cargoes.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.Cargoes.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        var gradeId = await _context.Grades.IgnoreQueryFilters().Where(_ => _.BusinessId == item.GradeId).Select(_ => _.Id).SingleAsync();
                        if (currentValue == null)
                        {
                            currentValue = new CargoModel()
                            {
                                UserId = userId,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id,
                                GradeId = gradeId,
                                Parcel = item.Parcel,
                                StartedOn = item.StartedOn,
                                CompletedOn = item.CompletedOn
                            };
                            cargoes.Add(currentValue);
                        }
                        else
                        {
                            currentValue.DateModified = item.DateModified;
                            currentValue.GradeId = gradeId;
                            currentValue.Parcel = item.Parcel;
                            currentValue.StartedOn = item.StartedOn;
                            currentValue.CompletedOn = item.CompletedOn;
                            currentValue.IsDeleted = item.IsDeleted;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (cargoes.Any())
                    {
                        await _context.BulkInsertAsync(cargoes, options =>
                        {
                            options.PreserveInsertOrder = true;
                        });
                    }
                    var cargoeDetails = new List<CargoDetailModel>();
                    foreach (var item in data.CargoDetails.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.CargoDetails.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        var cargoId = await _context.Cargoes.IgnoreQueryFilters().Where(_ => _.BusinessId == item.CargoId).Select(_ => _.Id).SingleAsync();
                        if (currentValue == null)
                        {
                            currentValue = new CargoDetailModel()
                            {
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id,
                                CargoId = cargoId,
                                Timestamp = item.Timestamp,
                                Quantity = item.Quantity
                            };
                            cargoeDetails.Add(currentValue);
                        }
                        else
                        {
                            currentValue.DateModified = item.DateModified;
                            currentValue.CargoId = cargoId;
                            currentValue.Timestamp = item.Timestamp;
                            currentValue.Quantity = item.Quantity;
                            currentValue.IsDeleted = item.IsDeleted;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (cargoeDetails.Any())
                    {
                        await _context.BulkInsertAsync(cargoeDetails, options =>
                        {
                            options.PreserveInsertOrder = true;
                        });
                    }
                    var bunkeringData = new List<BunkeringDataModel>();
                    foreach (var item in data.BunkeringData.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.BunkeringData.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        var portId = await _context.Port.IgnoreQueryFilters().Where(_ => _.BusinessId == item.PortId).Select(_ => _.Id).SingleAsync();
                        if (currentValue == null)
                        {
                            currentValue = new BunkeringDataModel()
                            {
                                UserId = userId,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id,
                                Bdn = item.Bdn,
                                PortId = portId,
                                FuelType = item.FuelType,
                                Supplier = item.Supplier,
                                Timestamp = item.Timestamp,
                                TotalAmount = item.TotalAmount,
                                Density = item.Density,
                                SulphurContent = item.SulphurContent,
                                Viscosity = item.Viscosity
                            };
                            bunkeringData.Add(currentValue);
                        }
                        else
                        {
                            currentValue.DateModified = item.DateModified;
                            currentValue.Bdn = item.Bdn;
                            currentValue.FuelType = item.FuelType;
                            currentValue.Supplier = item.Supplier;
                            currentValue.Timestamp = item.Timestamp;
                            currentValue.TotalAmount = item.TotalAmount;
                            currentValue.Density = item.Density;
                            currentValue.SulphurContent = item.SulphurContent;
                            currentValue.Viscosity = item.Viscosity;
                            currentValue.IsDeleted = item.IsDeleted;
                            currentValue.PortId = portId;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (bunkeringData.Any())
                    {
                        await _context.BulkInsertAsync(bunkeringData, options =>
                        {
                            options.PreserveInsertOrder = true;
                        });
                    }
                    var bunkeringDataTanks = new List<BunkeringTankDataModel>();
                    foreach (var item in data.BunkeringDataTanks.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.BunkeringDataTanks.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        var bunkeringDataId = await _context.BunkeringData.IgnoreQueryFilters().Where(_ => _.BusinessId == item.BunkeringDataId).Select(_ => _.Id).SingleAsync();
                        var comminglingId = await _context.BunkeringData.IgnoreQueryFilters().Where(_ => _.BusinessId == item.ComminglingId).Select(_ => (int?)_.Id).SingleOrDefaultAsync();
                        var tankId = await _context.Tanks.IgnoreQueryFilters().Where(_ => _.BusinessId == item.TankId).Select(_ => _.Id).SingleAsync();
                        if (currentValue == null)
                        {
                            currentValue = new BunkeringTankDataModel()
                            {
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id,
                                BunkeringDataId = bunkeringDataId,
                                ComminglingId = comminglingId,
                                TankId = tankId,
                                Amount = item.Amount
                            };
                            bunkeringDataTanks.Add(currentValue);
                        }
                        else
                        {
                            currentValue.DateModified = item.DateModified;
                            currentValue.Amount = item.Amount;
                            currentValue.IsDeleted = item.IsDeleted;
                            currentValue.BunkeringDataId = bunkeringDataId;
                            currentValue.ComminglingId = comminglingId;
                            currentValue.TankId = tankId;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (bunkeringDataTanks.Any())
                    {
                        await _context.BulkInsertAsync(bunkeringDataTanks, options =>
                        {
                            options.PreserveInsertOrder = true;
                        });
                    }
                    var voyages = new List<VoyageDataModel>();
                    foreach (var item in data.Voyages.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.Voyages.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        var currentConditionId = await _context.EventConditions.IgnoreQueryFilters().Where(_ => _.BusinessId == item.CurrentConditionId).Select(_ => _.Id).SingleAsync();
                        if (currentValue == null)
                        {
                            currentValue = new VoyageDataModel()
                            {
                                UserId = userId,
                                StartDate = item.StartDate,
                                EndDate = item.EndDate,
                                IsFinished = item.IsFinished,
                                CurrentConditionId = currentConditionId,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id
                            };
                            voyages.Add(currentValue);
                        }
                        else
                        {
                            currentValue.StartDate = item.StartDate;
                            currentValue.EndDate = item.EndDate;
                            currentValue.IsFinished = item.IsFinished;
                            currentValue.CurrentConditionId = currentConditionId;
                            currentValue.DateModified = item.DateModified;
                            currentValue.IsDeleted = item.IsDeleted;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (voyages.Any())
                    {
                        await _context.BulkInsertAsync(voyages, options =>
                        {
                            options.PreserveInsertOrder = true;
                        });
                    }
                    var events = new List<EventDataModel>();
                    foreach (var item in data.Events.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.Events.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        var statusId = await _context.EventStatuses.IgnoreQueryFilters().Where(_ => _.BusinessId == item.StatusId).Select(_ => _.Id).SingleAsync();
                        var eventTypeId = await _context.EventTypes.IgnoreQueryFilters().Where(_ => _.BusinessId == item.EventTypeId).Select(_ => _.Id).SingleAsync();
                        var conditionId = await _context.EventConditions.IgnoreQueryFilters().Where(_ => _.BusinessId == item.ConditionId).Select(_ => _.Id).SingleAsync();
                        var portId = await _context.Port.IgnoreQueryFilters().Where(_ => _.BusinessId == item.PortId).Select(_ => _.Id).SingleOrDefaultAsync();
                        var bunkeringDataId = await _context.BunkeringData.IgnoreQueryFilters().Where(_ => _.BusinessId == item.BunkeringDataId).Select(_ => (int?)_.Id).SingleOrDefaultAsync();
                        var voyageId = await _context.Voyages.IgnoreQueryFilters().Where(_ => _.BusinessId == item.VoyageId).Select(_ => (int?)_.Id).SingleOrDefaultAsync();
                        if (voyageId == null)
                            continue;
                        if (currentValue == null)
                        {
                            currentValue = new EventDataModel()
                            {
                                Timestamp = item.Timestamp,
                                UserId = userId,
                                Terminal = item.Terminal,
                                StatusId = statusId,
                                EventTypeId = eventTypeId,
                                ConditionId = conditionId,
                                VoyageId = voyageId.Value,
                                CurrentVoyageConditionKey = item.CurrentVoyageConditionKey,
                                NextVoyageConditionKey = item.NextVoyageConditionKey,
                                PreviousVoyageConditionKey = item.PreviousVoyageConditionKey,
                                Comment = item.Comment,
                                ConditionStartedDate = item.ConditionStartedDate,
                                CustomEventName = item.CustomEventName,
                                ExcludeFromStatement = item.ExcludeFromStatement,
                                HiddenDate = item.HiddenDate,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id,
                                LatDegrees = item.LatDegrees,
                                LatMinutes = item.LatMinutes,
                                LatSeconds = item.LatSeconds,
                                LongDegrees = item.LongDegrees,
                                LongMinutes = item.LongMinutes,
                                LongSeconds = item.LongSeconds,
                                Lat = item.Lat,
                                Lng = item.Lng,
                                BunkeringDataId = bunkeringDataId,
                                PortId = portId,
                            };
                            events.Add(currentValue);
                        }
                        else
                        {
                            currentValue.Terminal = item.Terminal;
                            currentValue.StatusId = statusId;
                            currentValue.Comment = item.Comment;
                            currentValue.CustomEventName = item.CustomEventName;
                            currentValue.ExcludeFromStatement = item.ExcludeFromStatement;
                            currentValue.HiddenDate = item.HiddenDate;
                            currentValue.DateModified = item.DateModified;
                            currentValue.LatDegrees = item.LatDegrees;
                            currentValue.LatMinutes = item.LatMinutes;
                            currentValue.LatSeconds = item.LatSeconds;
                            currentValue.LongDegrees = item.LongDegrees;
                            currentValue.LongMinutes = item.LongMinutes;
                            currentValue.LongSeconds = item.LongSeconds;
                            currentValue.Lat = item.Lat;
                            currentValue.Lng = item.Lng;

                            //if (canSync)
                            //{
                            currentValue.Timestamp = item.Timestamp;
                            currentValue.EventTypeId = eventTypeId;
                            currentValue.ConditionId = conditionId;
                            currentValue.VoyageId = voyageId.Value;
                            currentValue.CurrentVoyageConditionKey = item.CurrentVoyageConditionKey;
                            currentValue.NextVoyageConditionKey = item.NextVoyageConditionKey;
                            currentValue.PreviousVoyageConditionKey = item.PreviousVoyageConditionKey;
                            currentValue.ConditionStartedDate = item.ConditionStartedDate;
                            currentValue.IsDeleted = item.IsDeleted;
                            currentValue.BunkeringDataId = bunkeringDataId;
                            //}

                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (events.Any())
                    {
                        await _context.BulkInsertAsync(events, options =>
                        {
                            options.PreserveInsertOrder = true;
                        });
                    }
                    foreach (var item in data.Events.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.Events.IgnoreQueryFilters().SingleAsync(_ => _.BusinessId == item.Id);
                        var parentEventQuery = _context.Events.IgnoreQueryFilters().Where(_ => _.BusinessId == item.ParentEventId).AsQueryable();
                        EventDataModel parentEvent = null;
                        if (item.ParentEventId != null)
                        {
                            parentEvent = await parentEventQuery.SingleAsync();
                        }
                        currentValue.ParentEventId = parentEvent?.Id;
                        currentValue.DateModified = item.DateModified;
                        currentValue.IsDeleted = item.IsDeleted;
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    var reports = new List<ReportDataModel>();
                    foreach (var item in data.Reports.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.Reports.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        var targetEventId = await _context.Events.IgnoreQueryFilters().Where(_ => _.BusinessId == item.EventId).Select(_ => (int?)_.Id).SingleOrDefaultAsync();
                        if (targetEventId == null)
                            continue;
                        if (currentValue == null)
                        {
                            currentValue = new ReportDataModel()
                            {
                                EventId = targetEventId.Value,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id
                            };
                            reports.Add(currentValue);
                        }
                        else
                        {
                            currentValue.EventId = targetEventId.Value;
                            currentValue.DateModified = item.DateModified;
                            currentValue.IsDeleted = item.IsDeleted;

                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (reports.Any())
                    {
                        await _context.BulkInsertAsync(reports, options =>
                        {
                            options.PreserveInsertOrder = true;
                        });
                    }
                    var reportFieldValues = new List<ReportFieldValueDataModel>();
                    foreach (var item in data.ReportFieldValues.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.ReportFieldValues.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        var targetReportId = await _context.Reports.IgnoreQueryFilters().Where(_ => _.BusinessId == item.ReportId).Select(_ => (int?)_.Id).SingleOrDefaultAsync();
                        if (targetReportId == null)
                            continue;
                        var targetReportFieldId = await _context.ReportFields.IgnoreQueryFilters().Where(_ => _.BusinessId == item.ReportFieldId).Select(_ => _.Id).SingleAsync();

                        if (currentValue == null)
                        {
                            currentValue = new ReportFieldValueDataModel()
                            {
                                Value = item.Value,
                                ReportId = targetReportId.Value,
                                ReportFieldId = targetReportFieldId,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id
                            };
                            reportFieldValues.Add(currentValue);
                        }
                        else
                        {
                            currentValue.Value = item.Value;
                            currentValue.DateModified = item.DateModified;
                            currentValue.ReportId = targetReportId.Value;
                            currentValue.IsDeleted = item.IsDeleted;
                            currentValue.ReportFieldId = targetReportFieldId;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (reportFieldValues.Any())
                    {
                        await _context.BulkInsertAsync(reportFieldValues, options =>
                        {
                            options.PreserveInsertOrder = true;
                        });
                    }

                    var attachments = new List<EventAttachmentDataModel>();
                    foreach (var item in data.EventAttachments.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.EventAttachments.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        var targetEventId = await _context.Events.IgnoreQueryFilters().Where(_ => _.BusinessId == item.EventId).Select(_ => (int?)_.Id).SingleOrDefaultAsync();
                        var targetReportId = await _context.Reports.IgnoreQueryFilters().Where(_ => _.BusinessId == item.ReportId).Select(_ => (int?)_.Id).SingleOrDefaultAsync();
                        var targetReportFieldId = await _context.ReportFields.IgnoreQueryFilters().Where(_ => _.BusinessId == item.ReportFieldId).Select(_ => (int?)_.Id).SingleOrDefaultAsync();
                        var documentTypeId = await _context.DocumentType.IgnoreQueryFilters().Where(_ => _.BusinessId == item.DocumentTypeId).Select(_ => (int?)_.Id).SingleOrDefaultAsync();
                        var bunkeringDataId = await _context.BunkeringData.IgnoreQueryFilters().Where(_ => _.BusinessId == item.BunkeringDataId).Select(_ => (int?)_.Id).SingleOrDefaultAsync();
                        if (targetEventId == null)
                            continue;
                        if (currentValue == null)
                        {
                            currentValue = new EventAttachmentDataModel()
                            {
                                FileName = item.FileName,
                                FileSize = item.FileSize,
                                MimeType = item.MimeType,
                                ReportFieldId = targetReportFieldId,
                                ReportId = targetReportId,
                                EventId = targetEventId.Value,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id,
                                BunkeringDataId = bunkeringDataId
                            };
                            attachments.Add(currentValue);
                        }
                        else
                        {
                            currentValue.MimeType = item.MimeType;
                            currentValue.FileSize = item.FileSize;
                            currentValue.ReportFieldId = targetReportFieldId;
                            currentValue.ReportId = targetReportId;
                            currentValue.FileName = item.FileName;
                            currentValue.EventId = targetEventId.Value;
                            currentValue.DateModified = item.DateModified;
                            currentValue.IsDeleted = item.IsDeleted;
                            currentValue.DocumentTypeId = documentTypeId;
                            currentValue.BunkeringDataId = bunkeringDataId;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (attachments.Any())
                    {
                        await _context.BulkInsertAsync(attachments, options =>
                        {
                            options.PreserveInsertOrder = true;
                        });
                    }
                    var sof = new List<Models.StatementOfFact>();
                    foreach (var item in data.StatementOfFacts.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.StatementOfFact.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        var lastEventId = await _context.Events.IgnoreQueryFilters().Where(_ => _.BusinessId == item.LastEventId).Select(_ => _.Id).SingleOrDefaultAsync();
                        var firstEventId = await _context.Events.IgnoreQueryFilters().Where(_ => _.BusinessId == item.FirstEventId).Select(_ => _.Id).SingleOrDefaultAsync();
                        if (currentValue == null)
                        {
                            currentValue = new Models.StatementOfFact()
                            {
                                UserId = userId,
                                FromDate = item.FromDate,
                                ToDate = item.ToDate,
                                LastEventId = lastEventId,
                                FirstEventId = firstEventId,
                                Completed = item.Completed,
                                Date = item.Date,
                                OperationGrade = item.OperationGrade,
                                Voyage = item.Voyage,
                                Remarks = item.Remarks,
                                Terminal = item.Terminal,
                                CharterParty = item.CharterParty,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id
                            };
                            sof.Add(currentValue);
                        }
                        else
                        {
                            currentValue.FromDate = item.FromDate;
                            currentValue.ToDate = item.ToDate;
                            currentValue.LastEventId = lastEventId;
                            currentValue.FirstEventId = firstEventId;
                            currentValue.Completed = item.Completed;
                            currentValue.Date = item.Date;
                            currentValue.OperationGrade = item.OperationGrade;
                            currentValue.Voyage = item.Voyage;
                            currentValue.Remarks = item.Remarks;
                            currentValue.Terminal = item.Terminal;
                            currentValue.CharterParty = item.CharterParty;
                            currentValue.DateModified = item.DateModified;
                            currentValue.IsDeleted = item.IsDeleted;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (sof.Any())
                        await _context.BulkInsertAsync(sof, options =>
                    {
                        options.PreserveInsertOrder = true;
                    });

                    var hasVoyages = await _context.Voyages.IgnoreQueryFilters().Where(_ => _.UserId == userId).AnyAsync();

                    if (!_isInHouse && !hasVoyages)
                    {
                        var berthedConditionId = await _context.EventConditions.Where(_ => _.Name.Trim().ToLower() == "berthed").Select(_ => _.Id).SingleAsync();
                        var voyage = new VoyageDataModel()
                        {
                            UserId = userId,
                            CurrentConditionId = berthedConditionId,
                            IsFinished = false,
                            StartDate = DateTimeOffset.UtcNow,
                            DateModified = DateTime.UtcNow,
                            CurrentVoyageConditionKey = Guid.NewGuid()
                        };
                        _context.Voyages.Add(voyage);
                        await _context.SaveChangesAsync();
                    }
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    // Rollback the transaction if an exception occurs
                    await transaction.RollbackAsync();
                    throw; // Rethrow the exception to handle it at a higher level
                }
            }

            _context.SoftDeleteEnabled = true;
            _context.UpdateTimestamps = true;

        }
        public async Task<SyncResponseViewModel> GetReadOnlyDataAsync(SyncRequestViewModel data)
        {
            var userId = await _context.Users.Where(_ => _.Prefix.ToUpper() == data.UserPrefix.ToUpper()).Select(_ => _.Id).SingleAsync();

            var events = await _context.Events.IgnoreQueryFilters()
                .Include(_ => _.Status)
                .Where(_ => _.UserId == userId && _.DateModified > data.Events)
                .OrderBy(_ => _.Id)
                .Select(_ => new Event()
                {
                    Id = _.BusinessId,
                    StatusId = _.Status.BusinessId
                }).ToListAsync();

            var details = await GetVesselDetails(data.UserPrefix);

            var response = new SyncResponseViewModel()
            {
                User = data.UserPrefix,
                Operator = details.Operator,
                PitchPropeller = details.PitchPropeller,
                MainEngineMaxPower = details.MainEngineMaxPower,
                NonHafnia = details.NonHafnia,
                NonPool = details.NonPool,
                Data = new SyncResponseDataViewModel()
                {
                    Events = events
                }
            };

            return response;
        }
        public async Task<SyncResponseViewModel> GetDataAsync(SyncRequestViewModel data)
        {
            var userId = await _context.Users.Where(_ => _.Prefix.ToUpper() == data.UserPrefix.ToUpper()).Select(_ => _.Id).SingleAsync();

            var voyages = await _context.Voyages.IgnoreQueryFilters()
                .Include(_ => _.CurrentCondition)
                .Where(_ => _.UserId == userId && _.DateModified > data.Voyages)
                .OrderBy(_ => _.Id)
                .Select(_ => new Voyage()
                {
                    Id = _.BusinessId,
                    StartDate = _.StartDate,
                    EndDate = _.EndDate,
                    IsFinished = _.IsFinished,
                    CurrentConditionId = _.CurrentCondition.BusinessId,
                    CurrentVoyageConditionKey = _.CurrentVoyageConditionKey,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    DisplayIndex = _.Id
                }).ToListAsync();
            var events = await _context.Events.IgnoreQueryFilters()
                .Include(_ => _.Status)
                .Include(_ => _.EventType)
                .Include(_ => _.EventCondition)
                .Include(_ => _.Voyage)
                .Include(_ => _.ParentEvent)
                .Include(_ => _.Port)
                .Where(_ => _.UserId == userId && _.DateModified > data.Events)
                .OrderBy(_ => _.Id)
                .Select(_ => new Event()
                {
                    Id = _.BusinessId,
                    Timestamp = _.Timestamp,
                    StatusId = _.Status.BusinessId,
                    EventTypeId = _.EventType.BusinessId,
                    ConditionId = _.EventCondition.BusinessId,
                    VoyageId = _.Voyage.BusinessId,
                    CurrentVoyageConditionKey = _.CurrentVoyageConditionKey,
                    NextVoyageConditionKey = _.NextVoyageConditionKey,
                    PreviousVoyageConditionKey = _.PreviousVoyageConditionKey,
                    Comment = _.Comment,
                    ConditionStartedDate = _.ConditionStartedDate,
                    ParentEventId = _.ParentEventId.HasValue ? _.ParentEvent.BusinessId : null,
                    CustomEventName = _.CustomEventName,
                    ExcludeFromStatement = _.ExcludeFromStatement,
                    HiddenDate = _.HiddenDate,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    DisplayIndex = _.Id,
                    BunkeringDataId = _.BunkeringDataId.HasValue ? _.BunkeringData.BusinessId : null,
                    LatDegrees = _.LatDegrees,
                    LatMinutes = _.LatMinutes,
                    LatSeconds = _.LatSeconds,
                    LongDegrees = _.LongDegrees,
                    LongMinutes = _.LongMinutes,
                    LongSeconds = _.LongSeconds,
                    PortId = _.PortId.HasValue ? _.Port.BusinessId : null,
                    Lat = _.Lat,
                    Lng = _.Lng

                }).ToListAsync();
            var cargoes = await _context.Cargoes.IgnoreQueryFilters()
                .Where(_ => _.UserId == userId && _.DateModified > data.Cargoes)
                .OrderBy(_ => _.Id)
                .Select(_ => new Cargo()
                {
                    Id = _.BusinessId,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    DisplayIndex = _.Id,
                    GradeId = _.Grade.BusinessId,
                    Parcel = _.Parcel,
                    StartedOn = _.StartedOn,
                    CompletedOn = _.CompletedOn
                }).ToListAsync();
            var cargoDetails = await _context.CargoDetails.IgnoreQueryFilters()
                .Where(_ => _.Cargo.UserId == userId && _.DateModified > data.CargoDetails)
                .OrderBy(_ => _.Id)
                .Select(_ => new CargoDetail()
                {
                    Id = _.BusinessId,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    DisplayIndex = _.Id,
                    CargoId = _.Cargo.BusinessId,
                    Timestamp = _.Timestamp,
                    Quantity = _.Quantity
                }).ToListAsync();
            var eventAttachments = await _context.EventAttachments.IgnoreQueryFilters()
                .Include(_ => _.Event)
                .Include(_ => _.DocumentType)
                .Include(_ => _.Report)
                .Include(_ => _.ReportField)
                .Where(_ => _.Event.UserId == userId && _.DateModified > data.EventAttachments)
                .OrderBy(_ => _.Id)
                .Select(_ => new EventAttachment()
                {
                    Id = _.BusinessId,
                    FileName = _.FileName,
                    FileSize = _.FileSize,
                    MimeType = _.MimeType,
                    EventId = _.Event.BusinessId,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    DocumentTypeId = _.DocumentTypeId.HasValue ? (Guid?)_.DocumentType.BusinessId : null,
                    ReportId = _.ReportId.HasValue ? _.Report.BusinessId : null,
                    ReportFieldId = _.ReportFieldId.HasValue ? (Guid?)_.ReportField.BusinessId : null,
                    BunkeringDataId = _.BunkeringDataId.HasValue ? _.BunkeringData.BusinessId : null,
                    DisplayIndex = _.Id
                })
                .ToListAsync();
            var reports = await _context.Reports.IgnoreQueryFilters()
                .Include(_ => _.Event)
                .Where(_ => _.Event.UserId == userId && _.DateModified > data.Reports)
                .OrderBy(_ => _.Id)
                .Select(_ => new Report()
                {
                    Id = _.BusinessId,
                    EventId = _.Event.BusinessId,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    DisplayIndex = _.Id
                }).ToListAsync();
            var reportFieldValues = await _context.ReportFieldValues.IgnoreQueryFilters()
                .Include(_ => _.Report)
                .ThenInclude(_ => _.Event)
                .Include(_ => _.ReportField)
                .Where(_ => _.Report.Event.UserId == userId && _.DateModified > data.ReportFieldValues)
                .OrderBy(_ => _.Id)
                .Select(_ => new ReportFieldValue()
                {
                    Id = _.BusinessId,
                    Value = _.Value,
                    ReportId = _.Report.BusinessId,
                    ReportFieldId = _.ReportField.BusinessId,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    DisplayIndex = _.Id
                }).ToListAsync();
            var bunkeringData = await _context.BunkeringData.IgnoreQueryFilters()
                .Where(_ => _.UserId == userId && _.DateModified > data.BunkeringData)
                .Select(_ => new Models.Sync.Data.BunkeringData()
                {
                    Id = _.BusinessId,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    Bdn = _.Bdn,
                    FuelType = _.FuelType,
                    Supplier = _.Supplier,
                    PortId = _.Port.BusinessId,
                    Timestamp = _.Timestamp,
                    TotalAmount = _.TotalAmount,
                    Density = _.Density,
                    SulphurContent = _.SulphurContent,
                    Viscosity = _.Viscosity,
                    DisplayIndex = _.Id
                }).ToListAsync();
            var bunkeringDataTanks = await _context.BunkeringDataTanks.IgnoreQueryFilters()
                .Where(_ => _.BunkeringData.UserId == userId && _.DateModified > data.BunkeringDataTanks)
                .Select(_ => new Models.Sync.Data.BunkeringDataTank()
                {
                    Id = _.BusinessId,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    DisplayIndex = _.Id,
                    BunkeringDataId = _.BunkeringData.BusinessId,
                    TankId = _.Tank.BusinessId,
                    Amount = _.Amount,
                    ComminglingId = _.ComminglingData.BusinessId
                }).ToListAsync();
            var statementOfFacts = await _context.StatementOfFact.IgnoreQueryFilters()
                .Where(_ => _.UserId == userId && _.DateModified > data.StatementOfFacts)
                .OrderBy(_ => _.Id)
                .Select(_ => new StatementOfFact()
                {
                    Id = _.BusinessId,
                    FromDate = _.FromDate,
                    ToDate = _.ToDate,
                    Completed = _.Completed,
                    Date = _.Date,
                    OperationGrade = _.OperationGrade,
                    Voyage = _.Voyage,
                    Remarks = _.Remarks,
                    Terminal = _.Terminal,
                    CharterParty = _.CharterParty,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    DisplayIndex = _.Id
                }).ToListAsync();
            foreach (var item in statementOfFacts)
            {
                var sourceItem = await _context.StatementOfFact.IgnoreQueryFilters().SingleAsync(_ => _.BusinessId == item.Id);
                var lastEventId = await _context.Events.IgnoreQueryFilters().Where(_ => _.Id == sourceItem.LastEventId).Select(_ => _.BusinessId).SingleOrDefaultAsync();
                var firstEventId = await _context.Events.IgnoreQueryFilters().Where(_ => _.Id == sourceItem.FirstEventId).Select(_ => _.BusinessId).SingleOrDefaultAsync();
                item.LastEventId = lastEventId;
                item.FirstEventId = firstEventId;
            }

            var details = await GetVesselDetails(data.UserPrefix);

            var response = new SyncResponseViewModel()
            {
                User = data.UserPrefix,
                Operator = details.Operator,
                PitchPropeller = details.PitchPropeller,
                MainEngineMaxPower = details.MainEngineMaxPower,
                NonHafnia = details.NonHafnia,
                NonPool = details.NonPool,
                Data = new SyncResponseDataViewModel()
                {
                    Voyages = voyages,
                    Events = events,
                    EventAttachments = eventAttachments,
                    Reports = reports,
                    ReportFieldValues = reportFieldValues,
                    BunkeringData = bunkeringData,
                    BunkeringDataTanks = bunkeringDataTanks,
                    Cargoes = cargoes,
                    CargoDetails = cargoDetails,
                    StatementOfFacts = statementOfFacts
                }
            };

            return response;
        }
        private async Task<SyncResponseViewModel> FetchDataAsync(SyncRequestViewModel data)
        {

            var jsonData = JsonConvert.SerializeObject(data);

            var vesselDetails = await GetVesselDetails(data.UserPrefix);

            UriBuilder uriBuilder = new UriBuilder
            {
                Scheme = "http",
                Port = vesselDetails.RemotePort,
                Host = vesselDetails.RemoteAddress,
                Path = "/api/sync/data",
                Query = null
            };

            var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.ToString())
            {
                Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
            };

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var downloadedData = await new StreamReader(responseStream).ReadToEndAsync();
                    var responseObject = JsonConvert.DeserializeObject<SyncResponseViewModel>(downloadedData);
                    return responseObject;
                }
            }
        }
        public async Task SyncMasterValues(string userId)
        {
            if (_isInHouse)
            {
                throw new NotImplementedException();
            }

            var status = (await _context.EventStatuses.IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var tanks = (await _context.Tanks.IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var tankUserSpecs = (await _context.TankUserSpecs.IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var reportFieldGroups = (await _context.ReportFieldGroups.IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var reportFields = (await _context.ReportFields.IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var reportTypes = (await _context.ReportTypes.IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var reportFieldsRelations = (await _context.ReportFieldRelations.IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var conditions = (await _context.EventConditions.IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var eventTypes = (await _context.EventTypes.IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var eventTypesCondition = (await _context.EventTypesConditions.IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var documentTypes = (await _context.DocumentType.IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var grades = (await _context.Grades.IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var prerequisites = (await _context.EventTypePrerequisites.IgnoreQueryFilters().MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;

            var areas = (await _context.Area.MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var areaCoordinates = (await _context.AreaCoordinate.MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var regions = (await _context.Region.MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var countries = (await _context.Country.MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;
            var ports = (await _context.Port.MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;

            var userPrefix = await _context.Users.Where(_ => _.Id == userId).Select(_ => _.Prefix.ToUpper()).SingleAsync();

            var viewModel = new SyncRequestMasterViewModel()
            {
                User = userPrefix,
                Status = status,
                Tanks = tanks,
                TankUserSpecs = tankUserSpecs,
                ReportFieldGroups = reportFieldGroups,
                ReportFields = reportFields,
                ReportTypes = reportTypes,
                ReportFieldsRelations = reportFieldsRelations,
                Conditions = conditions,
                EventTypes = eventTypes,
                EventTypesCondition = eventTypesCondition,
                Areas = areas,
                AreaCoordinates = areaCoordinates,
                Regions = regions,
                Countries = countries,
                Ports = ports,
                DocumentTypes = documentTypes,
                Grades = grades,
                EventTypePrerequisites = prerequisites
            };

            var data = await FetchMasterDataAsync(viewModel);

            await SaveMasterData(data, userId);

        }
        private async Task SaveMasterData(SyncResponseMasterViewModel data, string userId)
        {

            if (_isInHouse)
            {
                throw new NotImplementedException();
            }

            _context.SoftDeleteEnabled = false;
            _context.UpdateTimestamps = false;

            var user = await _context.Users.Where(u => u.Id == userId).SingleAsync();

            user.Operator = new Enums.Operator(data.VesselDetails.Operator).ToString();
            user.PitchPropeller = data.VesselDetails.PitchPropeller;
            user.MainEngineMaxPower = data.VesselDetails.MainEngineMaxPower;
            user.NonHafnia = data.VesselDetails.NonHafnia;
            user.NonPool = data.VesselDetails.NonPool;
            user.AvailablePasscodeSlots = data.VesselDetails.AvailablePasscodeSlots;


            if (_context.ChangeTracker.HasChanges())
                await _context.SaveChangesAsync();

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    #region port
                    var areasToAdd = new List<Area>();
                    foreach (var item in data.Areas.OrderBy(_ => _.Id))
                    {
                        var val = await _context.Area.SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        if (val == null)
                        {
                            val = new Area()
                            {
                                Name = item.Name,
                                Code = item.Code,
                                DateModified = item.DateModified,
                                BusinessId = item.Id
                            };
                            areasToAdd.Add(val);
                        }
                        else
                        {
                            val.Name = item.Name;
                            val.Code = item.Code;
                            val.DateModified = item.DateModified;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (areasToAdd.Any())
                        await _context.BulkInsertAsync(areasToAdd, options => options.PreserveInsertOrder = true);
                    var coordinatesToAdd = new List<AreaCoordinate>();
                    foreach (var item in data.AreaCoordinates.OrderBy(_ => _.Id))
                    {
                        var val = await _context.AreaCoordinate.SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        var areaId = await _context.Area.Where(_ => _.BusinessId == item.AreaId).Select(_ => _.Id).SingleAsync();
                        if (val == null)
                        {
                            val = new AreaCoordinate()
                            {
                                AreaId = areaId,
                                Lng = item.Lng,
                                Lat = item.Lat,
                                PointIndex = item.PointIndex,
                                DateModified = item.DateModified,
                                BusinessId = item.Id
                            };
                            coordinatesToAdd.Add(val);
                        }
                        else
                        {
                            val.AreaId = areaId;
                            val.Lng = item.Lng;
                            val.Lat = item.Lat;
                            val.PointIndex = item.PointIndex;
                            val.DateModified = item.DateModified;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (coordinatesToAdd.Any())
                        await _context.BulkInsertAsync(coordinatesToAdd, options => options.PreserveInsertOrder = true);
                    var regionsToAdd = new List<Region>();
                    foreach (var item in data.Regions.OrderBy(_ => _.Id))
                    {
                        var areaId = await _context.Area.Where(_ => _.BusinessId == item.AreaId).Select(_ => _.Id).SingleOrDefaultAsync();
                        var val = await _context.Region.SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        if (val == null)
                        {
                            val = new Region()
                            {
                                Name = item.Name,
                                AreaId = areaId,
                                DateModified = item.DateModified,
                                BusinessId = item.Id
                            };
                            regionsToAdd.Add(val);
                        }
                        else
                        {
                            val.Name = item.Name;
                            val.AreaId = areaId;
                            val.DateModified = item.DateModified;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (regionsToAdd.Any())
                        await _context.BulkInsertAsync(regionsToAdd, options => options.PreserveInsertOrder = true);
                    var countriesToAdd = new List<Country>();
                    foreach (var item in data.Countries.OrderBy(_ => _.Id))
                    {
                        var regionId = await _context.Region.Where(_ => _.BusinessId == item.RegionId).Select(_ => _.Id).SingleOrDefaultAsync();
                        var val = await _context.Country.SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        if (val == null)
                        {
                            val = new Country()
                            {
                                Numerical = item.Numerical,
                                Name = item.Name,
                                Alpha2 = item.Alpha2,
                                Alpha3 = item.Alpha3,
                                Nationality = item.Nationality,
                                RegionId = regionId,
                                LloydsCode = item.LloydsCode,
                                PhoneCode = item.PhoneCode,
                                DateModified = item.DateModified,
                                BusinessId = item.Id
                            };
                            countriesToAdd.Add(val);
                        }
                        else
                        {
                            val.Numerical = item.Numerical;
                            val.Name = item.Name;
                            val.Alpha2 = item.Alpha2;
                            val.Alpha3 = item.Alpha3;
                            val.Nationality = item.Nationality;
                            val.RegionId = regionId;
                            val.LloydsCode = item.LloydsCode;
                            val.PhoneCode = item.PhoneCode;
                            val.DateModified = item.DateModified;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (countriesToAdd.Any())
                        await _context.BulkInsertAsync(countriesToAdd, options => options.PreserveInsertOrder = true);
                    var portsToAdd = new List<Models.Ports.Port>();
                    foreach (var item in data.Ports.OrderBy(_ => _.Id))
                    {
                        var countryId = await _context.Country.Where(_ => _.BusinessId == item.CountryId).Select(_ => _.Id).SingleAsync();
                        var regionId = await _context.Region.Where(_ => _.BusinessId == item.RegionId).Select(_ => _.Id).SingleAsync();
                        var areaId = await _context.Area.Where(_ => _.BusinessId == item.AreaId).Select(_ => _.Id).SingleAsync();
                        var val = await _context.Port.SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        if (val == null)
                        {
                            val = new Models.Ports.Port()
                            {
                                Name = item.Name,
                                Latitude = item.Latitude,
                                Longitude = item.Longitude,
                                Code = item.Code,
                                CountryId = countryId,
                                RegionId = regionId,
                                AreaId = areaId,
                                TimeZone = item.TimeZone,
                                DateModified = item.DateModified,
                                BusinessId = item.Id
                            };
                            portsToAdd.Add(val);
                        }
                        else
                        {
                            val.Name = item.Name;
                            val.Latitude = item.Latitude;
                            val.Longitude = item.Longitude;
                            val.Code = item.Code;
                            val.CountryId = countryId;
                            val.RegionId = regionId;
                            val.AreaId = areaId;
                            val.TimeZone = item.TimeZone;
                            val.DateModified = item.DateModified;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (portsToAdd.Any())
                        await _context.BulkInsertAsync(portsToAdd, options => options.PreserveInsertOrder = true);
                    #endregion
                    var status = new List<EventStatusDataModel>();
                    foreach (var item in data.Status.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.EventStatuses.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        if (currentValue == null)
                        {
                            currentValue = new EventStatusDataModel()
                            {
                                Name = item.Name,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id
                            };
                            status.Add(currentValue);
                        }
                        else
                        {
                            currentValue.Name = item.Name;
                            currentValue.DateModified = item.DateModified;
                            currentValue.IsDeleted = item.IsDeleted;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (status.Any())
                        await _context.BulkInsertAsync(status, options => options.PreserveInsertOrder = true);
                    var tanks = new List<TankDataModel>();
                    foreach (var item in data.Tanks.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.Tanks.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        if (currentValue == null)
                        {
                            currentValue = new TankDataModel()
                            {
                                Name = item.Name,
                                Storage = item.Storage,
                                Settling = item.Settling,
                                Serving = item.Serving,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id,
                            };
                            tanks.Add(currentValue);
                        }
                        else
                        {
                            currentValue.Name = item.Name;
                            currentValue.Storage = item.Storage;
                            currentValue.Settling = item.Settling;
                            currentValue.Serving = item.Serving;
                            currentValue.DateModified = item.DateModified;
                            currentValue.IsDeleted = item.IsDeleted;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (tanks.Any())
                        await _context.BulkInsertAsync(tanks, options => options.PreserveInsertOrder = true);
                    var tankUserSpecs = new List<TankUserSpecsDataModel>();
                    foreach (var item in data.TankUserSpecs.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.TankUserSpecs.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        var targetTankId = await _context.Tanks.IgnoreQueryFilters().Where(_ => _.BusinessId == item.TankId).Select(_ => _.Id).SingleAsync();
                        if (currentValue == null)
                        {
                            currentValue = new TankUserSpecsDataModel()
                            {
                                UserId = userId,
                                TankId = targetTankId,
                                MaxCapacity = item.MaxCapacity,
                                DisplayOrder = item.DisplayOrder,
                                TankName = item.TankName,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                IsActive = item.IsActive,
                                BusinessId = item.Id,
                            };
                            tankUserSpecs.Add(currentValue);
                        }
                        else
                        {
                            currentValue.TankId = targetTankId;
                            currentValue.MaxCapacity = item.MaxCapacity;
                            currentValue.DisplayOrder = item.DisplayOrder;
                            currentValue.TankName = item.TankName;
                            currentValue.DateModified = item.DateModified;
                            currentValue.IsDeleted = item.IsDeleted;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (tankUserSpecs.Any())
                        await _context.BulkInsertAsync(tankUserSpecs, options => options.PreserveInsertOrder = true);
                    var reportFieldGroups = new List<ReportFieldGroupDataModel>();
                    foreach (var item in data.ReportFieldGroups.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.ReportFieldGroups.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        if (currentValue == null)
                        {
                            currentValue = new ReportFieldGroupDataModel()
                            {
                                FieldGroupName = item.FieldGroupName,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id,
                            };
                            reportFieldGroups.Add(currentValue);
                        }
                        else
                        {
                            currentValue.FieldGroupName = item.FieldGroupName;
                            currentValue.DateModified = item.DateModified;
                            currentValue.IsDeleted = item.IsDeleted;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (reportFieldGroups.Any())
                        await _context.BulkInsertAsync(reportFieldGroups, options => options.PreserveInsertOrder = true);
                    var reportFields = new List<ReportFieldDataModel>();
                    foreach (var item in data.ReportFields.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.ReportFields.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        var targetGroup = await _context.ReportFieldGroups.IgnoreQueryFilters().Where(_ => _.BusinessId == item.GroupId).SingleOrDefaultAsync();
                        var targetTank = await _context.Tanks.IgnoreQueryFilters().Where(_ => _.BusinessId == item.TankId).SingleOrDefaultAsync();
                        if (currentValue == null)
                        {
                            currentValue = new ReportFieldDataModel()
                            {
                                Name = item.Name,
                                IsSubgroupMain = item.IsSubgroupMain,
                                GroupId = targetGroup?.Id,
                                TankId = targetTank?.Id,
                                ValidationKey = item.ValidationKey,
                                Description = item.Description,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id
                            };
                            reportFields.Add(currentValue);
                        }
                        else
                        {
                            currentValue.Name = item.Name;
                            currentValue.IsSubgroupMain = item.IsSubgroupMain;
                            currentValue.GroupId = targetGroup?.Id;
                            currentValue.TankId = targetTank?.Id;
                            currentValue.ValidationKey = item.ValidationKey;
                            currentValue.Description = item.Description;
                            currentValue.DateModified = item.DateModified;
                            currentValue.IsDeleted = item.IsDeleted;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (reportFields.Any())
                        await _context.BulkInsertAsync(reportFields, options => options.PreserveInsertOrder = true);
                    var reportTypes = new List<ReportTypeDataModel>();
                    foreach (var item in data.ReportTypes.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.ReportTypes.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        if (currentValue == null)
                        {
                            currentValue = new ReportTypeDataModel()
                            {
                                Name = item.Name,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id
                            };
                            reportTypes.Add(currentValue);
                        }
                        else
                        {
                            currentValue.Name = item.Name;
                            currentValue.DateModified = item.DateModified;
                            currentValue.IsDeleted = item.IsDeleted;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (reportTypes.Any())
                        await _context.BulkInsertAsync(reportTypes, options => options.PreserveInsertOrder = true);
                    var reportFieldRelations = new List<ReportFieldRelationDataModel>();
                    foreach (var item in data.ReportFieldsRelations.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.ReportFieldRelations.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        var targetReportTypeId = await _context.ReportTypes.IgnoreQueryFilters().Where(_ => _.BusinessId == item.ReportTypeId).Select(_ => _.Id).SingleAsync();
                        var targetReportFieldId = await _context.ReportFields.IgnoreQueryFilters().Where(_ => _.BusinessId == item.ReportFieldId).Select(_ => _.Id).SingleAsync();
                        if (currentValue == null)
                        {
                            currentValue = new ReportFieldRelationDataModel()
                            {
                                ReportFieldId = targetReportFieldId,
                                ReportTypeId = targetReportTypeId,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id
                            };
                            reportFieldRelations.Add(currentValue);
                        }
                        else
                        {
                            currentValue.ReportFieldId = targetReportFieldId;
                            currentValue.ReportTypeId = targetReportTypeId;
                            currentValue.DateModified = item.DateModified;
                            currentValue.IsDeleted = item.IsDeleted;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (reportFieldRelations.Any())
                        await _context.BulkInsertAsync(reportFieldRelations, options => options.PreserveInsertOrder = true);
                    var conditions = new List<EventConditionDataModel>();
                    foreach (var item in data.Conditions.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.EventConditions.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        if (currentValue == null)
                        {
                            currentValue = new EventConditionDataModel()
                            {
                                Name = item.Name,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id
                            };
                            conditions.Add(currentValue);
                        }
                        else
                        {
                            currentValue.Name = item.Name;
                            currentValue.DateModified = item.DateModified;
                            currentValue.IsDeleted = item.IsDeleted;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (conditions.Any())
                        await _context.BulkInsertAsync(conditions, options => options.PreserveInsertOrder = true);
                    var eventTypes = new List<EventTypeDataModel>();
                    foreach (var item in data.EventTypes.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.EventTypes.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        var nextConditionId = await _context.EventConditions.IgnoreQueryFilters().Where(_ => _.BusinessId == item.NextConditionId).Select(_ => (int?)_.Id).SingleOrDefaultAsync();
                        var reportTypeId = await _context.ReportTypes.IgnoreQueryFilters().Where(_ => _.BusinessId == item.ReportTypeId).Select(_ => (int?)_.Id).SingleOrDefaultAsync();
                        if (currentValue == null)
                        {
                            currentValue = new EventTypeDataModel()
                            {
                                Name = item.Name,
                                NextConditionId = nextConditionId,
                                Transit = item.Transit,
                                ReportTypeId = reportTypeId,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id
                            };
                            eventTypes.Add(currentValue);
                        }
                        else
                        {
                            currentValue.Name = item.Name;
                            currentValue.NextConditionId = nextConditionId;
                            currentValue.Transit = item.Transit;
                            currentValue.ReportTypeId = reportTypeId;
                            currentValue.DateModified = item.DateModified;
                            currentValue.IsDeleted = item.IsDeleted;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (eventTypes.Any())
                        await _context.BulkInsertAsync(eventTypes, options => options.PreserveInsertOrder = true);
                    foreach (var item in data.EventTypes.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.EventTypes.IgnoreQueryFilters().SingleAsync(_ => _.BusinessId == item.Id);
                        var pairedEventTypeQuery = _context.EventTypes.IgnoreQueryFilters().Where(_ => _.BusinessId == item.PairedEventTypeId).AsQueryable();
                        EventTypeDataModel pairedEventType = null;
                        if (item.PairedEventTypeId.HasValue)
                        {
                            pairedEventType = await pairedEventTypeQuery.SingleAsync();
                        }
                        currentValue.PairedEventTypeId = pairedEventType?.Id;
                        currentValue.DateModified = item.DateModified;
                        currentValue.IsDeleted = item.IsDeleted;
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    var eventTypesConditions = new List<EventTypesConditionsDataModel>();
                    foreach (var item in data.EventTypeConditions.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.EventTypesConditions.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        var eventTypeId = await _context.EventTypes.IgnoreQueryFilters().Where(_ => _.BusinessId == item.EventTypeId).Select(_ => _.Id).SingleAsync();
                        var conditionId = await _context.EventConditions.IgnoreQueryFilters().Where(_ => _.BusinessId == item.ConditionId).Select(_ => _.Id).SingleAsync();
                        if (currentValue == null)
                        {
                            currentValue = new EventTypesConditionsDataModel()
                            {
                                EventTypeId = eventTypeId,
                                ConditionId = conditionId,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id
                            };
                            eventTypesConditions.Add(currentValue);
                        }
                        else
                        {
                            currentValue.EventTypeId = eventTypeId;
                            currentValue.ConditionId = conditionId;
                            currentValue.DateModified = item.DateModified;
                            currentValue.IsDeleted = item.IsDeleted;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (eventTypesConditions.Any())
                        await _context.BulkInsertAsync(eventTypesConditions, options => options.PreserveInsertOrder = true);
                    var documentTypes = new List<Models.DocumentType>();
                    foreach (var item in data.DocumentTypes.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.DocumentType.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        if (currentValue == null)
                        {
                            currentValue = new Models.DocumentType()
                            {
                                Name = item.Name,
                                Code = item.Code,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id
                            };
                            documentTypes.Add(currentValue);
                        }
                        else
                        {
                            currentValue.Name = item.Name;
                            currentValue.Code = item.Code;
                            currentValue.DateModified = item.DateModified;
                            currentValue.IsDeleted = item.IsDeleted;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (documentTypes.Any())
                        await _context.BulkInsertAsync(documentTypes, options => options.PreserveInsertOrder = true);
                    var prerequisites = new List<EventTypePrerequisiteDataModel>();
                    foreach (var item in data.EventTypePrerequisites.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.EventTypePrerequisites.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        var eventTypeId = await _context.EventTypes.IgnoreQueryFilters().Where(_ => _.BusinessId == item.EventTypeId).Select(_ => _.Id).SingleAsync();
                        var availableAfterEventTypeId = await _context.EventTypes.IgnoreQueryFilters().Where(_ => _.BusinessId == item.AvailableAfterEventTypeId).Select(_ => _.Id).SingleAsync();
                        if (currentValue == null)
                        {
                            currentValue = new EventTypePrerequisiteDataModel()
                            {
                                EventTypeId = eventTypeId,
                                AvailableAfterEventTypeId = availableAfterEventTypeId,
                                Completed = item.Completed,
                                Override = item.Override,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id
                            };
                            prerequisites.Add(currentValue);
                        }
                        else
                        {
                            currentValue.Completed = item.Completed;
                            currentValue.Override = item.Override;
                            currentValue.DateModified = item.DateModified;
                            currentValue.IsDeleted = item.IsDeleted;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (prerequisites.Any())
                        await _context.BulkInsertAsync(prerequisites, options => options.PreserveInsertOrder = true);
                    var grades = new List<GradeModel>();
                    foreach (var item in data.Grades.OrderBy(_ => _.DisplayIndex))
                    {
                        var currentValue = await _context.Grades.IgnoreQueryFilters().SingleOrDefaultAsync(_ => _.BusinessId == item.Id);
                        if (currentValue == null)
                        {
                            currentValue = new GradeModel()
                            {
                                Name = item.Name,
                                DateModified = item.DateModified,
                                IsDeleted = item.IsDeleted,
                                BusinessId = item.Id
                            };
                            grades.Add(currentValue);
                        }
                        else
                        {
                            currentValue.Name = item.Name;
                            currentValue.DateModified = item.DateModified;
                            currentValue.IsDeleted = item.IsDeleted;
                        }
                    }
                    if (_context.ChangeTracker.HasChanges())
                        await _context.SaveChangesAsync();
                    if (grades.Any())
                        await _context.BulkInsertAsync(grades, options => options.PreserveInsertOrder = true);

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    // Rollback the transaction if an exception occurs
                    await transaction.RollbackAsync();
                    throw; // Rethrow the exception to handle it at a higher level
                }
            }

            _context.SoftDeleteEnabled = true;
            _context.UpdateTimestamps = true;

        }
        private async Task<User> UpdateOrCreateUser(VesselDetails vesselDetails, VesselDetails dbVesselDetails)
        {
            User user = null;

            if (dbVesselDetails == null)
            {
                user = new User();
                user.UserName = vesselDetails.Name.ToLower();
                user.Operator = new Enums.Operator(vesselDetails.Operator).ToString();
                user.AvailablePasscodeSlots = 2;
                user.Prefix = vesselDetails.Prefix.ToUpper();
                user.PitchPropeller = vesselDetails.PitchPropeller;
                user.MainEngineMaxPower = vesselDetails.MainEngineMaxPower;
                user.NonHafnia = vesselDetails.NonHafnia;
                user.NonPool = vesselDetails.NonPool;
            }
            else
            {
                user = await _context.Users.Where(_ => _.Prefix == vesselDetails.Prefix.ToUpper()).SingleAsync();
            }

            user.RemoteAddress = _remoteIp;
            user.RemotePort = 0;

            if (dbVesselDetails == null)
            {
                var result = await _userManager.CreateAsync(user, "123456");
            }
            else
            {
                if (_context.ChangeTracker.HasChanges())
                    await _context.SaveChangesAsync();
            }
            return user;
        }
        public async Task<SyncResponseMasterViewModel> GetMasterDataAsync(SyncRequestMasterViewModel data)
        {
            if (!_isInHouse)
            {
                throw new NotImplementedException();
            }

            var vesselDetails = await _context.Users.Where(_ => _.Prefix == data.User.ToUpper()).Select(u => new VesselDetails()
            {
                Operator = new Enums.Operator(u.Operator).ToString(),
                PitchPropeller = u.PitchPropeller,
                MainEngineMaxPower = u.MainEngineMaxPower,
                Prefix = u.Prefix.ToUpper(),
                RemoteAddress = u.RemoteAddress,
                RemotePort = u.RemotePort,
                NonHafnia = u.NonHafnia,
                NonPool = u.NonPool,
                AvailablePasscodeSlots = 0
            }).SingleOrDefaultAsync();

            var user = await UpdateOrCreateUser(data.VesselDetails, vesselDetails);

            var status = await _context.EventStatuses.IgnoreQueryFilters().Where(_ => _.DateModified > data.Status)
                .OrderBy(_ => _.Id)
                .Select(_ => new Sync.Status()
                {
                    Id = _.BusinessId,
                    Name = _.Name,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    DisplayIndex = _.Id
                }).ToListAsync();
            var tanks = await _context.Tanks.IgnoreQueryFilters().Where(_ => _.DateModified > data.Tanks)
                .OrderBy(_ => _.Id)
                .Select(_ => new Sync.Tank()
                {
                    Id = _.BusinessId,
                    Name = _.Name,
                    Storage = _.Storage,
                    Settling = _.Settling,
                    Serving = _.Serving,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    DisplayIndex = _.Id
                })
                .ToListAsync();
            var tankuserSpecs = await _context.TankUserSpecs.IgnoreQueryFilters().Include(_ => _.User).Include(_ => _.Tank).Where(_ => _.UserId == user.Id && _.DateModified > data.TankUserSpecs)
                .OrderBy(_ => _.Id)
                .Select(_ => new Sync.TankUserSpecs()
                {
                    Id = _.BusinessId,
                    TankId = _.Tank.BusinessId,
                    MaxCapacity = _.MaxCapacity,
                    DisplayOrder = _.DisplayOrder,
                    TankName = _.TankName,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    IsActive = _.IsActive,
                    DisplayIndex = _.Id
                }).ToListAsync();
            var reportFieldGroups = await _context.ReportFieldGroups.IgnoreQueryFilters().Where(_ => _.DateModified > data.ReportFieldGroups)
                .OrderBy(_ => _.Id)
                .Select(_ => new Sync.ReportFieldGroup()
                {
                    Id = _.BusinessId,
                    FieldGroupName = _.FieldGroupName,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    DisplayIndex = _.Id
                }).ToListAsync();
            var reportFields = await _context.ReportFields.IgnoreQueryFilters().Include(_ => _.Group).Include(_ => _.Tank).Where(_ => _.DateModified > data.ReportFields).OrderBy(_ => _.Id)
                .Select(_ => new Sync.ReportField()
                {
                    Id = _.BusinessId,
                    Name = _.Name,
                    IsSubgroupMain = _.IsSubgroupMain,
                    GroupId = _.GroupId.HasValue ? (Guid?)_.Group.BusinessId : null,
                    TankId = _.Tank.BusinessId,
                    ValidationKey = _.ValidationKey,
                    Description = _.Description,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    DisplayIndex = _.Id
                }).ToListAsync();
            var reportTypes = await _context.ReportTypes.IgnoreQueryFilters().Where(_ => _.DateModified > data.ReportTypes).OrderBy(_ => _.Id)
                .Select(_ => new Sync.ReportType()
                {
                    Id = _.BusinessId,
                    Name = _.Name,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    DisplayIndex = _.Id
                }).ToListAsync();
            var reportFieldsRelations = await _context.ReportFieldRelations.IgnoreQueryFilters().Include(_ => _.ReportField).Include(_ => _.ReportType).Where(_ => _.DateModified > data.ReportFieldsRelations).OrderBy(_ => _.Id)
                .Select(_ => new Sync.ReportFieldRelation()
                {
                    Id = _.BusinessId,
                    ReportTypeId = _.ReportType.BusinessId,
                    ReportFieldId = _.ReportField.BusinessId,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    DisplayIndex = _.Id
                }).ToListAsync();
            var conditions = await _context.EventConditions.IgnoreQueryFilters().Where(_ => _.DateModified > data.Conditions).OrderBy(_ => _.Id)
                .Select(_ => new Sync.Condition()
                {
                    Id = _.BusinessId,
                    Name = _.Name,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    DisplayIndex = _.Id
                }).ToListAsync();
            var eventTypes = await _context.EventTypes.IgnoreQueryFilters().Include(_ => _.ReportType).Include(_ => _.NextCondition).Include(_ => _.PairedEventType).Where(_ => _.DateModified > data.EventTypes).OrderBy(_ => _.Id)
                .Select(_ => new Sync.EventType()
                {
                    Id = _.BusinessId,
                    Name = _.Name,
                    PairedEventTypeId = _.PairedEventTypeId.HasValue ? (Guid?)_.PairedEventType.BusinessId : null,
                    NextConditionId = _.NextConditionId.HasValue ? (Guid?)_.NextCondition.BusinessId : null,
                    Transit = _.Transit,
                    ReportTypeId = _.ReportTypeId.HasValue ? (Guid?)_.ReportType.BusinessId : null,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    DisplayIndex = _.Id
                }).ToListAsync();
            var eventTypeConditions = await _context.EventTypesConditions.IgnoreQueryFilters().Include(_ => _.EventCondition).Include(_ => _.EventType).Where(_ => _.DateModified > data.EventTypesCondition).OrderBy(_ => _.Id)
                .Select(_ => new Sync.EventTypeCondition()
                {
                    Id = _.BusinessId,
                    EventTypeId = _.EventType.BusinessId,
                    ConditionId = _.EventCondition.BusinessId,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    DisplayIndex = _.Id
                }).ToListAsync();
            var documentTypes = await _context.DocumentType.IgnoreQueryFilters().Where(_ => _.DateModified > data.DocumentTypes).OrderBy(_ => _.Id)
                .Select(_ => new Sync.DocumentType()
                {
                    Id = _.BusinessId,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    Name = _.Name,
                    Code = _.Code
                }).ToListAsync();
            var grades = await _context.Grades.IgnoreQueryFilters().Where(_ => _.DateModified > data.Grades).OrderBy(_ => _.Id)
                .Select(_ => new Sync.Grade()
                {
                    Id = _.BusinessId,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    Name = _.Name,
                    DisplayIndex = _.Id
                }).ToListAsync();
            var prerequisites = await _context.EventTypePrerequisites.IgnoreQueryFilters().Where(_ => _.DateModified > data.Grades).OrderBy(_ => _.Id)
                .Select(_ => new Sync.EventTypePrerequisite()
                {
                    Id = _.BusinessId,
                    DateModified = _.DateModified,
                    IsDeleted = _.IsDeleted,
                    EventTypeId = _.EventType.BusinessId,
                    AvailableAfterEventTypeId = _.AvailableAfterEventType.BusinessId,
                    Completed = _.Completed,
                    Override = _.Override,
                    DisplayIndex = _.Id
                }).ToListAsync();

            var response = new SyncResponseMasterViewModel()
            {
                Status = status,
                Tanks = tanks,
                TankUserSpecs = tankuserSpecs,
                ReportFieldGroups = reportFieldGroups,
                ReportFields = reportFields,
                ReportTypes = reportTypes,
                ReportFieldsRelations = reportFieldsRelations,
                Conditions = conditions,
                EventTypes = eventTypes,
                EventTypeConditions = eventTypeConditions,
                DocumentTypes = documentTypes,
                Grades = grades,
                EventTypePrerequisites = prerequisites,
                VesselDetails = new VesselDetails()
                {
                    Operator = new Enums.Operator(user.Operator).ToString(),
                    PitchPropeller = user.PitchPropeller,
                    MainEngineMaxPower = user.MainEngineMaxPower,
                    NonHafnia = user.NonHafnia,
                    NonPool = user.NonPool,
                    AvailablePasscodeSlots = user.AvailablePasscodeSlots
                }
            };

            var areas = await _context
                .Area
                .Where(_ => _.DateModified > data.Areas)
                .OrderBy(_ => _.Id)
                .Select(_ => new Sync.Area()
                {
                    Id = _.BusinessId,
                    Name = _.Name,
                    Code = _.Code,
                    DateModified = _.DateModified
                })
                .ToListAsync();

            var areaCoordinateLast = (await _context.AreaCoordinate.MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;

            var areaCoordinates = await _context
                .AreaCoordinate
                .Include(_ => _.Area)
                .Where(_ => _.DateModified > data.AreaCoordinates)
                .OrderBy(_ => _.Id)
                .Select(_ => new Sync.AreaCoordinate()
                {
                    Id = _.BusinessId,
                    AreaId = _.Area.BusinessId,
                    Lng = _.Lng,
                    Lat = _.Lat,
                    PointIndex = _.PointIndex,
                    DateModified = _.DateModified
                })
                .ToListAsync();

            var regionLast = (await _context.Region.MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;

            var regions = await _context
                .Region
                .Include(_ => _.Area)
                .Where(_ => _.DateModified > data.Regions)
                .OrderBy(_ => _.Id)
                .Select(_ => new Sync.Region()
                {
                    Id = _.BusinessId,
                    Name = _.Name,
                    AreaId = _.AreaId.HasValue ? _.Area.BusinessId : null,
                    DateModified = _.DateModified
                })
                .ToListAsync();

            var countryLast = (await _context.Country.MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;

            var countries = await _context
                .Country
                .Include(_ => _.Region)
                .Where(_ => _.DateModified > data.Countries)
                .OrderBy(_ => _.Id)
                .Select(_ => new Sync.Country()
                {
                    Id = _.BusinessId,
                    Numerical = _.Numerical,
                    Name = _.Name,
                    Alpha2 = _.Alpha2,
                    Alpha3 = _.Alpha3,
                    Nationality = _.Nationality,
                    RegionId = _.RegionId.HasValue ? _.Region.BusinessId : null,
                    LloydsCode = _.LloydsCode,
                    PhoneCode = _.PhoneCode,
                    DateModified = _.DateModified
                })
                .ToListAsync();

            var portLast = (await _context.Port.MaxAsync(c => (DateTime?)c.DateModified)) ?? DateTime.MinValue;

            var ports = await _context
                .Port
                .Include(_ => _.Country)
                .Include(_ => _.Region)
                .Include(_ => _.Area)
                .Where(_ => _.DateModified > data.Ports)
                .OrderBy(_ => _.Id)
                .Select(_ => new Sync.Port()
                {
                    Id = _.BusinessId,
                    Name = _.Name,
                    Latitude = _.Latitude,
                    Longitude = _.Longitude,
                    Code = _.Code,
                    CountryId = _.Country.BusinessId,
                    RegionId = _.Region.BusinessId,
                    AreaId = _.Area.BusinessId,
                    TimeZone = _.TimeZone,
                    DateModified = _.DateModified
                })
            .ToListAsync();

            response.Areas = areas;
            response.AreaCoordinates = areaCoordinates;
            response.Regions = regions;
            response.Countries = countries;
            response.Ports = ports;

            return response;
        }
        private async Task<SyncResponseMasterViewModel> FetchMasterDataAsync(SyncRequestMasterViewModel data)
        {

            if (_isInHouse)
            {
                throw new NotImplementedException();
            }

            var vesselDetails = await GetVesselDetails(data.User);

            data.VesselDetails = vesselDetails;

            var jsonData = JsonConvert.SerializeObject(data);

            UriBuilder uriBuilder = new UriBuilder
            {
                Scheme = "http",
                Port = vesselDetails.RemotePort,
                Host = vesselDetails.RemoteAddress,
                Path = "/api/sync/master",
                Query = null
            };

            var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.ToString())
            {
                Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
            };

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {

                response.EnsureSuccessStatusCode();

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {

                    var downloadedData = await new StreamReader(responseStream).ReadToEndAsync();
                    var responseObject = JsonConvert.DeserializeObject<SyncResponseMasterViewModel>(downloadedData);
                    var encoding = Encoding.UTF8;
                    responseObject.LengthInBytes = encoding.GetByteCount(downloadedData);
                    return responseObject;
                }
            }
        }

    }
}
