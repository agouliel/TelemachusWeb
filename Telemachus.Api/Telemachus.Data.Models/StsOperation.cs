using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Telemachus.Data.Models.Authentication;
using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Models
{
    [Table("sts_operations")]
    public class StsOperation : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EventId { get; set; }
        public EventDataModel Event { get; set; }
        public bool ReverseLightering { get; set; }
        public string CompanyParticipatingVesselId { get; set; }
        public User CompanyParticipatingVessel { get; set; }
        [Required]
        public string ParticipatingVessel { get; set; }
        public bool SameSizeParticipatingVessel { get; set; }
        public bool RoughSeaState { get; set; }

        public string Comments { get; set; }

        [Required]
        public string BusinessId { get; set; }
    }
}
