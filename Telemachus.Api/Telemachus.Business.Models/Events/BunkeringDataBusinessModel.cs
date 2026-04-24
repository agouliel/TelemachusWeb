using System;
using System.Collections.Generic;

namespace Telemachus.Business.Models.Events
{
    public class BunkeringDataBusinessModel
    {
        public int Id { get; set; }
        public string Bdn { get; set; }
        public string SulphurContent { get; set; }
        public string Density { get; set; }
        public string Viscosity { get; set; }
        public int FuelType { get; set; }
        public string Supplier { get; set; }
        public string TotalAmount { get; set; }
        public string NamedAmount { get; set; }
        public string RobAmount { get; set; }
        public string RobAmountDiff { get; set; }
        public DateTimeOffset RobAmountDiffTimestamp { get; set; }
        public int PortId { get; set; }
        public string PortName { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string BusinessId { get; set; }
        public List<BunkeringTankBusinessModel> Tanks { get; set; }
        public string PortCountry { get; set; }
        public string UserId { get; set; }
    }
}
