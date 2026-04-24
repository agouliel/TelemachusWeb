namespace Telemachus.Models
{
    public class VesselDetails
    {
        public string Name { get; set; }
        public string Prefix { get; set; }
        public string Operator { get; set; }
        public int AvailablePasscodeSlots { get; set; }
        public double? PitchPropeller { get; set; }
        public double? MainEngineMaxPower { get; set; }
        public string RemoteAddress { get; set; }
        public int RemotePort { get; set; }
        public int ListenPort { get; set; }
        public string InitialPassword { get; set; }
        public bool NonHafnia { get; set; }
        public bool NonPool { get; set; }

    }
}
