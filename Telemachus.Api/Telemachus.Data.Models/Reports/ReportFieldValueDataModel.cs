using System.ComponentModel.DataAnnotations.Schema;

namespace Telemachus.Data.Models.Reports
{
    public class ReportFieldValueDataModel : EntityBase
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int ReportId { get; set; }
        public int ReportFieldId { get; set; }
        public ReportDataModel Report { get; set; }
        public ReportFieldDataModel ReportField { get; set; }


        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string BusinessId { get; set; }
        public double AsDouble()
        {
            double.TryParse(Value, out double numericValue);
            return numericValue;
        }

        public int? ReportContextId { get; set; }
        [ForeignKey(nameof(ReportContextId))]
        public ReportContextDataModel ReportContext { get; set; }
    }
}
