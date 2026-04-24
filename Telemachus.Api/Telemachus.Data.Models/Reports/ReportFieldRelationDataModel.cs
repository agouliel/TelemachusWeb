namespace Telemachus.Data.Models.Reports
{
    public class ReportFieldRelationDataModel : EntityMaster
    {
        public int Id { get; set; }
        public int ReportFieldId { get; set; }
        public int ReportTypeId { get; set; }
        public ReportFieldDataModel ReportField { get; set; }
        public ReportTypeDataModel ReportType { get; set; }

    }
}
