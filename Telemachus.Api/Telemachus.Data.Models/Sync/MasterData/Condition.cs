using System;

namespace Telemachus.Data.Models.Sync.MasterData
{
    public class Condition : EntityBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int DisplayIndex { get; set; }
    }
}
