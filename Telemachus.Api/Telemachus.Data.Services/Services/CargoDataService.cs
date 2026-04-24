using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Enums;

using Microsoft.EntityFrameworkCore;

using Telemachus.Data.Models.Cargo;
using Telemachus.Data.Models.Events;
using Telemachus.Data.Services.Context;
using Telemachus.Data.Services.Interfaces;

namespace Telemachus.Data.Services.Services
{
    public class CargoDataService : ICargoDataService
    {
        private readonly TelemachusContext _context;

        public CargoDataService(TelemachusContext context)
        {
            _context = context;
        }
        public async Task<List<CargoModel>> GetCargoStatus(string userId, DateTimeOffset timestamp)
        {

            var cargoDetailsQuery = _context.CargoDetails
                .AsNoTracking()
                .Include(cd => cd.Event)
                .ThenInclude(cd => cd.EventType)
                .Where(c =>
                c.Cargo.UserId == userId &&
                c.Cargo.StartedOn <= timestamp &&
                (c.Cargo.CompletedOn.HasValue == false || c.Cargo.CompletedOn > timestamp) &&
                c.Timestamp.HasValue &&
                c.Timestamp <= timestamp &&
                c.Quantity.HasValue &&
                (c.Quantity != 0)
                );

            var cargoDetails = await cargoDetailsQuery.ToListAsync();
            var cargoIds = cargoDetails.Select(cd => cd.CargoId).Distinct().ToList();
            var cargoes = await _context.Cargoes.AsNoTracking().Include(c => c.Grade).Where(c => cargoIds.Contains(c.Id)).ToListAsync();

            foreach (var cargo in cargoes)
            {
                int tonnage = (int)cargoDetails.Where(cd => cd.CargoId == cargo.Id).Sum(cd => cd.Quantity ?? 0);
                cargo.CargoTonnage = tonnage;

            }

            return cargoes;
        }

        public async Task<List<CargoDetailModel>> GetCargoDetailsInRange(string userId, DateTimeOffset minTimestamp, DateTimeOffset maxTimestamp)
        {
            return await _context.CargoDetails
                .AsNoTracking()
                .Include(cd => cd.Cargo).ThenInclude(c => c.Grade)
                .Where(c =>
                    c.Cargo.UserId == userId &&
                    c.Cargo.StartedOn <= maxTimestamp &&
                    (c.Cargo.CompletedOn.HasValue == false || c.Cargo.CompletedOn > minTimestamp) &&
                    c.Timestamp.HasValue &&
                    c.Timestamp <= maxTimestamp &&
                    c.Quantity.HasValue &&
                    c.Quantity != 0)
                .ToListAsync();
        }

        public async Task DeleteCargo(int eventId)
        {
            var targetEvent = await _context.Events
                .Include(e => e.EventType)
                .Include(e => e.ChildrenEvents)
                .Where(e => e.Id == eventId && EventType.ParcelCommenceGroup.Contains(e.EventType.BusinessId))
                .FirstOrDefaultAsync();

            if (targetEvent?.CargoDetailId == null)
            {
                return;
            }

            var eventTypeId = targetEvent.EventType.BusinessId;

            var cargo = await _context.CargoDetails
                .Where(cd => cd.Event.Id == eventId)
                .Include(cd => cd.Cargo)
                .ThenInclude(c => c.CargoDetails)
                .ThenInclude(c => c.Event)
                .ThenInclude(c => c.EventType)
                .Select(cd => cd.Cargo)
                .FirstOrDefaultAsync();

            if (cargo == null)
            {
                return;
            }

            if (eventTypeId == EventType.CommenceLoadingParcel)
            {

                if (cargo.CargoDetails.Any(cd => cd.Event.Timestamp > targetEvent.Timestamp))
                {
                    throw new CustomException("To delete this fact, first delete all paired cargo events.");
                }

            }

            var cargoToRemove = cargo.CargoDetails.Where(cd => cd.Id == targetEvent.CargoDetailId).Single();

            cargoToRemove.Event.CargoDetailId = null;

            await _context.SaveChangesAsync();

            _context.CargoDetails.Remove(cargoToRemove);

            var items = cargo.CargoDetails.Where(cd => cd.Id != cargoToRemove.Id).ToList();

            if (eventTypeId == EventType.CommenceLoadingParcel)
            {

                if (!items.Any())
                {
                    _context.Cargoes.Remove(cargo);
                }
                else
                {
                    cargo.StartedOn = items.Where(cd => cd.Event.EventType.BusinessId == EventType.CommenceLoadingParcel).Min(cd => cd.Timestamp);
                    var currentQuantity = items
                        .Sum(cd => cd.Quantity.GetValueOrDefault());
                    cargo.CompletedOn = null;
                    if (currentQuantity == 0)
                    {
                        var completedOn = items
                            .Where(cd => cd.Event.EventType.BusinessId == EventType.CommenceDischargingParcel && cd.Quantity.HasValue)
                            .Select(cd => cd.Timestamp)
                            .Max();
                        cargo.CompletedOn = completedOn;
                    }
                }

            }
            if (eventTypeId == EventType.CommenceDischargingParcel)
            {

                var currentQuantity = items
                    .Sum(cd => cd.Quantity.GetValueOrDefault());
                cargo.CompletedOn = null;
                if (currentQuantity == 0)
                {
                    var completedOn = items
                        .Where(cd => cd.Event.EventType.BusinessId == EventType.CommenceDischargingParcel && cd.Quantity.HasValue)
                        .Select(cd => cd.Timestamp)
                        .Max();

                    cargo.CompletedOn = completedOn;
                }
            }

            await _context.SaveChangesAsync();

            await UpdateVoyageDetails(targetEvent.UserId, targetEvent.Timestamp?.UtcDateTime);


            await _context.SaveChangesAsync();

            return;

        }

