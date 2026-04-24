using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Telemachus.Business.Interfaces.Authentication;
using Telemachus.Business.Interfaces.Events;
using Telemachus.Business.Interfaces.Reports.Design;
using Telemachus.Business.Models.Reports.Design;
using Telemachus.Data.Services.Interfaces;
using Telemachus.Helpers;
using Telemachus.Models;

namespace Telemachus.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IReportDesignService _reportDesignService;
        private readonly ISyncDataService _syncDataService;
        private string _attachmentsPath = "";
        private readonly IWebHostEnvironment _env;

        public AdminController(IWebHostEnvironment env, ISyncDataService syncDataService, IConfiguration config, IReportDesignService reportDesignService, IEventService eventService, IAuthenticationService authenticationService)
        {
            _eventService = eventService;
            _authenticationService = authenticationService;
            _reportDesignService = reportDesignService;
            _syncDataService = syncDataService;
            _attachmentsPath = config["AttachmentsPath"];
            if (string.IsNullOrEmpty(_attachmentsPath))
            {
                _attachmentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Telemachus");
            }
            if (config.GetSection("VesselDetails").Exists())
            {
                throw new Exception("This is an admin instance and does not have access to this endpoint");
            }
            _env = env;
        }

        [HttpPatch("event/{eventId}/approve")]
        public async Task<IActionResult> ApproveEvent(int eventId)
        {
            await _eventService.ApproveEventAsync(eventId);
            return Ok();
        }

        [HttpPatch("event/{eventId}/reject")]
        public async Task<IActionResult> RejectEvent(int eventId)
        {
            await _eventService.RejectEventAsync(eventId);
            if (await _eventService.HasCommenceBunkeringReport(eventId))
            {
                await _eventService.RejectRelatedNoonEvents(eventId);
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("user")]
        public async Task<IActionResult> CreateVesselUser([FromBody] CreateVesselViewModel vessel)
        {
            var errors = await _authenticationService.CreateVesselUserAsync(vessel.UserName, vessel.Password);
            if (string.IsNullOrEmpty(errors))
            {
                return Ok("Success");
            }
            return BadRequest(errors);
        }

        [AllowAnonymous]
        [HttpPatch("user")]
        public async Task<IActionResult> UpdateVesselUserPassword([FromBody] UpdateVesselPasswordViewModel details)
        {
            try
            {
                await _authenticationService.UpdateVesselUserPassword(details.UserName, details.Password);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return NoContent();
        }

        [HttpGet("reportField")]
        public async Task<IActionResult> GetReportFields()
        {
            var viewModel = await _reportDesignService.GetReportFieldsAsync();
            return Ok(viewModel);
        }

        [HttpPatch("reportField/{name}")]
        public async Task<IActionResult> UpdateReportField(string name, [FromBody] FieldDesignModel field)
        {
            var success = await _reportDesignService.UpdateField(name, field);

            if (success)
            {
                return NoContent();
            }

            return BadRequest();
        }

        [HttpDelete("reportField/{name}")]
        public async Task<IActionResult> DeleteReportField(string name, [FromQuery] bool hiddenOnly = true)
        {
            var success = await _reportDesignService.DeleteField(name, hiddenOnly);

            if (success)
            {
                return NoContent();
            }

            return BadRequest();
        }

        [HttpPost("reportField")]
        public async Task<IActionResult> CreateReportField([FromBody] FieldDesignModel field)
        {
            var success = await _reportDesignService.CreateField(field);

            if (success)
            {
                return NoContent();
            }

            return BadRequest();
        }

        [DisableRequestSizeLimit]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        [HttpPost("ports/sync")]
        public async Task<IActionResult> Portsync()
        {
            await _syncDataService.PortSync();

            return NoContent();
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import()
        {
            var vessels = await _eventService.Import();

            return Ok(vessels.Where(v => v.VesselId == "697dd359-75ad-475e-a7eb-0af1e22ba906").Select(v => new
            {
                v.VesselId,
                v.Voyage,
                Reports = v.Reports.OrderBy(r => r.Timestamp).ThenBy(r => r.Id).Select(r => new
                {
                    r.Id,
                    r.Timestamp,
                    r.EventTypeId,
                    r.ConditionId,
                    r.CurrentVoyageConditionKey,
                })
            }));
        }

        [Middlewares.RequestSizeLimit(1073741824)]
        [HttpPost("file")]
        public async Task<IActionResult> UploadDocument(List<IFormFile> files)
        {
            var userId = ClaimHelper.GetUserId(User);
            var vessel = await _authenticationService.GetVesselAsync(userId);
            var localPath = Path.Combine(
                _attachmentsPath,
                "Uploaded Files",
                vessel.Prefix.ToUpper()
            );
            if (files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var directory = new DirectoryInfo(localPath);
                        if (!directory.Exists)
                        {
                            directory.Create();
                        }
                        var filePath = Path.Combine(directory.FullName, file.FileName);
                        if (System.IO.File.Exists(filePath))
                        {
                            var info = new FileInfo(filePath);
                            return BadRequest($"File {info.Name} already uploaded!");
                        }
                    }
                }
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        string filePath = Path.Combine(localPath, file.FileName);
                        using (var fs = System.IO.File.Create(filePath))
                        {
                            await file.CopyToAsync(fs);
                        }
                    }
                }
            }
            return NoContent();
        }

        [HttpGet("eventTypes")]
        public async Task<IActionResult> GetEventTypes()
        {
            var viewModel = await _reportDesignService.GetEventTypesAsync();
            return Ok(viewModel);
        }

        [HttpPatch("eventTypes")]
        public async Task<IActionResult> UpdateEventTypes([FromBody] List<EventTypeBusinessModel> eventTypes)
        {
            await _reportDesignService.UpdateEventTypes(eventTypes);
            return NoContent();
        }

        [HttpPost("eventType")]
        public async Task<IActionResult> CreateEventType([FromBody] EventTypeBusinessModel eventType)
        {
            await _reportDesignService.CreateEventType(eventType);
            return NoContent();
        }

        [HttpGet("tankData")]
        public async Task<IActionResult> GetTankData()
        {
            var tankData = await _reportDesignService.GetTankData();
            return Ok(tankData);
        }

        [HttpGet("tanks/{userId?}")]
        public async Task<IActionResult> GetTanks(string userId)
        {
            var tanks = await _reportDesignService.GetTanks(userId);
            return Ok(tanks);
        }

        [HttpPatch("tanks")]
        public async Task<IActionResult> UpdateTanks([FromBody] List<TankViewModel> tanks)
        {
            await _reportDesignService.UpdateTanks(tanks);
            return NoContent();
        }

        [HttpPost("tanks")]
        public async Task<IActionResult> CreateTank([FromBody] TankViewModel tank)
        {
            await _reportDesignService.CreateTank(tank);
            return NoContent();
        }

        [HttpDelete("tanks/{tankId}")]
        public async Task<IActionResult> ArchiveTank(int tankId)
        {
            await _reportDesignService.ArchiveTank(tankId);
            return NoContent();
        }

        [HttpDelete("tanks/{tankId}/force")]
        public async Task<IActionResult> DeleteTank(int tankId)
        {
            await _reportDesignService.DeleteTank(tankId);
            return NoContent();
        }

        [HttpPost("debug")]
        public async Task<IActionResult> Debug()
        {
            await _reportDesignService.ProcessCorrectionFactors2();
            return NoContent();
        }
    }
}
