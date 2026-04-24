namespace Telemachus.Business.Models
{
    public class PortBusinessModel
    {
        public int Id { get; set; }
        public string BusinessId { get; set; }
        public string PortCode { get; set; }
        public string PortName { get; set; }
        public string AreaName { get; set; }
        public string RegionName { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public double? Distance { get; set; }

    }
}
