using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Telemachus.Business.Models.Events;

namespace Telemachus.Business.Models.Reports.Design
{
    public class EventTypeBusinessModel
    {
        public int? EventTypeId { get; set; }
        [Required]
        public string Name { get; set; }
        public string PairedEventName { get; set; }
        public int? PairedEventTypeId { get; set; }
        public EventTypeBusinessModel PairedEventType { get; set; }
        public int? NextConditionId { get; set; }
        public bool Transit { get; set; }
        public int? ReportTypeId { get; set; }
        public List<int> EventTypesConditions { get; set; }
        public List<EventTypePrerequisiteBusinessModel> Prerequisites { get; set; }
        public bool PairedConditionChange { get; set; }
        public bool? Distinct { get; set; } = false;
        public bool? OnePairPerTime { get; set; } = false;
    }
}
