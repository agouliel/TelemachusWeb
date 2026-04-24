using System;

namespace Telemachus.Business.Models.Events.Events
{
    public class EventBaseBusinessModel
    {
        public int Id { get; set; }
        public int? ParentEventId { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public int EventTypeId { get; set; }
        public int? PortId { get; set; }
        public string Comment { get; set; }
        public string Terminal { get; set; }
        public string CustomEventName { get; set; }
        public int? LatDegrees { get; set; }
        public int? LatMinutes { get; set; }
        public int? LatSeconds { get; set; }
        public int? LongDegrees { get; set; }
        public int? LongMinutes { get; set; }
        public int? LongSeconds { get; set; }
        public int? FuelType { get; set; }
        public int? BunkeringDataId { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }
        public int? GradeId { get; set; }
        public int? Parcel { get; set; }
        public int? CargoId { get; set; }
        public string UserId { get; set; }
        public StsOperationBusinessModel StsOperation { get; set; }
    }

}
