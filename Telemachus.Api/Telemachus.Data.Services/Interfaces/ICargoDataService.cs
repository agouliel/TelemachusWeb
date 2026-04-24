using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telemachus.Data.Models.Cargo;

namespace Telemachus.Data.Services.Interfaces
{
    public interface ICargoDataService
    {
        Task CreateCargoDetails(int eventId, CargoDetailModel cargoDetails);
        Task UpdateCargoDetails(int cargoDetailsId, CargoDetailModel cargoDetails);
        Task<CargoDetailModel> GetCargoDetails(int cardoDetailId);
        Task<List<GradeModel>> GetGrades();
        Task<List<CargoModel>> GetAvailableForDischarging(string userId, int? cargoDetailsId);
        Task<CargoModel> GetCargo(int cardoDetailId);
        Task DeleteCargo(int eventId);
        Task<List<CargoModel>> GetCargoStatus(string userId, DateTimeOffset timestamp);
        Task<List<CargoDetailModel>> GetCargoDetailsInRange(string userId, DateTimeOffset minTimestamp, DateTimeOffset maxTimestamp);
    }
}
