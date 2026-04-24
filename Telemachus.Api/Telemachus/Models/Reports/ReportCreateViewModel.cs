using System.Collections.Generic;

namespace Telemachus.Models.Reports
{
    public class ReportCreateViewModel
    {
        public List<ReportFieldValueViewModel> FieldValues { get; set; }
        public int? InternalTransferSourceTankId { get; set; }
        public int? InternalTransferTargetTankId { get; set; }
        public double? InternalTransferAmount { get; set; }
    }

    public class ReportFieldValueViewModel
    {
        public int FieldId { get; set; }
        public string Value { get; set; }
    }
}
