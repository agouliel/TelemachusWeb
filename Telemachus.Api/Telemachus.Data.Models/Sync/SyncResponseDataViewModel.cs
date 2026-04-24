using System.Collections.Generic;

namespace Telemachus.Data.Models.Sync
{
    public class SyncResponseDataViewModel : SyncResponseReadOnlyDataViewModel
    {
        public List<Data.Voyage> Voyages { get; set; }
        public List<Data.EventAttachment> EventAttachments { get; set; }
        public List<Data.BunkeringData> BunkeringData { get; set; }
        public List<Data.BunkeringDataTank> BunkeringDataTanks { get; set; }
        public List<Data.Report> Reports { get; set; }
        public List<Data.ReportFieldValue> ReportFieldValues { get; set; }
        public List<Data.StatementOfFact> StatementOfFacts { get; set; }
        public List<Data.Cargo> Cargoes { get; set; }
        public List<Data.CargoDetail> CargoDetails { get; set; }
    }
}
