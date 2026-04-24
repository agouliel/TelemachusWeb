using System.ComponentModel.DataAnnotations;

namespace Telemachus.Business.Models.Events.Events
{
    public class StsOperationBusinessModel
    {
        public int? Id { get; set; }
        public int? EventId { get; set; }
        public bool ReverseLightering { get; set; }
        public string CompanyParticipatingVesselId { get; set; }
        [Required]
        public string ParticipatingVessel { get; set; }
        public bool SameSizeParticipatingVessel { get; set; }

        public string Comments { get; set; }
    }

}
