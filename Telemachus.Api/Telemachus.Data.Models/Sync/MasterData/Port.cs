using System;

namespace Telemachus.Data.Models.Sync.MasterData
{
    public class Port
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Code { get; set; }
        public string CountryId { get; set; }
        public string RegionId { get; set; }
        public string AreaId { get; set; }
        public double? TimeZone { get; set; }
        public DateTime DateModified { get; set; }
    }
}
