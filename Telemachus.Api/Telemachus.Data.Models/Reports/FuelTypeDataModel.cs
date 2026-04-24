using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Enums;

namespace Telemachus.Data.Models.Reports
{
    public class FuelTypeDataModel : EntityMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual bool IsHfo()
        {
            return BusinessId == ReportType.Hfo;
        }
        public virtual bool IsMgo()
        {
            return BusinessId == ReportType.Mgo;
        }
    }
}
