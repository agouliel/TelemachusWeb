namespace Telemachus.Data.Models.Reports
{
    public class WaterConsumptionViewModel
    {
        public string VesselId { get; set; }
        public string VesselName { get; set; }
        public string VesselPrefix { get; set; }
        public int Year { get; set; }
        public double AmountConsumed { get; set; }
        public double AmountLoaded { get; set; }
        public double AmountLoadedAnchor { get; set; }
    }
}
