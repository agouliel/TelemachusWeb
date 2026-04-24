using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Telemachus.Data.Models.Sync;
using Telemachus.Data.Services.Interfaces;

namespace Telemachus.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [DisableRequestSizeLimit]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly ISyncDataService _syncDataService;
        private string _passcode;
        private bool _hasVesselConfiguration { get; set; } = false;

        public SyncController(ISyncDataService syncDataService, IConfiguration configuration)
        {
            _syncDataService = syncDataService;
            _passcode = configuration.GetValue<string>("SyncCredentials:Passcode");
            _hasVesselConfiguration = configuration.GetSection("VesselDetails").Exists();
        }

        [HttpPost("")]
        public async Task<IActionResult> Sync()
        {
            if (!_hasVesselConfiguration)
            {
                throw new NotImplementedException();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User not found");
            }

            var hasValidRemoteAddress = await _syncDataService.HasValidRemoteAddress(userId);

            if (!hasValidRemoteAddress)
            {
                return BadRequest("Invalid remote address");
            }
            try
            {
                await _syncDataService.SyncMasterValues(userId);
                await _syncDataService.SyncDataValues(userId);
                return NoContent();
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("master")]
        public async Task<IActionResult> GetMasterValues([FromBody] SyncRequestMasterViewModel data)
        {
            if (_hasVesselConfiguration)
            {
                throw new NotImplementedException();
            }

            if (string.IsNullOrEmpty(_passcode))
            {
                return Unauthorized("Credentials not found");
            }
            var passcodeValues = Request.Headers["X-Passcode"].ToString().Split(',');
            var passcode = passcodeValues.FirstOrDefault();

            if (passcode.Trim() != _passcode)
            {
                return Unauthorized();
            }
            var response = await _syncDataService.GetMasterDataAsync(data);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("data")]
        public async Task<IActionResult> GetDataValues([FromBody] SyncRequestViewModel data)
        {
            if (_hasVesselConfiguration)
            {
                return Unauthorized();
            }
            if (string.IsNullOrEmpty(_passcode))
            {
                return Unauthorized("Credentials not found");
            }
            var passcodeValues = Request.Headers["X-Passcode"].ToString().Split(',');
            var passcode = passcodeValues.FirstOrDefault();

            if (passcode.Trim() != _passcode)
            {
                return Unauthorized();
            }

            var response = await _syncDataService.GetDataAsync(data);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("timestamps")]
        public async Task<IActionResult> GetDataTimestamps([FromBody] string userPrefix)
        {
            if (_hasVesselConfiguration)
            {
                return Unauthorized();
            }
            if (string.IsNullOrEmpty(_passcode))
            {
                return Unauthorized("Credentials not found");
            }
            var passcodeValues = Request.Headers["X-Passcode"].ToString().Split(',');
            var passcode = passcodeValues.FirstOrDefault();

            if (passcode.Trim() != _passcode)
            {
                return Unauthorized();
            }

            var response = await _syncDataService.GetDataTimestamps(userPrefix);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("data")]
        public async Task<IActionResult> SendData([FromBody] SyncResponseViewModel data)
        {
            if (_hasVesselConfiguration)
            {
                return Unauthorized();
            }
            if (string.IsNullOrEmpty(_passcode))
            {
                return Unauthorized("Credentials not found");
            }
            var passcodeValues = Request.Headers["X-Passcode"].ToString().Split(',');
            var passcode = passcodeValues.FirstOrDefault();

            if (passcode.Trim() != _passcode)
            {
                return Unauthorized();
            }

            await _syncDataService.SaveData(data);

            var res = await _syncDataService.GetReadOnlyDataAsync(data.LocalTimestamps);

            return Ok(res);
        }
    }
}
