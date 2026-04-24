namespace Telemachus.Data.Models.Sync
{
    public class SyncResponseViewModel
    {
        public string User { get; set; }
        public string Operator { get; set; }
        public double? PitchPropeller { get; set; }
        public double? MainEngineMaxPower { get; set; }
        public bool NonHafnia { get; set; }
        public bool NonPool { get; set; }
        public SyncRequestViewModel LocalTimestamps { get; set; } = null;
        public SyncResponseDataViewModel Data { get; set; }
    }
}
