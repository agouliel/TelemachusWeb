using System;
using System.Collections.Generic;
using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Models.DataTransferModels
{
    public class ConditionEventsDataModel
    {
        public int ConditionId { get; set; }
        public Guid ConditionKey { get; set; }
        public string ConditionName { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string VesselName { get; set; }
        public List<EventDataModel> Events { get; set; }
    }
}
