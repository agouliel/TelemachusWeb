using System;

namespace Telemachus.Data.Models.Sync.MasterData
{
    public class Tank : EntityBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Storage { get; set; } = true;
        public bool Settling { get; set; } = false;
        public bool Serving { get; set; } = false;
        public int DisplayIndex { get; set; }
    }
}