        private async Task UpdateVoyageDetails(string userId, DateTime? timestamp)
        {
            if (timestamp == null)
            {
                return;
            }
            var events = await _context.Events.AsNoTracking()
                .Where(e => e.UserId == userId && e.Timestamp.HasValue && e.Timestamp >= timestamp)
                .Select(e => new EventDataModel()
                {
                    Id = e.Id,
                    Timestamp = e.Timestamp
                })
                .ToListAsync();
            foreach (var ev in events)
            {
                var cargoes = await GetCargoStatus(userId, ev.Timestamp.Value);
                var voyageEvent = await _context.MrvMisData.Where(e => e.EventId == ev.Id).FirstOrDefaultAsync();
                if (voyageEvent != null)
                {
                    var tonnage = cargoes.Sum(c => c.CargoTonnage);
                    voyageEvent.Cargo = tonnage > 1 ? tonnage : 0;
                    voyageEvent.CargoStatus = tonnage > 0 ? 1 : 0;
                }

            }
        }
        public async Task<List<GradeModel>> GetGrades()
        {
            return await _context.Grades.OrderBy(g => g.Name).ToListAsync();
        }
        public async Task<CargoDetailModel> GetCargoDetails(int cardoDetailId)
        {
            return await _context.CargoDetails
                .Where(d => d.Id == cardoDetailId).FirstOrDefaultAsync();
        }
        public async Task<CargoModel> GetCargo(int cardoDetailId)
        {
            return await _context.Cargoes
                .Include(c => c.Grade)
                .Include(c => c.CargoDetails)
                .ThenInclude(c => c.Event)
                .ThenInclude(c => c.Port)
                .ThenInclude(c => c.Country)
                .Include(c => c.CargoDetails)
                .ThenInclude(c => c.Event)
                .ThenInclude(c => c.EventType)
                .Where(d => d.CargoDetails.Any(cd => cd.Id == cardoDetailId))
                .SingleOrDefaultAsync();
        }

