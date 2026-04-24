using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Models.Reports
{
    public class ReportDataModel : EntityBase
    {
        public int Id { get; set; }
        public DateTimeOffset? CreatedDate { get; set; } = null;
        public int EventId { get; set; }
        public EventDataModel Event { get; set; }
        public ICollection<ReportFieldValueDataModel> FieldValues { get; set; }
        [NotMapped]
        public bool ReadOnly { get; set; } = false;
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string BusinessId { get; set; }
        public int? PrevReportId { get; set; }
        public ReportDataModel PrevReport { get; set; }
        public ICollection<EventAttachmentDataModel> Attachments { get; set; }
        public ICollection<ReportContextDataModel> ReportContext { get; set; }

    }
}
