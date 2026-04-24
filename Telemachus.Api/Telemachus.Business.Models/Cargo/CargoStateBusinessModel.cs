using System.Collections.Generic;

namespace Telemachus.Business.Models.Cargo
{
    public class CargoStateBusinessModel
    {
        public List<CargoBusinessModel> AvailableForDischarging { get; set; }
        public List<GradeBusinessModel> Grades { get; set; } = new List<GradeBusinessModel>();
        public CargoBusinessModel Cargo { get; set; }
    }
}
