using System;
using System.Collections.Generic;
using Telemachus.Business.Models.Login;

namespace Telemachus.Business.Models.Cargo
{
    public class CargoBusinessModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public UserBusinessModel User { get; set; }
        public int GradeId { get; set; }
        public GradeBusinessModel Grade { get; set; }
        public int Parcel { get; set; }
        public DateTimeOffset? StartedOn { get; set; }
        public DateTimeOffset? CompletedOn { get; set; }
        public string BusinessId { get; set; }
        public List<CargoDetailBusinessModel> CargoDetails { get; set; }
        public int? CargoTonnage { get; set; }
        public int? MaxQuantity { get; set; }
    }
}
