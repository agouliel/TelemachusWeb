using System;

namespace Telemachus.Data.Models.Sync
{
    public class SyncRequestViewModel
    {
        public string UserPrefix { get; set; }
        public DateTime Voyages { get; set; }
        public DateTime Events { get; set; }
        public DateTime EventAttachments { get; set; }
        public DateTime Reports { get; set; }
        public DateTime ReportFieldValues { get; set; }
        public DateTime StatementOfFacts { get; set; }
        public DateTime BunkeringData { get; set; }
        public DateTime BunkeringDataTanks { get; set; }
        public DateTime Cargoes { get; set; }
        public DateTime CargoDetails { get; set; }
    }
}
