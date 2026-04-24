using System.ComponentModel.DataAnnotations.Schema;

using Enums;

namespace Telemachus.Data.Models.Reports
{
    public class ReportFieldGroupDataModel : EntityMaster
    {
        public int Id { get; set; }
        public string FieldGroupName { get; set; }
        public int FuelTypeId { get; set; }
        [ForeignKey(nameof(FuelTypeId))]
        public FuelTypeDataModel FuelType { get; set; }

        public bool IsRobHfoActualGroup()
        {
            return BusinessId == ReportType.RobHfoActualGroup;
        }
        public bool IsRobHfoPoolGroup()
        {
            return BusinessId == ReportType.RobHfoPoolGroup;
        }
        public bool IsRobMgoActualGroup()
        {
            return BusinessId == ReportType.RobMgoActualGroup;
        }
        public bool IsRobMgoPoolGroup()
        {
            return BusinessId == ReportType.RobMgoPoolGroup;
        }
        public bool IsRobHfoBunkerGroup()
        {
            return BusinessId == ReportType.RobHfoBunkerGroup;
        }
        public bool IsRobMgoBunkerGroup()
        {
            return BusinessId == ReportType.RobMgoBunkerGroup;
        }
    }
}