        public async Task<List<CargoModel>> GetAvailableForDischarging(string userId, int? cargoDetailsId)
        {
            CargoModel targetCargo = cargoDetailsId.HasValue ? await GetCargo(cargoDetailsId.Value) : null;

            var query = _context.Cargoes
                .Include(c => c.Grade)
                .Include(c => c.CargoDetails)
                .ThenInclude(c => c.Event)
                .ThenInclude(c => c.Port)
                .ThenInclude(c => c.Country)
                .AsQueryable();

            query = query.Where(c => c.UserId == userId);

            if (targetCargo != null)
            {
                query = query.Where(c => c.StartedOn.Value <= targetCargo.StartedOn.Value);
            }
            else
            {
                // TODO: pass event timestamp and check:
                //query = query.Where(c => c.StartedOn.Value < targetCargo.StartedOn.Value);
            }

            var cargoes = await query.Where(c => c.CargoDetails.All(cd => cd.Timestamp.HasValue && cd.Quantity.HasValue) &&
                    c.CargoDetails.Sum(cd => cd.Quantity.Value) > 0)
                .OrderByDescending(c => c.StartedOn)
                .ToListAsync();
            cargoes.ForEach(c => c.CargoDetails = c.CargoDetails.OrderByDescending(cd => cd.Timestamp).ToList());
            return cargoes;
        }
        public async Task CreateCargoDetails(int eventId, CargoDetailModel cargoDetails)
        {
            var dbEvent = await _context.Events
                .Include(e => e.EventType)
                .FirstAsync(e => e.Id == eventId);

            if (cargoDetails.CargoId > 0)
            {
                var existingCargo = await _context.Cargoes.FirstAsync(c => c.Id == cargoDetails.CargoId);
                cargoDetails.Cargo = existingCargo;
            }
            else
            {
                var cargoToMergeWith = await _context.Cargoes.Where(c => c.UserId == dbEvent.UserId && c.GradeId == cargoDetails.Cargo.GradeId && c.Parcel == cargoDetails.Cargo.Parcel && !c.CompletedOn.HasValue && c.StartedOn < dbEvent.Timestamp)
                    .FirstOrDefaultAsync();

                if (cargoToMergeWith != null)
                {
                    cargoDetails.Cargo = cargoToMergeWith;
                }
                else
                {
                    if (EventType.CommenceLoadingParcel == dbEvent.EventType.BusinessId)
                    {
                        cargoDetails.Cargo.StartedOn = dbEvent.Timestamp;
                    }
                    _context.Cargoes.Add(cargoDetails.Cargo);
                    await _context.SaveChangesAsync();
                }
            }

            if (EventType.CommenceLoadingParcel == dbEvent.EventType.BusinessId)
            {
                cargoDetails.Quantity = 1;
                cargoDetails.Timestamp = dbEvent.Timestamp;
            }

            _context.CargoDetails.Add(cargoDetails);
            await _context.SaveChangesAsync();

            dbEvent.CargoDetailId = cargoDetails.Id;

            await _context.SaveChangesAsync();

            await UpdateVoyageDetails(dbEvent.UserId, dbEvent.Timestamp?.UtcDateTime);

            await _context.SaveChangesAsync();

        }
        public async Task UpdateCargoDetails(int eventId, CargoDetailModel cargoDetails)
        {
            if (cargoDetails.Id == 0)
            {
                await CreateCargoDetails(eventId, cargoDetails);
                return;
            }
            var dbItem = await _context.CargoDetails
                .Include(d => d.Cargo)
                .Include(d => d.Event)
                .ThenInclude(d => d.EventType)
                .Include(d => d.Event)
                .ThenInclude(d => d.ChildrenEvents)
                .ThenInclude(d => d.EventType)
                .FirstAsync(d => d.Id == cargoDetails.Id);
            dbItem.Cargo.GradeId = cargoDetails.Cargo.GradeId;
            dbItem.Cargo.Parcel = cargoDetails.Cargo.Parcel;
            dbItem.Quantity = cargoDetails.Quantity;

            var targetEvent = dbItem.Event.ChildEvent ?? dbItem.Event;

            if (EventType.CompleteLoadingParcel == targetEvent.EventType.BusinessId)
            {
                //dbItem.Timestamp = targetEvent.Timestamp;
                //dbItem.Cargo.StartedOn = targetEvent.Timestamp;
                var currentQuantity = await _context.CargoDetails
                    .Where(cd => cd.Cargo.Id == dbItem.CargoId && cd.Id != cargoDetails.Id)
                    .SumAsync(cd => cd.Quantity.GetValueOrDefault());

                currentQuantity = currentQuantity + cargoDetails.Quantity.Value;
                if (currentQuantity > 0)
                {
                    dbItem.Cargo.CompletedOn = null;
                }
                else if (currentQuantity == 0)
                {
                    var completedOn = await _context.CargoDetails
                        .Where(cd => cd.Cargo.Id == dbItem.CargoId && cd.Event.EventType.BusinessId == EventType.CommenceDischargingParcel && cd.Quantity.HasValue)
                        .MaxAsync(cd => cd.Timestamp);
                    dbItem.Cargo.CompletedOn = completedOn;
                }
                await _context.SaveChangesAsync();
            }

            if (EventType.CompleteDischargingParcel == targetEvent.EventType.BusinessId)
            {
                dbItem.Timestamp = targetEvent.Timestamp;
                dbItem.Quantity = -Math.Abs(cargoDetails.Quantity.Value);
                await _context.SaveChangesAsync();
                dbItem.Cargo.CompletedOn = null;
                var cargo = await _context.Cargoes
                    .Include(c => c.CargoDetails)
                    .Where(c => c.Id == dbItem.CargoId && c.CargoDetails.Any() && c.CargoDetails.All(cd => cd.Quantity.HasValue))
                    .FirstOrDefaultAsync();
                if (cargo != null)
                {
                    var remainingQuantity = cargo.CargoDetails.Sum(cd => cd.Quantity.Value);
                    if (remainingQuantity == 0)
                    {
                        dbItem.Cargo.CompletedOn = dbItem.Timestamp;

                    }
                }
                await _context.SaveChangesAsync();
            }


            await UpdateVoyageDetails(targetEvent.UserId, targetEvent.Timestamp?.UtcDateTime);

            await _context.SaveChangesAsync();


        }
    }
}
