using System;
using System.Collections.Generic;
using Telemachus.Business.Models.Events;

namespace Telemachus.Business.Models
{
    public class EventStateBusinessModel
    {
        public EventConditionBusinessModel Condition { get; set; }
        public List<EventTypeBusinessModel> EventTypes { get; set; }
        public List<BunkeringDataBusinessModel> BunkeringPlans { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public int? LatDegrees { get; set; }
        public int? LatMinutes { get; set; }
        public int? LatSeconds { get; set; }
        public int? LongDegrees { get; set; }
        public int? LongMinutes { get; set; }
        public int? LongSeconds { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }
        public PortBusinessModel Port { get; set; }
        public List<PortBusinessModel> NearestPorts { get; set; }
        public List<PortBusinessModel> DefaultPorts { get; set; }

    }
}
