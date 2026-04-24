using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Telemachus.Business.Interfaces.Cargo;
using Telemachus.Business.Models.Cargo;
using Telemachus.Helpers;

namespace Telemachus.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class CargoController : Controller
    {
        private readonly ICargoBusinessService _cargoBusinessService;

        public CargoController(ICargoBusinessService cargoBusinessService)
        {
            _cargoBusinessService = cargoBusinessService;
        }

        [HttpGet("state/{cargoDetailsId:int?}")]
        public async Task<IActionResult> GetCargoState(int? cargoDetailsId)
        {
            var userId = ClaimHelper.GetUserId(User);
            var model = new CargoStateBusinessModel();
            if (cargoDetailsId.HasValue)
            {
                model.Cargo = await _cargoBusinessService.GetCargo(cargoDetailsId.Value);
            }
            else
            {
                model.AvailableForDischarging = await _cargoBusinessService.GetAvailableForDischarging(userId, cargoDetailsId);
            }
            model.Grades = await _cargoBusinessService.GetGrades();
            return Ok(model);
        }
    }
}
