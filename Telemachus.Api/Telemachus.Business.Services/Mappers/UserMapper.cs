using Telemachus.Business.Models.Login;
using Telemachus.Business.Models.Reports.Design;
using Telemachus.Data.Models.Authentication;
using Telemachus.Data.Models.Reports;

namespace Telemachus.Business.Services.Mappers
{
    public static class UserMapper
    {
        public static UserBusinessModel ToBusinessModel(this User model)
        {
            if (model == null)
            {
                return null;
            }
            return new UserBusinessModel()
            {
                Id = model.Id,
                Name = model.UserName,
                Prefix = model.Prefix,
            };
        }
        public static TankViewModel ToBusinessModel(this TankUserSpecsDataModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new TankViewModel()
            {
                TankId = model.Id,
                TankName = model.TankName ?? model.Tank.Name,
                DisplayOrder = model.DisplayOrder,
                TankTypeId = model.Tank.TankType,
                IsArchived = !model.IsActive,
                MaxCapacity = model.MaxCapacity,
                VesselId = model.UserId,
                FuelTypeId = model.Tank.FuelTypeId,
                DateArchived = model.DateArchived
            };
        }
    }
}
