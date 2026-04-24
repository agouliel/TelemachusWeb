using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Telemachus.Data.Models.Events
{
    public class EventConditionDataModel : EntityMaster
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<EventDataModel> Events { get; set; }
        public ICollection<EventTypesConditionsDataModel> EventTypesConditions { get; set; }
        public ICollection<EventTypeDataModel> EventTypes { get; set; }
        public ICollection<VoyageDataModel> Voyages { get; set; }
    }
}
