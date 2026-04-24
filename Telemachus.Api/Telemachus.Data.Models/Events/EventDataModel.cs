using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

using NetTopologySuite.Geometries;

using Telemachus.Data.Models.Authentication;
using Telemachus.Data.Models.Cargo;
using Telemachus.Data.Models.Ports;
using Telemachus.Data.Models.Reports;

namespace Telemachus.Data.Models.Events
{
    public class EventDataModel : EntityBase
    {
        public int Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string BusinessId { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string Terminal { get; set; }
        public int StatusId { get; set; }
        public EventStatusDataModel Status { get; set; }
        public int EventTypeId { get; set; }
        public EventTypeDataModel EventType { get; set; }
        public int ConditionId { get; set; }
        public EventConditionDataModel EventCondition { get; set; }
        public int VoyageId { get; set; }
        public VoyageDataModel Voyage { get; set; }
        public Guid CurrentVoyageConditionKey { get; set; }
        public Guid? NextVoyageConditionKey { get; set; }
        public Guid? PreviousVoyageConditionKey { get; set; }
        public string Comment { get; set; }
        public ICollection<EventAttachmentDataModel> Attachments { get; set; }
        public DateTimeOffset? ConditionStartedDate { get; set; }
        public ICollection<ReportDataModel> Reports { get; set; }
        public int? ParentEventId { get; set; }
        public EventDataModel ParentEvent { get; set; }
        public ICollection<EventDataModel> ChildrenEvents { get; set; }
        public int? PortId { get; set; }
        [ForeignKey("PortId")]
        public Port Port { get; set; }
        public string CustomEventName { get; set; }
        public bool ExcludeFromStatement { get; set; }
        public bool HiddenDate { get; set; }
        [NotMapped]
        public string Error { get; set; }
        public int? LatDegrees { get; set; }
        public int? LatMinutes { get; set; }
        public int? LatSeconds { get; set; }
        public int? LongDegrees { get; set; }
        public int? LongMinutes { get; set; }
        public int? LongSeconds { get; set; }
        public string DebugData { get; set; }
        public int? BunkeringDataId { get; set; }
        [ForeignKey("BunkeringDataId")]
        public BunkeringDataModel BunkeringData { get; set; }

        [JsonIgnore]
        public virtual bool HasBunkeringData
        {
            get
            {
                return BunkeringData != null;
            }
        }

        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }
        public Point Point { get; set; }
        public int? CargoDetailId { get; set; }
        [ForeignKey(nameof(CargoDetailId))]
        public CargoDetailModel CargoDetail { get; set; }

        [NotMapped]

        public EventDataModel ChildEvent
        {
            get
            {
                return ChildrenEvents?.FirstOrDefault();
            }
        }
        [NotMapped]
        public List<CargoModel> Cargoes { get; set; }
        [NotMapped]
        public int? FuelType { get; set; }

        public virtual bool ChangedCondition()
        {
            if (EventType?.ChangesCondition() ?? false)
            {
                return true;
            }
            if (ChildEvent?.EventType?.ChangesCondition() ?? false)
            {
                if (ChildEvent.Timestamp.HasValue)
                {
                    return true;
                }
            }
            return false;
        }
        public StsOperation StsOperation { get; set; }

    }
}
