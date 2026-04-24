using System;

namespace Telemachus.Data.Models.Sync.Data
{
    public class Voyage : EntityBase
    {
        public string Id { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public bool IsFinished { get; set; }
        public Guid CurrentConditionId { get; set; }
        public Guid CurrentVoyageConditionKey { get; set; }
        public int DisplayIndex { get; set; }
    }
}
