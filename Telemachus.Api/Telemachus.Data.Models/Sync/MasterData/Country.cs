using System;

namespace Telemachus.Data.Models.Sync.MasterData
{
    public class Country
    {
        public string Id { get; set; }
        public string Numerical { get; set; }
        public string Name { get; set; }
        public string Alpha2 { get; set; }
        public string Alpha3 { get; set; }
        public string Nationality { get; set; }
        public string RegionId { get; set; }
        public string LloydsCode { get; set; }
        public string PhoneCode { get; set; }
        public DateTime DateModified { get; set; }
    }
}
