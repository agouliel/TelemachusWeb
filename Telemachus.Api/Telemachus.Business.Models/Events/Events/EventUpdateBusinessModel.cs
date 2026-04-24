using System;
using System.Collections.Generic;

using Telemachus.Data.Models;

namespace Telemachus.Business.Models.Events.Events
{
    public class EventUpdateBusinessModel
    {
        public int Id { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public int? PortId { get; set; }
        public string Comment { get; set; }
        public List<FileViewModel> Files { get; set; }
        public string CustomEventName { get; set; }
        public List<int> RemoveFileIds { get; set; }
        public int? LatDegrees { get; set; }
        public int? LatMinutes { get; set; }
        public int? LatSeconds { get; set; }
        public int? LongDegrees { get; set; }
        public int? LongMinutes { get; set; }
        public int? LongSeconds { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }
        public int? GradeId { get; set; }
        public int? Parcel { get; set; }
        public int? Quantity { get; set; }
        public int? CargoId { get; set; }
        public int? CargoDetailsId { get; set; }
        public StsOperationBusinessModel StsOperation { get; set; }
    }
}
