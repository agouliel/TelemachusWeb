using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Telemachus.Data.Models.Events
{
    public class EventTypePrerequisiteDataModel : EntityMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int EventTypeId { get; set; }
        [ForeignKey(nameof(EventTypeId))]
        public EventTypeDataModel EventType { get; set; }
        [Required]
        public int AvailableAfterEventTypeId { get; set; }
        [ForeignKey(nameof(AvailableAfterEventTypeId))]
        public EventTypeDataModel AvailableAfterEventType { get; set; }
        [Column("Optional")]
        public bool Override { get; set; } = false;
        public bool Completed { get; set; } = false;
        public bool Required { get; set; } = false;
        public bool RequiredForRepetition { get; set; } = false;
        [NotMapped]
        public string Comment { get; set; }
        [NotMapped]
        public bool IsInvalid { get; set; } = false;
    }
}
