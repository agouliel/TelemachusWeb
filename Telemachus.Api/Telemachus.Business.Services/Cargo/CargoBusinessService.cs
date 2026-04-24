using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Telemachus.Business.Interfaces.Cargo;
using Telemachus.Business.Models.Cargo;
using Telemachus.Business.Services.Mappers;
using Telemachus.Data.Models.Cargo;
using Telemachus.Data.Services.Interfaces;

namespace Telemachus.Business.Services.Cargo
{
    public class CargoBusinessService : ICargoBusinessService
    {
        private readonly ICargoDataService _cargoService;

        public CargoBusinessService(ICargoDataService cargoService)
        {
            _cargoService = cargoService;
        }

        public async Task<List<GradeBusinessModel>> GetGrades()
        {
            var grades = await _cargoService.GetGrades();
            return grades.ToBusinessModel();
        }
        public async Task<CargoDetailBusinessModel> GetCargoDetails(int cargoDetailsId)
        {
            var cargoDetails = await _cargoService.GetCargoDetails(cargoDetailsId);
            return cargoDetails.ToBusinessModel();
        }
        public async Task<CargoBusinessModel> GetCargo(int cargoDetailsId)
        {
            var cargo = await _cargoService.GetCargo(cargoDetailsId);
            var maxQuantity = cargo.CargoDetails.Where(cd => cd.Id != cargoDetailsId).Sum(c => c.Quantity);
            cargo.MaxQuantity = maxQuantity;
            return cargo.ToBusinessModel();
        }
        public async Task CreateCargoDetails(int eventId, CargoDetailModel cargo)
        {
            await _cargoService.CreateCargoDetails(eventId, cargo);
        }
        public async Task UpdateCargoDetails(int eventId, CargoDetailModel cargo)
        {
            await _cargoService.UpdateCargoDetails(eventId, cargo);
        }
        public async Task<List<CargoBusinessModel>> GetCargoStatus(string userId, DateTimeOffset timestamp)
        {
            var cargoes = await _cargoService.GetCargoStatus(userId, timestamp);
            return cargoes.Select(c => c.ToBusinessModel()).ToList();
        }
        public async Task<List<CargoBusinessModel>> GetAvailableForDischarging(string userId, int? cargoDetailsId)
        {
            var cargoes = await _cargoService.GetAvailableForDischarging(userId, cargoDetailsId);
            return cargoes.ToBusinessModel();
        }
    }
}
