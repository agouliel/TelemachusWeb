using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Telemachus.Data.Models.Reports
{
    public class ReportFieldDataModel : EntityMaster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsSubgroupMain { get; set; }
        //public int? Subgroup { get; set; }
        public ReportFieldGroupDataModel Group { get; set; }
        public int? GroupId { get; set; }
        [ForeignKey("Tank")]
        public int? TankId { get; set; }
        public TankDataModel Tank { get; set; }
        public string ValidationKey { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public TankUserSpecsDataModel UserTank { get; set; }
        public ICollection<ReportFieldValueDataModel> FieldValues { get; set; }
        public ICollection<ReportFieldRelationDataModel> ReportRelatedFields { get; set; }


    }
}
