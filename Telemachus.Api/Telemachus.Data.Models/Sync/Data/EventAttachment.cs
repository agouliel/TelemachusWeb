using System;

namespace Telemachus.Data.Models.Sync.Data
{
    public class EventAttachment : EntityBase
    {
        public string Id { get; set; }
        public string EventId { get; set; }
        public int DisplayIndex { get; set; }
        public string ReportId { get; set; }
        public Guid? ReportFieldId { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public long FileSize { get; set; }
        public Guid? DocumentTypeId { get; set; }
        public string BunkeringDataId { get; set; }
    }
}
