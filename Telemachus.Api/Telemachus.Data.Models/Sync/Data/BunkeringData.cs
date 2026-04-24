using System;

namespace Telemachus.Data.Models.Sync.Data
{
    public class BunkeringData : EntityBase
    {
        public string Id { get; set; }
        public string Bdn { get; set; }
        public int FuelType { get; set; }
        public string Supplier { get; set; }
        public string Density { get; set; }
        public string SulphurContent { get; set; }
        public string Viscosity { get; set; }
        public string PortId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string TotalAmount { get; set; }
        public int DisplayIndex { get; set; }
    }
}
