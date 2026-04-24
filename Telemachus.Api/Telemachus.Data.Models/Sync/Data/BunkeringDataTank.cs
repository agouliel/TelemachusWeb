using System;

namespace Telemachus.Data.Models.Sync.Data
{
    public class BunkeringDataTank : EntityBase
    {
        public string Id { get; set; }
        public int DisplayIndex { get; set; }
        public string BunkeringDataId { get; set; }
        public Guid TankId { get; set; }
        public string Amount { get; set; }
        public string ComminglingId { get; set; }
    }
}
