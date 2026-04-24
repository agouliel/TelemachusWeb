namespace Telemachus.Data.Models.Sync.Data
{
    public class Report : EntityBase
    {
        public string Id { get; set; }
        public string EventId { get; set; }
        public int DisplayIndex { get; set; }
    }
}
