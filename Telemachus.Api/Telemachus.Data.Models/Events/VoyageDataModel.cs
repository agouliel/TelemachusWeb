using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Telemachus.Data.Models.Authentication;

namespace Telemachus.Data.Models.Events
{
    public class VoyageDataModel : EntityBase
    {
        public int Id { get; set; }
        public ICollection<EventDataModel> Events { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public EventConditionDataModel CurrentCondition { get; set; }
        public bool IsFinished { get; set; }
        public int CurrentConditionId { get; set; }
        public Guid CurrentVoyageConditionKey { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string BusinessId { get; set; }
    }
}
