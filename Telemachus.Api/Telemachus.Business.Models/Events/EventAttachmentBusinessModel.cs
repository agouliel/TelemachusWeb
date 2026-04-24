namespace Telemachus.Business.Models.Events
{
    public class EventAttachmentBusinessModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public long FileSize { get; set; }
        public DocumentTypeBusinessModel DocumentType { get; set; }
        public int EventId { get; set; }
        public int? ReportId { get; set; }
        public int? ReportFieldId { get; set; }
        public int? BunkeringDataId { get; set; }
        public int? DocumentTypeId { get; set; }
        public string FilePath { get; set; }

    }
}
