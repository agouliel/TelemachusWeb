using Telemachus.Data.Models.Reports;

namespace Telemachus.Business.Models.Reports
{
    public class ReportFieldValueBusinessModel
    {
        public int ReportId { get; set; }
        public int Id { get; set; }
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public string Value { get; set; }
        public ReportFieldGroupDataModel Group { get; set; }
        public bool? IsSubgroupMainField { get; set; }
        //public int? SubgroupId { get; set; }
        public string TankName { get; set; }
        public int? TankId { get; set; }
        public string ValidationKey { get; set; }
        public string TankCapacity { get; set; }
        public string Description { get; set; }
        public int? TankDisplayOrder { get; set; }
        public bool? Storage { get; set; }
        public bool? Settling { get; set; }
        public bool? Serving { get; set; }
        public bool IsViscosityField()
        {
            return ValidationKey == "kinematicViscosity";
        }
        public bool IsWeightField()
        {
            return ValidationKey == "weight";
        }
    }
}
