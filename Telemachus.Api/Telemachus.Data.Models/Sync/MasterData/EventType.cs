using System;

namespace Telemachus.Data.Models.Sync.MasterData
{
    public class EventType : EntityBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? PairedEventTypeId { get; set; }
        public Guid? NextConditionId { get; set; }
        public bool Transit { get; set; }
        public Guid? ReportTypeId { get; set; }
        public int DisplayIndex { get; set; }
    }
}
