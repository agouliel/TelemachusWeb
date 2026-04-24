using Telemachus.Data.Models.Events;

namespace Telemachus.Data.Models.Reports
{
    public class EventTypeReportFieldDataModel : EntityBase
    {
        public int Id { get; set; }
        public int EventTypeId { get; set; }
        public int ReportFieldId { get; set; }
        public EventTypeDataModel EventType { get; set; }
        public ReportFieldDataModel ReportField { get; set; }
    }
}
