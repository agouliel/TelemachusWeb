using System.Collections.Generic;
using System.Linq;
using Telemachus.Business.Models.Reports;
using Telemachus.Models.Reports;

namespace Telemachus.Mappers
{
    public static class ReportMapper
    {
        public static ReportingPropsBusinessModel ToReportingPropsBusinessModel(this ReportCreateViewModel model)
        {
            if (model == null)
            {
                return null;
            }

            var props = new ReportingPropsBusinessModel()
            {
                InternalTransferSourceTankId = model.InternalTransferSourceTankId,
                InternalTransferTargetTankId = model.InternalTransferTargetTankId,
                InternalTransferAmount = model.InternalTransferAmount
            };

            return props;
        }
        public static List<ReportFieldValueBusinessModel> ToBusinessModel(this ReportCreateViewModel model)
        {
            if (model == null)
            {
                return new List<ReportFieldValueBusinessModel>();
            }

            if (model.FieldValues == null)
            {
                return new List<ReportFieldValueBusinessModel>();
            }

            return model.FieldValues.Select(ToBusinessModel).ToList();
        }

        public static ReportFieldValueBusinessModel ToBusinessModel(this ReportFieldValueViewModel model)
        {
            if (model == null)
            {
                return null;
            }

            return new ReportFieldValueBusinessModel()
            {
                FieldId = model.FieldId,
                Value = model.Value
            };
        }
    }
}
