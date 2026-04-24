using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telemachus.Business.Models.Cargo;
using Telemachus.Data.Models.Cargo;

namespace Telemachus.Business.Interfaces.Cargo
{
    public interface ICargoBusinessService
    {
        Task CreateCargoDetails(int eventId, CargoDetailModel cargo);
        Task UpdateCargoDetails(int eventId, CargoDetailModel cargo);
        Task<CargoDetailBusinessModel> GetCargoDetails(int cargoDetailsId);
        Task<List<GradeBusinessModel>> GetGrades();
        Task<List<CargoBusinessModel>> GetAvailableForDischarging(string userId, int? cargoDetailsId);
        Task<CargoBusinessModel> GetCargo(int cargoDetailsId);
        Task<List<CargoBusinessModel>> GetCargoStatus(string userId, DateTimeOffset timestamp);
    }
}
