using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Telemachus.Data.Models.Authentication;

namespace Telemachus.Data.Models.Cargo
{
    [Table("cargoes")]
    public class CargoModel : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        [Required]
        public User User { get; set; }
        public int GradeId { get; set; }
        [ForeignKey(nameof(GradeId))]
        public GradeModel Grade { get; set; }
        public int Parcel { get; set; }
        public DateTimeOffset? StartedOn { get; set; }
        public DateTimeOffset? CompletedOn { get; set; }
        [Required]
        public string BusinessId { get; set; }
        public ICollection<CargoDetailModel> CargoDetails { get; set; }
        [NotMapped]
        public int? CargoTonnage { get; set; }
        [NotMapped]
        public int? MaxQuantity { get; set; }

    }
}
