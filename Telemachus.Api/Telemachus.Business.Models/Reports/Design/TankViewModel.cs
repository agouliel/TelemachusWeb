using System;
using System.ComponentModel.DataAnnotations;

namespace Telemachus.Business.Models.Reports.Design
{
    public class TankViewModel
    {
        public int TankId { get; set; }
        [Required]
        public string TankName { get; set; }
        [Required]
        public string VesselId { get; set; }
        [Required]
        public string MaxCapacity { get; set; }
        [Required]
        public int DisplayOrder { get; set; }
        [Required]
        public Enums.TankType? TankTypeId { get; set; }
        [Required]
        public int FuelTypeId { get; set; }
        public bool IsArchived { get; set; }
        public DateTime? DateArchived { get; set; }

    }
}
