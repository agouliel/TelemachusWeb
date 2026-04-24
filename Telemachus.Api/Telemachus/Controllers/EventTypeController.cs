using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Telemachus.Business.Interfaces.Events;

namespace Telemachus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EventTypeController : ControllerBase
    {
        private readonly IEventTypeService _eventTypeService;

        public EventTypeController(IEventTypeService eventTypeService)
        {
            _eventTypeService = eventTypeService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllEventTypes()
        {
            var evenTypes = await _eventTypeService.GetEventTypesAsync();
            return Ok(evenTypes.OrderBy(a => a.Name));
        }

    }
}
