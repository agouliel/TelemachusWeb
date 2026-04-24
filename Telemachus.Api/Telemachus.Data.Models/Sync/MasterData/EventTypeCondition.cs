using System;

namespace Telemachus.Data.Models.Sync.MasterData
{
    public class EventTypeCondition : EntityBase
    {
        public Guid Id { get; set; }
        public Guid ConditionId { get; set; }
        public Guid EventTypeId { get; set; }
        public int DisplayIndex { get; set; }
    }
}
