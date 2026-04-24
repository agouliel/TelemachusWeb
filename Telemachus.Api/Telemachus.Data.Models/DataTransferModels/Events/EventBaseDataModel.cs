using System;

namespace Telemachus.Data.Models.DataTransferModels.Events
{
    public class EventBaseDataModel
    {
        public DateTimeOffset? Timestamp { get; set; }
        public int? ParentEventId { get; set; }
        public int ConditionId { get; set; }
        public int VoyageId { get; set; }
        public string UserId { get; set; }
        public int StatusId { get; set; }
        public int EventTypeId { get; set; }
        public int? PortId { get; set; }
        public string Comment { get; set; }
        public string Terminal { get; set; }
        public string CustomEventName { get; set; }
        public Guid CurrentVoyageConditionKey { get; set; }
        public DateTimeOffset? ConditionStartedDate { get; set; }
        public int? LatDegrees { get; set; }
        public int? LatMinutes { get; set; }
        public int? LatSeconds { get; set; }
        public int? LongDegrees { get; set; }
        public int? LongMinutes { get; set; }
        public int? LongSeconds { get; set; }
        public int? FuelType { get; set; }
        public Guid? EventTypeBusinessId { get; set; }
        public int? BunkeringDataId { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }
    }
}
