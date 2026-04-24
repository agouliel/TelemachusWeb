namespace Telemachus.Business.Models.Events
{
    public class BunkeringTankBusinessModel
    {
        public int Id { get; set; }
        public int BunkeringDataId { get; set; }
        public int TankId { get; set; }
        public string Amount { get; set; }
        public int? ComminglingId { get; set; }
        public string BusinessId { get; set; }
        public BunkeringDataBusinessModel ComminglingData { get; set; }
    }
}
