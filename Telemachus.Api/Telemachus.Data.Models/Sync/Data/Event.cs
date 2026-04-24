using System;

namespace Telemachus.Data.Models.Sync.Data
{
    public class Event : EntityBase
    {
        public string Id { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string Terminal { get; set; }
        public Guid StatusId { get; set; }
        public Guid EventTypeId { get; set; }
        public Guid ConditionId { get; set; }
        public string VoyageId { get; set; }
        public Guid CurrentVoyageConditionKey { get; set; }
        public Guid? NextVoyageConditionKey { get; set; }
        public Guid? PreviousVoyageConditionKey { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset? ConditionStartedDate { get; set; }
        public string ParentEventId { get; set; }
        public string PortId { get; set; }
        public string CustomEventName { get; set; }
        public bool ExcludeFromStatement { get; set; }
        public bool HiddenDate { get; set; }
        public int DisplayIndex { get; set; }
        public int? LatDegrees { get; set; }
        public int? LatMinutes { get; set; }
        public int? LatSeconds { get; set; }
        public int? LongDegrees { get; set; }
        public int? LongMinutes { get; set; }
        public int? LongSeconds { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }
        public string BunkeringDataId { get; set; }

    }
}
