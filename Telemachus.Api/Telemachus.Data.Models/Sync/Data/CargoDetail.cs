using System;

namespace Telemachus.Data.Models.Sync.Data
{
    public class CargoDetail : EntityBase
    {
        public string Id { get; set; }
        public string CargoId { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public int? Quantity { get; set; }
        public int DisplayIndex { get; set; }
    }
}
