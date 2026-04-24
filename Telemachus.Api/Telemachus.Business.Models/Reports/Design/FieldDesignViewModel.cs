
using System.Collections.Generic;
using Telemachus.Data.Models.Reports;

namespace Telemachus.Business.Models.Reports.Design
{
    public class FieldDesignViewModel
    {
        public List<ReportTypeDataModel> ReportTypes { get; set; } = new List<ReportTypeDataModel>();
        public List<ReportFieldGroupDataModel> Groups { get; set; } = new List<ReportFieldGroupDataModel>();
        public List<FieldDesignModel> Fields { get; set; } = new List<FieldDesignModel>();

    }
}
