using System.Collections.Generic;
using System.Linq;
using Telemachus.Business.Models.Cargo;
using Telemachus.Data.Models.Cargo;

namespace Telemachus.Business.Services.Mappers
{
    public static class CargoMapper
    {
        public static GradeBusinessModel ToBusinessModel(this GradeModel model)
        {
            if (model == null)
            {
                return null;
            }

            return new GradeBusinessModel()
            {
                Id = model.Id,
                Name = model.Name,
                BusinessId = model.BusinessId
            };
        }

        public static List<GradeBusinessModel> ToBusinessModel(this IEnumerable<GradeModel> model)
        {
            return model.Select(g => g.ToBusinessModel()).ToList();
        }

        public static CargoBusinessModel ToBusinessModel(this CargoModel model, bool includeDetails = true)
        {
            if (model == null)
            {
                return null;
            }

            return new CargoBusinessModel()
            {
                Id = model.Id,
                UserId = model.UserId,
                GradeId = model.GradeId,
                Grade = model.Grade.ToBusinessModel(),
                Parcel = model.Parcel,
                StartedOn = model.StartedOn,
                CompletedOn = model.CompletedOn,
                BusinessId = model.BusinessId,
                CargoDetails = includeDetails ? model.CargoDetails?.ToBusinessModel(false) : null,
                CargoTonnage = model.CargoTonnage,
                MaxQuantity = model.MaxQuantity,

            };
        }

        public static List<CargoBusinessModel> ToBusinessModel(this IEnumerable<CargoModel> model, bool includeDetails = true)
        {
            return model.Select(c => c.ToBusinessModel(includeDetails)).ToList();
        }
        public static CargoDetailBusinessModel ToBusinessModel(this CargoDetailModel model, bool includeCargo = true)
        {
            if (model == null)
            {
                return null;
            }

            return new CargoDetailBusinessModel()
            {
                Id = model.Id,
                CargoId = model.CargoId,
                Cargo = includeCargo ? model.Cargo?.ToBusinessModel(false) : null,
                Timestamp = model.Timestamp,
                Quantity = model.Quantity,
                QuantityLimit = model.QuantityLimit,
                BusinessId = model.BusinessId,
                PortName = model.Event?.Port?.Name,
                PortCountry = model.Event?.Port?.Country?.Name,
                EventTypeId = model.Event?.EventType?.BusinessId.ToString()
            };
        }

        public static List<CargoDetailBusinessModel> ToBusinessModel(this IEnumerable<CargoDetailModel> model, bool includeCargo = true)
        {
            return model.Select(cd => cd.ToBusinessModel(includeCargo)).ToList();
        }

    }
}
