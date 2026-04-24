using System;

namespace Telemachus.Data.Models.Sync.MasterData
{
    public class EventTypePrerequisite : EntityBase
    {
        public Guid Id { get; set; }
        public Guid EventTypeId { get; set; }
        public Guid? AvailableAfterEventTypeId { get; set; }
        public bool Completed { get; set; }
        public bool Override { get; set; }
        public int DisplayIndex { get; set; }
    }
}
