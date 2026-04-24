using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Telemachus.Data.Models.Authentication;

namespace Telemachus.Data.Models.Reports
{
    [Table("tank_user_specs")]
    public class TankUserSpecsDataModel : EntityMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }
        [Required]
        [ForeignKey("Tank")]
        public int TankId { get; set; }
        public TankDataModel Tank { get; set; }
        [Column(TypeName = "varchar(512)")]
        [Required]
        public string MaxCapacity { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public string TankName { get; set; } = null;
        public bool IsActive { get; set; } = true;

        public DateTime? DateArchived { get; set; }

    }
}
