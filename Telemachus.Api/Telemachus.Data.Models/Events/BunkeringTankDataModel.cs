using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Telemachus.Data.Models.Reports;

namespace Telemachus.Data.Models.Events
{
    [Table("bunkering_tank_data")]
    public class BunkeringTankDataModel : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BunkeringDataId { get; set; }
        [ForeignKey(nameof(BunkeringDataId))]
        public BunkeringDataModel BunkeringData { get; set; }
        public int TankId { get; set; }
        [ForeignKey(nameof(TankId))]
        public TankDataModel Tank { get; set; }
        public string Amount { get; set; }
        public int? ComminglingId { get; set; }
        [ForeignKey(nameof(ComminglingId))]
        public BunkeringDataModel ComminglingData { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public string BusinessId { get; set; }

    }
}
