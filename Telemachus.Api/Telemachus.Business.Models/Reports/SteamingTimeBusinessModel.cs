using Enums;

namespace Telemachus.Business.Models.Reports
{
    public class SteamingTimeBusinessModel
    {
        public int EventId { get; set; }
        public double SteamingTime { get; set; } = 0;
        public SulphurOil Oil { get; set; } = SulphurOil.Unspecified;
    }
}
