using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Telemachus.Data.Models.Reports;

namespace Telemachus.Data.Models.Events
{
    public class EventTypeDataModel : EntityMaster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<EventDataModel> Events { get; set; }
        public ICollection<EventTypesConditionsDataModel> EventTypesConditions { get; set; }
        public ICollection<EventTypePrerequisiteDataModel> Prerequisites { get; set; }
        public int? PairedEventTypeId { get; set; }
        public EventTypeDataModel PairedEventType { get; set; }
        public int? NextConditionId { get; set; }
        public bool Transit { get; set; }
        public int? AvailableAfterEventTypeId { get; set; }
        // TODO: To be deleted
        public EventTypeSettingEnum EventType { get; set; }
        public EventConditionDataModel NextCondition { get; set; }
        //public ICollection<EventTypeReportFieldDataModel> AvailableReportFields { get; set; }

        public int? ReportTypeId { get; set; }
        public bool? OnePairPerTime { get; set; } = false;
        public ReportTypeDataModel ReportType { get; set; }
        [NotMapped]
        public string Comment { get; set; }
        [NotMapped]
        public bool Available { get; set; } = false;
        [NotMapped]
        public bool IsPairedEvent { get; set; } = false;
        [NotMapped]
        public bool Relevant { get; set; } = false;
        [NotMapped]
        public bool Suggested { get; set; } = false;
        [NotMapped]
        public bool PairedConditionChange { get; set; } = false;
        //[NotMapped]
        //public List<int> EventTypesConditionIds { get; set; }

        [JsonIgnore]
        public bool IsCommenceBunkering
        {
            get
            {
                return BusinessId == Enums.EventType.CommenceBunkering;
            }
        }
        [JsonIgnore]
        public virtual bool IsCompleteBunkering
        {
            get
            {
                return BusinessId == Enums.EventType.CommenceBunkeringComplete;
            }
        }
        [JsonIgnore]
        public virtual bool IsBunkeringPlan
        {
            get
            {
                return BusinessId == Enums.EventType.BunkeringPlan;
            }
        }
        [JsonIgnore]
        public virtual bool IsBunkeringPlanProjected
        {
            get
            {
                return BusinessId == Enums.EventType.BunkeringPlanProjected;
            }
        }
        [JsonIgnore]
        public virtual bool BelongsToBunkeringPlanGroup
        {
            get
            {
                return Enums.EventType.BunkeringPlanGroup.Contains(BusinessId);
            }
        }
        [JsonIgnore]
        public virtual bool BelongsToPerformanceGroup
        {
            get
            {
                return Enums.ReportType.Performance.Contains(ReportTypeId.Value);
            }
        }
        [JsonIgnore]
        public virtual bool BelongsToBunkeringCompleteOrProjectedGroup
        {
            get
            {
                return Enums.EventType.BunkeringCompleteOrProjectedGroup.Contains(BusinessId);
            }
        }
        [JsonIgnore]
        public virtual bool BelongsToBunkeringGroups
        {
            get
            {
                return Enums.EventType.BunkeringGroup.Contains(BusinessId);
            }
        }

        // TODO: EventTypeSettingEnum
        //public virtual bool HasPairedEventBehaviour()
        //{
        //    return EventType == EventTypeSettingEnum.PairedEvent;
        //}
        //public virtual bool HasOneTimeEventBehaviour()
        //{
        //    return EventType == EventTypeSettingEnum.OneTimeEvent;
        //}
        //public virtual bool HasRecurringBehaviour()
        //{
        //    return EventType == EventTypeSettingEnum.Recurring;
        //}
        //public virtual bool HasRecurringAfterSecondEventBehaviour()
        //{
        //    return EventType == EventTypeSettingEnum.RecurringAfterSecondEvent;
        //}
        //public virtual bool HasOneEventFinishBehaviour()
        //{
        //    return EventType == EventTypeSettingEnum.OneEventFinish;
        //}
        //public virtual bool HasCustomBehaviour()
        //{
        //    return EventType == EventTypeSettingEnum.Custom;
        //}
        //public virtual bool HasRecurringAfterSecondEventPairedEventIsInProgressBehaviour()
        //{
        //    return EventType == EventTypeSettingEnum.RecurringAfterSecondEventPairedEventIsInProgress;
        //}

        // END

        public virtual bool IsCommenceMooring()
        {
            return BusinessId == Enums.EventType.CommenceMooring;
        }
        public virtual bool IsCompleteMooring()
        {
            return BusinessId == Enums.EventType.CompleteMooring;
        }
        public virtual bool IsCommenceUnMooring()
        {
            return BusinessId == Enums.EventType.CommenceUnMooring;
        }
        public virtual bool IsCompleteUnMooring()
        {
            return BusinessId == Enums.EventType.CompleteUnmooring;
        }
        public virtual bool IsAnchorUp()
        {
            return BusinessId == Enums.EventType.AnchorUp;
        }
        public virtual bool IsDropAnchor()
        {
            return BusinessId == Enums.EventType.DropAnchor;
        }
        public virtual bool IsCustom()
        {
            return BusinessId == Enums.EventType.Custom;
        }
        public virtual bool ChangesCondition()
        {
            return NextConditionId.HasValue;
        }
    }
}
