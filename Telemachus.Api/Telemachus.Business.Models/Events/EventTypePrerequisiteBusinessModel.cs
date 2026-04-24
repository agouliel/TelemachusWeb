using System;

namespace Telemachus.Business.Models.Events
{
    public class EventTypePrerequisiteBusinessModel
    {
        public int? Id { get; set; }
        public int EventTypeId { get; set; }
        public EventTypeBusinessModel EventType { get; set; }
        public int AvailableAfterEventTypeId { get; set; }
        public EventTypeBusinessModel AvailableAfterEvent { get; set; }
        public bool? Override { get; set; } = false;
        public bool? Completed { get; set; } = false;
        public Guid? BusinessId { get; set; }
        public bool? Required { get; set; } = false;
        public bool? RequiredForRepetition { get; set; } = false;
    }
}
