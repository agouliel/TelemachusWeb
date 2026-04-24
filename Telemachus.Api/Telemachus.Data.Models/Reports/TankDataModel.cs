using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Telemachus.Data.Models.Reports
{
    [Table("tanks")]
    public class TankDataModel : EntityMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "varchar(512)")]
        [Required]
        public string Name { get; set; }
        public bool Storage { get; set; } = false;
        public bool Settling { get; set; } = false;
        public bool Serving { get; set; } = false;
        public bool Overflow { get; set; } = false;
        public int FuelTypeId { get; set; }
        [ForeignKey(nameof(FuelTypeId))]
        public FuelTypeDataModel FuelType { get; set; }
        [NotMapped]
        public Enums.TankType? TankType
        {
            get
            {
                if (Storage)
                    return Enums.TankType.Storage;
                if (Settling)
                    return Enums.TankType.Settling;
                if (Serving)
                    return Enums.TankType.Serving;
                if (Overflow)
                    return Enums.TankType.Overflow;
                return null;
            }
            set
            {
                Storage = value == Enums.TankType.Storage;
                Settling = value == Enums.TankType.Settling;
                Serving = value == Enums.TankType.Serving;
                Overflow = value == Enums.TankType.Overflow;
            }
        }
        public ICollection<ReportFieldDataModel> ReportFields { get; set; } = new Collection<ReportFieldDataModel>();
    }
}
