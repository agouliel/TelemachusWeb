using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Telemachus.Data.Models.Reports;

namespace Telemachus.Data.Models.Events
{
    public class EventAttachmentDataModel : EntityBase
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public EventDataModel Event { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string BusinessId { get; set; }
        public int? ReportId { get; set; }
        public ReportDataModel Report { get; set; }
        public int? ReportFieldId { get; set; }
        public ReportFieldDataModel ReportField { get; set; }
        [StringLength(255)]
        public string FileName { get; set; }
        [StringLength(255)]
        public string MimeType { get; set; } = "application/octet-stream";
        public long FileSize { get; set; }
        [JsonIgnore]
        [NotMapped]
        public string FilePath { get; set; }
        public int? DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; }
        public int? BunkeringDataId { get; set; }
        [ForeignKey(nameof(BunkeringDataId))]
        public BunkeringDataModel BunkeringData { get; set; }
    }
}
