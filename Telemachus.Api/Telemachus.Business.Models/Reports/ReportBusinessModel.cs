using System.Collections.Generic;

using Telemachus.Business.Models.Events;

namespace Telemachus.Business.Models.Reports
{
    public class ReportBusinessModel
    {
        public int Id { get; set; }
        public EventBusinessModel Event { get; set; }
        public List<ReportFieldValueBusinessModel> ReportFields { get; set; }
        public ReportBusinessModel RelatedReport { get; set; }
        public ReportPerformanceBusinessModel Performance { get; set; } = null;
        public ReportingPropsBusinessModel ReportingProps { get; set; }
        public List<BunkeringDataBusinessModel> BunkeringData { get; set; } = new List<BunkeringDataBusinessModel>();

    }
}
