namespace Telemachus.Data.Models.Events
{
    public class EventTypesConditionsDataModel : EntityMaster
    {
        public int Id { get; set; }
        public int ConditionId { get; set; }
        public int EventTypeId { get; set; }
        public EventConditionDataModel EventCondition { get; set; }
        public EventTypeDataModel EventType { get; set; }
    }
}
