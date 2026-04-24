using System;
using System.Collections.Generic;

using Telemachus.Business.Models.Cargo;
using Telemachus.Business.Models.Events.Events;

namespace Telemachus.Business.Models.Events
{
    public class EventBusinessModel
    {

        public int? PairedEventTypeId { get; set; }
        public string UserId { get; set; }
        public int Id { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string Terminal { get; set; }
        public string Comment { get; set; }
        //public int NumberOfAttachments { get; set; }
        public int StatusId { get; set; }
        public string EventTypeName { get; set; }
        public string StatusBusinessId { get; set; }
        public string CustomEventName { get; set; }
        public int EventTypeId { get; set; }
        //public int ConditionId { get; set; }
        public string ConditionName { get; set; }
        public string ConditionBusinessId { get; set; }
        public int? BunkeringDataId { get; set; }
        public int VoyageId { get; set; }
        public string VoyageBusinessId { get; set; }
        public List<EventAttachmentBusinessModel> Attachments { get; set; }
        public int? ReportId { get; set; }
        //public bool IsAvailableForDelete { get; set; }
        public int? PortId { get; set; }
        public string PortBusinessId { get; set; }
        public int? ParentEventId { get; set; }
        //public string Error { get; set; }
        public int? ReportTypeId { get; set; }
        public int? LatDegrees { get; set; }
        public int? LatMinutes { get; set; }
        public int? LatSeconds { get; set; }
        public int? LongDegrees { get; set; }
        public int? LongMinutes { get; set; }
        public int? LongSeconds { get; set; }
        public Guid? EventTypeBusinessId { get; set; }
        public string PortName { get; set; }
        public string PortCountry { get; set; }
        public int? PortIsEu { get; set; }
        public BunkeringDataBusinessModel BunkeringData { get; set; }
        public string ParentEventBusinessId { get; set; }
        public string BusinessId { get; set; }
        //public Guid? PairedEventTypeBusinessId { get; set; }
        //public Guid? ReportTypeBusinessId { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }
        public int? CargoDetailId { get; set; }
        public CargoDetailBusinessModel CargoDetails { get; set; }
        public EventBusinessModel ParentEvent { get; set; }
        public List<CargoBusinessModel> Cargoes { get; set; }
        public string UserName { get; set; }
        public StsOperationBusinessModel StsOperation { get; set; }


    }
}
