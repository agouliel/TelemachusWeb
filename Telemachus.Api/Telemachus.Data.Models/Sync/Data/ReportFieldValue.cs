using System;

namespace Telemachus.Data.Models.Sync.Data
{
    public class ReportFieldValue : EntityBase
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public string ReportId { get; set; }
        public Guid ReportFieldId { get; set; }
        public int DisplayIndex { get; set; }

    }
}
