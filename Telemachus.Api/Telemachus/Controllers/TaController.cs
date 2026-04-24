using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Telemachus.Business.Interfaces.Authentication;
using Telemachus.Business.Interfaces.Events;
using Telemachus.Business.Interfaces.Reports.Design;
using Telemachus.Data.Services.Interfaces;

namespace Telemachus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IReportDesignService _reportDesignService;
        private readonly ISyncDataService _syncDataService;
        private string _attachmentsPath = "";
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<TaController> _logger;

        public TaController(ILogger<TaController> logger, IWebHostEnvironment env, ISyncDataService syncDataService, IConfiguration config, IReportDesignService reportDesignService, IEventService eventService, IAuthenticationService authenticationService)
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

            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("")]
        public async Task<IActionResult> GetTaResults([FromQuery] string q)
        {
            try
            {
                var results = await _eventService.GetTaResults(q);
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null,
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(results, options);
                _logger.LogError($"{q}+{json}");
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}
