using System.Collections.Generic;

namespace Telemachus.Business.Models.Reports
{
    public class ReportPerformanceBusinessModel
    {
        public double? MainEngineMaxPower { get; set; } = null;
        public double? PitchPropeller { get; set; } = null;
        public double TotalDistanceOverGround { get; set; } = 0;
        public TotalConsumptionBusinessModel TotalConsumption { get; set; } = null;
        public List<SteamingTimeBusinessModel> SteamingTime { get; set; } = new List<SteamingTimeBusinessModel>();
    }
}
