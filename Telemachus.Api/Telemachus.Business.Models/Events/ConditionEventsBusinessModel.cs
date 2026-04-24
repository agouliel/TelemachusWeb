using System;
using System.Collections.Generic;

namespace Telemachus.Business.Models.Events
{
    public class ConditionEventsBusinessModel
    {
        public int ConditionId { get; set; }
        public Guid ConditionKey { get; set; }
        public bool IsCurrentCondition { get; set; }
        public int VoyageId { get; set; }
        public int InProgressEventsCount { get; set; }
        public string ConditionName { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string VesselName { get; set; }
        public IEnumerable<EventBusinessModel> Events { get; set; }
    }
}
