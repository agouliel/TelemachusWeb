using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telemachus.Business.Services.Voyages;
using Telemachus.Helpers;

namespace Telemachus.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Basic")]
    [Route("api/[controller]")]
    [ApiController]
    public class VoyagesController : ControllerBase
    {
        private string userId => ClaimHelper.GetUserId(User);
        private readonly IVoyageService _voyageService;
        public VoyagesController(IVoyageService voyageService)
        {
            _voyageService = voyageService;
        }
    }
}
