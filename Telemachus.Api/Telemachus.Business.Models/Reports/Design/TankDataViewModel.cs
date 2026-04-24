using System.Collections.Generic;

using Telemachus.Business.Models.Login;
using Telemachus.Data.Models.Reports;

namespace Telemachus.Business.Models.Reports.Design
{
    public class TankDataViewModel
    {
        public List<UserBusinessModel> Vessels { get; set; } = new List<UserBusinessModel>();
        public List<FuelTypeDataModel> FuelTypes { get; set; } = new List<FuelTypeDataModel>();
    }
}
