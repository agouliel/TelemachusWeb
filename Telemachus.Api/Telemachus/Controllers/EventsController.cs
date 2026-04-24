using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Telemachus.Business.Interfaces.Cargo;
using Telemachus.Business.Interfaces.Events;
using Telemachus.Business.Models.Events;
using Telemachus.Business.Services.Mappers;
using Telemachus.Data.Models.Authentication;
using Telemachus.Helpers;
using Telemachus.Mappers;
using Telemachus.Models;
using Telemachus.Models.Events;

namespace Telemachus.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IEventTypeService _eventTypeService;
        private readonly ICargoBusinessService _cargoBusinessService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(ILogger<EventsController> logger, ICargoBusinessService cargoBusinessService, IEventTypeService eventTypeService, IEventService eventService, IWebHostEnvironment env, UserManager<User> userManager)
        {
            _eventService = eventService;
            _userManager = userManager;
            _eventTypeService = eventTypeService;
            _cargoBusinessService = cargoBusinessService;
            _env = env;
            _logger = logger;
        }

        [HttpGet("ports")]
        public async Task<IActionResult> ListPorts([FromQuery] string query)
        {
            var userId = ClaimHelper.GetUserId(User);
            return Ok(await _eventService.ListPorts(userId, query));
        }

        [HttpGet("typeahead")]
        public async Task<IActionResult> Search([FromQuery] string target, [FromQuery] string query)
        {
            return Ok(await _eventService.Search(target, query));
        }

        [HttpGet("ports/{id:int}")]
        public async Task<IActionResult> GetPort(int id)
        {
            return Ok(await _eventService.GetPort(id));
        }

        [HttpPost("facts")]
        public async Task<IActionResult> GetUserFacts([FromBody] FactFilterViewModel filter, [FromQuery] string vesselId, [FromQuery] int page = 1, [FromQuery] int pageSize = 30)
        {
            var userId = ClaimHelper.GetUserId(User);
            var groupedFacts = await _eventService.GetUserEventsAsync(userId, page, pageSize, filter?.EventTypeIds, filter?.EventStatuses, filter?.DateFrom, filter?.DateTo);
            var totalCount = await _eventService.GetUserEventsCountAsync(userId, filter?.EventTypeIds, filter?.EventStatuses, filter?.DateFrom, filter?.DateTo);
            return Ok(new PagedResult<ConditionEventsBusinessModel>()
            {
                Items = groupedFacts,
                TotalCount = totalCount
            });
        }

        [HttpDelete("fact/{eventId}")]
        public async Task<IActionResult> DeleteUserFact(int eventId)
        {
            var result = await _eventService.DeleteEventAsync(eventId);
            return Ok(result);
        }

        [HttpGet("{eventId}/relatedEvent")]
        public async Task<IActionResult> GetRelatedEvent(int eventId)
        {
            var result = await _eventService.GetRelatedEvent(eventId);
            return Ok(result);
        }

        [HttpPost("current/{typeId}")]
        public async Task<IActionResult> CreateUserFact(int typeId, [FromForm] EventCreateBaseViewModel eventModel)
        {
            var userId = ClaimHelper.GetUserId(User);
            var info = await _eventService.CreateCurrentEventAsync(eventModel.ToBusinessModel(typeId, userId));
            return Ok(info);
        }

        [HttpPatch("fact/{id}")]
        public async Task<IActionResult> UpdateUserFact(int id, [FromForm] EventUpdateViewModel eventModel)
        {
            var info = await _eventService.UpdateEventAsync(eventModel.ToBusinessModel(id));
            return Ok(info);
        }

        [HttpPost("debug")]
        public async Task<IActionResult> Debug()
        {
            var id = ClaimHelper.GetUserId(User);

            if (id != "273f3d80-3aa1-4f51-b866-01183feaab23")
            {
                throw new UnauthorizedAccessException();
            }

            return NoContent();
        }

        [HttpGet("getEventState")]
        public async Task<IActionResult> GetEventState([FromQuery] DateTimeOffset? timestamp)
        {
            var userId = ClaimHelper.GetUserId(User);
            try
            {
                var eventState = await _eventService.GetEventState(userId, timestamp);
                return Ok(eventState);
            }
            catch (CustomException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("coords")]
        public async Task<IActionResult> GetCoords()
        {
            var userId = ClaimHelper.GetUserId(User);
            var coords = await _eventService.GetCoords(userId);
            return Ok(coords);
        }

        [AllowAnonymous]
        [HttpPost("error")]
        public IActionResult JavascriptErrorLog([FromBody] object error)
        {
            _logger.LogError("JavaScript Error: {Error}", error.ToString());
            return NoContent();
        }

    }
}
