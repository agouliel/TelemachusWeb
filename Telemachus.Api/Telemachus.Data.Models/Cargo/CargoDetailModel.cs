using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Models.Cargo
{
    [Table("cargo_details")]
    public class CargoDetailModel : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CargoId { get; set; }
        [ForeignKey(nameof(CargoId))]
        public CargoModel Cargo { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public int? Quantity { get; set; }
        [NotMapped]
        public int? QuantityLimit { get; set; }
        [Required]
        public string BusinessId { get; set; }
        public EventDataModel Event { get; set; }
    }
}
