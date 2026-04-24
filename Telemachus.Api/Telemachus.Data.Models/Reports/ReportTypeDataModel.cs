using System.Collections.Generic;

namespace Telemachus.Data.Models.Reports
{
    public class ReportTypeDataModel : EntityMaster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ReportFieldRelationDataModel> AvailableReportFields { get; set; }
    }
}
