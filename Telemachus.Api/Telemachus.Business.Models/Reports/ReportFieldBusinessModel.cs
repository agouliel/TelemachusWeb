using System.Collections.Generic;
using Telemachus.Business.Models.Events;
using Telemachus.Data.Models.Reports;

namespace Telemachus.Business.Models.Reports
{
    public class ReportMatchBusinessModel
    {
        public List<ReportFieldBusinessModel> ReportFields { get; set; }
        public ReportBusinessModel RelatedReport { get; set; } = null;
        public ReportPerformanceBusinessModel Performance { get; set; } = null;
        public EventBusinessModel Event { get; set; }
        public List<BunkeringDataBusinessModel> BunkeringData { get; set; } = new List<BunkeringDataBusinessModel>();
    }
    public class ReportFieldBusinessModel
    {
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public ReportFieldGroupDataModel Group { get; set; }
        //public int? BunkeringDataGroupId { get; set; }
        public bool? IsSubgroupMainField { get; set; }
        //public int? SubgroupId { get; set; }
        //public bool IsManagedByBunkProc { get; set; }
        public string Value { get; set; }
        public string MaxValue { get; set; }
        public string TankName { get; set; }
        public int? TankId { get; set; }
        public int? TankDisplayOrder { get; set; }
        public bool? Storage { get; set; }
        public bool? Settling { get; set; }
        public bool? Serving { get; set; }
        public string ValidationKey { get; set; }
        public string Description { get; set; }
        public string CurrMaxValue { get; set; }
        public string TankCapacity { get; set; }
    }
}
