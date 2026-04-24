using System;

namespace Telemachus.Data.Models.Sync.MasterData
{
    public class TankUserSpecs : EntityBase
    {
        public Guid Id { get; set; }
        public Guid TankId { get; set; }
        public string MaxCapacity { get; set; }
        public int DisplayOrder { get; set; }
        public string TankName { get; set; }
        public bool IsActive { get; set; }
        public int DisplayIndex { get; set; }
    }
}
